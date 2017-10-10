using System.Diagnostics;
using System.Threading;
using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Harvesters;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;
using ConDep.Execution;
using ConDep.Execution.Validation;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Aws.Ec2
{
    internal class AwsBootstrapOperation : LocalOperation
    {
        private readonly AwsBootstrapOptionsValues _options;

        public AwsBootstrapOperation(AwsBootstrapOptionsValues options)
        {
            _options = options;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(_options, settings);

            var bootstrapper = new Ec2Bootstrapper(_options, settings);
            var ec2Config = bootstrapper.Boostrap();
            var instances = new List<Ec2Instance>();
            foreach (var instance in ec2Config.Instances)
            {
                instances.Add(instance);
                settings.Config.Servers.Add(new ServerConfig
                {
                    DeploymentUser = new DeploymentUserConfig
                    {
                        UserName = instance.UserName,
                        Password = instance.Password
                    },
                    Name = instance.ManagementAddress,
                    PowerShell = new PowerShellConfig() { HttpPort = 5985, HttpsPort = 5986 },
                    Node = new NodeConfig() { Port = 4444, TimeoutInSeconds = 100 }
                });
            }

            var serverValidator = new RemoteServerValidator(settings.Config.Servers, HarvesterFactory.GetHarvester(settings), new PowerShellExecutor());
            if (!serverValidator.Validate())
            {
                throw new ConDepValidationException("Not all servers fulfill ConDep's requirements. Aborting execution.");
            }

            ConDepConfigurationExecutor.ExecutePreOps(settings, token);
            var result = Result.SuccessChanged();
            result.Data.Instances = instances;
            return result;
        }


        private void ValidateMandatoryOptions(AwsBootstrapOptionsValues options, ConDepSettings conDepSettings)
        {
            if (string.IsNullOrWhiteSpace(options.InstanceRequest.SubnetId) && !PrimaryNetworkInterfaceDefined(options)) throw new OperationConfigException(string.Format("Missing value for SubnetId. Please specify in code (using SubnetId or specify in NetworkInterface) or in config."));
            if (string.IsNullOrWhiteSpace(options.InstanceRequest.KeyName)) throw new OperationConfigException(string.Format("Missing value for PublicKeyName. Please specify in code or in config."));
            if (!conDepSettings.Config.DeploymentUser.IsDefined())
            {
                if (string.IsNullOrWhiteSpace(options.PrivateKeyFileLocation)) throw new OperationConfigException(string.Format("Missing value for PrivateKeyFileLocation. Please specify in code or in config."));
            }

            if (options.RegionEndpoint == null) throw new OperationConfigException(string.Format("Missing value for Region. Please specify in code or in config."));
            if (options.Credentials == null) throw new OperationConfigException(string.Format("Missing value for Credentials. Please specify in code or in config."));

            if (options.InstanceRequest.Placement == null) throw new OperationConfigException(string.Format("Missing value for AvailabilityZone. Please specify in code or in config."));
            if (options.InstanceRequest.SecurityGroupIds.Count == 0) Logger.Warn("No value for SecurityGroupIds given. Default Security Group will be used.");
            if (string.IsNullOrWhiteSpace(options.InstanceRequest.SubnetId)) Logger.Warn("No value for SubnetId given. Default Subnet will be used."); 
            if (!options.Image.HasImageFilter() &&  !options.Image.HasImageId() && !options.Image.HasLatestImageDefined()) Logger.Warn("No value for Image given. Latest defined Windows Image will be used.");
            if (string.IsNullOrWhiteSpace(options.InstanceRequest.InstanceType)) Logger.Warn("No value for InstanceType given. Default Instance Type will be used.");
        }

        private bool PrimaryNetworkInterfaceDefined(AwsBootstrapOptionsValues options)
        {
            if (options.NetworkInterfaceValues == null || options.NetworkInterfaceValues.NetworkInterfaces.Count == 0)
                return false;

            return !string.IsNullOrWhiteSpace(options.NetworkInterfaceValues.NetworkInterfaces[0].SubnetId) ;
        }

        private void LoadOptionsFromConfig(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || (settings.Config.OperationsConfig.AwsBootstrapOperation == null && settings.Config.OperationsConfig.AwsBootstrapOperation == null && settings.Config.OperationsConfig.Aws == null))
            {
                return;
            }

            var dynamicBootstrapConfig = settings.Config.OperationsConfig.AwsBootstrapOperation;
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            try
            {
                if (dynamicAwsConfig != null)
                {
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.KeyName) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.PublicKeyName)) _options.InstanceRequest.KeyName = dynamicAwsConfig.PublicKeyName;
                    if (string.IsNullOrWhiteSpace(_options.PrivateKeyFileLocation) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.PrivateKeyFileLocation)) _options.PrivateKeyFileLocation = dynamicAwsConfig.PrivateKeyFileLocation;
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.SubnetId) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.SubnetId)) _options.InstanceRequest.SubnetId = dynamicAwsConfig.SubnetId;
                    if (_options.InstanceRequest.Placement == null && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.AvailabilityZone)) _options.InstanceRequest.Placement = new Placement((string)dynamicAwsConfig.AvailabilityZone);
                    if (_options.RegionEndpoint == null && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.Region)) _options.RegionEndpoint = RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region);

                    if (_options.Credentials == null)
                    {
                        string profileName = dynamicAwsConfig.Credentials.ProfileName;
                        if (string.IsNullOrEmpty(profileName))
                        {
                            if (dynamicAwsConfig.Credentials.AccessKey == null)
                                throw new OperationConfigException(string.Format("Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.", GetType().Name));
                            if (dynamicAwsConfig.Credentials.SecretKey == null)
                                throw new OperationConfigException(string.Format("Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.", GetType().Name));

                            _options.Credentials = new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
                        }
                        else
                        {
                            _options.Credentials = new StoredProfileAWSCredentials((string)dynamicAwsConfig.Credentials.ProfileName);
                        }
                    }
                }

                if (dynamicBootstrapConfig != null)
                {
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.SubnetId) && !string.IsNullOrWhiteSpace((string)dynamicBootstrapConfig.SubnetId)) _options.InstanceRequest.SubnetId = dynamicBootstrapConfig.SubnetId;
                }
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.", GetType().Name), binderException);
            }
        }

        public override string Name
        {
            get { return "AWS Bootstrap"; }
        }
    }
}
