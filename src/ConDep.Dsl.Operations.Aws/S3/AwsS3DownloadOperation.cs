using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Aws.S3
{
    internal class AwsS3DownloadOperation : LocalOperation
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

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            var client = new Amazon.S3.AmazonS3Client(GetAwsCredentials(dynamicAwsConfig), RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region));

            var response = client.GetObject(_bucket, _key);
            response.WriteResponseStreamToFile(_dstFile);

            var result = Result.SuccessUnChanged();
            result.Data.BucketName = response.BucketName;
            result.Data.ContentLength = response.ContentLength;
            result.Data.Expiration = response.Expiration;
            result.Data.Expires = response.Expires;
            result.Data.HttpStatusCode = response.HttpStatusCode;
            result.Data.Key = response.Key;
            result.Data.LastModified = response.LastModified;
            result.Data.ServerSideEncryptionCustomerMethod = response.ServerSideEncryptionCustomerMethod;
            result.Data.ServerSideEncryptionMethod = response.ServerSideEncryptionMethod;
            result.Data.StorageClass = response.StorageClass;
            result.Data.VersionId = response.VersionId;
            result.Data.WebsiteRedirectLocation = response.WebsiteRedirectLocation;
            return result;
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