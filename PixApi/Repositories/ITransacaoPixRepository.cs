using PixApi.Models;
using PixApi.Services;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace PixApi.Repositories;

public interface ITransacaoPixRepository
{
    Task SalvarAsync(TransacaoPix transacao, ResultadoTransacao resultado);
}