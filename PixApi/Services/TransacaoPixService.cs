using PixApi.Models;

namespace PixApi.Services;

/// <summary>
/// Implementação do serviço de transações PIX.
/// </summary>
public class TransacaoPixService : ITransacaoPixService
{
    // Usa o serviço de conta para consultar e atualizar o limite.
    private readonly IContaService _contaService;

    public TransacaoPixService(IContaService contaService)
    {
        _contaService = contaService;
    }

    /// <summary>
    /// Processa uma transação PIX: verifica limite e desconta se aprovado.
    /// </summary>
    public async Task<ResultadoTransacao> ProcessarTransacaoAsync(TransacaoPix transacao)
    {
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
        if (transacao.Valor > conta.LimiteDisponivel)
        {
            return new ResultadoTransacao
            {
                Aprovada = false,
                Mensagem = "Limite insuficiente."
            };
        }

        // Desconta o valor autorizado do limite disponível da conta.
        conta.LimiteDisponivel -= transacao.Valor;

        // Retorna o resultado aprovado.
        return new ResultadoTransacao
        {
            Aprovada = true,
            Mensagem = "Transação aprovada."
        };
    }
}