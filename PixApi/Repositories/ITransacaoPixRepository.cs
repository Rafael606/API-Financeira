using PixApi.Models;

namespace PixApi.Repositories;

public interface ITransacaoPixRepository
{
    Task SalvarAsync(TransacaoPix transacao);
}