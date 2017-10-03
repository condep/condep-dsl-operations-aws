using System;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using Amazon.EC2;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    internal class Ec2ImageDeregisterer
    {
        private readonly AwsImageDescribeOptionsValues options;
        private readonly AwsImageDeregisterOptionsValues deregisterOptions;
        private readonly Ec2ImageHandler _imageHandler;
        private readonly IAmazonEC2 _client;

        public Ec2ImageDeregisterer(AwsImageDescribeOptionsValues options, AwsImageDeregisterOptionsValues deregisterOptions)
        {
            _client = new AmazonEC2Client(options.Credentials, options.RegionEndpoint);
            _imageHandler = new Ec2ImageHandler(_client);
            this.options = options;
            this.deregisterOptions = deregisterOptions;
        }

        internal void Deregister()
        {
            _imageHandler.Deregister(this.options, this.deregisterOptions);
        }
    }
}