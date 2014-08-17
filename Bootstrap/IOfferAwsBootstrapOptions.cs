using System.Linq;

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
        IOfferAwsBootstrapOptions RemoteManagementConnectionType(RemoteManagementConnectionType address);
    }

    public enum RemoteManagementConnectionType
    {
        PublicDns,
        PublicIp,
        PrivateDns,
        PrivateIp
    }

    public class AwsBootstrapOptions : IOfferAwsBootstrapOptions
    {
        private readonly AwsBootstrapInputValues _values = new AwsBootstrapInputValues();

        public IOfferAwsBootstrapImageOptions Image { get; private set; }

        public IOfferAwsBootstrapUserDataOptions UserData { get; private set; }

        public IOfferAwsBootstrapNetworkInterfacesOptions NetworkInterfaces { get; private set; }

        public IOfferAwsBootstrapDisksOptions Disks { get; private set; }

        public IOfferAwsBootstrapOptions InstanceType(string instanceType)
        {
            _values.InstanceType = instanceType;
            return this;
        }

        public IOfferAwsBootstrapOptions InstanceCount(int min, int max)
        {
            _values.InstanceCountMin = min;
            _values.InstanceCountMax = max;
            return this;
        }

        public IOfferAwsBootstrapOptions ShutdownBehavior(AwsShutdownBehavior behavior)
        {
            _values.ShutdownBehavior = behavior;
            return this;
        }

        public IOfferAwsBootstrapOptions Monitor(bool monitor)
        {
            _values.Monitor = monitor;
            return this;
        }

        public IOfferAwsBootstrapOptions AvailabilityZone(string zone)
        {
            _values.AvailabilityZone = zone;
            return this;
        }

        public IOfferAwsBootstrapOptions PrivatePrimaryIp(string ip)
        {
            _values.PrivatePrimaryIp = ip;
            return this;
        }

        public IOfferAwsBootstrapOptions SubnetId(string subnetId)
        {
            _values.SubnetId = subnetId;
            return this;
        }

        public IOfferAwsBootstrapOptions SecurityGroupIds(params string[] ids)
        {
            _values.SecurityGroupIds = ids.ToList();
            return this;
        }

        public IOfferAwsBootstrapOptions RemoteManagementConnectionType(RemoteManagementConnectionType address)
        {
            _values.RemoteManagementConnectionType = address;
            return this;
        }

        public AwsBootstrapInputValues Values
        {
            get { return _values; }
        }
    }
}