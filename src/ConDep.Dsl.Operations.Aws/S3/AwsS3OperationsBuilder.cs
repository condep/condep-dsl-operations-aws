namespace ConDep.Dsl
{
    public class AwsS3OperationsBuilder : IOfferAwsS3Operations
    {
        private readonly IOfferAwsOperations _awsOps;

        public AwsS3OperationsBuilder(IOfferAwsOperations awsOps)
        {
            _awsOps = awsOps;
        }

        public IOfferAwsOperations AwsOperations
        {
            get { return _awsOps; }
        }
    }
}