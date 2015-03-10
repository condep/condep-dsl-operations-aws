namespace ConDep.Dsl
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
}