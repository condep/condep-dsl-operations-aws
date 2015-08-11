using System;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.Elb;

namespace ConDep.Dsl
{
    public static class AwsElbExtensions
    {
        public static IOfferAwsOperations CreateLoadBalancer(this IOfferAwsElbOperations elb, string name, Action<IOfferAwsElbOptions> options = null)
        {
            var elbBuilder = elb as AwsElbOperationsBuilder;
            var localBuilder = ((AwsOperationsBuilder) elbBuilder.AwsOperations).LocalOperations;

            var builder = new AwsElbOptionsBuilder(name);
            if (options != null)
            {
                options(builder);
            }

            var op = new AwsElbOperation(builder.Values);
            Configure.Operation(localBuilder, op);
            return elbBuilder.AwsOperations;
        }
    }
}