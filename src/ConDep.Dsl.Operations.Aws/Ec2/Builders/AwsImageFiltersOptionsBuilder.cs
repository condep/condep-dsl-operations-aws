using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    class AwsImageFiltersOptionsBuilder : AwsFiltersOptionsBuilder, IOfferAwsImageFilterOptions
    {
        private AwsBootstrapImageValues _values;

        public AwsImageFiltersOptionsBuilder(AwsBootstrapImageValues values) : base(values.Filters)
        {
            this._values = values;
        }
        
        public void Owner(string owner)
        {
            _values.FilterByOwner = owner;
        }
    }
}
