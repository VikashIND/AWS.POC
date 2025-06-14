using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StudentManager;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var tableName = "students"; // Ensure this matches your DynamoDB table name
        AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        DynamoDBContext dBContext = new DynamoDBContext(client);

        if (request.RouteKey.Contains("GET /"))
        {
            try
            {

                var data = await dBContext.ScanAsync<Student>(new List<ScanCondition>()).GetRemainingAsync();
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 200,
                    Body = System.Text.Json.JsonSerializer.Serialize(data),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        else if (request.RouteKey.Contains("POST /"))
        {
            var student = System.Text.Json.JsonSerializer.Deserialize<Student>(request.Body);
            if (student != null)
            {
                await dBContext.SaveAsync(student);
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 201,
                    Body = System.Text.Json.JsonSerializer.Serialize(student),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }

        // Ensure all code paths return a value
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 400,
            Body = "Invalid request",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };
    }
    public async Task<bool> TableExistsAsync(string tableName)
    {
        var client = new AmazonDynamoDBClient();
        try
        {
            var response = await client.DescribeTableAsync(tableName);
            return response.Table.TableStatus == "ACTIVE";
        }
        catch (ResourceNotFoundException)
        {
            return false;
        }
    }
}
