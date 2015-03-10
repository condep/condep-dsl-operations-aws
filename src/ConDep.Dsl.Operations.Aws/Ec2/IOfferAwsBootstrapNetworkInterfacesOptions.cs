using System;

namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapNetworkInterfacesOptions
    {
        IOfferAwsBootstrapOptions Add(int index, string subnetId, Action<IOfferAwsBootstrapNetworkInterfaceOptions> network);
        IOfferAwsBootstrapOptions Add(int index, string interfaceId);
    }
}