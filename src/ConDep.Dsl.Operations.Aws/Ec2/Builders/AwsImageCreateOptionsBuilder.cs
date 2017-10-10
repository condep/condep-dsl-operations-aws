using ConDep.Dsl.Operations.Aws.Ec2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsImageCreateOptionsBuilder : IOfferAwsImageCreateOptions
    {

        private readonly AwsImageCreateOptionsValues _values;
        private readonly IOfferAwsImageCreateDisksOptions _disks;

        public AwsImageCreateOptionsBuilder(string instanceId, string name)
        {
            _values = new AwsImageCreateOptionsValues(instanceId, name);
            _disks = new AwsImageCreateDisksOptionsBuilder(_values.ImageRequest.BlockDeviceMappings, this);
        }

        public AwsImageCreateOptionsBuilder(string name)
        {
            _values = new AwsImageCreateOptionsValues(name);
            _disks = new AwsImageCreateDisksOptionsBuilder(_values.ImageRequest.BlockDeviceMappings, this);
        }

        public IOfferAwsImageCreateDisksOptions Disks { get { return _disks; } }

        public IOfferAwsImageCreateOptions Description(string description)
        {
            _values.ImageRequest.Description = description;
            return this;
        }

        public IOfferAwsImageCreateOptions NoReboot(bool noReboot)
        {
            _values.ImageRequest.NoReboot = noReboot;
            return this;
        }

        public IOfferAwsImageCreateOptions WaitForShutdown(bool waitForShutdown)
        {
            _values.WaitForShutdown = waitForShutdown;
            return this;
        }
        public AwsImageCreateOptionsValues Values { get { return _values; } }
    }
}