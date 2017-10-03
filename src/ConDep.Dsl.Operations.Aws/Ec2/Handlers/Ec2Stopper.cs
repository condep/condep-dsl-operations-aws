using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    public class Ec2Stopper
    {
        private readonly AwsStopOptionsValues _options;
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;

        public Ec2Stopper(AwsStopOptionsValues options)
        {
            _options = options;
            var config = new AmazonEC2Config { RegionEndpoint = _options.RegionEndpoint };
            _client = new AmazonEC2Client(_options.Credentials, config);
            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public IEnumerable<Instance> Stop()
        {
            var bootstrapId = _options.InstanceRequest.ClientToken;
            IEnumerable<Instance> instances = _instanceHandler.GetInstances(bootstrapId);
            _instanceHandler.Stop(bootstrapId);
            return instances;
        }
    }
}
