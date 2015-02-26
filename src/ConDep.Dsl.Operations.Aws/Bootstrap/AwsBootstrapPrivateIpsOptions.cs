using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl
{
    class AwsBootstrapPrivateIpsOptions : IOfferAwsBootstrapPrivateIpsOptions
    {
        private readonly List<PrivateIpAddressSpecification> _values;

        public AwsBootstrapPrivateIpsOptions(List<PrivateIpAddressSpecification> values)
        {
            _values = values;
        }

        public IOfferAwsBootstrapPrivateIpsOptions Add(string ip, bool isPrimary = false)
        {
            _values.Add(new PrivateIpAddressSpecification{PrivateIpAddress = ip, Primary = isPrimary});
            return this;
        }
    }
}