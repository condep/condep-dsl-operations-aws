using System;
using System.Net;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;

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

        public override Result Execute(ConDepSettings settings, CancellationToken token)
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
                        return Result.SuccessUnChanged();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Verbose("Failed to get Amazon S3 Object metadata. Exception was:");
                    Logger.Verbose(ex.Message);
                    Logger.Warn("Exception during Amazon S3 Object lookup. Assuming object allready deleted.");
                    return Result.Failed();
                }

                Logger.Info("Deleting Amazon S3 Object with key {0} in Bucket {1}", _key, _bucket);
                client.DeleteObject(_bucket, _key);
                return Result.SuccessChanged();
            }

        }

        public override string Name
        {
            get { return "Delete S3 Object"; }
        }

        private BasicAWSCredentials GetAwsCredentials(dynamic dynamicAwsConfig)
        {
            return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
        }
    }
}