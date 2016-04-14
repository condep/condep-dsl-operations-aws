using System;
using Amazon.S3.Model;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.S3;

// ReSharper disable once CheckNamespace
namespace ConDep.Dsl
{
    /// <summary>
    /// Expose operations for Amazon S3
    /// </summary>
    public static class AwsS3Extensions
    {
        /// <summary>
        /// Will upload a file to Amazon S3
        /// </summary>
        /// <param name="s3"></param>
        /// <param name="srcFile">The source file to upload</param>
        /// <param name="targetBucket">The Amazon S3 bucket to upload file to</param>
        /// <param name="options">Additional options</param>
        /// <returns></returns>
        public static IOfferAwsOperations Upload(this IOfferAwsS3Operations s3, string srcFile, string targetBucket, Action<IOfferAwsS3UploadOptions> options = null)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;

            var opt = new AwsS3UploadOptionsBuilder();
            if (options != null) options(opt);

            var operation = new AwsS3UploadOperation(srcFile, targetBucket, opt);
            OperationExecutor.Execute((LocalBuilder)s3, operation);
            return s3Builder.AwsOperations;
        }

        /// <summary>
        /// Will download a file from Amazon S3
        /// </summary>
        /// <param name="s3"></param>
        /// <param name="bucket">The Amazon S3 bucket where the file is located</param>
        /// <param name="key">The Amazon S3 key (path) for the file to download</param>
        /// <param name="dstFile">The destination file path to download to</param>
        /// <returns></returns>
        public static IOfferAwsOperations Download(this IOfferAwsS3Operations s3, string bucket, string key, string dstFile)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;

            var operation = new AwsS3DownloadOperation(bucket, key, dstFile);
            OperationExecutor.Execute((LocalBuilder)s3, operation);
            return s3Builder.AwsOperations;
        }

        /// <summary>
        /// Will create a new bucket in Amazon S3
        /// </summary>
        /// <param name="s3"></param>
        /// <param name="bucket">Name of the Amazon S3 bucket</param>
        /// <returns></returns>
        public static IOfferAwsOperations CreateBucket(this IOfferAwsS3Operations s3, string bucket)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;

            var operation = new AwsS3CreateBucketOperation(bucket);
            OperationExecutor.Execute((LocalBuilder)s3, operation);
            return s3Builder.AwsOperations;
        }

        /// <summary>
        /// Will delete a Amazon S3 bucket
        /// </summary>
        /// <param name="s3"></param>
        /// <param name="bucket">Name of the bucket to delete</param>
        /// <returns></returns>
        public static IOfferAwsOperations DeleteBucket(this IOfferAwsS3Operations s3, string bucket)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;

            var operation = new AwsS3DeleteBucketOperation(bucket);
            OperationExecutor.Execute((LocalBuilder)s3, operation);
            return s3Builder.AwsOperations;
        }

        /// <summary>
        /// Will delete a object/file from a Amazon S3 bucket
        /// </summary>
        /// <param name="s3"></param>
        /// <param name="bucket">The Amazon S3 bucket where the object/file is located</param>
        /// <param name="key">The Amazon S3 key (path) to the object/file to delete</param>
        /// <returns></returns>
        public static IOfferAwsOperations DeleteObject(this IOfferAwsS3Operations s3, string bucket, string key)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;

            var operation = new AwsS3DeleteObjectOperation(bucket, key);
            OperationExecutor.Execute((LocalBuilder)s3, operation);
            return s3Builder.AwsOperations;
        }
    }
}