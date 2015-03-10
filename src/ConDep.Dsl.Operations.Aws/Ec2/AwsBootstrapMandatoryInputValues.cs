using Amazon;

namespace ConDep.Dsl
{
    public class AwsBootstrapMandatoryInputValues
    {
        private readonly string _bootstrapId;
        private readonly AwsBootstrapMandatoryCredentials _credentials = new AwsBootstrapMandatoryCredentials();

        public AwsBootstrapMandatoryInputValues(string bootstrapId)
        {
            _bootstrapId = bootstrapId;
        }

        public AwsBootstrapMandatoryCredentials Credentials { get { return _credentials; } }
        public string PublicKeyName { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public string SubnetId { get; set; }
        public string Region { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public string BootstrapId { get { return _bootstrapId; } }
    }
}