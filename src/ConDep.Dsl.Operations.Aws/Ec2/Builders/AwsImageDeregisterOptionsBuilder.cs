using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsImageDeregisterOptionsBuilder : IOfferAwsImageDeregisterOptions
    {
        AwsImageDeregisterOptionsValues _values;
        public AwsImageDeregisterOptionsBuilder()
        {
            _values = new AwsImageDeregisterOptionsValues();
        }

        public void RemoveSnapshots(bool removeSnapshots)
        {
            _values.RemoveSnapshots = removeSnapshots;
        }
        internal AwsImageDeregisterOptionsValues Values { get { return _values; } }
    }
}