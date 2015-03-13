using System.Collections.Generic;
using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;

namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    internal class AwsBootstrapOptionsValues
    {
        private readonly RunInstancesRequest _request;
        private readonly List<KeyValuePair<string, string>> _tags = new List<KeyValuePair<string, string>>();
        private readonly AwsBootstrapImageValues _image = new AwsBootstrapImageValues();

        public AwsBootstrapOptionsValues(string bootstrapId)
        {
            _request = new RunInstancesRequest {ClientToken = bootstrapId, MinCount = 1, MaxCount = 1};
        }

        public RunInstancesRequest InstanceRequest
        {
            get { return _request; }
        }

        public AWSCredentials Credentials { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public AwsBootstrapImageValues Image { get { return _image; } }
        public RemoteManagementAddressType? ManagementAddressType { get; set; }
        public AwsBootstrapNetworkInterfaceOptionsValues NetworkInterfaceValues { get; set; }
        public List<KeyValuePair<string, string>> Tags { get { return _tags; } }
    }
}