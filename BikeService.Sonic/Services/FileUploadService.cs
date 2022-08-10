using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace BikeService.Sonic.Services;

public class FileUploadService
{
    public static async Task<string> UploadBase64Image(string imageBase64, string fileName)
    {
        var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
        var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        var s3Client = new AmazonS3Client(credentials);
        await using var inputStream = new MemoryStream(Convert.FromBase64String(imageBase64));
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            InputStream = inputStream,
            ContentType = "application/jpg",
            BucketName = "bike-rental-fe",
            Key = $"Images/{fileName}.png",
            CannedACL = S3CannedACL.BucketOwnerFullControl
        });

        return $"https://bike-rental-fe.s3.amazonaws.com/Images/{fileName}.png";
    }
}
