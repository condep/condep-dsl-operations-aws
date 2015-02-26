using System;
using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    internal class AwsBootstrapNetworkInterfacesOptions : IOfferAwsBootstrapNetworkInterfacesOptions
    {
        private readonly List<InstanceNetworkInterfaceSpecification> _values;
        private readonly IOfferAwsBootstrapOptions _options;

        public AwsBootstrapNetworkInterfacesOptions(List<InstanceNetworkInterfaceSpecification> values, IOfferAwsBootstrapOptions options)
        {
            _values = values;
            _options = options;
        }

        public IOfferAwsBootstrapOptions Add(int index, string subnetId, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network)
        {
            var options = new AwsBootstrapNetworkInterfaceOptions(index, subnetId);
            network(options);

            if (options.UseForRemoteManagement)
            {
                RemoteManagementInterfaceIndex = index;
            }
            _values.Add(options.Values);
            return _options;
        }

        public IOfferAwsBootstrapOptions Add(int index, string interfaceId)
        {
            _values.Add(new InstanceNetworkInterfaceSpecification
            {
                DeviceIndex = index,
                NetworkInterfaceId = interfaceId
            });
            return _options;
        }

        internal int? RemoteManagementInterfaceIndex { get; private set; }
    }
}