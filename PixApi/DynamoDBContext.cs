using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon;

namespace PixApi.Infrastructure;

public class DynamoDbContext
{
    public AmazonDynamoDBClient Client { get; }

    public DynamoDbContext()
    {
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            RegionEndpoint = RegionEndpoint.USEast1 // importante mesmo no local
        };

        var credentials = new BasicAWSCredentials("fakeKey", "fakeSecret");

        Client = new AmazonDynamoDBClient(credentials, config);
    }
}