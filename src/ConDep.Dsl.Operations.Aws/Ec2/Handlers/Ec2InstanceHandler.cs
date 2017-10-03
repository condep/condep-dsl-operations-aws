using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    internal class Ec2InstanceHandler
    {
        private readonly IAmazonEC2 _client;

        public Ec2InstanceHandler(IAmazonEC2 client)
        {
            _client = client;
        }

        public IEnumerable<Instance> GetInstances(IEnumerable<string> instanceIds)
        {
            var instancesRequest = new DescribeInstancesRequest();
            instancesRequest.InstanceIds.AddRange(instanceIds);
            var instances = _client.DescribeInstances(instancesRequest);
            return instances.Reservations.SelectMany(x => x.Instances);
        }

        public bool AllreadyBootstrapped(AwsBootstrapOptionsValues options)
        {
            if (options.IdempotencyType == AwsEc2IdempotencyType.ClientToken) return GetInstances(options.InstanceRequest.ClientToken).Any();
            if (options.IdempotencyType == AwsEc2IdempotencyType.Tags) return GetInstances(options.IdempotencyTags).Any();

            throw new ArgumentException("options.IdempotencyType");
        }

        public IEnumerable<Instance> GetInstances(string bootstrapId)
        {
            var instancesRequest = new DescribeInstancesRequest
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Name = "client-token",
                        Values = new[] {bootstrapId}.ToList()
                    }
                }
            };
            return GetInstances(instancesRequest);
        }

        public IEnumerable<Instance> GetInstances(IEnumerable<KeyValuePair<string, string>> idempotencyTags)
        {
            var instancesRequest = new DescribeInstancesRequest
            {
                Filters = CreateIdempotencyTagsFilter(idempotencyTags).ToList()
            };
            return GetInstances(instancesRequest);
        }

        private IEnumerable<Instance> GetInstances(DescribeInstancesRequest request)
        {
            var instances = _client.DescribeInstances(request);
            Logger.Info("Found instances: {0}", string.Join(", ", instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId + "(" + y.State.Name + ")"))));
            return instances.Reservations.SelectMany(x => x.Instances).Where(x => x.State.Name != "terminated");
        } 

        private IEnumerable<Filter> CreateIdempotencyTagsFilter(IEnumerable<KeyValuePair<string, string>> idempotencyTags)
        {
            return idempotencyTags.Select(tag => new Filter("tag:" + tag.Key, new [] {tag.Value}.ToList()));
        }

        public IEnumerable<string> CreateInstances(AwsBootstrapOptionsValues request)
        {
            RunInstancesResponse runResponse = _client.RunInstances(request.InstanceRequest);
            return runResponse.Reservation.Instances.Select(x => x.InstanceId);
        }

        public void WaitForInstancesStatus(IEnumerable<string> instanceIds, Ec2InstanceState state)
        {
            Thread.Sleep(15000);
            var instances = GetInstances(instanceIds).ToList();
            var states = instances.Select(y => y.State);

            if (states.Any(x => x.Code != (int)state))
            {
                Logger.Info("One or more instances is not in state {0}, waiting 15 seconds...", state.ToString());
                WaitForInstancesStatus(instanceIds, state);
            }
        }

        internal void Stop(string bootstrapId)
        {
            Logger.Info("Stopping instances");
            var instanceRequest = new DescribeInstancesRequest
            {
                Filters = new[]
                {
                    new Filter
                    {
                        Name = "client-token",
                        Values = new[] {bootstrapId}.ToList()
                    }
                }.ToList()
            };
            var instances = _client.DescribeInstances(instanceRequest);

            var stopRequest = new StopInstancesRequest();
            var instanceIds = instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId)).ToList();
            stopRequest.InstanceIds.AddRange(instanceIds);
            _client.StopInstances(stopRequest);
            Logger.WithLogSection("Waiting for instances to stop", () => WaitForInstancesToStop(instanceIds));
        }

        public void Terminate(string bootstrapId)
        {
            Logger.Info("Terminating instances");
            var instanceRequest = new DescribeInstancesRequest
            {
                Filters = new[]
                {
                    new Filter
                    {
                        Name = "client-token",
                        Values = new[] {bootstrapId}.ToList()
                    }
                }.ToList()
            };
            var instances = _client.DescribeInstances(instanceRequest);

            var terminationRequest = new TerminateInstancesRequest();
            var instanceIds = instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId)).ToList();
            terminationRequest.InstanceIds.AddRange(instanceIds);

            _client.TerminateInstances(terminationRequest);
            Logger.WithLogSection("Waiting for instances to terminate", () => WaitForInstancesToTerminate(instanceIds));
        }

        private void WaitForInstancesToTerminate(List<string> instanceIds)
        {
            WaitForInstancesStatus(instanceIds, Ec2InstanceState.Terminated);
        }
        private void WaitForInstancesToStop(List<string> instanceIds)
        {
            WaitForInstancesStatus(instanceIds, Ec2InstanceState.Stopped );
        }


        public void TagInstances(List<string> instanceIds, List<KeyValuePair<string, string>> tags)
        {
            if (tags.Count == 0) return;

            var request = new CreateTagsRequest(instanceIds, tags.Select(x => new Tag(x.Key, x.Value)).ToList());
            _client.CreateTags(request);
        }
    }
}