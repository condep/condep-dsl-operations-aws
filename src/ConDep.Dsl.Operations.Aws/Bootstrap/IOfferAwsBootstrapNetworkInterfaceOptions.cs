
using System.Collections.Generic;
using System.Linq;
using Amazon.EC2.Model;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;

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

    class AwsBootstrapNetworkInterfaceOptions : IOfferAwsBootstrapNetworkInterfaceOptions
    {
        private readonly InstanceNetworkInterfaceSpecification _values;
        private readonly IOfferAwsBootstrapPrivateIpsOptions _privateIps;

        public AwsBootstrapNetworkInterfaceOptions(int index, string subnetId)
        {
            _values = new InstanceNetworkInterfaceSpecification {DeviceIndex = index, SubnetId = subnetId};
            _privateIps = new AwsBootstrapPrivateIpsOptions(_values.PrivateIpAddresses);
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions AutoAssignPublicIp(bool assign)
        {
            _values.AssociatePublicIpAddress = assign;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions DeleteOnTermination(bool deleteOnTermination)
        {
            _values.DeleteOnTermination = deleteOnTermination;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions Description(string description)
        {
            _values.Description = description;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions SecurityGroups(params string[] securityGroupId)
        {
            _values.Groups.AddRange(securityGroupId);
            return this;
        }

        public IOfferAwsBootstrapPrivateIpsOptions PrivateIps { get { return _privateIps; } }

        public IOfferAwsBootstrapNetworkInterfaceOptions PrivateIp(string ip)
        {
            _values.PrivateIpAddress = ip;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions NumberOfSecondaryPrivateIps(int count)
        {
            _values.SecondaryPrivateIpAddressCount = count;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions UseAsRemoteManagementInterface(bool use)
        {
            UseForRemoteManagement = use;
            return this;
        }

        internal bool UseForRemoteManagement { get; private set; }

        public IOfferAwsBootstrapNetworkInterfaceOptions SubnetId(string id)
        {
            _values.SubnetId = id;
            return this;
        }

        public InstanceNetworkInterfaceSpecification Values { get { return _values; } }
    }
}