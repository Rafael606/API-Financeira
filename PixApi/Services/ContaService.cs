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

    /// Cria uma nova conta. Retorna false se já existir.
    public Task<bool> CriarContaAsync(Conta conta) =>
        _contaRepository.CriarAsync(conta);

    /// Busca uma conta por CPF e número da conta.
    public Task<Conta?> BuscarContaAsync(string cpf, string numeroConta) =>
        _contaRepository.BuscarAsync(cpf, numeroConta);

    /// Atualiza o limite de uma conta.
    public Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite) =>
        _contaRepository.AtualizarLimiteAsync(cpf, numeroConta, novoLimite);

    /// Remove uma conta.
    public Task<bool> RemoverContaAsync(string cpf, string numeroConta) =>
        _contaRepository.RemoverAsync(cpf, numeroConta);
}