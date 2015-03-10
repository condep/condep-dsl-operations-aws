using System;
using ConDep.Dsl.Operations.Aws;

namespace ConDep.Dsl
{
    public static class AwsExtensions
    {
        public static IOfferLocalOperations Aws(this IOfferLocalOperations local, Action<IOfferAwsOperations> aws)
        {
            var opsBuilder = new AwsOperationsBuilder(local);

            if (aws != null)
            {
                aws(opsBuilder);
            }
            return local;
        }
    }
}