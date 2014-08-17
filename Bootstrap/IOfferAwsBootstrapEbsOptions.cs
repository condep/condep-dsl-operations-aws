namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapEbsOptions
    {
        IOfferAwsBootstrapEbsOptions DeleteOnTermination(bool delete);
        IOfferAwsBootstrapEbsOptions Encrypted(bool encrypted);
        IOfferAwsBootstrapEbsOptions Iops(int iops);
        IOfferAwsBootstrapEbsOptions SnapshotId(string id);
        IOfferAwsBootstrapEbsOptions VolumeSize(int size);
        IOfferAwsBootstrapEbsOptions VolumeType(string type);
        IOfferAwsBootstrapEbsOptions VolumeType(AwsBootstrapVolumeType type);
    }
}