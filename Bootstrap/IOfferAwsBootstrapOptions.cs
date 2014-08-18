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

    internal class AwsBootstrapOptions : IOfferAwsBootstrapOptions
    {
        private readonly AwsBootstrapInputValues _values = new AwsBootstrapInputValues();
        private readonly IOfferAwsBootstrapImageOptions _image;
        private readonly IOfferAwsBootstrapUserDataOptions _userData;
        private readonly IOfferAwsBootstrapNetworkInterfacesOptions _networkInterfaces;
        private readonly IOfferAwsBootstrapDisksOptions _disks;

        public AwsBootstrapOptions()
        {
            _image = new AwsBootstrapImageOptions(_values, this);
            _userData = new AwsBootstrapUserDataOptions(_values, this);
            _networkInterfaces = new AwsBootstrapNetworkInterfacesOptions(_values, this);
            _disks = new AwsBootstrapDisksOptions(_values, this);
        }

        public IOfferAwsBootstrapImageOptions Image { get { return _image; } }

        public IOfferAwsBootstrapUserDataOptions UserData { get { return _userData; } }

        public IOfferAwsBootstrapNetworkInterfacesOptions NetworkInterfaces { get { return _networkInterfaces; } }

        public IOfferAwsBootstrapDisksOptions Disks { get { return _disks; } }

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