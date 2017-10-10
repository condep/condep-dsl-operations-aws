using ConDep.Dsl.Operations.Aws.Ec2;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using System;

namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapImageOptions
    {
        IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image);
        IOfferAwsBootstrapOptions WithId(string imageId);
        IOfferAwsBootstrapOptions LatestMatching(Action<IOfferAwsImageFilterOptions> filter);
    }
}