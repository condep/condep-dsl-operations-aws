using System.IO;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using Amazon.S3.Model;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.S3
{
    internal class AwsS3UploadOperation : LocalOperation
    {
        private readonly string _srcFile;
        private readonly string _bucket;
        private readonly AwsS3UploadOptionsBuilder _options;

        public AwsS3UploadOperation(string srcFile, string bucket, AwsS3UploadOptionsBuilder options)
        {
            _srcFile = srcFile;
            _bucket = bucket;
            _options = options;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;
            var fileName = Path.GetFileName(_srcFile);
 
            var client = new Amazon.S3.AmazonS3Client(GetAwsCredentials(dynamicAwsConfig), RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region));
            var objRequest = new PutObjectRequest
            {
                BucketName = _bucket,
                Key =
                    string.IsNullOrWhiteSpace(_options.Values.TargetFolderPath)
                        ? fileName
                        : Path.Combine(_options.Values.TargetFolderPath, fileName).Replace('\\', '/'),
                FilePath = _srcFile
            };

            var response = client.PutObject(objRequest);
            return Result(response);
        }

        private static Result Result(PutObjectResponse response)
        {
            var result = Dsl.Result.SuccessChanged();
            result.Data.HttpStatusCode = response.HttpStatusCode;
            result.Data.Expiration = response.Expiration;
            result.Data.ServerSideEncryptionKeyManagementServiceKeyId = response.ServerSideEncryptionKeyManagementServiceKeyId;
            result.Data.ServerSideEncryptionMethod = response.ServerSideEncryptionMethod;
            result.Data.VersionId = response.VersionId;
            return result;
        }

        public override string Name
        {
            get { return "Aws S3 Upload"; }
        }

        private BasicAWSCredentials GetAwsCredentials(dynamic dynamicAwsConfig)
        {
            return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
        }
    }
}