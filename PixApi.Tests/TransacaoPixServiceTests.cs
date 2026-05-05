using Xunit;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Tests;

/// <summary>
/// Testes para o serviço de transações PIX.
/// </summary>
public class TransacaoPixServiceTests
{
    private readonly IContaService _contaService;
    private readonly ITransacaoPixService _transacaoService;

    public TransacaoPixServiceTests()
    {
        _contaService = new ContaService();
        _transacaoService = new TransacaoPixService(_contaService);
    }

    [Fact]
    public async Task ProcessarTransacaoAsync_TransacaoAprovada_DeveDescontarLimite()
    {
        // Arrange
        var conta = new Conta
        {
            Cpf = "12345678901",
            Agencia = "0001",
            NumeroConta = "12345",
            LimiteDisponivel = 1000
        };
        await _contaService.CriarContaAsync(conta);

        var transacao = new TransacaoPix
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            Valor = 500
        };

        // Act
        var resultado = await _transacaoService.ProcessarTransacaoAsync(transacao);

        // Assert
        Assert.True(resultado.Aprovada);
        Assert.Equal("Transação aprovada.", resultado.Mensagem);

        var contaAtualizada = await _contaService.BuscarContaAsync("12345678901", "12345");
        Assert.Equal(500, contaAtualizada?.LimiteDisponivel);
    }

    [Fact]
    public async Task ProcessarTransacaoAsync_LimiteInsuficiente_DeveNegar()
    {
        // Arrange
        var conta = new Conta
        {
            Cpf = "12345678901",
            Agencia = "0001",
            NumeroConta = "12345",
            LimiteDisponivel = 100
        };
        await _contaService.CriarContaAsync(conta);

        var transacao = new TransacaoPix
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            Valor = 200
        };

        // Act
        var resultado = await _transacaoService.ProcessarTransacaoAsync(transacao);

        // Assert
        Assert.False(resultado.Aprovada);
        Assert.Equal("Limite insuficiente.", resultado.Mensagem);

        var contaAtualizada = await _contaService.BuscarContaAsync("12345678901", "12345");
        Assert.Equal(100, contaAtualizada?.LimiteDisponivel); // Limite não alterado
    }

    [Fact]
    public async Task ProcessarTransacaoAsync_ContaNaoEncontrada_DeveNegar()
    {
        // Arrange
        var transacao = new TransacaoPix
        {
            Cpf = "99999999999",
            NumeroConta = "99999",
            Valor = 100
        };

        // Act
        var resultado = await _transacaoService.ProcessarTransacaoAsync(transacao);

        // Assert
        Assert.False(resultado.Aprovada);
        Assert.Equal("Conta não encontrada.", resultado.Mensagem);
    }
}