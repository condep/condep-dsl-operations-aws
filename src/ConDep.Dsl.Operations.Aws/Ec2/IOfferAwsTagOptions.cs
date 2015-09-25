namespace ConDep.Dsl
{
    public interface IOfferAwsTagOptions
    {
        IOfferAwsTagOptions Add(string name, string value);
        IOfferAwsTagOptions Add(string name);
        IOfferAwsTagOptions AddName(string value);
    }
}