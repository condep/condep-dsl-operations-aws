using System;
using System.Net;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.S3
{
    internal class AwsS3DeleteObjectOperation : LocalOperation
    {
        private readonly string _bucket;
        private readonly string _key;

        public AwsS3DeleteObjectOperation(string bucket, string key)
        {
            _bucket = bucket;
            _key = key;
        }

        public override void Execute(IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;

            using (var client = new Amazon.S3.AmazonS3Client(GetAwsCredentials(dynamicAwsConfig), RegionEndpoint.GetBySystemName((string) dynamicAwsConfig.Region)))
            {
                Logger.Verbose("Trying to get object metadata from S3");
                try
                {
                    var obj = client.GetObjectMetadata(_bucket, _key);
                    if (obj.HttpStatusCode != HttpStatusCode.OK)
                    {
                        Logger.Verbose("Failed to get Amazon S3 Object metadata. Http status code was {0}", obj.HttpStatusCode);
                        Logger.Info("Could not find Amazon S3 Object {0} in bucket {1}- assuming allready deleted.", _key, _bucket);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Verbose("Failed to get Amazon S3 Object metadata. Exception was:");
                    Logger.Verbose(ex.Message);
                    Logger.Warn("Exception during Amazon S3 Object lookup. Assuming object allready deleted.");
                }

                Logger.Info("Deleting Amazon S3 Object with key {0} in Bucket {1}", _key, _bucket);
                client.DeleteObject(_bucket, _key);
            }

        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name { get { return "Delete S3 Object"; } }

        private BasicAWSCredentials GetAwsCredentials(dynamic dynamicAwsConfig)
        {
            return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
        }
    }
}