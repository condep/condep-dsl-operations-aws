using System;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Aws;

namespace ConDep.Dsl
{
    /// <summary>
    /// 
    /// </summary>
    public static class AwsExtensions
    {
        /// <summary>
        /// Provide operations for Amazon AWS
        /// </summary>
        /// <param name="local"></param>
        /// <param name="aws">The Amazon AWS operations</param>
        /// <returns></returns>
        public static IOfferLocalOperations Aws(this IOfferLocalOperations local, Action<IOfferAwsOperations> aws)
        {
            var builder = local as LocalOperationsBuilder;
            var opsBuilder = new AwsOperationsBuilder(local, builder.Settings, builder.Token);

            if (aws != null)
            {
                aws(opsBuilder);
            }
            return local;
        }
    }
}