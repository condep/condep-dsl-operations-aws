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
        

        public AwsOperationsBuilder(IOfferLocalOperations localOps)
        {
            _localOps = localOps;
            _ec2 = new AwsEc2OperationsBuilder(this);
            _elb = new AwsElbOperationsBuilder(this);
        }

        public IOfferAwsEc2Operations Ec2 { get { return _ec2; } }
        public IOfferAwsVpcOperations Vpc { get { return _vpc; } }
        public IOfferAwsElbOperations Elb { get { return _elb; } }

        public IOfferLocalOperations LocalOperations { get { return _localOps; } }
    }
}