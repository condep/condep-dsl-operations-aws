using Amazon.EC2.Model;
using BlockDeviceMapping = Amazon.AutoScaling.Model.BlockDeviceMapping;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapEbsOptions
    {
        IOfferAwsBootstrapEbsOptions DeleteOnTermination(bool delete);
        IOfferAwsBootstrapEbsOptions Encrypted(bool encrypted);
        IOfferAwsBootstrapEbsOptions Iops(int iops);
        IOfferAwsBootstrapEbsOptions SnapshotId(string id);
        IOfferAwsBootstrapEbsOptions VolumeSize(int sizeInGb);
        IOfferAwsBootstrapEbsOptions VolumeType(string type);
        IOfferAwsBootstrapEbsOptions VolumeType(AwsBootstrapVolumeType type);
    }

    class AwsBootstrapEbsOptions : IOfferAwsBootstrapEbsOptions
    {
        private readonly EbsBlockDevice _values;

        public AwsBootstrapEbsOptions(EbsBlockDevice values)
        {
            _values = values;
        }

        public IOfferAwsBootstrapEbsOptions DeleteOnTermination(bool delete)
        {
            _values.DeleteOnTermination = delete;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions Encrypted(bool encrypted)
        {
            _values.Encrypted = encrypted;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions Iops(int iops)
        {
            _values.Iops = iops;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions SnapshotId(string id)
        {
            _values.SnapshotId = id;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions VolumeSize(int size)
        {
            _values.VolumeSize = size;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions VolumeType(string type)
        {
            _values.VolumeType = type;
            return this;
        }

        public IOfferAwsBootstrapEbsOptions VolumeType(AwsBootstrapVolumeType type)
        {
            switch (type)
            {
                case AwsBootstrapVolumeType.GeneralPurpose_SSD:
                    _values.VolumeType = "gp2";
                    break;
                case AwsBootstrapVolumeType.Provisioned_IOPS_SSD:
                    _values.VolumeType = "io1";
                    break;
                case AwsBootstrapVolumeType.Standard:
                    _values.VolumeType = "standard";
                    break;
            }
            return this;
        }
    }
}