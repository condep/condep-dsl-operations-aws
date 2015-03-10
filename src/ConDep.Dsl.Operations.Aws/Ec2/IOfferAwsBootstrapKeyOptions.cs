namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapKeyOptions
    {
        IOfferAwsBootstrapKeyOptions Public(string name);
        IOfferAwsBootstrapKeyOptions Private(string filePath);
    }
}