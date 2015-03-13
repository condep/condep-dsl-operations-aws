namespace ConDep.Dsl
{
    public interface IOfferAwsTagOptions
    {
        IOfferAwsBootstrapOptions Add(string name, string value);
        IOfferAwsBootstrapOptions AddName(string value);
    }
}