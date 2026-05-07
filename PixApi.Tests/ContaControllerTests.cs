using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PixApi.Controllers;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Tests
{
    /// <summary>
    /// Testes unitários do ContaController, validando respostas HTTP e integração com o ContaService.
    /// </summary>
    
    [TestClass]
    public class ContaControllerTests
    {
        private Mock<IContaService> _contaServiceMock;
        private ContaController _controller;

        [TestInitialize]
        public void Setup()
        {
            _contaServiceMock = new Mock<IContaService>();
            _controller = new ContaController(_contaServiceMock.Object);
        }

        #region CriarConta Tests

        [TestMethod]
        public async Task ValidarCriarContaRetorna201QuandoSucesso()
        {
            var conta = new Conta { Cpf = "12345678901", AgenciaConta = "0001", NumeroConta = "12345", LimitePIX = 1000.00m };

            _contaServiceMock.Setup(x => x.CriarContaAsync(It.Is<Conta>(c => c.Cpf == conta.Cpf))).ReturnsAsync(true);

            var result = await _controller.CriarConta(conta);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(conta, createdResult.Value);
        }

        [TestMethod]
        public async Task ValidarCriarContaRetorna409QuandoContaJaExiste()
        {
            var conta = new Conta { Cpf = "12345678901", AgenciaConta = "0001", NumeroConta = "12345", LimitePIX = 1000.00m };

            _contaServiceMock.Setup(x => x.CriarContaAsync(It.Is<Conta>(c => c.Cpf == conta.Cpf && c.NumeroConta == conta.NumeroConta))).ReturnsAsync(false);

            var result = await _controller.CriarConta(conta);

            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual("Conta já existe.", conflictResult.Value);
        }

        [TestMethod]
        public async Task ValidarCriarContaRetorna400CPFInvalido()
        {
            var conta = new Conta { Cpf = "123", AgenciaConta = "0001", NumeroConta = "12345", LimitePIX = 1000.00m };
            _controller.ModelState.AddModelError("Cpf", "CPF deve ter 11 dígitos.");

            var result = await _controller.CriarConta(conta);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        #endregion

        #region BuscarConta Tests

        [TestMethod]
        public async Task ValidarBuscarContaRetorna200QuandoEncontrada()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";
            var contaEsperada = new Conta { Cpf = cpf, AgenciaConta = "0001", NumeroConta = numeroConta, LimitePIX = 1000.00m };

            _contaServiceMock.Setup(x => x.BuscarContaAsync(cpf, numeroConta)).ReturnsAsync(contaEsperada);

            var result = await _controller.BuscarConta(cpf, numeroConta);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(contaEsperada, okResult.Value);
        }

        [TestMethod]
        public async Task ValidarBuscarContaRetorna404QuandoNaoEncontrada()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";

            _contaServiceMock.Setup(x => x.BuscarContaAsync(cpf, numeroConta)).ReturnsAsync((Conta)null);

            var result = await _controller.BuscarConta(cpf, numeroConta);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Conta não encontrada.", notFoundResult.Value);
        }

        #endregion

        #region AtualizarLimite Tests

        [TestMethod]
        public async Task ValidarAtualizarLimiteRetorna200QuandoSucesso()
        {
            var conta = new Conta { Cpf = "12345678901", NumeroConta = "12345", LimitePIX = 2000.00m };

            _contaServiceMock.Setup(x => x.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX)).ReturnsAsync(true);

            var result = await _controller.AtualizarLimite(conta);

            var contentResult = result as ContentResult;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual("Limite atualizado com sucesso.", contentResult.Content);
        }

        [TestMethod]
        public async Task ValidarAtualizarLimiteRetorna404QuandoContaNaoEncontrada()
        {
            var conta = new Conta { Cpf = "12345678901", NumeroConta = "12345", LimitePIX = 2000.00m };

            _contaServiceMock.Setup(x => x.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX)).ReturnsAsync(false);

            var result = await _controller.AtualizarLimite(conta);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Conta não encontrada.", notFoundResult.Value);
        }

        #endregion

        #region RemoverConta Tests

        [TestMethod]
        public async Task ValidarRemoverContaRetorna200QuandoSucesso()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";

            _contaServiceMock.Setup(x => x.RemoverContaAsync(cpf, numeroConta)).ReturnsAsync(true);

            var result = await _controller.RemoverConta(cpf, numeroConta);

            var contentResult = result as ContentResult;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual("Conta removida com sucesso.", contentResult.Content);
        }

        [TestMethod]
        public async Task ValidarRemoverContaRetorna404QuandoNaoEncontrada()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";

            _contaServiceMock.Setup(x => x.RemoverContaAsync(cpf, numeroConta)).ReturnsAsync(false);

            var result = await _controller.RemoverConta(cpf, numeroConta);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Conta não encontrada.", notFoundResult.Value);
        }

        #endregion

        #region Chamadas ao Service

        [TestMethod]
        public async Task ValidarCriarContaChamaServiceUmaVez()
        {
            var conta = new Conta { Cpf = "12345678901", AgenciaConta = "0001", NumeroConta = "12345", LimitePIX = 1000.00m };

            _contaServiceMock.Setup(x => x.CriarContaAsync(It.IsAny<Conta>())).ReturnsAsync(true);

            await _controller.CriarConta(conta);

            _contaServiceMock.Verify(
                x => x.CriarContaAsync(It.Is<Conta>(c => c.Cpf == conta.Cpf && c.NumeroConta == conta.NumeroConta)),Times.Once);
        }

        [TestMethod]
        public async Task ValidarBuscarContaChamaServiceUmaVez()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";

            _contaServiceMock.Setup(x => x.BuscarContaAsync(cpf, numeroConta)).ReturnsAsync(new Conta());

            await _controller.BuscarConta(cpf, numeroConta);

            _contaServiceMock.Verify(x => x.BuscarContaAsync(cpf, numeroConta), Times.Once);
        }

        [TestMethod]
        public async Task ValidarAtualizarLimiteChamaServiceUmaVez()
        {
            var conta = new Conta { Cpf = "12345678901", NumeroConta = "12345", LimitePIX = 2000.00m };

            _contaServiceMock.Setup(x => x.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX)).ReturnsAsync(true);

            await _controller.AtualizarLimite(conta);

            _contaServiceMock.Verify(x => x.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX), Times.Once);
        }

        [TestMethod]
        public async Task ValidarRemoverContaChamaServiceUmaVez()
        {
            var cpf = "12345678901";
            var numeroConta = "12345";

            _contaServiceMock.Setup(x => x.RemoverContaAsync(cpf, numeroConta)).ReturnsAsync(true);

            await _controller.RemoverConta(cpf, numeroConta);

            _contaServiceMock.Verify(x => x.RemoverContaAsync(cpf, numeroConta), Times.Once);
        }

        #endregion
    }
}