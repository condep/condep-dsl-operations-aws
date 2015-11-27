using System.Collections.Generic;
using Amazon.DynamoDBv2;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapTagOptionsBuilder : IOfferAwsTagOptions
    {
        private readonly List<KeyValuePair<string, string>> _tags = new List<KeyValuePair<string, string>>();

        public AwsBootstrapTagOptionsBuilder()
        {
            
        }

        public AwsBootstrapTagOptionsBuilder(List<KeyValuePair<string, string>> tags)
        {
            _tags = tags;
        }

        public IOfferAwsTagOptions Add(string name, string value)
        {
            _tags.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public IOfferAwsTagOptions Add(string name)
        {
            return Add(name, "");
        }

        public IOfferAwsTagOptions AddName(string value)
        {
            _tags.Add(new KeyValuePair<string, string>("Name", value));
            return this;
        }
    }
}