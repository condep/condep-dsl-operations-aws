
using System.Collections.Generic;

namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapNetworkInterfaceOptions
    {
        IOfferAwsBootstrapNetworkInterfaceOptions AutoAssignPublicIp(bool assign);
        IOfferAwsBootstrapNetworkInterfaceOptions DeleteOnTermination(bool deleteOnTermination);
        IOfferAwsBootstrapNetworkInterfaceOptions Description(string description);
        IOfferAwsBootstrapNetworkInterfaceOptions SecurityGroups(params string[] securityGroup);
        IOfferAwsBootstrapPrivateIpsOptions PrivateIps { get; }
        IOfferAwsBootstrapNetworkInterfaceOptions NumberOfSecondaryPrivateIps(int count);
        IOfferAwsBootstrapNetworkInterfaceOptions SubnetId(string id);
    }

    class AwsBootstrapNetworkInterfaceOptions : IOfferAwsBootstrapNetworkInterfaceOptions
    {
        private readonly AwsNetworkInterfaceValues _values;
        private readonly IOfferAwsBootstrapPrivateIpsOptions _privateIps;

        public AwsBootstrapNetworkInterfaceOptions(int index)
        {
            _values = new AwsNetworkInterfaceValues(index);
            Index = index;
            _privateIps = new AwsBootstrapPrivateIpsOptions(_values);
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions AutoAssignPublicIp(bool assign)
        {
            _values.AutoAssignPublicIp = assign;
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
            _values.SecurityGroupIds = securityGroupId;
            return this;
        }

        public IOfferAwsBootstrapPrivateIpsOptions PrivateIps { get { return _privateIps; } }

        public IOfferAwsBootstrapNetworkInterfaceOptions NumberOfSecondaryPrivateIps(int count)
        {
            _values.NumberOfSecondaryPrivateIps = count;
            return this;
        }

        public IOfferAwsBootstrapNetworkInterfaceOptions SubnetId(string id)
        {
            _values.SubnetId = id;
            return this;
        }

        public AwsNetworkInterfaceValues Values { get { return _values; } }

        public int Index
        {
            get; private set; 
        }
    }

    internal class AwsNetworkInterfaceValues
    {
        public AwsNetworkInterfaceValues(int index)
        {
            Index = index;
        }

        public int Index { get; private set; }

        private readonly List<AwsPrivateIp> _privateIps = new List<AwsPrivateIp>();
 
        public bool AutoAssignPublicIp { get; set; }
        public bool DeleteOnTermination { get; set; }
        public string Description { get; set; }
        public int NumberOfSecondaryPrivateIps { get; set; }
        public string SubnetId { get; set; }
        public string[] SecurityGroupIds { get; set; }
        public List<AwsPrivateIp> PrivateIps { get { return _privateIps; } }
        public string InterfaceId { get; set; }
    }
}