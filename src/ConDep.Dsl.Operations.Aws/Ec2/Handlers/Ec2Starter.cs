using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    class Ec2Starter
    {
        private readonly AwsStartOptionsValues _options;
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;

        public Ec2Starter(AwsStartOptionsValues options)
        {
            _options = options;
            var config = new AmazonEC2Config { RegionEndpoint = _options.RegionEndpoint };
            _client = new AmazonEC2Client(_options.Credentials, config);
            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public IEnumerable<Instance> Start()
        {
            var bootstrapId = _options.InstanceRequest.ClientToken;
            IEnumerable<Instance> instances = _instanceHandler.GetInstances(bootstrapId);
            _instanceHandler.Start(bootstrapId);
            return instances;
        }
    }
}
