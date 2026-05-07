using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using PixApi.Models;
using Amazon.DynamoDBv2.Model;
using System.Globalization;

namespace PixApi.Repositories;

public class TransacaoPixRepositoryDynamo : ITransacaoPixRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private const string TableName = "Transacao";

    public TransacaoPixRepositoryDynamo(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task SalvarAsync(TransacaoPix transacao)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "CPF", new AttributeValue { S = transacao.Cpf } },
                { "NumeroConta", new AttributeValue { S = transacao.NumeroConta } },
                { "ValorTransacao", new AttributeValue { N = transacao.Valor.ToString(CultureInfo.InvariantCulture) } },
                { "DataTransacao", new AttributeValue { S = DateTime.UtcNow.AddHours(-3).ToString("o") } }
            }
        };

        await _dynamoDb.PutItemAsync(request);
    }
}