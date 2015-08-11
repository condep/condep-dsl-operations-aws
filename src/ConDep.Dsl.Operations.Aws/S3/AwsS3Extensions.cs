using System;
using ConDep.Dsl.Operations.Aws;
using ConDep.Dsl.Operations.Aws.S3;

namespace ConDep.Dsl
{
    public static class AwsS3Extensions
    {
        public static IOfferAwsOperations Upload(this IOfferAwsS3Operations s3, string srcFile, string targetBucket, Action<IOfferAwsS3UploadOptions> options = null)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;
            var awsOpsBuilder = s3Builder.AwsOperations as AwsOperationsBuilder;

            var opt = new AwsS3UploadOptionsBuilder();
            if (options != null) options(opt);

            var operation = new AwsS3UploadOperation(srcFile, targetBucket, opt);
            Configure.Operation(awsOpsBuilder.LocalOperations, operation);
            return s3Builder.AwsOperations;
        }

        public static IOfferAwsOperations Download(this IOfferAwsS3Operations s3, string bucket, string key, string dstFile)
        {
            var s3Builder = s3 as AwsS3OperationsBuilder;
            var awsOpsBuilder = s3Builder.AwsOperations as AwsOperationsBuilder;

            var operation = new AwsS3DownloadOperation(bucket, key, dstFile);
            Configure.Operation(awsOpsBuilder.LocalOperations, operation);
            return s3Builder.AwsOperations;
        }
    }

    public interface IOfferAwsS3UploadOptions
    {
        /// <summary>
        /// Optionally set which folder path in S3 you want your file to be put in
        /// </summary>
        /// <param name="path">The S3 folder path (The Key in the API - will be path + srcFileName)</param>
        /// <returns></returns>
        IOfferAwsS3UploadOptions TargetFolderPath(string path);
    }

    class AwsS3UploadOptionsBuilder : IOfferAwsS3UploadOptions
    {
        AwsS3UploadOptionsBuilderValues _values = new AwsS3UploadOptionsBuilderValues();

        public IOfferAwsS3UploadOptions TargetFolderPath(string path)
        {
            _values.TargetFolderPath = path;
            return this;
        }

        internal class AwsS3UploadOptionsBuilderValues
        {
            public string TargetFolderPath { get; set; }
        }

        public AwsS3UploadOptionsBuilderValues Values { get { return _values; } }
    }
}