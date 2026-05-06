using PixApi.Services;
using PixApi.Repositories;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
// Força redirecionamento de HTTP para HTTPS para mais segurança.
app.UseHttpsRedirection();

// Mapeia as rotas dos controllers para o pipeline HTTP.
app.MapControllers();

app.Run();
