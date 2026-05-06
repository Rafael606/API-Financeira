using Microsoft.AspNetCore.Mvc;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Controllers;

/// <summary>
/// Controller para processamento de transações PIX.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacaoPixController : ControllerBase
{
    // Serviço de transações injetado para delegar a lógica de negócio.
    private readonly ITransacaoPixService _transacaoService;

    public TransacaoPixController(ITransacaoPixService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    /// <summary>
    /// Processa uma transação PIX.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessarTransacao([FromBody] TransacaoPix transacao)
    {
        // Console.WriteLine($"Recebendo transação: CPF={transacao.Cpf}, Conta={transacao.NumeroConta}, Valor={transacao.Valor}");
        // Valida o payload enviado pelo cliente.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Processa a transação através do serviço.
        var resultado = await _transacaoService.ProcessarTransacaoAsync(transacao);

        // Retorna o resultado da aprovação ou negação.
        return Ok(resultado);
    }
}