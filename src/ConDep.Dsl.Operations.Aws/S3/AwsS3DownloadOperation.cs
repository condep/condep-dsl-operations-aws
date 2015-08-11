using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.S3
{
    public class AwsS3DownloadOperation : LocalOperation
    {
        private readonly string _bucket;
        private readonly string _key;
        private readonly string _dstFile;

        public AwsS3DownloadOperation(string bucket, string key, string dstFile)
        {
            _bucket = bucket;
            _key = key;
            _dstFile = dstFile;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            var client = new Amazon.S3.AmazonS3Client(GetAwsCredentials(dynamicAwsConfig), RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region));

            var response = client.GetObject(_bucket, _key);
            response.WriteResponseStreamToFile(_dstFile);
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Aws S3 Download"; }
        }

        private BasicAWSCredentials GetAwsCredentials(dynamic dynamicAwsConfig)
        {
            return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
        }
    }
}