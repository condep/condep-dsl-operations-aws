using System;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapDisksOptions
    {
        IOfferAwsBootstrapOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null);
        IOfferAwsBootstrapOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null);
    }

    class AwsBootstrapDisksOptions : IOfferAwsBootstrapDisksOptions
    {
        private readonly AwsBootstrapInputValues _values;
        private readonly AwsBootstrapOptions _options;

        public AwsBootstrapDisksOptions(AwsBootstrapInputValues values, AwsBootstrapOptions options)
        {
            _values = values;
            _options = options;
        }

        public IOfferAwsBootstrapOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null)
        {
            _values.Disks.Add(new AwsDisk
            {
                DeviceType = AwsDiskType.InstanceStoreVolume,
                DeviceName = deviceName,
                VirtualName = virtualName,
                DeviceToSupressFromImage = deviceToSuppressFromImage
            });
            return _options;
        }

        public IOfferAwsBootstrapOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null)
        {
            var ebsData = new AwsEbsDiskData();
            var ebsValues = new AwsBootstrapEbsOptions(ebsData);
            ebs(ebsValues);

            _values.Disks.Add(new AwsDisk
            {
                DeviceType = AwsDiskType.Ebs,
                DeviceName = deviceName,
                DeviceToSupressFromImage = deviceToSuppressFromImage,
                Ebs = ebsData
            });
            return _options;
        }
    }

    internal class AwsEbsDiskData
    {
        public bool DeleteOnTermination { get; set; }
        public bool Encrypted { get; set; }
        public int Iops { get; set; }
        public string SnapshotId { get; set; }
        public int VolumeSize { get; set; }
        public string VolumeType { get; set; }
    }

    internal enum AwsDiskType
    {
        Ebs,
        InstanceStoreVolume
    }

    internal class AwsDisk
    {
        public string DeviceName { get; set; }
        public string VirtualName { get; set; }
        public string DeviceToSupressFromImage { get; set; }
        public AwsDiskType DeviceType { get; set; }
        public AwsEbsDiskData Ebs { get; set; }
    }
}