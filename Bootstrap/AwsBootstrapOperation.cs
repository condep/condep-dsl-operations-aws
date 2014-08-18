using System;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Execution;
using ConDep.Dsl.Harvesters;
using ConDep.Dsl.Operations.Application.Local;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Operations.Aws.Bootstrap.Ec2;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;
using Microsoft.CSharp.RuntimeBinder;

namespace ConDep.Dsl.Operations.Aws.Bootstrap
{
    internal class AwsBootstrapOperation : LocalOperation
    {
        private readonly AwsBootstrapMandatoryInputValues _mandatoryOptions;
        private readonly AwsBootstrapInputValues _options;

        public AwsBootstrapOperation(AwsBootstrapMandatoryInputValues mandatoryOptions)
        {
            _mandatoryOptions = mandatoryOptions;
        }

        public AwsBootstrapOperation(AwsBootstrapMandatoryInputValues mandatoryOptions, AwsBootstrapInputValues options)
        {
            _mandatoryOptions = mandatoryOptions;
            _options = options;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            ValidateMandatoryOptions(settings);
            var options = GetOptions();
            var bootstrapper = new Ec2Bootstrapper(_mandatoryOptions, options);
            var ec2Config = bootstrapper.Boostrap();

            foreach (var instance in ec2Config.Instances)
            {
                var hostName = GetHostName(_options, instance);
                settings.Config.Servers.Add(new ServerConfig
                {
                    DeploymentUser = new DeploymentUserConfig
                    {
                        UserName = instance.UserName,
                        Password = instance.Password
                    },
                    Name = hostName
                });
            }

            var serverValidator = new RemoteServerValidator(settings.Config.Servers, HarvesterFactory.GetHarvester(settings));
            if (!serverValidator.IsValid())
            {
                throw new ConDepValidationException("Not all servers fulfill ConDep's requirements. Aborting execution.");
            }
            ConDepConfigurationExecutor.ExecutePreOps(settings, status, token);
        }

        private string GetHostName(AwsBootstrapInputValues options, Ec2Instance instance)
        {
            if (options != null && options.RemoteManagementConnectionType != null)
            {
                switch (options.RemoteManagementConnectionType)
                {
                    case RemoteManagementConnectionType.PublicDns:
                        return instance.PublicDns;
                    case RemoteManagementConnectionType.PublicIp:
                        return instance.PublicIp;
                    case RemoteManagementConnectionType.PrivateDns:
                        return instance.PrivateDns;
                    case RemoteManagementConnectionType.PrivateIp:
                        return instance.PrivateIp;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(instance.PublicDns))
                {
                    return instance.PublicDns;
                }
                if (!string.IsNullOrWhiteSpace(instance.PublicIp))
                {
                    return instance.PublicDns;
                }
                if (!string.IsNullOrWhiteSpace(instance.PrivateDns))
                {
                    return instance.PublicDns;
                }
                if (!string.IsNullOrWhiteSpace(instance.PrivateIp))
                {
                    return instance.PublicDns;
                }
            }
            throw new Exception("No remote management address found.");
        }

        private AwsBootstrapInputValues GetOptions()
        {
            if (_options != null) return _options;

            return new AwsBootstrapInputValues();
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

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "AWS Bootstrap"; }
        }
    }
}