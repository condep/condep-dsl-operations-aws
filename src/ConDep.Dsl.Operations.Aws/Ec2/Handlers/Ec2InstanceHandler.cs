using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Validation;

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

        internal void WaitForInstanceToReachOneOfStates(string instanceId, ICollection<Ec2InstanceState> states)
        {

            Logger.Info(
                "Waiting for instance {0}  to reach one of the states {1}",
                instanceId,
                string.Join(", ", states.Select(s => Enum.GetName(typeof(Ec2InstanceState), s)))
                );
            Instance instance = null;
            while (!states.Select(s => (int)s).Contains((instance = GetInstances(new List<string> { instanceId }).Single()).State.Code))
            {
                Logger.Verbose("Instance Id: {0}  Status: {1}", instanceId, instance.State.Code);
                Thread.Sleep(15000);
            }
        }

        internal void Stop(string bootstrapId)
        {
            Logger.Info("Stopping instances");
            IEnumerable<string> instanceIds = FindInstanceIdsFromBootstrapId(bootstrapId);

            var stopRequest = new StopInstancesRequest();
            stopRequest.InstanceIds.AddRange(instanceIds);

            _client.StopInstances(stopRequest);
            Logger.WithLogSection("Waiting for instances to stop", () => WaitForInstancesToStop(instanceIds));
        }

        public void Terminate(string bootstrapId)
        {
            Logger.Info("Terminating instances");
            IEnumerable<string> instanceIds = FindInstanceIdsFromBootstrapId(bootstrapId);

            var terminationRequest = new TerminateInstancesRequest();
            terminationRequest.InstanceIds.AddRange(instanceIds);

            _client.TerminateInstances(terminationRequest);
            Logger.WithLogSection("Waiting for instances to terminate", () => WaitForInstancesToTerminate(instanceIds));
        }
        public void Start(string bootstrapId)
        {
            Logger.Info("Starting instances");
            StartInstancesRequest startRequest = new StartInstancesRequest();
            IEnumerable<string> instanceIds = FindInstanceIdsFromBootstrapId(bootstrapId);
            startRequest.InstanceIds.AddRange(instanceIds);

            _client.StartInstances(startRequest);
            Logger.WithLogSection("Waiting for instances to start running", () => WaitForInstancesToRun(instanceIds));
        }

        private IEnumerable<string> FindInstanceIdsFromBootstrapId(string bootstrapId)
        {
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
            return instances.Reservations.SelectMany(x => x.Instances.Select(y => y.InstanceId)).ToList();
        }

        private void WaitForInstancesToTerminate(IEnumerable<string> instanceIds)
        {
            WaitForInstancesStatus(instanceIds, Ec2InstanceState.Terminated);
        }
        private void WaitForInstancesToStop(IEnumerable<string> instanceIds)
        {
            WaitForInstancesStatus(instanceIds, Ec2InstanceState.Stopped );
        }
        private void WaitForInstancesToRun(IEnumerable<string> instanceIds)
        {
            WaitForInstancesStatus(instanceIds, Ec2InstanceState.Running);
        }


        public void TagInstances(List<string> instanceIds, List<KeyValuePair<string, string>> tags)
        {
            if (tags.Count == 0) return;

            var request = new CreateTagsRequest(instanceIds, tags.Select(x => new Tag(x.Key, x.Value)).ToList());
            _client.CreateTags(request);
        }

        public string GetManagementAddress(Instance instance, RemoteManagementAddressType? managementAddressType = null, int? remoteManagementInterfaceIndex = null)
        {
            var mngmntInterface = GetManagementInterface(instance, remoteManagementInterfaceIndex);

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
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateIpAddress))
                {
                    return mngmntInterface.PrivateIpAddress;
                }
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateDnsName))
                {
                    return mngmntInterface.PrivateDnsName;
                }
            }
            throw new Exception("No remote management address found.");
        }

        private static InstanceNetworkInterface GetManagementInterface(Instance instance, int? remoteManagementInterfaceIndex = null)
        {
            if(instance.NetworkInterfaces.Count > 0)
            {
                if(remoteManagementInterfaceIndex != null)
                {
                    return instance.NetworkInterfaces[remoteManagementInterfaceIndex.Value];
                }
            }
            return instance.NetworkInterfaces[0];
        }
    }
}