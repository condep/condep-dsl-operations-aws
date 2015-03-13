using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    internal class AwsBootstrapNetworkInterfaceOptionsValues
    {
        private readonly List<InstanceNetworkInterfaceSpecification> _networkInterfaces;

        public AwsBootstrapNetworkInterfaceOptionsValues(List<InstanceNetworkInterfaceSpecification> interfaces)
        {
            _networkInterfaces = interfaces;
        }

        public List<InstanceNetworkInterfaceSpecification> NetworkInterfaces { get { return _networkInterfaces; } }

        public int? RemoteManagementInterfaceIndex { get; set; }
    }
}