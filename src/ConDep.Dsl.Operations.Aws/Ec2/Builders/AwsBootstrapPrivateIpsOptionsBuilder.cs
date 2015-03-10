using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapPrivateIpsOptionsBuilder : IOfferAwsBootstrapPrivateIpsOptions
    {
        private readonly List<PrivateIpAddressSpecification> _values;

        public AwsBootstrapPrivateIpsOptionsBuilder(List<PrivateIpAddressSpecification> values)
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