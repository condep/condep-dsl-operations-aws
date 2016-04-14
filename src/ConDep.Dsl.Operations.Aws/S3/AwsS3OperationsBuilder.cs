using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws;

namespace ConDep.Dsl
{
    internal class AwsS3OperationsBuilder : LocalBuilder, IOfferAwsS3Operations
    {
        public AwsS3OperationsBuilder(IOfferAwsOperations awsOps, ConDepSettings settings, CancellationToken token) : base(settings, token)
        {
            AwsOperations = awsOps;
        }

        public IOfferAwsOperations AwsOperations { get; }

        public override IOfferLocalOperations Dsl => ((AwsOperationsBuilder) AwsOperations).Dsl;
    }
}