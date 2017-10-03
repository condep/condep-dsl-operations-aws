namespace ConDep.Dsl
{
    public interface IOfferAwsImageDescribeOptions
    {
        IOfferAwsFilterOptions Filters();
        IOfferAwsImageDescribeOptions ExceptNewest(int number);
    }
}