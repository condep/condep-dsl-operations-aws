using System;
using System.Threading;
using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;
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

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var client = new AmazonElasticLoadBalancingClient(GetCredentials(settings), GetEndpoint(settings));
            var response = client.CreateLoadBalancer(_request);

            foreach (var server in settings.Config.Servers)
            {
                server.LoadBalancerFarm = _request.LoadBalancerName;
            }
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
                            throw new OperationConfigException(
                                string.Format(
                                    "Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                    GetType().Name));
                        if (dynamicAwsConfig.Credentials.SecretKey == null)
                            throw new OperationConfigException(
                                string.Format(
                                    "Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                    GetType().Name));

                        return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
                    }
                    return new StoredProfileAWSCredentials((string)dynamicAwsConfig.Credentials.ProfileName);
                }
                throw new Exception();
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(
                    string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.",
                        GetType().Name), binderException);
            }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Create AWS Elastic Load Balancer"; }
        }
    }
}