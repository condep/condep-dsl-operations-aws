namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapImageOptions
    {
        IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image);
        IOfferAwsBootstrapOptions WithId(string imageId);
    }

    internal class AwsBootstrapImageOptions : IOfferAwsBootstrapImageOptions
    {
        private readonly AwsBootstrapImageValues _values;
        private readonly IOfferAwsBootstrapOptions _bootstrapOptions;

        public AwsBootstrapImageOptions(IOfferAwsBootstrapOptions bootstrapOptions)
        {
            _values = new AwsBootstrapImageValues();
            _bootstrapOptions = bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image)
        {
            _values.LatestImage = image;
            return _bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions WithId(string imageId)
        {
            _values.Id = imageId;
            return _bootstrapOptions;
        }

        public AwsBootstrapImageValues Values
        {
            get { return _values; }
        }
    }

    internal class AwsBootstrapImageValues
    {
        public AwsWindowsImage? LatestImage { get; set; }
        public string Id { get; set; }

        public bool HasImageId()
        {
            return !string.IsNullOrWhiteSpace(Id);
        }

        public bool HasLatestImageDefined()
        {
            return !HasImageId() && LatestImage.HasValue;
        }
    }

}