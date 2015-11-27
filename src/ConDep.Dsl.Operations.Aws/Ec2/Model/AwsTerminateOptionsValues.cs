using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;

namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    public class AwsTerminateOptionsValues
    {
        private readonly RunInstancesRequest _request;        
        private readonly AwsEc2IdempotencyType _idempotencyType;

        public AwsTerminateOptionsValues(string bootstrapId)
        {
            _idempotencyType = AwsEc2IdempotencyType.ClientToken;
            _request = new RunInstancesRequest { ClientToken = bootstrapId, MinCount = 1, MaxCount = 1 };
        }

        public RunInstancesRequest InstanceRequest
        {
            get { return _request; }
        }

        public AWSCredentials Credentials { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
    }
}