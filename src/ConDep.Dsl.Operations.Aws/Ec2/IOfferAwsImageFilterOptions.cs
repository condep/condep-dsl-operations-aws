namespace ConDep.Dsl.Operations.Aws.Ec2
{
    public interface IOfferAwsImageFilterOptions : IOfferAwsFilterOptions
    {
        void Owner(string owner);
    }
}