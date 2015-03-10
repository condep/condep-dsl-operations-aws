using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;
using ConDep.Dsl;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;

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

        public bool AllreadyBootstrapped(string bootstrapId)
        {
            return GetInstances(bootstrapId).Any();
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
            var instances = _client.DescribeInstances(instancesRequest);
            Logger.Info("Found instances: {0}", string.Join(", ", instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId))));
            return instances.Reservations.SelectMany(x => x.Instances);
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
            var instances = GetInstances(instanceIds).ToList();
            var states = instances.Select(y => y.State);

            Logger.WithLogSection("Status of instances", () =>
            {
                foreach (var instance in instances)
                {
                    Logger.Info("Instance Id: {0}  Status: {1}", instance.InstanceId, instance.State.Name);
                }
            });

            if (states.Any(x => x.Name != "terminated"))
            {
                Thread.Sleep(5000);
                WaitForInstancesToTerminate(instanceIds);
            }
        }
    }
}