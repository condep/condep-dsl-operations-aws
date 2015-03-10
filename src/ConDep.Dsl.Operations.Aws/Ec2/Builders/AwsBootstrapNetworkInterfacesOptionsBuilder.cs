using System;
using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapNetworkInterfacesOptionsBuilder : IOfferAwsBootstrapNetworkInterfacesOptions
    {
        private readonly IOfferAwsBootstrapOptions _options;
        private readonly AwsBootstrapNetworkInterfaceOptionsValues _values;

        public AwsBootstrapNetworkInterfacesOptionsBuilder(AwsBootstrapNetworkInterfaceOptionsValues values, IOfferAwsBootstrapOptions options)
        {
            _values = values;
            _options = options;
        }

        public IOfferAwsBootstrapOptions Add(int index, string subnetId, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network)
        {
            var options = new AwsBootstrapNetworkInterfaceOptionsBuilder(index, subnetId);
            network(options);

            if (options.UseForRemoteManagement)
            {
                _values.RemoteManagementInterfaceIndex = index;
            }
            _values.NetworkInterfaces.Add(options.Values);
            return _options;
        }

        public IOfferAwsBootstrapOptions Add(int index, string interfaceId)
        {
            _values.NetworkInterfaces.Add(new InstanceNetworkInterfaceSpecification
            {
                DeviceIndex = index,
                NetworkInterfaceId = interfaceId
            });
            return _options;
        }
    }
}