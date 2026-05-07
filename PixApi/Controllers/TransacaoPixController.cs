using Microsoft.AspNetCore.Mvc;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacaoPixController : ControllerBase
{
    private readonly ITransacaoPixService _transacaoService;

    public TransacaoPixController(ITransacaoPixService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessarTransacao([FromBody] TransacaoPix transacao)
    {
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