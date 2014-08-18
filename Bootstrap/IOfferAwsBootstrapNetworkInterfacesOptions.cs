using System;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapNetworkInterfacesOptions
    {
        IOfferAwsBootstrapOptions Add(int index, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network);
        IOfferAwsBootstrapOptions Add(int index, string interfaceId);
    }

    internal class AwsBootstrapNetworkInterfacesOptions : IOfferAwsBootstrapNetworkInterfacesOptions
    {
        private readonly AwsBootstrapInputValues _values;
        private readonly IOfferAwsBootstrapOptions _options;

        public AwsBootstrapNetworkInterfacesOptions(AwsBootstrapInputValues values, IOfferAwsBootstrapOptions options)
        {
            _values = values;
            _options = options;
        }

        public IOfferAwsBootstrapOptions Add(int index, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network)
        {
            var options = new AwsBootstrapNetworkInterfaceOptions(index);
            network(options);

            _values.NetworkInterfaces.Add(options.Values);
            return _options;
        }

        public IOfferAwsBootstrapOptions Add(int index, string interfaceId)
        {
            _values.NetworkInterfaces.Add(new AwsNetworkInterfaceValues(index)
            {
                InterfaceId = interfaceId
            });
            return _options;
        }
    }
}