using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PixApi.Controllers;
using PixApi.Models;
using PixApi.Services;

public class ContaControllerTests
{
    private readonly Mock<IContaService> _servicoMock;
    private readonly ContaController _controller;

    public ContaControllerTests()
    {
        _servicoMock = new Mock<IContaService>();
        _controller = new ContaController(_servicoMock.Object);
    }

    // =====================================================
    // POST - SUCESSO
    // =====================================================

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_Created_Quando_Sucesso()
    {
        _servicoMock
            .Setup(x => x.CriarContaAsync(It.IsAny<Conta>()))
            .ReturnsAsync(true);

        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "0001",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        var resultado = await _controller.CriarConta(conta);

        var criado = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(201, criado.StatusCode);
    }

    // =====================================================
    // POST - INSUCESSO (regra de negócio)
    // =====================================================

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_Conflito_Quando_Conta_Ja_Existe()
    {
        _servicoMock
            .Setup(x => x.CriarContaAsync(It.IsAny<Conta>()))
            .ReturnsAsync(false);

        var contaInvalida = new Conta
        {
            Cpf = "00000000000",
            AgenciaConta = "9999",
            NumeroConta = "00000",
            LimitePIX = -100
        };

        var resultado = await _controller.CriarConta(contaInvalida);

        Assert.IsType<ConflictObjectResult>(resultado);
    }

    // =====================================================
    // POST - INSUCESSO (validação de campos)
    // =====================================================

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_BadRequest_Quando_Cpf_Invalido()
    {
        _controller.ModelState.AddModelError("Cpf", "CPF inválido");

        var conta = new Conta
        {
            Cpf = "123",
            AgenciaConta = "0001",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        var resultado = await _controller.CriarConta(conta);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_BadRequest_Quando_Agencia_Invalida()
    {
        _controller.ModelState.AddModelError("AgenciaConta", "Agência inválida");

        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "ABC",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        var resultado = await _controller.CriarConta(conta);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_BadRequest_Quando_Numero_Conta_Invalido()
    {
        _controller.ModelState.AddModelError("NumeroConta", "Número inválido");

        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "0001",
            NumeroConta = "ABC",
            LimitePIX = 1000
        };

        var resultado = await _controller.CriarConta(conta);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task Criar_Conta_Deve_Retornar_BadRequest_Quando_Limite_PIX_Invalido()
    {
        _controller.ModelState.AddModelError("LimitePIX", "Limite inválido");

        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "0001",
            NumeroConta = "12345",
            LimitePIX = -100
        };

        var resultado = await _controller.CriarConta(conta);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.IsType<SerializableError>(badRequest.Value);
    }

    // =====================================================
    // GET - SUCESSO
    // =====================================================

    [Fact]
    public async Task Buscar_Conta_Deve_Retornar_Ok_Quando_Encontrada()
    {
        var conta = new Conta
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        _servicoMock
            .Setup(x => x.BuscarContaAsync("12345678901", "12345"))
            .ReturnsAsync(conta);

        var resultado = await _controller.BuscarConta("12345678901", "12345");

        var ok = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(conta, ok.Value);
    }

    // =====================================================
    // GET - INSUCESSO
    // =====================================================

    [Fact]
    public async Task Buscar_Conta_Deve_Retornar_NotFound_Quando_Nao_Existe()
    {
        _servicoMock
            .Setup(x => x.BuscarContaAsync("99999999999", "99999"))
            .ReturnsAsync((Conta)null);

        var resultado = await _controller.BuscarConta("99999999999", "99999");

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        Assert.Equal("Conta não encontrada.", notFound.Value);
    }

    // =====================================================
    // PUT - SUCESSO
    // =====================================================

    [Fact]
    public async Task Atualizar_Limite_Deve_Retornar_Sucesso_Quando_Valido()
    {
        _servicoMock
            .Setup(x => x.AtualizarLimiteAsync("12345678901", "12345", 500))
            .ReturnsAsync(true);

        var conta = new Conta
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            LimitePIX = 500
        };

        var resultado = await _controller.AtualizarLimite(conta);

        var ok = Assert.IsType<ContentResult>(resultado);
        Assert.Equal("Limite atualizado com sucesso.", ok.Content);
    }

    // =====================================================
    // PUT - INSUCESSO
    // =====================================================

    [Fact]
    public async Task Atualizar_Limite_Deve_Retornar_NotFound_Quando_Conta_Nao_Existe()
    {
        _servicoMock
            .Setup(x => x.AtualizarLimiteAsync("00000000000", "99999", 999999))
            .ReturnsAsync(false);

        var contaInexistente = new Conta
        {
            Cpf = "00000000000",
            NumeroConta = "99999",
            LimitePIX = 999999
        };

        var resultado = await _controller.AtualizarLimite(contaInexistente);

        Assert.IsType<NotFoundResult>(resultado);
    }

    // =====================================================
    // DELETE - SUCESSO
    // =====================================================

    [Fact]
    public async Task Remover_Conta_Deve_Retornar_Sucesso_Quando_Valido()
    {
        _servicoMock
            .Setup(x => x.RemoverContaAsync("12345678901", "12345"))
            .ReturnsAsync(true);

        var resultado = await _controller.RemoverConta("12345678901", "12345");

        var content = Assert.IsType<ContentResult>(resultado);
        Assert.Equal("Conta removida com sucesso.", content.Content);
    }

    // =====================================================
    // DELETE - INSUCESSO
    // =====================================================

    [Fact]
    public async Task Remover_Conta_Deve_Retornar_NotFound_Quando_Nao_Existe()
    {
        _servicoMock
            .Setup(x => x.RemoverContaAsync("00000000000", "99999"))
            .ReturnsAsync(false);

        var resultado = await _controller.RemoverConta("00000000000", "99999");

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        Assert.Equal("Conta não encontrada.", notFound.Value);
    }
}