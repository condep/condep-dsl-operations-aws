using Amazon.EC2;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Terminate
{
    internal class Ec2Terminator
    {
        private readonly AwsBootstrapOptionsValues _options;
        private readonly IAmazonEC2 _client;
        private Ec2InstanceHandler _instanceHandler;

        public Ec2Terminator(AwsBootstrapOptionsValues options)
        {
            _options = options;
            var config = new AmazonEC2Config { RegionEndpoint = _options.RegionEndpoint };
            _client = new AmazonEC2Client(_options.Credentials, config);
            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public void Terminate()
        {
            _instanceHandler.Terminate(_options.InstanceRequest.ClientToken);
        }
    }
}