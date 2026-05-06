using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon;
using Microsoft.Extensions.Configuration;
using PixApi.Models;

namespace PixApi.Repositories;

public class TransacaoPixRepositoryDynamo : ITransacaoPixRepository
{
    private readonly Table _table;

    public TransacaoPixRepositoryDynamo(IConfiguration configuration)
    {
        var awsSection = configuration.GetSection("AWS");
        var accessKey = awsSection["AccessKey"];
        var secretKey = awsSection["SecretKey"];
        var serviceUrl = awsSection["ServiceUrl"];
        var region = awsSection["Region"];

        var credentials = new BasicAWSCredentials(accessKey, secretKey);

        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = serviceUrl,
            AuthenticationRegion = region
        };

        var client = new AmazonDynamoDBClient(credentials, config);

        // v3.7: usa TableBuilder e retorna ITable
        _table = new TableBuilder(client, "Transacoes").Build();
    }

    public async Task SalvarAsync(TransacaoPix transacao, ResultadoTransacao resultado)
    {
        var doc = new Document
        {
            ["Cpf"]          = transacao.Cpf,
            ["NumeroConta"]  = transacao.NumeroConta,
            ["Valor"]        = transacao.Valor,
            ["Timestamp"]    = DateTime.UtcNow.ToString("o")
        };

        await _table.PutItemAsync(doc);
    }

    public async Task<List<TransacaoPix>> ObterPorContaAsync(string cpf, string numeroConta)
    {
        // v3.7: usa ScanFilter em vez de List<ScanCondition>
        var filter = new ScanFilter();
        filter.AddCondition("Cpf", ScanOperator.Equal, cpf);
        filter.AddCondition("NumeroConta", ScanOperator.Equal, numeroConta);

        var search = _table.Scan(filter);
        var documents = await search.GetRemainingAsync();

        var transacoes = new List<TransacaoPix>();
        foreach (var doc in documents)
        {
            transacoes.Add(new TransacaoPix
            {
                Cpf         = doc["Cpf"].AsString(),
                NumeroConta = doc["NumeroConta"].AsString(),
                Valor       = doc["Valor"].AsDecimal()
            });
        }

        return transacoes;
    }
}