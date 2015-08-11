using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Elb;

namespace ConDep.Dsl.Operations.Aws
{
    internal class AwsOperationsBuilder : IOfferAwsOperations
    {
        private readonly IOfferLocalOperations _localOps;
        private readonly IOfferAwsEc2Operations _ec2;
        private readonly IOfferAwsVpcOperations _vpc;
        private readonly IOfferAwsElbOperations _elb;
        private readonly IOfferAwsS3Operations _s3;
        

        public AwsOperationsBuilder(IOfferLocalOperations localOps)
        {
            _localOps = localOps;
            _ec2 = new AwsEc2OperationsBuilder(this);
            _elb = new AwsElbOperationsBuilder(this);
            _s3 = new AwsS3OperationsBuilder(this);
        }

        public IOfferAwsEc2Operations Ec2 { get { return _ec2; } }
        public IOfferAwsVpcOperations Vpc { get { return _vpc; } }
        public IOfferAwsElbOperations Elb { get { return _elb; } }
        public IOfferAwsS3Operations S3 { get { return _s3; } }

        public IOfferLocalOperations LocalOperations { get { return _localOps; } }
    }
}