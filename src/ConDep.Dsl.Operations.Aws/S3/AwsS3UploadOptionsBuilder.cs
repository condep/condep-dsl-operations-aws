namespace ConDep.Dsl
{
    class AwsS3UploadOptionsBuilder : IOfferAwsS3UploadOptions
    {
        AwsS3UploadOptionsBuilderValues _values = new AwsS3UploadOptionsBuilderValues();

        public IOfferAwsS3UploadOptions TargetFolderPath(string path)
        {
            _values.TargetFolderPath = path;
            return this;
        }

        internal class AwsS3UploadOptionsBuilderValues
        {
            public string TargetFolderPath { get; set; }
        }

        public AwsS3UploadOptionsBuilderValues Values { get { return _values; } }
    }
}