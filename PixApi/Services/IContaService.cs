using PixApi.Models;

namespace PixApi.Services;

public interface IContaService
{
    /// Cria uma nova conta.
    Task<bool> CriarContaAsync(Conta conta);

    /// Busca uma conta por CPF e número da conta.
    Task<Conta?> BuscarContaAsync(string cpf, string numeroConta);

    /// Atualiza o limite de uma conta.
    Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite);

    /// Remove uma conta.
    Task<bool> RemoverContaAsync(string cpf, string numeroConta);
}

