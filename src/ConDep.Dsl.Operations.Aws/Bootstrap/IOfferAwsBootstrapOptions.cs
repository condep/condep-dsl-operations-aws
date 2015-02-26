namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapOptions
    {
        IOfferAwsBootstrapImageOptions Image { get; }
        IOfferAwsBootstrapOptions InstanceType(string instanceType);
        IOfferAwsBootstrapOptions InstanceCount(int min, int max);
        IOfferAwsBootstrapUserDataOptions UserData { get; }
        IOfferAwsBootstrapOptions ShutdownBehavior(AwsShutdownBehavior behavior);
        IOfferAwsBootstrapOptions Monitor(bool monitor);
        IOfferAwsBootstrapOptions AvailabilityZone(string zone);
        IOfferAwsBootstrapOptions PrivatePrimaryIp(string ip);
        IOfferAwsBootstrapOptions SubnetId(string subnetId);
        IOfferAwsBootstrapNetworkInterfacesOptions NetworkInterfaces { get; }
        IOfferAwsBootstrapDisksOptions Disks { get; }
        IOfferAwsBootstrapOptions SecurityGroupIds(params string[] ids);
        IOfferAwsBootstrapOptions RemoteManagementAddressType(RemoteManagementAddressType type);
    }
}