using System;
using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public interface IOfferAwsBootstrapDisksOptions
    {
        IOfferAwsBootstrapOptions Add(string deviceName, string virtualName, string deviceToSuppressFromImage = null);
        IOfferAwsBootstrapOptions Add(string deviceName, Action<IOfferAwsBootstrapEbsOptions> ebs, string deviceToSuppressFromImage = null);
    }

    class AwsBootstrapDisksOptions : IOfferAwsBootstrapDisksOptions
    {
        private readonly List<BlockDeviceMapping> _values;
        private readonly AwsBootstrapOptions _options;

        public AwsBootstrapDisksOptions(List<BlockDeviceMapping> values, AwsBootstrapOptions options)
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

            var ebsValues = new AwsBootstrapEbsOptions(blockDevice.Ebs);
            ebs(ebsValues);

            _values.Add(blockDevice);
            return _options;
        }
    }
}