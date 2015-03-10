
namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapNetworkInterfaceOptions
    {
        IOfferAwsBootstrapNetworkInterfaceOptions AutoAssignPublicIp(bool assign);
        IOfferAwsBootstrapNetworkInterfaceOptions DeleteOnTermination(bool deleteOnTermination);
        IOfferAwsBootstrapNetworkInterfaceOptions Description(string description);
        IOfferAwsBootstrapNetworkInterfaceOptions SecurityGroups(params string[] securityGroup);
        IOfferAwsBootstrapPrivateIpsOptions PrivateIps { get; }
        IOfferAwsBootstrapNetworkInterfaceOptions PrivateIp(string ip);
        IOfferAwsBootstrapNetworkInterfaceOptions NumberOfSecondaryPrivateIps(int count);
        IOfferAwsBootstrapNetworkInterfaceOptions UseAsRemoteManagementInterface(bool use);
    }
}