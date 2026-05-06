using PixApi.Models;
using PixApi.Repositories;

namespace PixApi.Services;


public class ContaService : IContaService
{
    private readonly IContaRepository _contaRepository;

    public ContaService(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    /// <summary>
    /// Cria uma nova conta. Retorna false se já existir.
    /// </summary>
    public Task<bool> CriarContaAsync(Conta conta) =>
        _contaRepository.CriarAsync(conta);

    /// <summary>
    /// Busca uma conta por CPF e número da conta.
    /// </summary>
    public Task<Conta?> BuscarContaAsync(string cpf, string numeroConta) =>
        _contaRepository.BuscarAsync(cpf, numeroConta);

    /// <summary>
    /// Atualiza o limite de uma conta.
    /// </summary>
    public Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite) =>
        _contaRepository.AtualizarLimiteAsync(cpf, numeroConta, novoLimite);

    /// <summary>
    /// Remove uma conta.
    /// </summary>
    public Task<bool> RemoverContaAsync(string cpf, string numeroConta) =>
        _contaRepository.RemoverAsync(cpf, numeroConta);
}