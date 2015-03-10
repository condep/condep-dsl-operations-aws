namespace ConDep.Dsl
{
    public class AwsBootstrapMandatoryCredentials
    {
        public bool UseProfile { get { return !string.IsNullOrEmpty(ProfileName); } }
        public string ProfileName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}