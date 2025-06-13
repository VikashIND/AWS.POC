using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;

namespace S3.Demo.API.Controllers
{
    public class BucketsController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        public BucketsController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            // Use AmazonS3Util to check if the bucket exists
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");
            await _s3Client.PutBucketAsync(bucketName);
            return Ok($"Bucket {bucketName} created.");
        }
        [HttpGet("list")]
        public async Task<IActionResult> ListBucketsAsync()
        {
            var response = await _s3Client.ListBucketsAsync();
            return Ok(response.Buckets.Select(b => b.BucketName));
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            // Use AmazonS3Util to check if the bucket exists
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            // Delete the bucket
            await _s3Client.DeleteBucketAsync(bucketName);
            return Ok($"Bucket {bucketName} deleted.");
        }
    }
}
