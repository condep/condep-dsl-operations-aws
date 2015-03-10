namespace ConDep.Dsl
{
    public interface IOfferAwsBootstrapImageOptions
    {
        IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image);
        IOfferAwsBootstrapOptions WithId(string imageId);
    }
}