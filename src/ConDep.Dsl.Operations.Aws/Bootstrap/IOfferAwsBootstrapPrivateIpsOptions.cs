namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapPrivateIpsOptions
    {
        IOfferAwsBootstrapPrivateIpsOptions Add(string ip, bool isPrimary = false);
    }
}