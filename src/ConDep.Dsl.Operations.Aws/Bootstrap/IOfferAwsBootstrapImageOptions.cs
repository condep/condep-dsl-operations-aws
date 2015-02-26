namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapImageOptions
    {
        IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image);
        IOfferAwsBootstrapOptions WithId(string imageId);
    }
}