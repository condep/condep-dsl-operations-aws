using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Elb;

namespace ConDep.Dsl.Operations.Aws
{
    internal class AwsOperationsBuilder : LocalBuilder, IOfferAwsOperations
    {
        public AwsOperationsBuilder(IOfferLocalOperations localOps, ConDepSettings settings, CancellationToken token) : base(settings, token)
        {
            LocalOperations = localOps;
            Ec2 = new AwsEc2OperationsBuilder(this, settings, token);
            Elb = new AwsElbOperationsBuilder(this, settings, token);
            S3 = new AwsS3OperationsBuilder(this, settings, token);
        }


        //public AwsOperationsBuilder(IOfferLocalOperations localOps)
        //{
        //    _localOps = localOps;
        //    _ec2 = new AwsEc2OperationsBuilder(this);
        //    _elb = new AwsElbOperationsBuilder(this);
        //    _s3 = new AwsS3OperationsBuilder(this);
        //}

        public IOfferAwsEc2Operations Ec2 { get; private set; }

        public IOfferAwsElbOperations Elb { get; private set; }

        public IOfferAwsS3Operations S3 { get; private set; }

        public IOfferLocalOperations LocalOperations { get; private set; }

        public override IOfferLocalOperations Dsl
        {
            get { return ((LocalOperationsBuilder) LocalOperations).Dsl; }
        }
    }
}