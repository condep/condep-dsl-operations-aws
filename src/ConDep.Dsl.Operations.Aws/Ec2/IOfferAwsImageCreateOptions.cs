namespace ConDep.Dsl.Operations.Aws.Ec2
{
    public interface IOfferAwsImageCreateOptions
    {
        IOfferAwsImageCreateOptions Description(string description);
        IOfferAwsImageCreateOptions NoReboot(bool noReboot);
        IOfferAwsImageCreateDisksOptions Disks { get; }
    }
}