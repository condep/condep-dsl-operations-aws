using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Ec2.Operations
{
    public abstract class AwsIdentifiedOperation : LocalOperation
    {

        private IOfferAwsOperationsOptionValues _options;
        public AwsIdentifiedOperation(IOfferAwsOperationsOptionValues options)
        {
            this._options = options;
        }

        public void LoadOptionsFromConfig(ConDepSettings settings)
        {
            if (settings.Config.OperationsConfig == null || (settings.Config.OperationsConfig.AwsBootstrapOperation == null && settings.Config.OperationsConfig.AwsBootstrapOperation == null))
            {
                return;
            }
            
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            try
            {
                if (dynamicAwsConfig != null)
                {
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
                
            }
            catch (RuntimeBinderException binderException)
            {
                throw new OperationConfigException(
                    string.Format("Configuration extraction for {0} failed during binding. Please check inner exception for details.",
                        GetType().Name), binderException);
            }
        }
        protected void ValidateMandatoryOptions(IOfferAwsOperationsOptionValues options)
        {
            if (string.IsNullOrWhiteSpace(options.PrivateKeyFileLocation)) throw new OperationConfigException(string.Format("Missing value for PrivateKeyFileLocation. Please specify in code or in config."));
            if (options.RegionEndpoint == null) throw new OperationConfigException(string.Format("Missing value for Region. Please specify in code or in config."));
            if (options.Credentials == null) throw new OperationConfigException(string.Format("Missing value for Credentials. Please specify in code or in config."));
        }
    }
}
