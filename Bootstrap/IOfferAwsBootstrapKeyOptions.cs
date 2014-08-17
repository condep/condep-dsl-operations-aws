namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapKeyOptions
    {
        IOfferAwsBootstrapKeyOptions Public(string name);
        IOfferAwsBootstrapKeyOptions Private(string filePath);
    }
}