namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsEc2OperationsBuilder : IOfferAwsEc2Operations
    {
        private readonly IOfferAwsOperations _awsOps;

        public AwsEc2OperationsBuilder(IOfferAwsOperations awsOps)
        {
            _awsOps = awsOps;
        }

        public IOfferAwsOperations AwsOperations { get { return _awsOps; } }
    }
}