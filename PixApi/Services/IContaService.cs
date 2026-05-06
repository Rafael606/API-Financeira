using PixApi.Models;

namespace PixApi.Services;

/// <summary>
/// Interface para o serviço de contas.
/// </summary>
public interface IContaService
{
    /// <summary>
    /// Cria uma nova conta.
    /// </summary>
    Task<bool> CriarContaAsync(Conta conta);

    /// <summary>
    /// Busca uma conta por CPF e número da conta.
    /// </summary>
    Task<Conta?> BuscarContaAsync(string cpf, string numeroConta);

    /// <summary>
    /// Atualiza o limite de uma conta.
    /// </summary>
    Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite);

    /// <summary>
    /// Remove uma conta.
    /// </summary>
    Task<bool> RemoverContaAsync(string cpf, string numeroConta);
}

