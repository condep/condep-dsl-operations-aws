using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl
{
    internal class AwsBootstrapImageValues
    {
        internal string FilterByOwner = null;

        public AwsWindowsImage? LatestImage { get; set; }
        public string Id { get; set; }
        public List<Filter> Filters = new List<Filter>();

        public bool HasImageId()
        {
            return !string.IsNullOrWhiteSpace(Id);
        }

        public bool HasImageFilter()
        {
            return Filters.Count > 0;
        }

        public bool HasLatestImageDefined()
        {
            return !HasImageId() && LatestImage.HasValue;
        }
    }
}