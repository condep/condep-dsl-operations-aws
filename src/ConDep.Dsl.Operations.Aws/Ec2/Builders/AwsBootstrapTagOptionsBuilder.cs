using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapTagOptionsBuilder : IOfferAwsTagOptions
    {
        private readonly List<KeyValuePair<string, string>> _tags;
        private readonly IOfferAwsBootstrapOptions _awsBootstrapOptionsBuilder;

        public AwsBootstrapTagOptionsBuilder(List<KeyValuePair<string, string>> tags, IOfferAwsBootstrapOptions awsBootstrapOptionsBuilder)
        {
            _tags = tags;
            _awsBootstrapOptionsBuilder = awsBootstrapOptionsBuilder;
        }

        public IOfferAwsBootstrapOptions Add(string name, string value)
        {
            _tags.Add(new KeyValuePair<string, string>(name, value));
            return _awsBootstrapOptionsBuilder;
        }

        public IOfferAwsBootstrapOptions AddName(string value)
        {
            _tags.Add(new KeyValuePair<string, string>("Name", value));
            return _awsBootstrapOptionsBuilder;
        }
    }
}