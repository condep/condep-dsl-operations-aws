using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public class AwsBootstrapInputValues
    {
        private readonly AwsBootstrapImageValues _image = new AwsBootstrapImageValues();
        public string InstanceType { get; set; }
        public int InstanceCountMin { get; set; }
        public int InstanceCountMax { get; set; }
        public AwsShutdownBehavior ShutdownBehavior { get; set; }
        public bool Monitor { get; set; }
        public string AvailabilityZone { get; set; }
        public string PrivatePrimaryIp { get; set; }
        public string SubnetId { get; set; }
        public List<string> SecurityGroupIds { get; set; }
        public AwsBootstrapImageValues Image {get { return _image; }}
        public RemoteManagementConnectionType? RemoteManagementConnectionType { get; set; }
    }

    public class AwsBootstrapImageValues
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