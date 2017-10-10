using System;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapImageOptionsBuilder : IOfferAwsBootstrapImageOptions
    {
        private readonly IOfferAwsBootstrapOptions _bootstrapOptions;
        private readonly AwsBootstrapImageValues _values;

        public AwsBootstrapImageOptionsBuilder(AwsBootstrapImageValues imageValues, IOfferAwsBootstrapOptions bootstrapOptions)
        {
            _values = imageValues;
            _bootstrapOptions = bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions LatestBaseWindowsImage(AwsWindowsImage image)
        {
            _values.LatestImage = image;
            return _bootstrapOptions;
        }

        public IOfferAwsBootstrapOptions LatestMatching(Action<IOfferAwsImageFilterOptions> filter)
        {
            var searchFilter = new AwsImageFiltersOptionsBuilder(_values);
            filter(searchFilter);
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
}