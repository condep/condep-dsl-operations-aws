using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsEc2OperationsBuilder : LocalBuilder, IOfferAwsEc2Operations
    {
        public AwsEc2OperationsBuilder(IOfferAwsOperations awsOps, ConDepSettings settings, CancellationToken token) : base(settings, token)
        {
            AwsOperations = awsOps;
        }

        public IOfferAwsOperations AwsOperations { get; private set; }

        public override IOfferLocalOperations Dsl
        {
            get { return ((AwsOperationsBuilder) AwsOperations).LocalOperations; }
        }
    }
}