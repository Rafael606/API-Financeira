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

    public Task<bool> CriarContaAsync(Conta conta) =>
        _contaRepository.CriarAsync(conta);

    public Task<Conta?> BuscarContaAsync(string cpf, string numeroConta) =>
        _contaRepository.BuscarAsync(cpf, numeroConta);

    public Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite) =>
        _contaRepository.AtualizarLimiteAsync(cpf, numeroConta, novoLimite);

    public Task<bool> RemoverContaAsync(string cpf, string numeroConta) =>
        _contaRepository.RemoverAsync(cpf, numeroConta);
}