namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
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