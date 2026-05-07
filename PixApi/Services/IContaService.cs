using PixApi.Models;

namespace PixApi.Services;

public interface IContaService
{
    Task<bool> CriarContaAsync(Conta conta);

    Task<Conta?> BuscarContaAsync(string cpf, string numeroConta);

    Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite);

    Task<bool> RemoverContaAsync(string cpf, string numeroConta);
}

