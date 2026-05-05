using PixApi.Models;

namespace PixApi.Services;

/// <summary>
/// Interface para o serviço de transações PIX.
/// </summary>
public interface ITransacaoPixService
{
    /// <summary>
    /// Processa uma transação PIX.
    /// </summary>
    Task<ResultadoTransacao> ProcessarTransacaoAsync(TransacaoPix transacao);
}