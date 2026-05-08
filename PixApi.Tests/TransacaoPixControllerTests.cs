using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PixApi.Controllers;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Tests
{
    /// <summary>
    /// Testes unitários do TransacaoPixController, validando respostas HTTP e integração com o TransacaoPixService.
    /// </summary>

    [TestClass]
    public class TransacaoPixControllerTests
    {
        private Mock<ITransacaoPixService> _transacaoServiceMock = null!;
        private TransacaoPixController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _transacaoServiceMock = new Mock<ITransacaoPixService>();
            _controller = new TransacaoPixController(_transacaoServiceMock.Object);
        }

        #region Transacao Aprovada

        [TestMethod]
        public async Task ValidarProcessarTransacaoAprovadaQuandoLimiteSuficiente()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 500.00m };
            var resultadoEsperado = new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor))).ReturnsAsync(resultadoEsperado);

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsNotNull(retorno);
            Assert.IsTrue(retorno.Aprovada);
            Assert.AreEqual("Transação aprovada.", retorno.Mensagem);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoAprovadaComValorDiferente()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 1500.50m };

            _transacaoServiceMock
                .Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Valor == 1500.50m)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsTrue(retorno!.Aprovada);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoAprovadaComValorMaximo()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 1000000.00m };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Valor == 1000000.00m)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsTrue(retorno!.Aprovada);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoAprovadaComValorMinimo()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 0.01m };
            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Valor == 0.01m)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsTrue(retorno!.Aprovada);
        }

        #endregion

        #region Transacao Negada

        [TestMethod]
        public async Task ValidarProcessarTransacaoNegadaQuandoLimiteInsuficiente()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 5000.00m };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = false, Mensagem = "Limite insuficiente." });

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsNotNull(retorno);
            Assert.IsFalse(retorno.Aprovada);
            Assert.AreEqual("Limite insuficiente.", retorno.Mensagem);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoNegadaQuandoContaNaoEncontrada()
        {
            var transacao = new TransacaoPix { Cpf = "99999999999", NumeroConta = "99999", Valor = 100.00m };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = false, Mensagem = "Conta não encontrada." });

            var result = await _controller.ProcessarTransacao(transacao);

            var retorno = (result as OkObjectResult)?.Value as ResultadoTransacao;
            Assert.IsNotNull(retorno);
            Assert.IsFalse(retorno.Aprovada);
            Assert.AreEqual("Conta não encontrada.", retorno.Mensagem);
        }

        #endregion

        #region Validacao ModelState

        [TestMethod]
        public async Task ValidarProcessarTransacaoRetorna400QuandoCpfVazio()
        {
            var transacao = new TransacaoPix { Cpf = "", NumeroConta = "12345", Valor = 100.00m };
            _controller.ModelState.AddModelError("Cpf", "CPF é obrigatório.");

            var result = await _controller.ProcessarTransacao(transacao);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoRetorna400QuandoCpfInvalido()
        {
            var transacao = new TransacaoPix { Cpf = "123", NumeroConta = "12345", Valor = 100.00m };
            _controller.ModelState.AddModelError("Cpf", "CPF deve ter 11 dígitos.");

            var result = await _controller.ProcessarTransacao(transacao);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoRetorna400QuandoValorNegativo()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = -100.00m };
            _controller.ModelState.AddModelError("Valor", "Valor deve ser maior que zero.");

            var result = await _controller.ProcessarTransacao(transacao);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoRetorna400QuandoValorZero()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 0.00m };
            _controller.ModelState.AddModelError("Valor", "Valor deve ser maior que zero.");

            var result = await _controller.ProcessarTransacao(transacao);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoRetorna400QuandoNumeroContaVazio()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "", Valor = 100.00m };
            _controller.ModelState.AddModelError("NumeroConta", "Número da conta é obrigatório.");

            var result = await _controller.ProcessarTransacao(transacao);

            Assert.IsNotNull(result as BadRequestObjectResult);
        }

        #endregion

        #region Chamadas ao Service - Regra de Negócio

        [TestMethod]
        public async Task ValidarProcessarTransacaoChamaServiceUmaVez()
        {
            var transacao = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 500.00m };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            await _controller.ProcessarTransacao(transacao);

            _transacaoServiceMock.Verify(
                x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t =>
                    t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor)),
                Times.Once);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoNaoChamaServiceQuandoModelStateInvalido()
        {
            var transacao = new TransacaoPix { Cpf = "", NumeroConta = "12345", Valor = 100.00m };
            _controller.ModelState.AddModelError("Cpf", "CPF é obrigatório.");

            await _controller.ProcessarTransacao(transacao);

            _transacaoServiceMock.Verify(x => x.ProcessarTransacaoAsync(It.IsAny<TransacaoPix>()), Times.Never);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoPassaParametrosCorretosAoService()
        {
            var transacao = new TransacaoPix { Cpf = "11122233344", NumeroConta = "54321", Valor = 750.50m };

            _transacaoServiceMock.Setup(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == transacao.Cpf && t.NumeroConta == transacao.NumeroConta && t.Valor == transacao.Valor)))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            await _controller.ProcessarTransacao(transacao);

            _transacaoServiceMock.Verify(
                x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t =>
                    t.Cpf == "11122233344" && t.NumeroConta == "54321" && t.Valor == 750.50m)),
                Times.Once);
        }

        [TestMethod]
        public async Task ValidarProcessarMultiplasTransacoesParaMesmaConta()
        {
            var transacao1 = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 100m };
            var transacao2 = new TransacaoPix { Cpf = "12345678901", NumeroConta = "12345", Valor = 200m };

            _transacaoServiceMock.SetupSequence(x => x.ProcessarTransacaoAsync(It.Is<TransacaoPix>(t => t.Cpf == "12345678901" && t.NumeroConta == "12345")))
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." })
                .ReturnsAsync(new ResultadoTransacao { Aprovada = true, Mensagem = "Transação aprovada." });

            var result1 = await _controller.ProcessarTransacao(transacao1);
            var result2 = await _controller.ProcessarTransacao(transacao2);

            Assert.IsNotNull(result1 as OkObjectResult);
            Assert.IsNotNull(result2 as OkObjectResult);
            _transacaoServiceMock.Verify(x => x.ProcessarTransacaoAsync(It.IsAny<TransacaoPix>()), Times.Exactly(2));
        }

        #endregion
    }
}