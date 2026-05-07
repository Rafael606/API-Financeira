using Microsoft.AspNetCore.Mvc;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContaController : ControllerBase
{
    private readonly IContaService _contaService;

    public ContaController(IContaService contaService)
    {
        _contaService = contaService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarConta([FromBody] Conta conta)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var sucesso = await _contaService.CriarContaAsync(conta);

    if (!sucesso)
        return Conflict("Conta já existe.");

    return CreatedAtAction(nameof(BuscarConta),
        new { cpf = conta.Cpf, agencia = conta.AgenciaConta, numeroConta = conta.NumeroConta, limitePIX = conta.LimitePIX },
        conta);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarConta(
    [FromQuery] string cpf,
    [FromQuery] string numeroConta)
    {
    var conta = await _contaService.BuscarContaAsync(cpf, numeroConta);

    if (conta == null)
        return NotFound("Conta não encontrada.");

    return Ok(conta);
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarLimite([FromBody] Conta conta)
    {
    var sucesso = await _contaService.AtualizarLimiteAsync(
        conta.Cpf,
        conta.NumeroConta,
        conta.LimitePIX);

    if (!sucesso)
        return NotFound("Conta não encontrada.");

    return Content("Limite atualizado com sucesso.");
    }

    [HttpDelete]
    public async Task<IActionResult> RemoverConta(
    [FromQuery] string cpf,
    [FromQuery] string numeroConta)
    {
    var sucesso = await _contaService.RemoverContaAsync(cpf, numeroConta);

    if (!sucesso)
        return NotFound("Conta não encontrada.");

    return Content("Conta removida com sucesso.");
    }

}