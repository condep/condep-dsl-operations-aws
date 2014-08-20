using System.Collections.Generic;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws;
using ConDep.Dsl.Operations.Aws.Bootstrap.Ec2;

namespace ConDep.Dsl.Operations.Aws.Terminate
{
    public class Ec2Terminator
    {
        private readonly AwsBootstrapMandatoryInputValues _mandatoryOptions;
        private readonly IAmazonEC2 _client;
        private Ec2InstanceHandler _instanceHandler;

        public Ec2Terminator(AwsBootstrapMandatoryInputValues mandatoryOptions)
        {
            _mandatoryOptions = mandatoryOptions;

            AWSCredentials creds = _mandatoryOptions.Credentials.UseProfile ? (AWSCredentials)new StoredProfileAWSCredentials(_mandatoryOptions.Credentials.ProfileName) : new BasicAWSCredentials(_mandatoryOptions.Credentials.AccessKey, _mandatoryOptions.Credentials.SecretKey);
            _client = AWSClientFactory.CreateAmazonEC2Client(creds);

            _instanceHandler = new Ec2InstanceHandler(_client);
        }

        public void Terminate()
        {
            _instanceHandler.Terminate(_mandatoryOptions.BootstrapId);
        }
    }
}