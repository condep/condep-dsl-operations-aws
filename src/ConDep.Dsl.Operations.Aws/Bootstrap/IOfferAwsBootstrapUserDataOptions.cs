namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapUserDataOptions
    {
        IOfferAwsBootstrapOptions PowerShell(string script);
        IOfferAwsBootstrapOptions RunCmd(string script);
    }
}