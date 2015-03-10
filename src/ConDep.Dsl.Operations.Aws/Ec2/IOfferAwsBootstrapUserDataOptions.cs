namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapUserDataOptions
    {
        IOfferAwsBootstrapOptions PowerShell(string script);
        IOfferAwsBootstrapOptions RunCmd(string script);
    }
}