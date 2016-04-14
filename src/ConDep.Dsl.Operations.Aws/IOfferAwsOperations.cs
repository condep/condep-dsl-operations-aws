namespace ConDep.Dsl
{
    public interface IOfferAwsOperations
    {
        IOfferAwsEc2Operations Ec2 { get; }
        IOfferAwsElbOperations Elb { get; }
        IOfferAwsS3Operations S3 { get; }
    }
}