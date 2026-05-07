using PixApi.Models;

namespace PixApi.Services;

public interface ITransacaoPixService
{
    Task<ResultadoTransacao> ProcessarTransacaoAsync(TransacaoPix transacao);
}