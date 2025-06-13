using Amazon.Lambda.Core;
using Amazon.S3;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.Demo;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> FunctionHandler(ILambdaContext context)
    {
        var s3Client = new AmazonS3Client();
        var data = await s3Client.ListBucketsAsync();
        var buckets = data.Buckets.Select(b => b.BucketName);
        return buckets;
        
    }
}
