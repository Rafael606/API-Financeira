using Xunit; // Framework de testes unitários
using Moq; // Biblioteca para criação de mocks
using Microsoft.AspNetCore.Mvc; // Tipos de retorno da API (Ok, Created, etc)
using PixApi.Controllers; // Controller que será testado
using PixApi.Models; // Model da Conta
using PixApi.Services; // Interface do serviço

public class ContaControllerTests
{
    // Mock do serviço de conta (simula comportamento real sem banco)
    private readonly Mock<IContaService> _servicoMock;

    // Controller que será testada
    private readonly ContaController _controller;

    public ContaControllerTests()
    {
        // Inicializa o mock do serviço
        _servicoMock = new Mock<IContaService>();

        // Injeta o mock dentro da controller (Dependency Injection manual)
        _controller = new ContaController(_servicoMock.Object);
    }

    // =====================================================
    // POST - SUCESSO
    // =====================================================

    [Fact] // Marca o método como teste unitário
    public async Task ValidarCriacaoContaComSucesso()
    {
        // Simula que o serviço vai retornar sucesso ao criar conta
        _servicoMock
            .Setup(x => x.CriarContaAsync(It.IsAny<Conta>()))
            .ReturnsAsync(true);

        // Cria uma conta válida para teste
        var conta = new Conta
        {
            Cpf = "12345678901", // CPF válido
            AgenciaConta = "0001", // Agência válida
            NumeroConta = "12345", // Conta válida
            LimitePIX = 1000 // Limite válido
        };

        // Chama o método da controller
        var resultado = await _controller.CriarConta(conta);

        // Verifica se retornou CreatedAtAction (201)
        var criado = Assert.IsType<CreatedAtActionResult>(resultado);

        // Valida status HTTP 201
        Assert.Equal(201, criado.StatusCode);
    }

    // =====================================================
    // POST - INSUCESSO (regra de negócio)
    // =====================================================

    [Fact]
    public async Task ValidarCriacaoContaJaExistente()
    {
        // Simula que o serviço não conseguiu criar (conta já existe)
        _servicoMock
            .Setup(x => x.CriarContaAsync(It.IsAny<Conta>()))
            .ReturnsAsync(false);

        // Cria uma conta inválida propositalmente
        var conta = new Conta
        {
            Cpf = "00000000000", // CPF inválido lógico
            AgenciaConta = "9999", // Agência inválida
            NumeroConta = "00000", // Conta inválida
            LimitePIX = -100 // Limite negativo inválido
        };

        // Executa a controller
        var resultado = await _controller.CriarConta(conta);

        // Verifica se retornou conflito (409)
        Assert.IsType<ConflictObjectResult>(resultado);
    }

    // =====================================================
    // POST - INSUCESSO (validação de campos)
    // =====================================================

    [Fact]
    public async Task ValidarCriacaoContaCpfInvalido()
    {
        // Simula erro de validação no ModelState (CPF inválido)
        _controller.ModelState.AddModelError("Cpf", "CPF inválido");

        // Cria conta com CPF inválido
        var conta = new Conta
        {
            Cpf = "123", // CPF inválido
            AgenciaConta = "0001",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        // Executa controller
        var resultado = await _controller.CriarConta(conta);

        // Verifica retorno 400 BadRequest
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);

        // Verifica se retorno contém erro estruturado
        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task ValidarCriacaoContaAgenciaInvalida()
    {
        // Simula erro de validação na agência
        _controller.ModelState.AddModelError("AgenciaConta", "Agência inválida");

        // Cria conta com agência inválida
        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "ABC", // inválido
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        // Executa controller
        var resultado = await _controller.CriarConta(conta);

        // Valida retorno de erro 400
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);

        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task ValidarCriacaoContaNumeroContaInvalido()
    {
        // Simula erro no número da conta
        _controller.ModelState.AddModelError("NumeroConta", "Número inválido");

        // Cria conta com número inválido
        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "0001",
            NumeroConta = "ABC", // inválido
            LimitePIX = 1000
        };

        // Executa controller
        var resultado = await _controller.CriarConta(conta);

        // Valida erro 400
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);

        Assert.IsType<SerializableError>(badRequest.Value);
    }

    [Fact]
    public async Task ValidarCriacaoContaLimitePixInvalido()
    {
        // Simula erro de limite PIX inválido
        _controller.ModelState.AddModelError("LimitePIX", "Limite inválido");

        // Cria conta com limite negativo
        var conta = new Conta
        {
            Cpf = "12345678901",
            AgenciaConta = "0001",
            NumeroConta = "12345",
            LimitePIX = -100 // inválido
        };

        // Executa controller
        var resultado = await _controller.CriarConta(conta);

        // Valida retorno 400
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);

        Assert.IsType<SerializableError>(badRequest.Value);
    }

    // =====================================================
    // GET - SUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarBuscaContaComSucesso()
    {
        // Simula conta encontrada no serviço
        var conta = new Conta
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            LimitePIX = 1000
        };

        // Configura mock para retorno da conta
        _servicoMock
            .Setup(x => x.BuscarContaAsync("12345678901", "12345"))
            .ReturnsAsync(conta);

        // Executa busca
        var resultado = await _controller.BuscarConta("12345678901", "12345");

        // Verifica retorno 200 OK
        var ok = Assert.IsType<OkObjectResult>(resultado);

        // Valida objeto retornado
        Assert.Equal(conta, ok.Value);
    }

    // =====================================================
    // GET - INSUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarBuscaContaNaoEncontrada()
    {
        // Simula conta inexistente
        _servicoMock
            .Setup(x => x.BuscarContaAsync("99999999999", "99999"))
            .ReturnsAsync((Conta)null);

        // Executa busca
        var resultado = await _controller.BuscarConta("99999999999", "99999");

        // Valida retorno 404
        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);

        // Valida mensagem
        Assert.Equal("Conta não encontrada.", notFound.Value);
    }

    // =====================================================
    // PUT - SUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarAtualizacaoLimiteComSucesso()
    {
        // Simula atualização bem-sucedida
        _servicoMock
            .Setup(x => x.AtualizarLimiteAsync("12345678901", "12345", 500))
            .ReturnsAsync(true);

        // Cria conta com novo limite
        var conta = new Conta
        {
            Cpf = "12345678901",
            NumeroConta = "12345",
            LimitePIX = 500
        };

        // Executa atualização
        var resultado = await _controller.AtualizarLimite(conta);

        // Verifica retorno de sucesso
        var ok = Assert.IsType<ContentResult>(resultado);

        // Valida mensagem
        Assert.Equal("Limite atualizado com sucesso.", ok.Content);
    }

    // =====================================================
    // PUT - INSUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarAtualizacaoLimiteContaNaoExiste()
    {
        // Simula falha na atualização
        _servicoMock
            .Setup(x => x.AtualizarLimiteAsync("00000000000", "99999", 999999))
            .ReturnsAsync(false);

        // Cria conta inexistente
        var conta = new Conta
        {
            Cpf = "00000000000",
            NumeroConta = "99999",
            LimitePIX = 999999
        };

        // Executa controller
        var resultado = await _controller.AtualizarLimite(conta);

        // Valida retorno 404
        Assert.IsType<NotFoundResult>(resultado);
    }

    // =====================================================
    // DELETE - SUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarRemocaoContaComSucesso()
    {
        // Simula remoção bem-sucedida
        _servicoMock
            .Setup(x => x.RemoverContaAsync("12345678901", "12345"))
            .ReturnsAsync(true);

        // Executa remoção
        var resultado = await _controller.RemoverConta("12345678901", "12345");

        // Verifica retorno de sucesso
        var content = Assert.IsType<ContentResult>(resultado);

        // Valida mensagem
        Assert.Equal("Conta removida com sucesso.", content.Content);
    }

    // =====================================================
    // DELETE - INSUCESSO
    // =====================================================

    [Fact]
    public async Task ValidarRemocaoContaNaoExiste()
    {
        // Simula falha na remoção
        _servicoMock
            .Setup(x => x.RemoverContaAsync("00000000000", "99999"))
            .ReturnsAsync(false);

        // Executa controller
        var resultado = await _controller.RemoverConta("00000000000", "99999");

        // Valida retorno 404
        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);

        // Valida mensagem
        Assert.Equal("Conta não encontrada.", notFound.Value);
    }
}