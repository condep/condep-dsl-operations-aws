namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public class AwsBootstrapMandatoryCredentials
    {
        public bool UseProfile { get; set; }
        public string ProfileName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}