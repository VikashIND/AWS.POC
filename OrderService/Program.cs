using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using OrderService.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapPost("/", async (CreateOrderRequest request, IAmazonSimpleNotificationService sns) =>
{
    try
    {
        // assume the incoming request is processed and saved to the database.

        // create notification
        var notification = new OrderCreatedNotification(request.OrderId, request.CustomerId, request.ProductDetails);

        // create topic if needed
        var response = await sns.ListTopicsAsync();
        var topicName = "OrderCreated";
        Topic topicExists = null;
        var topicArn = string.Empty;
        if(response.Topics !=null)
         topicExists = await sns.FindTopicAsync(topicName);
        if (topicExists != null)
        {
            topicArn = topicExists.TopicArn;
        }
        else
        {
            var newTopic = await sns.CreateTopicAsync(topicName);
            topicArn = newTopic.TopicArn;
        }

        // create publish request
        var publishRequest = new PublishRequest()
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(notification),
            Subject = $"Order#{request.OrderId}",
            MessageAttributes = new Dictionary<string, MessageAttributeValue>()

        };

        publishRequest.MessageAttributes.Add("Scope", new MessageAttributeValue()
        {
            DataType = "String",
            StringValue = "Lambda"
        });

        await sns.PublishAsync(publishRequest);
    }
    catch (Exception ex)
    {

        throw ex;
    }
    
});
app.Run();
