﻿using System.Linq;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Aws.S3
{
    internal class AwsS3DeleteBucketOperation : LocalOperation
    {
        private readonly string _bucket;

        public AwsS3DeleteBucketOperation(string bucket)
        {
            _bucket = bucket;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var dynamicAwsConfig = settings.Config.OperationsConfig.Aws;
            var client = new Amazon.S3.AmazonS3Client(GetAwsCredentials(dynamicAwsConfig), RegionEndpoint.GetBySystemName((string)dynamicAwsConfig.Region));

            Logger.Verbose("Getting all buckets from S3");
            var listBucketsResponse = client.ListBuckets();
            Logger.Verbose("{0} buckets found in S3", listBucketsResponse.Buckets.Count);

            Logger.Verbose("Checking if bucket exist in S3");
            if (!listBucketsResponse.Buckets.Any(x => x.BucketName.Equals(_bucket)))
            {
                Logger.Info("Bucket {0} is allready removed from Amazon S3", _bucket);
                return Result.SuccessUnChanged();
            }

            Logger.Verbose("Bucket {0} found in S3", _bucket);
            Logger.Info("Deleting bucket {0} from Amazon S3", _bucket);
            client.DeleteBucket(_bucket);
            return Result.SuccessChanged();
        }

        public override string Name
        {
            get { return "Delete S3 Bucket"; }
        }

        private BasicAWSCredentials GetAwsCredentials(dynamic dynamicAwsConfig)
        {
            return new BasicAWSCredentials((string)dynamicAwsConfig.Credentials.AccessKey, (string)dynamicAwsConfig.Credentials.SecretKey);
        }
    }
}