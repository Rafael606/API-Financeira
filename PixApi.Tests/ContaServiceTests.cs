using Xunit;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Tests;

/// <summary>
/// Testes para o serviço de contas.
/// </summary>
public class ContaServiceTests
{
    private readonly IContaService _contaService;

    public ContaServiceTests()
    {
        _contaService = new ContaService();
    }

    [Fact]
    public async Task AtualizarLimiteAsync_ContaExistente_DeveAtualizar()
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

        // Act
        var sucesso = await _contaService.AtualizarLimiteAsync("12345678901", "12345", 1500);

        // Assert
        Assert.True(sucesso);
        var contaAtualizada = await _contaService.BuscarContaAsync("12345678901", "12345");
        Assert.Equal(1500, contaAtualizada?.LimiteDisponivel);
    }

    [Fact]
    public async Task AtualizarLimiteAsync_ContaNaoExistente_DeveRetornarFalse()
    {
        // Act
        var sucesso = await _contaService.AtualizarLimiteAsync("99999999999", "99999", 1500);

        // Assert
        Assert.False(sucesso);
    }
}