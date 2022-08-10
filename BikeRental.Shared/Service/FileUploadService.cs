using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Shared.Service;

public class FileUploadService
{
    public static async Task<string> UploadBase64Image(string imageBase64, string fileName)
    {
        var credentials = new BasicAWSCredentials("AKIA2JUZUHJXXYGN5HW6", "sq5204rfq6zNWot1j9Ekf7W56ExM7h1U3B0q3Dyd");
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
