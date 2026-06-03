using Amazon.S3;
using Amazon.S3.Model;
using IndividualWorkAPI.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IndividualWorkAPI.Services;

public class S3ContainerService : IS3ContainerService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3ContainerService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client), "S3 Client is null");
        
        _bucketName = configuration["S3:BucketName"] 
                      ?? throw new InvalidOperationException("AWS:BucketName not configured");
    }

    public async Task<string> GeneratePresignedPutUrlAsync(string key, string contentType, int expirationMinutes)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,  
            Key = key,                
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Verb = HttpVerb.PUT,
            ContentType = contentType,
           
        };

        return await _s3Client.GetPreSignedURLAsync(request); 
    }

    public async Task<string> GeneratePresignedGetUrlAsync(string key, int expirationMinutes = 60)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Verb = HttpVerb.GET
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<bool> DeleteObjectAsync(string key)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting S3 object: {ex.Message}");
            return false;
        }
    }
    
}