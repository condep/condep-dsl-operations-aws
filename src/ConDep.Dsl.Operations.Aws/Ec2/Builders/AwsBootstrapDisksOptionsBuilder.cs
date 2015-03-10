using System;
using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Ec2.Builders
{
    internal class AwsBootstrapDisksOptionsBuilder : IOfferAwsBootstrapDisksOptions
    {
        private readonly List<BlockDeviceMapping> _values;
        private readonly AwsBootstrapOptionsBuilder _options;

        public AwsBootstrapDisksOptionsBuilder(List<BlockDeviceMapping> values, AwsBootstrapOptionsBuilder options)
        {
            _values = values;
            _options = options;
        }

        public IOfferAwsBootstrapOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null)
        {
            _values.Add(new BlockDeviceMapping
            {
                DeviceName = deviceName,
                VirtualName = virtualName,
                NoDevice = deviceToSuppressFromImage
            });
            return _options;
        }

        public IOfferAwsBootstrapOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null)
        {
            var blockDevice = new BlockDeviceMapping
            {
                DeviceName = deviceName,
                NoDevice = deviceToSuppressFromImage,
                Ebs = new EbsBlockDevice()
            };

            var ebsValues = new AwsBootstrapEbsOptionsBuilder(blockDevice.Ebs);
            ebs(ebsValues);

            _values.Add(blockDevice);
            return _options;
        }
    }
}