namespace ConDep.Dsl.Operations.Aws.Bootstrap.Ec2
{
    public class Ec2Instance
    {
        public string InstanceId { get; set; }
        public string Tag { get; set; }
        public string PublicDns { get; set; }
        public string PublicIp { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PrivateDns { get; set; }
        public string PrivateIp { get; set; }
    }
}