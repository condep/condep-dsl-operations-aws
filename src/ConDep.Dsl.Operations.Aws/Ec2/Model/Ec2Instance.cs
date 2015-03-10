using Amazon.EC2.Model;

namespace ConDep.Dsl
{
    internal class Ec2Instance
    {
        public string InstanceId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Instance AwsInstance { get; set; }
        public string ManagementAddress { get; set; }
    }
}