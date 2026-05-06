using PixApi.Models;
using PixApi.Repositories;

namespace PixApi.Services;

/// <summary>
/// Implementação do serviço de transações PIX.
/// </summary>
public class TransacaoPixService : ITransacaoPixService
{
    // Usa o serviço de conta para consultar e atualizar o limite.
    private readonly IContaService _contaService;
    // Usa o repositório de transações para armazenar histórico.
    private readonly ITransacaoPixRepository _transacaoRepository;

    public TransacaoPixService(IContaService contaService, ITransacaoPixRepository transacaoRepository)
    {
        _contaService = contaService;
        _transacaoRepository = transacaoRepository;
    }

    /// <summary>
    /// Processa uma transação PIX: verifica limite e desconta se aprovado.
    /// </summary>
    public async Task<ResultadoTransacao> ProcessarTransacaoAsync(TransacaoPix transacao)
    {
        // Salva a transação no histórico (independente de aprovação ou não)
        await _transacaoRepository.SalvarAsync(transacao, resultado: null); // Salva sem resultado inicialmente

        // Encontra a conta pelo CPF e número da conta.
        var conta = await _contaService.BuscarContaAsync(transacao.Cpf, transacao.NumeroConta);
        if (conta == null)
        {
            // Se a conta não existir, a transação é negada.
            return new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Conta não encontrada."
            };
        }

        // Verifica se o valor da transação é maior que o limite disponível.
        if (transacao.Valor > conta.LimitePIX)
        {
            return new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Limite insuficiente."
            };
        }

        // Desconta o valor autorizado do limite disponível da conta.
        conta.LimitePIX -= transacao.Valor;
        await _contaService.AtualizarLimiteAsync(conta.Cpf, conta.NumeroConta, conta.LimitePIX);

        // Retorna o resultado aprovado.
        return new ResultadoTransacao
        {
            Aprovada = true,
            Mensagem = "Transação aprovada."
        };
    }
}