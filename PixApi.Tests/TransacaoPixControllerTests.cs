using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PixApi.Controllers;
using PixApi.Models;
using PixApi.Services;

public class TransacaoPixControllerTests
{
    private readonly Mock<ITransacaoPixService> _serviceMock;
    private readonly TransacaoPixController _controller;

    public TransacaoPixControllerTests()
    {
        _serviceMock = new Mock<ITransacaoPixService>();
        _controller = new TransacaoPixController(_serviceMock.Object);
    }

    // =====================================================
    // POST - SUCESSO
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Retornar_Ok_Quando_Aprovada()
    {
        var transacao = new TransacaoPix
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            Valor = 100
        };

        _serviceMock
            .Setup(x => x.ProcessarTransacaoAsync(transacao))
            .ReturnsAsync(new ResultadoTransacao
            {
                Aprovada = true,
                Mensagem = "Transação aprovada."
            });

        var resultado = await _controller.ProcessarTransacao(transacao);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var body = Assert.IsType<ResultadoTransacao>(ok.Value);

        Assert.True(body.Aprovada);
    }

    // =====================================================
    // POST - INSUCESSO (conta não encontrada)
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Retornar_Falha_Conta_Nao_Existe()
    {
        var transacao = new TransacaoPix
        {
            Cpf = "99999999999",
            NumeroConta = "99999",
            Valor = 100
        };

        _serviceMock
            .Setup(x => x.ProcessarTransacaoAsync(transacao))
            .ReturnsAsync(new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Conta não encontrada."
            });

        var resultado = await _controller.ProcessarTransacao(transacao);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var body = Assert.IsType<ResultadoTransacao>(ok.Value);

        Assert.False(body.Aprovada);
    }

    // =====================================================
    // POST - INSUCESSO (limite insuficiente)
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Retornar_Falha_Limite_Insuficiente()
    {
        var transacao = new TransacaoPix
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            Valor = 999999
        };

        _serviceMock
            .Setup(x => x.ProcessarTransacaoAsync(transacao))
            .ReturnsAsync(new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Limite insuficiente."
            });

        var resultado = await _controller.ProcessarTransacao(transacao);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var body = Assert.IsType<ResultadoTransacao>(ok.Value);

        Assert.False(body.Aprovada);
    }

    // =====================================================
    // POST - INSUCESSO (validação Valor negativo)
    // =====================================================

    [Fact]
    public async Task Processar_Transacao_Deve_Retornar_BadRequest_Quando_ValorNegativo_Invalido()
    {
        _controller.ModelState.AddModelError("Valor", "Valor inválido");

        var transacao = new TransacaoPix
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            Valor = -10
        };

        var resultado = await _controller.ProcessarTransacao(transacao);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }
}