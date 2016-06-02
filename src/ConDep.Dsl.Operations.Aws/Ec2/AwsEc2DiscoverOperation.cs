using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Harvesters;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;
using ConDep.Execution;
using ConDep.Execution.Validation;

namespace ConDep.Dsl.Operations.Aws.Ec2
{
    internal class AwsEc2DiscoverOperation : LocalOperation
    {
        private readonly List<KeyValuePair<string, string>> _tags;
        private readonly AwsEc2DiscoverOptionsValues _awsOptions;

        public AwsEc2DiscoverOperation(List<KeyValuePair<string, string>> tagValues, AwsEc2DiscoverOptionsValues awsOptions)
        {
            _tags = tagValues;
            _awsOptions = awsOptions;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var client = GetAwsClient(settings, _awsOptions);

            var searchFilter = GetSearchFilters(_tags).ToList();
            var request = new DescribeInstancesRequest {Filters = searchFilter};
            var response = client.DescribeInstances(request);

            var resultInstances = new List<dynamic>();
            foreach (var reservation in response.Reservations)
            {
                foreach (var instance in reservation.Instances.Where(x => x.State.Name == "Running"))
                {
                    var serverAddress = GetManagementAddress(_awsOptions.RemoteManagementAddressType, instance);
                    resultInstances.Add(new
                    {
                        Server = serverAddress
                    });
                    settings.Config.AddServer(serverAddress);
                }
            }

            var serverInfoHarvester = HarvesterFactory.GetHarvester(settings);
            var serverValidator = new RemoteServerValidator(settings.Config.Servers, serverInfoHarvester, new PowerShellExecutor());
            if (!serverValidator.Validate())
            {
                throw new ConDepValidationException("Not all servers fulfill ConDep's requirements. Aborting execution.");
            }

            ConDepConfigurationExecutor.ExecutePreOps(settings, token);
            var result = Result.SuccessUnChanged();
            result.Data.Instances = resultInstances;
            result.Data.HttpStatusCode = response.HttpStatusCode;
            return result;
        }

        private IEnumerable<Filter> GetSearchFilters(List<KeyValuePair<string, string>> tags)
        {
            var groups = tags.GroupBy(x => x.Key);
            return groups.Select(@group => new Filter("tag:" + @group.Key, new List<string>(tags.Where(x => x.Key == @group.Key).Select(x => x.Value))));
        }

        private IAmazonEC2 GetAwsClient(ConDepSettings settings, AwsEc2DiscoverOptionsValues awsOptions)
        {
            var creds = GetAwsCredentials(settings, awsOptions);
            var region = GetAwsRegion(settings, awsOptions);
            return new AmazonEC2Client(creds, region);
        }

        private RegionEndpoint GetAwsRegion(ConDepSettings settings, AwsEc2DiscoverOptionsValues awsOptions)
        {
            if (awsOptions.RegionEndpoint != null) return awsOptions.RegionEndpoint;

            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;
            if (dynamicAwsConfig != null)
            {
                if (!string.IsNullOrWhiteSpace((string) dynamicAwsConfig.Region))
                {
                    return RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region);
                }
            }

            throw new OperationConfigException(string.Format("AWS Region must be provided either through DSL or in configuration for operation {0}.", GetType().Name));
        }

        private AWSCredentials GetAwsCredentials(ConDepSettings settings, AwsEc2DiscoverOptionsValues awsOptions)
        {
            if (awsOptions.Credentials != null) return awsOptions.Credentials;

            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;
            if (dynamicAwsConfig != null)
            {
                string profileName = dynamicAwsConfig.Credentials.ProfileName;
                if (string.IsNullOrEmpty(profileName))
                {
                    if (dynamicAwsConfig.Credentials.AccessKey == null)
                        throw new OperationConfigException(string.Format("Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.", GetType().Name));
                    if (dynamicAwsConfig.Credentials.SecretKey == null)
                        throw new OperationConfigException(string.Format("Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.", GetType().Name));

                    return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
                }

                return new StoredProfileAWSCredentials((string)dynamicAwsConfig.Credentials.ProfileName);
            }

            throw new OperationConfigException(string.Format("AWS Credentials must be provided either through DSL or in configuration for operation {0}.", GetType().Name));
        }

        private string GetManagementAddress(RemoteManagementAddressType? managementAddressType, Instance instance)
        {
            var mngmntInterface = instance.NetworkInterfaces[0];

            if (managementAddressType != null)
            {
                switch (managementAddressType)
                {
                    case RemoteManagementAddressType.PublicDns:
                        if (mngmntInterface.Association == null || string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicDnsName)) throw new ConDepInvalidSetupException("Instance has no public DNS name.");
                        return mngmntInterface.Association.PublicDnsName;
                    case RemoteManagementAddressType.PublicIp:
                        if (mngmntInterface.Association == null || string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicIp)) throw new ConDepInvalidSetupException("Instance has no public IP.");
                        return mngmntInterface.Association.PublicIp;
                    case RemoteManagementAddressType.PrivateDns:
                        if (string.IsNullOrWhiteSpace(mngmntInterface.PrivateDnsName)) throw new ConDepInvalidSetupException("Instance has no private DNS name.");
                        return mngmntInterface.PrivateDnsName;
                    case RemoteManagementAddressType.PrivateIp:
                        return mngmntInterface.PrivateIpAddress;
                }
            }
            else
            {
                if (mngmntInterface.Association != null && !string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicDnsName))
                {
                    return mngmntInterface.Association.PublicDnsName;
                }
                if (mngmntInterface.Association != null && !string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicIp))
                {
                    return mngmntInterface.Association.PublicIp;
                }
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateDnsName))
                {
                    return mngmntInterface.PrivateDnsName;
                }
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateIpAddress))
                {
                    return mngmntInterface.PrivateIpAddress;
                }
            }
            throw new Exception("No remote management address found.");
        }

        public override string Name
        {
            get { return "Discover Amazon Ec2 Instances"; }
        }
    }
}