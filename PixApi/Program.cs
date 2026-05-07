using PixApi.Services;
using PixApi.Repositories;
using Amazon.DynamoDBv2;

var builder = WebApplication.CreateBuilder(args);

// Registra o cliente do DynamoDB via AWS SDK
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var config = new AmazonDynamoDBConfig
    {
        ServiceURL = "http://localhost:8000"
    };

    return new AmazonDynamoDBClient(
        new Amazon.Runtime.BasicAWSCredentials("fake", "fake"),
        config
    );
});

// Registra os repositórios e serviços
builder.Services.AddSingleton<IContaRepository, ContaRepositoryDynamo>();
builder.Services.AddSingleton<ITransacaoPixRepository, TransacaoPixRepositoryDynamo>();
builder.Services.AddSingleton<IContaService, ContaService>();
builder.Services.AddSingleton<ITransacaoPixService, TransacaoPixService>();
builder.Services.AddControllers();

var app = builder.Build();

// Força redirecionamento de HTTP para HTTPS para mais segurança.
app.UseHttpsRedirection();

// Mapeia as rotas dos controllers para o pipeline HTTP.
app.MapControllers();

app.Run();
