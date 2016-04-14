using System;
using System.Net;
using System.Threading;
using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Elb
{
    internal class AwsElbOperation : LocalOperation
    {
        private readonly CreateLoadBalancerRequest _request;

        public AwsElbOperation(CreateLoadBalancerRequest lbRequest)
        {
            _request = lbRequest;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var client = new AmazonElasticLoadBalancingClient(GetCredentials(settings), GetEndpoint(settings));
            var response = client.CreateLoadBalancer(_request);

            foreach (var server in settings.Config.Servers)
            {
                server.LoadBalancerFarm = _request.LoadBalancerName;
            }

            var result = response.HttpStatusCode == HttpStatusCode.Created ? Result.SuccessChanged() : Result.SuccessUnChanged();

            result.Data.LoadBalancerDnsName = response.DNSName;
            result.Data.HttpStatusCode = response.HttpStatusCode;
            return result;
        }

        //private bool LoadBalancerExists(AmazonElasticLoadBalancingClient client, CreateLoadBalancerRequest request)
        //{
        //    var lbRequest = new DescribeLoadBalancersRequest(new List<string> {request.LoadBalancerName});
        //    var lbResult = client.DescribeLoadBalancers(lbRequest);
        //    return lbResult.LoadBalancerDescriptions.Any();
        //}

        private RegionEndpoint GetEndpoint(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || settings.Config.OperationsConfig.AwsBootstrapOperation == null)
            {
                throw new Exception();
            }

            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;
            return RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region);
        }

        private AWSCredentials GetCredentials(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || settings.Config.OperationsConfig.AwsBootstrapOperation == null)
            {
                throw new Exception();
            }

            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            try
            {
                if (dynamicAwsConfig != null)
                {
                    string profileName = dynamicAwsConfig.Credentials.ProfileName;
                    if (string.IsNullOrEmpty(profileName))
                    {
                        if (dynamicAwsConfig.Credentials.AccessKey == null)
                            throw new OperationConfigException($"Configuration in environment configuration file for Credentials.AccessKey must be present for operation {GetType().Name}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.");
                        if (dynamicAwsConfig.Credentials.SecretKey == null)
                            throw new OperationConfigException($"Configuration in environment configuration file for Credentials.SecretKey must be present for operation {GetType().Name}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.");

                        return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
                    }
                    return new StoredProfileAWSCredentials((string)dynamicAwsConfig.Credentials.ProfileName);
                }
                throw new Exception();
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException($"Configuration extraction for {GetType().Name} failed during binding. Please check inner exception for details.", binderException);
            }
        }

        public override string Name => "Create AWS Elastic Load Balancer";
    }
}