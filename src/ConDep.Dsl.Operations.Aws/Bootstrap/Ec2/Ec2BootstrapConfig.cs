using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Aws.Bootstrap.Ec2
{
    public class Ec2BootstrapConfig
    {
        private readonly List<Ec2Instance> _instances = new List<Ec2Instance>();

        public Ec2BootstrapConfig(string bootstrapId)
        {
            BootstrapId = bootstrapId;
        }

        public string BootstrapId { get; private set; }

        public List<Ec2Instance> Instances
        {
            get { return _instances; }
        }

        public string VpcId { get; set; }
        public string AwsProfileName { get; set; }
        public string SecurityGroupId { get; set; }
        public string SecurityGroupTag { get; set; }
    }
}