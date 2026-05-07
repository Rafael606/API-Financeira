using Xunit;
using Moq;
using PixApi.Models;
using PixApi.Services;
using PixApi.Repositories;

public class TransacaoPixServiceTests
{
    private readonly Mock<IContaService> _contaMock;
    private readonly Mock<ITransacaoPixRepository> _repoMock;
    private readonly TransacaoPixService _service;

    public TransacaoPixServiceTests()
    {
        _contaMock = new Mock<IContaService>();
        _repoMock = new Mock<ITransacaoPixRepository>();

        _service = new TransacaoPixService(_contaMock.Object, _repoMock.Object);
    }

    // =====================================================
    // SUCESSO - transação aprovada
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Aprovar_Quando_Limite_Suficiente()
    {
        var transacao = new TransacaoPix
        {
            Cpf = "123",
            NumeroConta = "1",
            Valor = 100
        };

        var conta = new Conta
        {
            Cpf = "123",
            NumeroConta = "1",
            LimitePIX = 500
        };

        _contaMock
            .Setup(x => x.BuscarContaAsync("123", "1"))
            .ReturnsAsync(conta);

        _repoMock
            .Setup(x => x.SalvarAsync(transacao))
            .Returns(Task.CompletedTask);

        _contaMock
            .Setup(x => x.AtualizarLimiteAsync("123", "1", 400))
            .ReturnsAsync(true);

        var resultado = await _service.ProcessarTransacaoAsync(transacao);

        Assert.True(resultado.Aprovada);
        Assert.Equal("Transação aprovada.", resultado.Mensagem);
    }

    // =====================================================
    // INSUCESSO - conta não existe
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Rejeitar_Quando_Conta_Nao_Existe()
    {
        _contaMock
            .Setup(x => x.BuscarContaAsync("123", "1"))
            .ReturnsAsync((Conta)null);

        var transacao = new TransacaoPix
        {
            Cpf = "123",
            NumeroConta = "1",
            Valor = 100
        };

        var resultado = await _service.ProcessarTransacaoAsync(transacao);

        Assert.False(resultado.Aprovada);
        Assert.Equal("Conta não encontrada.", resultado.Mensagem);
    }

    // =====================================================
    // INSUCESSO - limite insuficiente
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Rejeitar_Quando_Limite_Insuficiente()
    {
        var conta = new Conta
        {
            Cpf = "123",
            NumeroConta = "12",
            LimitePIX = 50
        };

        _contaMock
            .Setup(x => x.BuscarContaAsync("123", "1"))
            .ReturnsAsync(conta);

        var transacao = new TransacaoPix
        {
            Cpf = "123",
            NumeroConta = "1",
            Valor = 100
        };

        var resultado = await _service.ProcessarTransacaoAsync(transacao);

        Assert.False(resultado.Aprovada);
        Assert.Equal("Limite insuficiente.", resultado.Mensagem);
    }
}