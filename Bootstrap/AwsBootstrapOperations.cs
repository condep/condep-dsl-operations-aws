namespace ConDep.Dsl.Operations.Aws.Bootstrap
{
    public class AwsBootstrapOperations : IOfferAwsBootstrapOperations
    {
        private readonly IOfferLocalOperations _local;

        public AwsBootstrapOperations(IOfferLocalOperations local)
        {
            _local = local;
        }

        public IOfferLocalOperations LocalOperations
        {
            get { return _local; }
        }
    }
}