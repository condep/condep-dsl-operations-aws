using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;

namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    class AwsImageCreateOptionsValues : IOfferAwsOperationsOptionValues
    {
        private readonly CreateImageRequest _request;

        public AwsImageCreateOptionsValues(string name)
        {
            _request = new CreateImageRequest
            {
                Name = name
            };
        }

        public AwsImageCreateOptionsValues(string instanceId, string name)
        {
            _request = new CreateImageRequest(instanceId, name);
        }

        public CreateImageRequest ImageRequest
        {
            get { return _request; }
        }
        
        public AWSCredentials Credentials { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
    }
}
