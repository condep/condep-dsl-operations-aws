namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapImageOptions
    {
        IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image);
        IOfferAwsBootstrapOptions WithId(string imageId);
    }

    class AwsBootstrapImageOptions : IOfferAwsBootstrapImageOptions
    {
        private readonly AwsBootstrapInputValues _values;
        private readonly IOfferAwsBootstrapOptions _bootstrapOptions;

        public AwsBootstrapImageOptions(AwsBootstrapInputValues values, IOfferAwsBootstrapOptions bootstrapOptions)
        {
            _values = values;
            _bootstrapOptions = bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image)
        {
            _values.Image.LatestImage = image;
            return _bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions WithId(string imageId)
        {
            _values.Image.Id = imageId;
            return _bootstrapOptions;
        }
    }
}