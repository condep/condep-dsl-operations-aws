using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsImageDescribeOptionsBuilder : IOfferAwsImageDescribeOptions
    {

        private readonly AwsImageDescribeOptionsValues _values = new AwsImageDescribeOptionsValues();
        private readonly IOfferAwsFilterOptions _filters;
        public AwsImageDescribeOptionsBuilder()
        {
            _filters = new AwsFiltersOptionsBuilder(_values.DescribeImagesRequest.Filters);
        }

        public IOfferAwsImageDescribeOptions ExceptNewest(int number)
        {
            _values.ExceptNewest = number;
            return this;
        }

        public IOfferAwsFilterOptions Filters()
        {
            return _filters;
        }

        internal AwsImageDescribeOptionsValues Values { get { return _values; } }
    }
}