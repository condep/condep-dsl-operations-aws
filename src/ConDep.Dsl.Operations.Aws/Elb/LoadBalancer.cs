using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using Microsoft.CSharp.RuntimeBinder;
using Instance = Amazon.ElasticLoadBalancing.Model.Instance;

namespace ConDep.Dsl.Operations.Aws.Elb
{
    public class LoadBalancer : ILoadBalance
    {
        private readonly LoadBalancerConfig _config;

        public LoadBalancer(LoadBalancerConfig config)
        {
            _config = config;
        }

        public void BringOffline(string serverName, string farm, LoadBalancerSuspendMethod suspendMethod, IReportStatus status)
        {
            var credentials = GetCredentials(_config.CustomConfig);
            var endpoint = GetEndpoint(_config.CustomConfig);

            var ec2Client = new AmazonEC2Client(credentials, endpoint);
            var client = new AmazonElasticLoadBalancingClient(credentials, endpoint);

            var request = new DeregisterInstancesFromLoadBalancerRequest(farm, new List<Instance> { new Instance(GetInstanceId(ec2Client, serverName)) });
            var response = client.DeregisterInstancesFromLoadBalancer(request);
        }

        public void BringOnline(string serverName, string farm, IReportStatus status)
        {
            var credentials = GetCredentials(_config.CustomConfig);
            var endpoint = GetEndpoint(_config.CustomConfig);

            var ec2Client = new AmazonEC2Client(credentials, endpoint);
            var client = new AmazonElasticLoadBalancingClient(credentials, endpoint);

            var request = new RegisterInstancesWithLoadBalancerRequest(farm, new List<Instance> { new Instance(GetInstanceId(ec2Client, serverName)) });
            var response = client.RegisterInstancesWithLoadBalancer(request);
        }

        public string GetInstanceId(AmazonEC2Client client, string serverName)
        {
            var instancesRequest = new DescribeInstancesRequest
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Name = "dns-name",
                        Values = new[] {serverName}.ToList()
                    }
                }
            };
            var instances = client.DescribeInstances(instancesRequest);
            Logger.Info("Found instances: {0}", string.Join(", ", instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId))));
            return instances.Reservations.Single().Instances.Single().InstanceId;
        }

        private RegionEndpoint GetEndpoint(dynamic customConfig)
        {
            if (customConfig == null || customConfig.Region == null)
            {
                throw new Exception();
            }

            return RegionEndpoint.GetBySystemName((string)customConfig.Region);
        }

        private AWSCredentials GetCredentials(dynamic customConfig)
        {
            //if (customConfig == null || customConfig.Credentials == null || customConfig.Credentials.AccessKey == null || customConfig.Credentials.SecretKey == null)
            //{
            //    throw new Exception();
            //}

            try
            {
                return new BasicAWSCredentials((string)customConfig.Credentials.AccessKey, (string)customConfig.Credentials.SecretKey);
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(
                    string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.",
                        GetType().Name), binderException);
            }
        }

        public LbMode Mode { get; set; }
    }
}