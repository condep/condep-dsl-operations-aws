namespace ConDep.Dsl.Operations.Aws
{
    internal class AwsCredentials
    {
        public bool UseProfile { get { return !string.IsNullOrEmpty(ProfileName); } }
        public string ProfileName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}