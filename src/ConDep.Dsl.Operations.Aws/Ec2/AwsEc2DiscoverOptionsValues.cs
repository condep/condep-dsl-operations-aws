using Amazon;
using Amazon.Runtime;

namespace ConDep.Dsl
{
    internal class AwsEc2DiscoverOptionsValues
    {
        public AWSCredentials Credentials { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public RemoteManagementAddressType? RemoteManagementAddressType { get; set; }
    }
}