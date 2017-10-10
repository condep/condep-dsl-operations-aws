using Amazon.EC2;
using Amazon.EC2.Model;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Aws.Ec2.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConDep.Dsl.Operations.Aws.Ec2.Handlers
{
    class Ec2Starter
    {
        private readonly AwsStartOptionsValues _options;
        private readonly ConDepSettings _settings;
        private readonly IAmazonEC2 _client;
        private readonly Ec2InstanceHandler _instanceHandler;
        private readonly Ec2InstancePasswordHandler _passwordHandler;

        public Ec2Starter(AwsStartOptionsValues options, ConDepSettings settings)
        {
            _options = options;
            _settings = settings;
            var config = new AmazonEC2Config { RegionEndpoint = _options.RegionEndpoint };
            _client = new AmazonEC2Client(_options.Credentials, config);
            _instanceHandler = new Ec2InstanceHandler(_client);
            _passwordHandler = new Ec2InstancePasswordHandler(_client);
        }

        public IEnumerable<Ec2Instance> Start()
        {
            var bootstrapId = _options.InstanceRequest.ClientToken;
            IEnumerable<Instance> instances = _instanceHandler.GetInstances(bootstrapId);

            List<Tuple<string, string>> existingPasswords = null;

            if (!_settings.Config.DeploymentUser.IsDefined())
            {
                existingPasswords = _passwordHandler.WaitForPassword(instances.Select(x => x.InstanceId),
                    _options.PrivateKeyFileLocation);
            }

            List<Ec2Instance> ec2instances = new List<Ec2Instance>();
            foreach(var instance in instances)
            {
                ec2instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    UserName = _settings.Config.DeploymentUser.IsDefined() ? _settings.Config.DeploymentUser.UserName : ".\\Administrator",
                    Password = _settings.Config.DeploymentUser.IsDefined() ? _settings.Config.DeploymentUser.Password : existingPasswords.Single(x => x.Item1 == instance.InstanceId).Item2,
                    AwsInstance = instance,
                    ManagementAddress = _instanceHandler.GetManagementAddress(instance)
                });
            }
            _instanceHandler.Start(bootstrapId);
            
            return ec2instances;
        }
    }
}
