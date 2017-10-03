using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;

namespace ConDep.Dsl.Operations.Aws.Ec2.Model
{
    public class AwsImageDescribeOptionsValues : IOfferAwsOperationsOptionValues
    {
        private readonly DescribeImagesRequest _request;

        public AwsImageDescribeOptionsValues()
        {
            _request = new DescribeImagesRequest();
            ExceptNewest = 0;
        }

        public DescribeImagesRequest DescribeImagesRequest {
            get { return _request;  }
        }
        public int ExceptNewest { get; internal set; }
        public string PrivateKeyFileLocation { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public AWSCredentials Credentials { get; set; }
    }
}