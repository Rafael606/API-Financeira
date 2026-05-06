using PixApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Globalization;

namespace PixApi.Repositories;

public class ContaRepositoryDynamo : IContaRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private const string TableName = "Conta";

    public ContaRepositoryDynamo(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<Conta?> BuscarAsync(string cpf, string numeroConta)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "CPF", new AttributeValue { S = cpf } },
                { "NumeroConta", new AttributeValue { S = numeroConta } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request);

        if (!response.IsItemSet) return null;

        var item = response.Item;

        return new Conta
        {
            Cpf = item["CPF"].S,
            NumeroConta = item["NumeroConta"].S,
            AgenciaConta = item["AgenciaConta"].S,
            LimitePIX = decimal.Parse(item["LimitePIX"].N, CultureInfo.InvariantCulture)
        };
    }

    public async Task<bool> CriarAsync(Conta conta)
    {
        var existente = await BuscarAsync(conta.Cpf, conta.NumeroConta);
        if (existente != null) return false;

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "CPF", new AttributeValue { S = conta.Cpf } },
                { "NumeroConta", new AttributeValue { S = conta.NumeroConta } },
                { "AgenciaConta", new AttributeValue { S = conta.AgenciaConta } },
                { "LimitePIX", new AttributeValue { N = conta.LimitePIX.ToString(CultureInfo.InvariantCulture) } }
            }
        };

        await _dynamoDb.PutItemAsync(request);
        return true;
    }

    public async Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite)
    {
        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "CPF", new AttributeValue { S = cpf } },
                { "NumeroConta", new AttributeValue { S = numeroConta } }
            },
            UpdateExpression = "SET LimitePIX = :limite",
            ConditionExpression = "attribute_exists(CPF) AND attribute_exists(NumeroConta)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":limite", new AttributeValue { N = novoLimite.ToString(CultureInfo.InvariantCulture) } }
            },
        };

        try
        {
            await _dynamoDb.UpdateItemAsync(request);
            return true;
        }
        catch (ConditionalCheckFailedException)
        {
            return false;
        }
    }

    public async Task<bool> RemoverAsync(string cpf, string numeroConta)
    {
        var existente = await BuscarAsync(cpf, numeroConta);
        if (existente == null) return false;

        var request = new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "CPF", new AttributeValue { S = cpf } },
                { "NumeroConta", new AttributeValue { S = numeroConta } }
            }
        };

        await _dynamoDb.DeleteItemAsync(request);
        return true;
    }
}