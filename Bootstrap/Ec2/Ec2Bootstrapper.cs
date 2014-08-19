using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.Bootstrap.Ec2
{
    internal class Ec2Bootstrapper
    {
        private readonly AwsBootstrapMandatoryInputValues _mandatoryOptions;
        private AwsBootstrapOptions _options;
        private readonly string _awsProfileName;
        private readonly IAmazonEC2 _client;
        private Ec2InstanceHandler _instanceHandler;
        //private Ec2SecurityGroupHandler _securityGroupHandler;
        //private Ec2TagHandler _tagHandler;
        private Ec2InstancePasswordHandler _passwordHandler;
        //private Ec2SnapshotHandler _snapshotHandler;

        public Ec2Bootstrapper(AwsBootstrapMandatoryInputValues mandatoryOptions, AwsBootstrapOptions options)
        {
            _mandatoryOptions = mandatoryOptions;
            _options = options;

            AWSCredentials creds = _mandatoryOptions.Credentials.UseProfile ? (AWSCredentials)new StoredProfileAWSCredentials(mandatoryOptions.Credentials.ProfileName) : new BasicAWSCredentials(mandatoryOptions.Credentials.AccessKey, mandatoryOptions.Credentials.SecretKey);
            var endpoint = RegionEndpoint.GetBySystemName(_mandatoryOptions.Region);
            _client = AWSClientFactory.CreateAmazonEC2Client(creds, endpoint);

            _instanceHandler = new Ec2InstanceHandler(_client);
            //_securityGroupHandler = new Ec2SecurityGroupHandler(_client);
            //_tagHandler = new Ec2TagHandler(_client);
            _passwordHandler = new Ec2InstancePasswordHandler(_client);
            //_snapshotHandler = new Ec2SnapshotHandler(_client, _tagHandler);
        }

        public Ec2BootstrapConfig Boostrap()
        {
            var config = new Ec2BootstrapConfig(_mandatoryOptions.BootstrapId);

            if (_instanceHandler.AllreadyBootstrapped(_mandatoryOptions.BootstrapId))
            {
                return GetConfigFromExisting(config);
            }

            return BootstrapNew(config);
        }

        private Ec2BootstrapConfig BootstrapNew(Ec2BootstrapConfig config)
        {
            var amiLocator = new Ec2AmiLocator(_client);
            var imageValues = ((AwsBootstrapImageOptions) _options.Image).Values;

            if (imageValues.HasImageId())
            {
                _options.Values.ImageId = imageValues.Id;
            }
            else if (imageValues.HasLatestImageDefined())
            {
                switch (imageValues.LatestImage)
                {
                    case AwsWindowsImage.Win2008:
                        _options.Values.ImageId = amiLocator.Find2008Core();
                        break;
                    case AwsWindowsImage.Win2008R2:
                        _options.Values.ImageId = amiLocator.Find2008R2Core();
                        break;
                    case AwsWindowsImage.Win2012:
                        _options.Values.ImageId = amiLocator.Find2012Core();
                        break;
                    case AwsWindowsImage.Win2012R2:
                        _options.Values.ImageId = amiLocator.Find2012R2Core();
                        break;
                    default:
                        throw new Exception("Image " + imageValues.LatestImage + " currently not supported.");
                }
            }
            else
            {
                _options.Values.ImageId = amiLocator.Find2012R2Core();
            }

            Logger.Info("Creating instances...");
            var instanceIds = _instanceHandler.CreateInstances(config.BootstrapId, _mandatoryOptions, _options.Values).ToList();

            //Logger.Info("Tagging instances.            //var instanceTag = _tagHandler.CreateNameTags(config.BootstrapId, instanceIds);

            Logger.WithLogSection("Waiting for instances to be ready",
                () => _instanceHandler.WaitForInstancesStatus(instanceIds, Ec2InstanceState.Running));

            List<Tuple<string, string>> passwords = null;

            Logger.WithLogSection("Waiting for Windows password to be available",
                () => { passwords = _passwordHandler.WaitForPassword(instanceIds, _mandatoryOptions.PrivateKeyFileLocation); });

            var instances = _instanceHandler.GetInstances(instanceIds).ToList();

            foreach (var instance in instances)
            {
                config.Instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    UserName = "Administrator",
                    Password = passwords.Single(x => x.Item1 == instance.InstanceId).Item2,
                    ManagementAddress = GetManagementAddress(_options, instance)
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

        private Ec2BootstrapConfig GetConfigFromExisting(Ec2BootstrapConfig config)
        {
            Logger.Info("Allready bootstrapped. Getting server information.");
            var existingInstances = _instanceHandler.GetInstances(_mandatoryOptions.BootstrapId).ToList();

            var existingPasswords = _passwordHandler.WaitForPassword(existingInstances.Select(x => x.InstanceId),
                _mandatoryOptions.PrivateKeyFileLocation);

            foreach (var instance in existingInstances)
            {
                config.Instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    UserName = "Administrator",
                    Password = existingPasswords.Single(x => x.Item1 == instance.InstanceId).Item2,
                    AwsInstance = instance,
                    ManagementAddress = GetManagementAddress(_options, instance)
                });
            }
            return config;
        }

        private string GetManagementAddress(AwsBootstrapOptions options, Instance instance)
        {
            var mngmntInterface = GetManagementInterface(options, instance);

            if (options.ManagementAddressType != null)
            {
                switch (options.ManagementAddressType)
                {
                    case RemoteManagementAddressType.PublicDns:
                        if(mngmntInterface.Association == null || string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicDnsName)) throw new ConDepInvalidSetupException("Instance has no public DNS name.");
                        return mngmntInterface.Association.PublicDnsName;
                    case RemoteManagementAddressType.PublicIp:
                        if(mngmntInterface.Association == null || string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicIp)) throw new ConDepInvalidSetupException("Instance has no public IP.");
                        return mngmntInterface.Association.PublicIp;
                    case RemoteManagementAddressType.PrivateDns:
                        if(string.IsNullOrWhiteSpace(mngmntInterface.PrivateDnsName)) throw new ConDepInvalidSetupException("Instance has no private DNS name.");
                        return mngmntInterface.PrivateDnsName;
                    case RemoteManagementAddressType.PrivateIp:
                        return mngmntInterface.PrivateIpAddress;
                }
            }
            else
            {
                if (mngmntInterface.Association != null && !string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicDnsName))
                {
                    return mngmntInterface.Association.PublicDnsName;
                }
                if (mngmntInterface.Association != null && !string.IsNullOrWhiteSpace(mngmntInterface.Association.PublicIp))
                {
                    return mngmntInterface.Association.PublicIp;
                }
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateIpAddress))
                {
                    return mngmntInterface.PrivateIpAddress;
                }
                if (!string.IsNullOrWhiteSpace(mngmntInterface.PrivateDnsName))
                {
                    return mngmntInterface.PrivateDnsName;
                }
            }
            throw new Exception("No remote management address found.");
        }

        private static InstanceNetworkInterface GetManagementInterface(AwsBootstrapOptions options, Instance instance)
        {
            if (options.Values.NetworkInterfaces.Count > 0)
            {
                if (options.ManagementInterfaceIndex != null)
                {
                    return instance.NetworkInterfaces[options.ManagementInterfaceIndex.Value];
                }
            }
            return instance.NetworkInterfaces[0];
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
        private static void HaveAccessToServer(Ec2Instance instance, int attemptNum, int numOfRetries)
        {
            Logger.WithLogSection(
                string.Format("({1}/{2}) Checking if WinRM (Remote PowerShell) can be used to reach remote server [{0}]...",
                              instance.ManagementAddress, attemptNum, numOfRetries), () =>
                              {
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
                                      if (numOfRetries > 0)
                                      {
                                          Logger.Info(string.Format("Unable to reach server [{0}] using WinRM (Remote PowerShell)",
                                              instance.ManagementAddress));
                                          Logger.Info("Waiting 30 seconds before retry...");
                                          Thread.Sleep(30000);
                                          HaveAccessToServer(instance, ++attemptNum, --numOfRetries);
                                      }
                                      else
                                      {
                                          Logger.Error(string.Format("Unable to reach server [{0}] using WinRM (Remote PowerShell)",
                                              instance.ManagementAddress));
                                          Logger.Error(string.Format("Details: {0}", errorMessage));
                                          Logger.Error("Max number of retries exceeded. Please check your Amazon Network firewall for why WinRM cannot connect.");
                                      }
                                  }
                              });
        }

    }
}