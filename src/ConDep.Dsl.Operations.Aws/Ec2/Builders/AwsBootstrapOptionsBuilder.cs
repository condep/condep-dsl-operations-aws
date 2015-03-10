using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapOptionsValues
    {
        private readonly RunInstancesRequest _request;
        private readonly string _bootstrapId;

        public AwsBootstrapOptionsValues(RunInstancesRequest request)
        {
            _request = request;
        }

        public RunInstancesRequest InstanceRequest
        {
            get { return _request; }
        }

        public AWSCredentials Credentials { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public string SubnetId { get; set; }
        public string Region { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public AwsBootstrapImageValues Image { get; set; }
        public RemoteManagementAddressType? ManagementAddressType { get; set; }
        public AwsBootstrapNetworkInterfaceOptionsValues NetworkInterfaceValues { get; set; }
    }

    internal class AwsBootstrapOptionsBuilder : IOfferAwsBootstrapOptions
    {
        private readonly AwsBootstrapOptionsValues _values;
        private readonly IOfferAwsBootstrapImageOptions _image;
        private readonly IOfferAwsBootstrapUserDataOptions _userData;
        private readonly AwsBootstrapNetworkInterfacesOptionsBuilder _networkInterfaces;
        private readonly IOfferAwsBootstrapDisksOptions _disks;
        private AwsBootstrapNetworkInterfaceOptionsValues _networkInterfaceValues;

        public AwsBootstrapOptionsBuilder(string bootstrapId)
        {
            _values = new AwsBootstrapOptionsValues(new RunInstancesRequest());
            _values.InstanceRequest.ClientToken = bootstrapId;

            _values.Image = new AwsBootstrapImageValues();

            _image = new AwsBootstrapImageOptionsBuilder(_values.Image, this);
            _userData = new AwsBootstrapUserDataOptionsBuilder(_values.InstanceRequest, this);

            _values.NetworkInterfaceValues = new AwsBootstrapNetworkInterfaceOptionsValues(_values.InstanceRequest.NetworkInterfaces);

            _networkInterfaces = new AwsBootstrapNetworkInterfacesOptionsBuilder(_values.NetworkInterfaceValues, this);
            _disks = new AwsBootstrapDisksOptionsBuilder(_values.InstanceRequest.BlockDeviceMappings, this);
            InstanceCount(1, 1);
        }

        public IOfferAwsBootstrapOptions Credentials(string profileName)
        {
            _values.Credentials = new StoredProfileAWSCredentials(profileName);
            return this;
        }

        public IOfferAwsBootstrapOptions Credentials(string accessKey, string secretKey)
        {
            _values.Credentials = new BasicAWSCredentials(accessKey, secretKey);
            return this;
        }

        public IOfferAwsBootstrapOptions Region(string region)
        {
            _values.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
            return this;
        }
 
        public IOfferAwsBootstrapImageOptions Image { get { return _image; } }

        public IOfferAwsBootstrapUserDataOptions UserData { get { return _userData; } }

        public IOfferAwsBootstrapNetworkInterfacesOptions NetworkInterfaces { get { return _networkInterfaces; } }

        public IOfferAwsBootstrapDisksOptions Disks { get { return _disks; } }

        public IOfferAwsBootstrapOptions InstanceType(string instanceType)
        {
            _values.InstanceRequest.InstanceType = instanceType;
            return this;
        }

        public IOfferAwsBootstrapOptions InstanceType(AwsInstanceType instanceType)
        {
            _values.InstanceRequest.InstanceType = instanceType.ToString();
            return this;
        }

        public IOfferAwsBootstrapOptions InstanceCount(int min, int max)
        {
            _values.InstanceRequest.MinCount = min;
            _values.InstanceRequest.MaxCount = max;
            return this;
        }

        public IOfferAwsBootstrapOptions ShutdownBehavior(AwsShutdownBehavior behavior)
        {
            switch (behavior)
            {
                case AwsShutdownBehavior.Stop:
                    _values.InstanceRequest.InstanceInitiatedShutdownBehavior = new ShutdownBehavior("stop");
                    break;
                case AwsShutdownBehavior.Terminate:
                    _values.InstanceRequest.InstanceInitiatedShutdownBehavior = new ShutdownBehavior("terminate");
                    break;
            }
            return this;
        }

        public IOfferAwsBootstrapOptions Monitor(bool monitor)
        {
            _values.InstanceRequest.Monitoring = monitor;
            return this;
        }

        public IOfferAwsBootstrapOptions AvailabilityZone(string zone)
        {
            _values.InstanceRequest.Placement = new Placement
            {
                AvailabilityZone = zone,

            };
            return this;
        }

        public IOfferAwsBootstrapOptions PrivatePrimaryIp(string ip)
        {
            _values.InstanceRequest.PrivateIpAddress = ip;
            return this;
        }

        public IOfferAwsBootstrapOptions SubnetId(string subnetId)
        {
            _values.InstanceRequest.SubnetId = subnetId;
            return this;
        }

        public IOfferAwsBootstrapOptions SecurityGroupIds(params string[] ids)
        {
            _values.InstanceRequest.SecurityGroupIds = ids.ToList();
            return this;
        }

        public IOfferAwsBootstrapOptions KeyPair(string publicKeyName, string privateKeyFileLocation)
        {
            _values.InstanceRequest.KeyName = publicKeyName;
            _values.PrivateKeyFileLocation = privateKeyFileLocation;
            return this;
        }

        public AwsBootstrapOptionsValues Values
        {
            get { return _values; }
        }

        public IOfferAwsBootstrapOptions RemoteManagementAddressType(RemoteManagementAddressType type)
        {
            _values.ManagementAddressType = type;
            return this;
        }
    }

    internal class AwsBootstrapNetworkInterfaceOptionsValues
    {
        private readonly List<InstanceNetworkInterfaceSpecification> _networkInterfaces;

        public AwsBootstrapNetworkInterfaceOptionsValues(List<InstanceNetworkInterfaceSpecification> interfaces)
        {
            _networkInterfaces = interfaces;
        }

        public List<InstanceNetworkInterfaceSpecification> NetworkInterfaces { get { return _networkInterfaces; } }

        public int? RemoteManagementInterfaceIndex { get; set; }
    }
}