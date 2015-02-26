using System.Threading;
using Amazon;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Validation;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Terminate
{
    public class AwsTerminateOperation : LocalOperation
    {
        private readonly AwsBootstrapMandatoryInputValues _mandatoryOptions;

        public AwsTerminateOperation(AwsBootstrapMandatoryInputValues mandatoryOptions)
        {
            _mandatoryOptions = mandatoryOptions;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            ValidateMandatoryOptions(settings);
            var terminator = new Ec2Terminator(_mandatoryOptions);
            terminator.Terminate();
        }

        private void ValidateMandatoryOptions(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || settings.Config.OperationsConfig.AwsBootstrapOperation == null)
            {
                return;
            }

            var config = settings.Config.OperationsConfig.AwsBootstrapOperation;
            try
            {
                if (config.SubnetId == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for SubnetId must be present for operation {0}.", GetType().Name));
                if (config.PublicKeyName == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for PublicKeyName must be present for operation {0}.", GetType().Name));
                if (config.PrivateKeyFileLocation == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for PrivateKeyFileLocation must be present for operation {0}.", GetType().Name));
                if (config.PublicKeyName == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for PublicKeyName must be present for operation {0}.", GetType().Name));
                if (config.Credentials == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for Credentials must be present for operation {0}.", GetType().Name));
                if (config.Region == null) throw new OperationConfigException(string.Format("Configuration in environment configuration file for Region must be present for operation {0}.", GetType().Name));

                _mandatoryOptions.PublicKeyName = config.PublicKeyName;
                _mandatoryOptions.PrivateKeyFileLocation = config.PrivateKeyFileLocation;
                _mandatoryOptions.SubnetId = config.SubnetId;
                _mandatoryOptions.Region = config.Region;
                _mandatoryOptions.RegionEndpoint = GetRegionEndpoint((string) config.Region);

                string profileName = config.Credentials.ProfileName;
                if (string.IsNullOrEmpty(profileName))
                {
                    _mandatoryOptions.Credentials.UseProfile = false;
                    if (config.Credentials.AccessKey == null)
                        throw new OperationConfigException(
                            string.Format(
                                "Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                GetType().Name));
                    if (config.Credentials.SecretKey == null)
                        throw new OperationConfigException(
                            string.Format(
                                "Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                GetType().Name));

                    _mandatoryOptions.Credentials.AccessKey = config.Credentials.AccessKey;
                    _mandatoryOptions.Credentials.SecretKey = config.Credentials.SecretKey;
                }
                else
                {
                    _mandatoryOptions.Credentials.UseProfile = true;
                    _mandatoryOptions.Credentials.ProfileName = config.Credentials.ProfileName;
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