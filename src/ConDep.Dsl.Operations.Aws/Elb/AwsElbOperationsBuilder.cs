using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Aws.Elb
{
    internal class AwsElbOperationsBuilder : LocalBuilder, IOfferAwsElbOperations
    {
        public AwsElbOperationsBuilder(IOfferAwsOperations awsOps, ConDepSettings settings, CancellationToken token) : base(settings, token)
        {
            AwsOperations = awsOps;
        }

        public IOfferAwsOperations AwsOperations { get; }

        public override IOfferLocalOperations Dsl => ((AwsOperationsBuilder) AwsOperations).Dsl;
    }
}