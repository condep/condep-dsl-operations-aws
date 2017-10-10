using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Operations.Aws.Ec2.Handlers;
using Microsoft.CSharp.RuntimeBinder;
using ConDep.Execution.Validation;
using ConDep.Dsl.Harvesters;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;
using ConDep.Execution;
using ConDep.Dsl.Logging;
using static ConDep.Dsl.Operations.Remote.RestartComputerOperation;
using System.Diagnostics;

namespace ConDep.Dsl.Operations.Aws.Ec2.Operations
{
    class AwsStartOperation : AwsIdentifiedOperation
    {
        private AwsStartOptionsValues _options;

        public AwsStartOperation(AwsStartOptionsValues options) : base(options)
        {
            this._options = options;
        }

        public override string Name => "Start instances";

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            LoadOptionsFromConfig(settings);
            ValidateMandatoryOptions(_options);
            var starter = new Ec2Starter(_options, settings);
            var ec2instances = starter.Start();

            var instances = new List<Ec2Instance>();
            var servers = new List<ServerConfig>();
            foreach (var instance in ec2instances)
            {
                instances.Add(instance);
                var server = new ServerConfig
                {
                    DeploymentUser = new DeploymentUserConfig
                    {
                        UserName = instance.UserName,
                        Password = instance.Password
                    },
                    Name = instance.ManagementAddress,
                    PowerShell = new PowerShellConfig() { HttpPort = 5985, HttpsPort = 5986 },
                    Node = new NodeConfig() { Port = 4444, TimeoutInSeconds = 100 }
                };
                settings.Config.Servers.Add(server);
                servers.Add(server);
            }

            Logger.Verbose("Waiting for WinRM to succeed");
            foreach(var server in servers)
            {
                Logger.Verbose($"Waiting for WinRM to succeed on {server.Name}");
                WaitForWinRm(WaitForStatus.Success, server);

            }
            var serverValidator = new RemoteServerValidator(servers, HarvesterFactory.GetHarvester(settings), new PowerShellExecutor());
            if (!serverValidator.Validate()) { 
                throw new ConDepValidationException("Not all servers fulfill ConDep's requirements. Aborting execution.");
            }

            ConDepConfigurationExecutor.ExecutePreOps(settings, token);

            var result = Result.SuccessChanged();
            result.Data.Instances = instances;
            return result;
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

        private void WaitForWinRm(WaitForStatus status, ServerConfig server)
        {
            try
            {
                var cmd = server.DeploymentUser.IsDefined()
                    ? $"id -r:{server.Name} -u:{server.DeploymentUser.UserName} -p:\"{server.DeploymentUser.Password}\""
                    : $"id -r:{server.Name}";

                var path = Environment.ExpandEnvironmentVariables(@"%windir%\system32\WinRM.cmd");
                var startInfo = new ProcessStartInfo(path)
                {
                    Arguments = cmd,
                    Verb = "RunAs",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                var process = Process.Start(startInfo);
                process.WaitForExit();

                switch (status)
                {
                    case WaitForStatus.Failure:
                        if (process.ExitCode == 0)
                        {
                            Thread.Sleep(5000);
                            WaitForWinRm(status, server);
                        }
                        break;
                    case WaitForStatus.Success:
                        if (process.ExitCode != 0)
                        {
                            Thread.Sleep(5000);
                            WaitForWinRm(status, server);
                        }
                        break;
                }
            }
            catch
            {
                switch (status)
                {
                    case WaitForStatus.Failure:
                        return;
                    case WaitForStatus.Success:
                        Thread.Sleep(5000);
                        WaitForWinRm(status, server);
                        break;
                }
            }
        }
    }
}
