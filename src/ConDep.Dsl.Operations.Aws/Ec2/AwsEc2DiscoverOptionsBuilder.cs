using Amazon;
using Amazon.Runtime;

namespace ConDep.Dsl.Operations.Aws.Ec2
{
    internal class AwsEc2DiscoverOptionsBuilder : IOfferAwsEc2DiscoverOptions
    {
        private AwsEc2DiscoverOptionsValues _values = new AwsEc2DiscoverOptionsValues();
        internal AwsEc2DiscoverOptionsValues Values { get { return _values; } }

        public IOfferAwsEc2DiscoverOptions Credentials(string profileName)
        {
            _values.Credentials = new StoredProfileAWSCredentials(profileName);
            return this;
        }

        public IOfferAwsEc2DiscoverOptions Credentials(string accessKey, string secretKey)
        {
            _values.Credentials = new BasicAWSCredentials(accessKey, secretKey);
            return this;
        }

        public IOfferAwsEc2DiscoverOptions Region(string region)
        {
            _values.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
            return this;
        }

        public IOfferAwsEc2DiscoverOptions RemoteManagementAddressType(RemoteManagementAddressType managementType)
        {
            _values.RemoteManagementAddressType = managementType;
            return this;
        }

        //public IOfferAwsBootstrapOptions KeyPair(string publicKeyName, string privateKeyFileLocation)
        //{
        //    _values.InstanceRequest.KeyName = publicKeyName;
        //    _values.PrivateKeyFileLocation = privateKeyFileLocation;
        //    return this;
        //}
    }
}