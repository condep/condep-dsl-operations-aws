using Amazon.EC2;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System;
using System.Net;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    class Ec2ImageCreator
    {
        private readonly AwsImageCreateOptionsValues _options;
        private readonly Ec2ImageHandler _imageHandler;
        private readonly IAmazonEC2 _client;

        public Ec2ImageCreator(AwsImageCreateOptionsValues options)
        {
            _client = new AmazonEC2Client(options.Credentials, options.RegionEndpoint);
            _options = options;
            
            _imageHandler = new Ec2ImageHandler(_client);
        }

        internal string Create()
        {
            return _imageHandler.Create(_options);
        }
    }
}
