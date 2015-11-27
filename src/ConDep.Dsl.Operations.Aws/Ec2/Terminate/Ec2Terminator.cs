using Amazon.EC2;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Terminate
{
    public class Ec2Terminator
    {
        private readonly AwsTerminateOptionsValues _options;
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;

        public Ec2Terminator(AwsTerminateOptionsValues options)
        {
            _options = options;
            var config = new AmazonEC2Config { RegionEndpoint = _options.RegionEndpoint };
            _client = new AmazonEC2Client(_options.Credentials, config);
            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public void Terminate()
        {
            var bootstrapId = _options.InstanceRequest.ClientToken;
            _instanceHandler.Terminate(bootstrapId);
        }
    }
}