using PixApi.Models;

namespace PixApi.Services;

/// <summary>
/// Implementação do serviço de contas usando armazenamento em memória.
/// </summary>
public class ContaService : IContaService
{
    // Dicionário em memória que guarda contas com chave composta por CPF e número da conta.
    private readonly Dictionary<string, Conta> _contas = new();

    /// <summary>
    /// Cria uma nova conta. Retorna false se já existir.
    /// </summary>
    public Task<bool> CriarContaAsync(Conta conta)
    {
        // Cria uma chave única para a conta usando CPF e número da conta.
        var chave = $"{conta.Cpf}_{conta.NumeroConta}";
        if (_contas.ContainsKey(chave))
        {
            // Conta já existe, não cria novamente.
            return Task.FromResult(false);
        }

        // Adiciona a conta ao armazenamento em memória.
        _contas[chave] = conta;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Busca uma conta por CPF e número da conta.
    /// </summary>
    public Task<Conta?> BuscarContaAsync(string cpf, string numeroConta)
    {
        // Gera a mesma chave usada no cadastro.
        var chave = $"{cpf}_{numeroConta}";
        _contas.TryGetValue(chave, out var conta);

        // Retorna a conta ou null se não existir.
        return Task.FromResult(conta);
    }

    /// <summary>
    /// Atualiza o limite de uma conta.
    /// </summary>
    public Task<bool> AtualizarLimiteAsync(string cpf, string numeroConta, decimal novoLimite)
    {
        // Busca a conta pelo identificador.
        var chave = $"{cpf}_{numeroConta}";
        if (_contas.TryGetValue(chave, out var conta))
        {
            // Atualiza apenas o campo de limite disponível.
            conta.LimiteDisponivel = novoLimite;
            return Task.FromResult(true);
        }

        // Conta não encontrada.
        return Task.FromResult(false);
    }

    /// <summary>
    /// Remove uma conta.
    /// </summary>
    public Task<bool> RemoverContaAsync(string cpf, string numeroConta)
    {
        var chave = $"{cpf}_{numeroConta}";
        // Remove a conta e retorna se a operação foi bem-sucedida.
        return Task.FromResult(_contas.Remove(chave));
    }
}