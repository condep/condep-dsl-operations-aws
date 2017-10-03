using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using Microsoft.CSharp.RuntimeBinder;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using System.Linq;
using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Aws.Ec2.Operations
{
    internal class AwsTerminateOperation : AwsIdentifiedOperation
    {
        private readonly AwsTerminateOptionsValues _options;

        public AwsTerminateOperation(AwsTerminateOptionsValues options) : base(options)
        {
            _options = options;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(_options);
            var terminator = new Ec2Terminator(_options);
            var instances = terminator.Terminate();
            // Select the stopped instances and remove them from condep server list
            var instanceAddresses = instances.SelectMany(i =>
            {
                return i.NetworkInterfaces.SelectMany(ni => new string[] { ni.PrivateDnsName, ni.PrivateIpAddress, ni.Association?.PublicDnsName, ni.Association?.PublicIp });
            });
            var stoppedServers = settings.Config.Servers.Where(s => instanceAddresses.Contains(s.Name)).ToList();
            foreach (var server in stoppedServers)
            {
                settings.Config.Servers.Remove(server);
            }

            return Result.SuccessChanged();
        }

        public void LoadOptionsFromConfig(ConDepSettings settings)
        {
            base.LoadOptionsFromConfig(settings);

            var dynamicBootstrapConfig = settings.Config.OperationsConfig.AwsBootstrapOperation;
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            try
            {
                if (dynamicAwsConfig != null)
                {
                    if (string.IsNullOrWhiteSpace(_options.InstanceRequest.KeyName) && !string.IsNullOrWhiteSpace((string)dynamicAwsConfig.PublicKeyName)) _options.InstanceRequest.KeyName = dynamicAwsConfig.PublicKeyName;
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

        public override string Name
        {
            get { return "Aws Terminate Instance"; }
        }
    }
}