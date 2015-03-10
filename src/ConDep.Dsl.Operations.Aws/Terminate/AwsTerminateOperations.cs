namespace ConDep.Dsl.Terminate
{
    public class AwsTerminateOperations : IOfferAwsTerminateOperations
    {
        private readonly IOfferLocalOperations _local;

        public AwsTerminateOperations(IOfferLocalOperations local)
        {
            _local = local;
        }

        public IOfferLocalOperations LocalOperations
        {
            get { return _local; }
        }
    }
}