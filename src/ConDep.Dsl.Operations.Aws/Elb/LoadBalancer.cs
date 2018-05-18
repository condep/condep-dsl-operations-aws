using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.LoadBalancer;
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

        public Result BringOffline(string serverName, string farm, LoadBalancerSuspendMethod suspendMethod)
        {
            var credentials = GetCredentials(_config.CustomConfig);
            var endpoint = GetEndpoint(_config.CustomConfig);

            var ec2Client = new AmazonEC2Client(credentials, endpoint);
            var client = new AmazonElasticLoadBalancingClient(credentials, endpoint);

            var request = new DeregisterInstancesFromLoadBalancerRequest(farm, new List<Instance> { new Instance(GetInstanceId(ec2Client, serverName)) });
            var response = client.DeregisterInstancesFromLoadBalancer(request);
            var result = Result.SuccessChanged();
            result.Data.HttpStatusCode = response.HttpStatusCode;
            result.Data.ActiveInstances = response.Instances;
            return result;
        }

        public Result BringOnline(string serverName, string farm)
        {
            var credentials = GetCredentials(_config.CustomConfig);
            var endpoint = GetEndpoint(_config.CustomConfig);

            var ec2Client = new AmazonEC2Client(credentials, endpoint);
            var client = new AmazonElasticLoadBalancingClient(credentials, endpoint);

            var request = new RegisterInstancesWithLoadBalancerRequest(farm, new List<Instance> { new Instance(GetInstanceId(ec2Client, serverName)) });
            var response = client.RegisterInstancesWithLoadBalancer(request);
            var result = Result.SuccessChanged();
            result.Data.HttpStatusCode = response.HttpStatusCode;
            result.Data.ActiveInstances = response.Instances;
            return result;
        }

        public LoadBalanceState GetServerState(string serverName, string farm)
        {
            throw new NotImplementedException();
        }

        public string GetInstanceId(AmazonEC2Client client, string serverName)
        {
            var filter = new Filter();

            if (IsIp(serverName))
            {
                filter.Name = "ip-address";
                filter.Values = new[] {serverName}.ToList();
            }
            else
            {
                filter.Name = "dns-name";
                filter.Values = new[] {serverName}.ToList();
            }

            var instancesRequest = new DescribeInstancesRequest
            {
                Filters = new List<Filter>{ filter }
            };

            var instances = client.DescribeInstances(instancesRequest);
            Logger.Info("Found instances: {0}", string.Join(", ", instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId))));
            return instances.Reservations.Single().Instances.Single().InstanceId;
        }

        private bool IsIp(string serverName)
        {
            IPAddress ip;
            return IPAddress.TryParse(serverName, out ip);
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

        public LoadBalancerMode Mode { get; set; }
    }
}