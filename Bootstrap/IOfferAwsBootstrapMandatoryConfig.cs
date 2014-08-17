using System;
using System.IO;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapMandatoryConfig
    {
        IOfferAwsBootstrapMandatoryConfig Credentials(string profileName);
        IOfferAwsBootstrapMandatoryConfig Credentials(string accessKey, string secretKey);
        IOfferAwsBootstrapMandatoryConfig KeyPair(string publicKeyName, string privateKeyFileLocation);
    }
}