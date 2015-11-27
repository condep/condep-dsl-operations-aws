namespace ConDep.Dsl
{
    public interface IOfferAwsS3UploadOptions
    {
        /// <summary>
        /// Optionally set which folder path in S3 you want your file to be put in
        /// </summary>
        /// <param name="path">The S3 folder path (The Key in the API - will be path + srcFileName)</param>
        /// <returns></returns>
        IOfferAwsS3UploadOptions TargetFolderPath(string path);
    }
}