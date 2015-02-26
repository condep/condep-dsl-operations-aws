using System;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapNetworkInterfacesOptions
    {
        IOfferAwsBootstrapOptions Add(int index, string subnetId, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network);
        IOfferAwsBootstrapOptions Add(int index, string interfaceId);
    }
}