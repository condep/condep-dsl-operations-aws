namespace ConDep.Dsl.Operations.Aws.Elb
{
    internal class AwsElbOperationsBuilder : IOfferAwsElbOperations
    {
        private readonly IOfferAwsOperations _awsOps;

        public AwsElbOperationsBuilder(IOfferAwsOperations awsOps)
        {
            _awsOps = awsOps;
        }

        public IOfferAwsOperations AwsOperations
        {
            get { return _awsOps; }
        }
    }
}