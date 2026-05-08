using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PixApi.Models;
using PixApi.Services;
using PixApi.Repositories;

namespace PixApi.Tests
{
    /// <summary>
    /// Testes unitários do TransacaoPixService, validando regras de negócio como aprovação e rejeição de transações PIX.
    /// </summary>
    [TestClass]
    public class TransacaoPixServiceTests
    {
        private Mock<IContaService> _contaMock;
        private Mock<ITransacaoPixRepository> _repoMock;
        private TransacaoPixService _service;

        [TestInitialize]
        public void Setup()
        {
            _contaMock = new Mock<IContaService>();
            _repoMock = new Mock<ITransacaoPixRepository>();
            _service = new TransacaoPixService(_contaMock.Object, _repoMock.Object);
        }

        #region Transacao Aprovada

        [TestMethod]
        public async Task ValidarProcessarTransacaoAprovadaQuandoLimiteSuficiente()
        {
            var transacao = new TransacaoPix { Cpf = "123", NumeroConta = "1", Valor = 100 };
            var conta = new Conta { Cpf = "123", NumeroConta = "1", LimitePIX = 500 };

            _contaMock.Setup(x => x.BuscarContaAsync("123", "1")).ReturnsAsync(conta);
            _repoMock.Setup(x => x.SalvarAsync(transacao)).Returns(Task.CompletedTask);
            _contaMock.Setup(x => x.AtualizarLimiteAsync("123", "1", 400)).ReturnsAsync(true);

            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            Assert.IsTrue(resultado.Aprovada);
            Assert.AreEqual("Transação aprovada.", resultado.Mensagem);
        }

        #endregion

        #region Transacao Negada

        [TestMethod]
        public async Task ValidarProcessarTransacaoNegadaQuandoContaNaoExiste()
        {
            _contaMock.Setup(x => x.BuscarContaAsync("123", "1")).ReturnsAsync((Conta?)null);

            var transacao = new TransacaoPix { Cpf = "123", NumeroConta = "1", Valor = 100 };

            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            Assert.IsFalse(resultado.Aprovada);
            Assert.AreEqual("Conta não encontrada.", resultado.Mensagem);
        }

        [TestMethod]
        public async Task ValidarProcessarTransacaoNegadaQuandoLimiteInsuficiente()
        {
            var conta = new Conta { Cpf = "123", NumeroConta = "12", LimitePIX = 50 };
            _contaMock.Setup(x => x.BuscarContaAsync("123", "1")).ReturnsAsync(conta);

            var transacao = new TransacaoPix { Cpf = "123", NumeroConta = "1", Valor = 100 };

            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            Assert.IsFalse(resultado.Aprovada);
            Assert.AreEqual("Limite insuficiente.", resultado.Mensagem);
        }

        #endregion
    }
}