using System.Threading;
using Amazon;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Validation;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Terminate
{
    internal class AwsTerminateOperation : LocalOperation
    {
        private readonly AwsBootstrapOptionsValues _options;

        public AwsTerminateOperation(AwsBootstrapOptionsValues options)
        {
            _options = options;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            ValidateMandatoryOptions(settings);
            var terminator = new Ec2Terminator(_options);
            terminator.Terminate();
        }

        private void ValidateMandatoryOptions(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || settings.Config.OperationsConfig.AwsBootstrapOperation == null)
            {
                return;
            }

            var ec2DynamicConfig = settings.Config.OperationsConfig.AwsBootstrapOperation;
            try
            {
                if (string.IsNullOrWhiteSpace(_options.InstanceRequest.KeyName)) _options.InstanceRequest.KeyName = ec2DynamicConfig.PublicKeyName;
                if (string.IsNullOrWhiteSpace(_options.PrivateKeyFileLocation)) _options.PrivateKeyFileLocation = ec2DynamicConfig.PrivateKeyFileLocation;
                if (string.IsNullOrWhiteSpace(_options.InstanceRequest.SubnetId)) _options.InstanceRequest.SubnetId = ec2DynamicConfig.SubnetId;
                if (_options.RegionEndpoint == null) _options.RegionEndpoint = RegionEndpoint.GetBySystemName((string)ec2DynamicConfig.Region);
                if (_options.InstanceRequest.Placement == null) _options.InstanceRequest.Placement = new Placement(ec2DynamicConfig.AvailabilityZone);

                string profileName = ec2DynamicConfig.Credentials.ProfileName;
                if (string.IsNullOrEmpty(profileName))
                {
                    if (ec2DynamicConfig.Credentials.AccessKey == null)
                        throw new OperationConfigException(
                            string.Format(
                                "Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                GetType().Name));
                    if (ec2DynamicConfig.Credentials.SecretKey == null)
                        throw new OperationConfigException(
                            string.Format(
                                "Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                GetType().Name));

                    _options.Credentials = new BasicAWSCredentials(ec2DynamicConfig.Credentials.AccessKey, ec2DynamicConfig.Credentials.SecretKey);
                }
                else
                {
                    _options.Credentials = new StoredProfileAWSCredentials(ec2DynamicConfig.Credentials.ProfileName);
                }
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(
                    string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.",
                        GetType().Name), binderException);
            }
        }

        private RegionEndpoint GetRegionEndpoint(string region)
        {
            if (region == "us-east-1")
                return RegionEndpoint.USEast1;
            if (region == "us-west-1")
                return RegionEndpoint.USWest1;
            if (region == "us-west-2")
                return RegionEndpoint.USWest2;
            if (region == "ap-southeast-1")
                return RegionEndpoint.APSoutheast1;
            if (region == "ap-southeast-2")
                return RegionEndpoint.APSoutheast2;
            if (region == "ap-northeast-1")
                return RegionEndpoint.APNortheast1;
            if (region == "sa-east-1")
                return RegionEndpoint.SAEast1;
            return RegionEndpoint.EUWest1;
        }


        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Aws Terminate Instance"; }
        }
    }
}