using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    internal class AwsBootstrapInputValues
    {
        private readonly AwsBootstrapImageValues _image = new AwsBootstrapImageValues();
        private readonly List<AwsDisk> _disks = new List<AwsDisk>();

        public AwsBootstrapInputValues()
        {
            InstanceType = "t2.micro";
            InstanceCountMin = 1;
            InstanceCountMax = 1;
        }

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
        public string UserData { get; set; }
        public List<AwsDisk> Disks { get { return _disks; } }
        public List<AwsNetworkInterfaceValues> NetworkInterfaces { get; set; }
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