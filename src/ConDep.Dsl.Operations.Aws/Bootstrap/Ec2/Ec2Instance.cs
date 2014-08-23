using System.Collections.Generic;
using Amazon.EC2.Model;

namespace ConDep.Dsl.Operations.Aws.Bootstrap.Ec2
{
    public class Ec2Instance
    {
        public string InstanceId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Instance AwsInstance { get; set; }
        public string ManagementAddress { get; set; }
    }
}