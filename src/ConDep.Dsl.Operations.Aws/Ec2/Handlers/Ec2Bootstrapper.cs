using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.RDS.Model;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Aws.Ec2.Builders;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    internal class Ec2Bootstrapper
    {
        private readonly AwsBootstrapOptionsValues _options;
        private readonly ConDepSettings _conDepSettings;
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;
        private readonly Ec2InstancePasswordHandler _passwordHandler;

        public Ec2Bootstrapper(AwsBootstrapOptionsValues options, ConDepSettings conDepSettings)
        {
            _options = options;
            _conDepSettings = conDepSettings;
            _client = new AmazonEC2Client(_options.Credentials, _options.RegionEndpoint);

            _instanceHandler = new Ec2InstanceHandler(_client);
            _passwordHandler = new Ec2InstancePasswordHandler(_client);
        }

        public Ec2BootstrapConfig Boostrap()
        {
            Ec2BootstrapConfig config;

            config = _options.IdempotencyType == AwsEc2IdempotencyType.ClientToken ? 
                new Ec2BootstrapConfig(_options.InstanceRequest.ClientToken) : 
                new Ec2BootstrapConfig(_options.IdempotencyTags);

            if (_instanceHandler.AllreadyBootstrapped(_options))
            {
                return GetConfigFromExisting(_options, config);
            }

            return BootstrapNew(config);
        }

        private Ec2BootstrapConfig BootstrapNew(Ec2BootstrapConfig config)
        {
            var amiLocator = new Ec2AmiLocator(_client);
            var imageValues = _options.Image;

            if (imageValues.HasImageId())
            {
                _options.InstanceRequest.ImageId = imageValues.Id;
            }
            else if (imageValues.HasLatestImageDefined())
            {
                switch (imageValues.LatestImage)
                {
                    case AwsWindowsImage.Win2008:
                        _options.InstanceRequest.ImageId = amiLocator.Find2008Core();
                        break;
                    case AwsWindowsImage.Win2008R2:
                        _options.InstanceRequest.ImageId = amiLocator.Find2008R2Core();
                        break;
                    case AwsWindowsImage.Win2012:
                        _options.InstanceRequest.ImageId = amiLocator.Find2012Core();
                        break;
                    case AwsWindowsImage.Win2012R2:
                        _options.InstanceRequest.ImageId = amiLocator.Find2012R2Core();
                        break;
                    default:
                        throw new Exception("Image " + imageValues.LatestImage + " currently not supported. Please specify image id as a string instead.");
                }
            }
            else if(imageValues.HasImageFilter())
            {
                _options.InstanceRequest.ImageId = amiLocator.FindWithFilters(imageValues.Filters, imageValues.FilterByOwner);
            }
            else
            {
                _options.InstanceRequest.ImageId = amiLocator.Find2012R2Core();
            }

            Logger.Info("Creating instances...");
            var instanceIds = _instanceHandler.CreateInstances(_options).ToList();

            Thread.Sleep(10000);
            //Logger.Info("Tagging instances.            //var instanceTag = _tagHandler.CreateNameTags(config.BootstrapId, instanceIds);

            Logger.WithLogSection("Waiting for instances to be ready",
                () => _instanceHandler.WaitForInstancesStatus(instanceIds, Ec2InstanceState.Running));

            List<Tuple<string, string>> passwords = null;

            if (!_conDepSettings.Config.DeploymentUser.IsDefined())
            {
                Logger.WithLogSection("Waiting for Windows password to be available",
                    () => { passwords = _passwordHandler.WaitForPassword(instanceIds, _options.PrivateKeyFileLocation); });
            }

            _instanceHandler.TagInstances(instanceIds, _options.Tags);

            var instances = _instanceHandler.GetInstances(instanceIds).ToList();


            foreach (var instance in instances)
            {
                config.Instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    UserName = _conDepSettings.Config.DeploymentUser.IsDefined() ? _conDepSettings.Config.DeploymentUser.UserName : @".\Administrator",
                    Password = _conDepSettings.Config.DeploymentUser.IsDefined() ? _conDepSettings.Config.DeploymentUser.Password : passwords.Single(x => x.Item1 == instance.InstanceId).Item2,
                    ManagementAddress = GetManagementAddress(instance)
                });
            }

            HaveAccessToServers(config);

            //StopServers(config);

            //if (takeSnapshots)
            //{
            //    Logger.Info("Taking snapshots of disks to enable resets...");
            //    _snapshotHandler.TakeSnapshots(instances, config);
            //}
            //else
            //{
            //    Logger.Warn("Snapshots disabled. Note that reset will not work without snapshots.");
            //}

            //StartServers(config);

            //Logger.Info("Storing configuration...");
            //var configHandler = new BootstrapConfigHandler(config.BootstrapId);
            //configHandler.Write(config);
            //return config.BootstrapId;
            return config;
        }

        private Ec2BootstrapConfig GetConfigFromExisting(AwsBootstrapOptionsValues options, Ec2BootstrapConfig config)
        {
            Logger.Info("Allready bootstrapped. Getting server information.");
            
            var existingInstances = options.IdempotencyType == AwsEc2IdempotencyType.ClientToken ? 
                _instanceHandler.GetInstances(_options.InstanceRequest.ClientToken).ToList() : 
                _instanceHandler.GetInstances(_options.IdempotencyTags).ToList();

            List<Tuple<string, string>> existingPasswords = null;

            if (!_conDepSettings.Config.DeploymentUser.IsDefined())
            {
                existingPasswords = _passwordHandler.WaitForPassword(existingInstances.Select(x => x.InstanceId),
                    _options.PrivateKeyFileLocation);
            }


            foreach (var instance in existingInstances.Where(x => x.State.Name == "Running"))
            {
                config.Instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    UserName = _conDepSettings.Config.DeploymentUser.IsDefined() ? _conDepSettings.Config.DeploymentUser.UserName : ".\\Administrator",
                    Password = _conDepSettings.Config.DeploymentUser.IsDefined() ? _conDepSettings.Config.DeploymentUser.Password : existingPasswords.Single(x => x.Item1 == instance.InstanceId).Item2,
                    AwsInstance = instance,
                    ManagementAddress = GetManagementAddress(instance)
                });
            }
            return config;
        }

        private string GetManagementAddress(Instance instance)
        {
            return _instanceHandler.GetManagementAddress(instance, _options.ManagementAddressType, _options.NetworkInterfaceValues.RemoteManagementInterfaceIndex);
        }

        private static void HaveAccessToServers(Ec2BootstrapConfig config)
        {
            Logger.WithLogSection("Checking access to servers", () =>
            {
                foreach (var instance in config.Instances)
                {
                    HaveAccessToServer(instance, 1, 5);
                }
            });
        }

        private static void HaveAccessToServer(Ec2Instance instance, int attemptNum, int maxRetries)
        {
            Logger.Info(string.Format("({1}/{2}) Checking if WinRM (Remote PowerShell) can be used to reach remote server [{0}]...", instance.InstanceId, attemptNum, maxRetries));

            var cmd = string.Format("id -r:{0} -u:{1} -p:\"{2}\"", instance.ManagementAddress, instance.UserName, instance.Password);

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

            if (process.ExitCode == 0)
            {
                var message = process.StandardOutput.ReadToEnd();
                Logger.Info(string.Format("Contact was made with server [{0}] using WinRM (Remote PowerShell). ",
                                        instance.ManagementAddress));
                Logger.Info(string.Format("Details: {0} ", message));
            }
            else
            {
                var errorMessage = process.StandardError.ReadToEnd();
                if (attemptNum <= maxRetries)
                {
                    Logger.Info(string.Format("Unable to reach server [{0}] using WinRM (Remote PowerShell)",
                        instance.ManagementAddress));
                    Logger.Info("Waiting 30 seconds before retry...");
                    Thread.Sleep(30000);
                    HaveAccessToServer(instance, ++attemptNum, maxRetries);
                }
                else
                {
                    Logger.Error(string.Format("Unable to reach server [{0}] using WinRM (Remote PowerShell)",
                        instance.ManagementAddress));
                    Logger.Error(string.Format("Details: {0}", errorMessage));
                    Logger.Error("Max number of retries exceeded. Please check your Amazon Network firewall for why WinRM cannot connect.");
                }
            }
        }

    }
}