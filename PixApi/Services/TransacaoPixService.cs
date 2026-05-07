using PixApi.Models;
using PixApi.Repositories;

namespace PixApi.Services;

public class TransacaoPixService : ITransacaoPixService
{
    private readonly IContaService _contaService;
    private readonly ITransacaoPixRepository _transacaoRepository;

    public TransacaoPixService(IContaService contaService, ITransacaoPixRepository transacaoRepository)
    {
        _contaService = contaService;
        _transacaoRepository = transacaoRepository;
    }

    public async Task<ResultadoTransacao> ProcessarTransacaoAsync(TransacaoPix transacao)
    {
        // Encontra a conta pelo CPF e número da conta.
        var conta = await _contaService.BuscarContaAsync(transacao.Cpf, transacao.NumeroConta);
        if (conta == null)
        {
            return new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Conta não encontrada."
            };
        }

        if (transacao.Valor > conta.LimitePIX)
        {
            return new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Limite insuficiente."
            };
        };

        conta.LimitePIX -= transacao.Valor;

        await _transacaoRepository.SalvarAsync(transacao);
        await _contaService.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX);


        return new ResultadoTransacao
        {
            Aprovada = true,
            Mensagem = "Transação aprovada."
        };
    }
}