
namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapNetworkInterfaceOptions
    {
        IOfferAwsBootstrapNetworkInterfaceOptions AutoAssignPublicIp(bool assign);
        IOfferAwsBootstrapNetworkInterfaceOptions DeleteOnTermination(bool deleteOnTermination);
        IOfferAwsBootstrapNetworkInterfaceOptions Description(string description);
        IOfferAwsBootstrapSecurityGroupsOptions SecurityGroups { get; }
        IOfferAwsBootstrapPrivateIpsOptions PrivateIps { get; }
        IOfferAwsBootstrapNetworkInterfaceOptions NumberOfSecondaryPrivateIps(int count);
        IOfferAwsBootstrapNetworkInterfaceOptions SubnetId(string id);
    }
}