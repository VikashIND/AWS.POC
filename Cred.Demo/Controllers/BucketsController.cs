using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace Cred.Demo.Controllers
{
    public class BucketsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        public BucketsController(IConfiguration configuration, IAmazonS3 amazonS3)
        {
            _configuration = configuration;
            _s3Client = amazonS3;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
                var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
                var region = _configuration.GetValue<string>("AWS:Region");
                var s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
                //var s3Client = new AmazonS3Client("AKIAYNQXHALNHSUU7TZN", "cSvZ/pS9/pLm2cO1Q85IH2sYDEiNdjKx7F88jVri", Amazon.RegionEndpoint.USEast1);
                var data = await s3Client.ListBucketsAsync();
                var buckets = data.Buckets.Select(b => { return b.BucketName; });
                return Ok(buckets);
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        [HttpGet("list2")]
        public async Task<IActionResult> List2Async()
        {
            try
            {
                var data = await _s3Client.ListBucketsAsync();
                var buckets = data.Buckets.Select(b => { return b.BucketName; });
                return Ok(buckets);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
