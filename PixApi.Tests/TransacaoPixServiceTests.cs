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

        #region ProcessarTransacao Tests - Sucesso 

        // =====================================================
        // SUCESSO - transação aprovada
        // =====================================================

        [TestMethod]
        public async Task Processar_Transacao_Deve_Aprovar_Quando_Limite_Suficiente()
        {
            // Arrange
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

            // Act
            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            // Assert
            Assert.IsTrue(resultado.Aprovada);
            Assert.AreEqual("Transação aprovada.", resultado.Mensagem);
        }
        #endregion

         #region ProcessarTransacao Tests - Insucesso

        // =====================================================
        // INSUCESSO - conta não existe
        // =====================================================

        [TestMethod]
        public async Task Processar_Transacao_Deve_Rejeitar_Quando_Conta_Nao_Existe()
        {
            // Arrange
            _contaMock
                .Setup(x => x.BuscarContaAsync("123", "1"))
                .ReturnsAsync((Conta)null);

            var transacao = new TransacaoPix
            {
                Cpf = "123",
                NumeroConta = "1",
                Valor = 100
            };

            // Act
            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            // Assert
            Assert.IsFalse(resultado.Aprovada);
            Assert.AreEqual("Conta não encontrada.", resultado.Mensagem);
        }

        // =====================================================
        // INSUCESSO - limite insuficiente
        // =====================================================

        [TestMethod]
        public async Task Processar_Transacao_Deve_Rejeitar_Quando_Limite_Insuficiente()
        {
            // Arrange
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

            // Act
            var resultado = await _service.ProcessarTransacaoAsync(transacao);

            // Assert
            Assert.IsFalse(resultado.Aprovada);
            Assert.AreEqual("Limite insuficiente.", resultado.Mensagem);
        }
        #endregion
    }
}