using System;

namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapDisksOptions
    {
        IOfferAwsBootstrapOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null);
        IOfferAwsBootstrapOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null);
    }
}