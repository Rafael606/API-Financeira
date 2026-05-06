using PixApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace PixApi.Repositories;

public interface IContaRepository
{
    Task<Conta?> BuscarAsync(string cpf, string numeroConta);
    Task<bool> CriarAsync(Conta conta);
    Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite);
    Task<bool> RemoverAsync(string cpf, string numeroConta);
}

