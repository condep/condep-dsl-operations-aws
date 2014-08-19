using System.Linq;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public enum RemoteManagementAddressType
    {
        PublicDns,
        PublicIp,
        PrivateDns,
        PrivateIp
    }

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

    internal class AwsBootstrapOptions : IOfferAwsBootstrapOptions
    {
        private readonly RunInstancesRequest _values = new RunInstancesRequest();
        private readonly IOfferAwsBootstrapImageOptions _image;
        private readonly IOfferAwsBootstrapUserDataOptions _userData;
        private readonly AwsBootstrapNetworkInterfacesOptions _networkInterfaces;
        private readonly IOfferAwsBootstrapDisksOptions _disks;

        public AwsBootstrapOptions()
        {
            _image = new AwsBootstrapImageOptions(this);
            _userData = new AwsBootstrapUserDataOptions(_values, this);
            _networkInterfaces = new AwsBootstrapNetworkInterfacesOptions(_values.NetworkInterfaces, this);
            _disks = new AwsBootstrapDisksOptions(_values.BlockDeviceMappings, this);
            InstanceCount(1, 1);
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
            _values.MinCount = min;
            _values.MaxCount = max;
            return this;
        }

        public IOfferAwsBootstrapOptions ShutdownBehavior(AwsShutdownBehavior behavior)
        {
            switch (behavior)
            {
                case AwsShutdownBehavior.Stop:
                    _values.InstanceInitiatedShutdownBehavior = new ShutdownBehavior("stop");
                    break;
                case AwsShutdownBehavior.Terminate:
                    _values.InstanceInitiatedShutdownBehavior = new ShutdownBehavior("terminate");
                    break;
            }
            return this;
        }

        public IOfferAwsBootstrapOptions Monitor(bool monitor)
        {
            _values.Monitoring = monitor;
            return this;
        }

        public IOfferAwsBootstrapOptions AvailabilityZone(string zone)
        {
            _values.Placement = new Placement
            {
                AvailabilityZone = zone,

            };
            return this;
        }

        public IOfferAwsBootstrapOptions PrivatePrimaryIp(string ip)
        {
            _values.PrivateIpAddress = ip;
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

        public RunInstancesRequest Values
        {
            get { return _values; }
        }

        public IOfferAwsBootstrapOptions RemoteManagementAddressType(RemoteManagementAddressType type)
        {
            ManagementAddressType = type;
            return this;
        }

        internal RemoteManagementAddressType? ManagementAddressType { get; private set; }

        internal int? ManagementInterfaceIndex
        {
            get { return _networkInterfaces.RemoteManagementInterfaceIndex; }
        }

    }
}