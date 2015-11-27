namespace ConDep.Dsl
{
    public interface IOfferAwsEc2DiscoverOptions
    {
        IOfferAwsEc2DiscoverOptions Credentials(string profileName);
        IOfferAwsEc2DiscoverOptions Credentials(string accessKey, string secretKey);
        IOfferAwsEc2DiscoverOptions Region(string region);
        IOfferAwsEc2DiscoverOptions RemoteManagementAddressType(RemoteManagementAddressType managementType);
    }
}