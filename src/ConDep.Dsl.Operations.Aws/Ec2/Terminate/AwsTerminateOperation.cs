using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Validation;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Ec2.Terminate
{
    public class AwsTerminateOperation : LocalOperation
    {
        private readonly AwsTerminateOptionsValues _options;

        public AwsTerminateOperation(AwsTerminateOptionsValues options)
        {
            _options = options;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(_options);
            var terminator = new Ec2Terminator(_options);
            terminator.Terminate();
        }

        private void LoadOptionsFromConfig(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || (settings.Config.OperationsConfig.AwsBootstrapOperation == null && settings.Config.OperationsConfig.AwsBootstrapOperation == null))
            {
                return;
            }

            var dynamicBootstrapConfig = settings.Config.OperationsConfig.AwsBootstrapOperation;
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            try
            {
                if (dynamicAwsConfig != null)
                {
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.KeyName) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.PublicKeyName)) _options.InstanceRequest.KeyName = dynamicAwsConfig.PublicKeyName;
                    if (string.IsNullOrWhiteSpace(_options.PrivateKeyFileLocation) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.PrivateKeyFileLocation)) _options.PrivateKeyFileLocation = dynamicAwsConfig.PrivateKeyFileLocation;
                    if (_options.RegionEndpoint == null && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.Region)) _options.RegionEndpoint = RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region);

                    if (_options.Credentials == null)
                    {
                        string profileName = dynamicAwsConfig.Credentials.ProfileName;
                        if (string.IsNullOrEmpty(profileName))
                        {
                            if (dynamicAwsConfig.Credentials.AccessKey == null)
                                throw new OperationConfigException(
                                    string.Format(
                                        "Configuration in environment configuration file for Credentials.AccessKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                        GetType().Name));
                            if (dynamicAwsConfig.Credentials.SecretKey == null)
                                throw new OperationConfigException(
                                    string.Format(
                                        "Configuration in environment configuration file for Credentials.SecretKey must be present for operation {0}. Optionally you can use AWS credential profile instead, but then ProfileName must be present.",
                                        GetType().Name));

                            _options.Credentials = new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
                        }
                        else
                        {
                            _options.Credentials = new StoredProfileAWSCredentials((string)dynamicAwsConfig.Credentials.ProfileName);
                        }
                    }
                }

                if (dynamicBootstrapConfig != null)
                {
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.SubnetId) && !string.IsNullOrWhiteSpace((string)dynamicBootstrapConfig.SubnetId)) _options.InstanceRequest.SubnetId = dynamicBootstrapConfig.SubnetId;
                }
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(
                    string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.",
                        GetType().Name), binderException);
            }
        }

        private void ValidateMandatoryOptions(AwsTerminateOptionsValues options)
        {
            if (string.IsNullOrWhiteSpace(options.PrivateKeyFileLocation)) throw new OperationConfigException(string.Format("Missing value for PrivateKeyFileLocation. Please specify in code or in config."));
            if (options.RegionEndpoint == null) throw new OperationConfigException(string.Format("Missing value for Region. Please specify in code or in config."));
            if (options.Credentials == null) throw new OperationConfigException(string.Format("Missing value for Credentials. Please specify in code or in config."));
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