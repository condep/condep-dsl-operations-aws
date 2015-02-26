using Amazon.Runtime;
using ConDep.Dsl.Operations.Application.Local;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Operations.Builders;

namespace ConDep.Dsl.Operations.Aws.Terminate
{
    public static class TerminateExtensions
    {
        public static IOfferAwsTerminateOperations Aws(this IOfferTerminateOperations terminate)
        {
            return new AwsTerminateOperations(((TerminateOperationsBuilder)terminate).LocalOperations);
        }

        public static IOfferLocalOperations VpcInstance(this IOfferAwsTerminateOperations terminate, AwsBootstrapMandatoryInputValues mandatoryInputValues)
        {
            var op = new AwsTerminateOperation(mandatoryInputValues);
            var local = ((AwsTerminateOperations) terminate).LocalOperations;
            Configure.Operation(local, op);
            return local;
        }
    }
}