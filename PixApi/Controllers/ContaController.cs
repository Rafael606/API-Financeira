using Microsoft.AspNetCore.Mvc;
using PixApi.Models;
using PixApi.Services;

namespace PixApi.Controllers;

/// <summary>
/// Controller para operações CRUD de contas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContaController : ControllerBase
{
    // Serviço de conta injetado via DI para manter a controller leve.
    private readonly IContaService _contaService;

    public ContaController(IContaService contaService)
    {
        // Guarda a instância injetada do serviço de conta.
        _contaService = contaService;
    }

    /// <summary>
    /// Cria uma nova conta.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarConta([FromBody] Conta conta)
    {
        // Verifica se os dados enviados estão corretos.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Tenta criar a conta no serviço.
        var sucesso = await _contaService.CriarContaAsync(conta);
        if (!sucesso)
        {
            // Retorna conflito se a conta já existir.
            return Conflict("Conta já existe.");
        }

        // Retorna 201 Created com o local da conta criada.
        return CreatedAtAction(nameof(BuscarConta), new { cpf = conta.Cpf, numeroConta = conta.NumeroConta }, conta);
    }

    /// <summary>
    /// Busca uma conta por CPF e número da conta.
    /// </summary>
    [HttpGet("{cpf}/{numeroConta}")]
    public async Task<IActionResult> BuscarConta(string cpf, string numeroConta)
    {
        // Consulta a conta pelo serviço.
        var conta = await _contaService.BuscarContaAsync(cpf, numeroConta);
        if (conta == null)
        {
            // Retorna 404 se não existir.
            return NotFound("Conta não encontrada.");
        }

        // Retorna a conta encontrada.
        return Ok(conta);
    }

    /// <summary>
    /// Atualiza o limite de uma conta.
    /// </summary>
    [HttpPut("{cpf}/{numeroConta}/limite")]
    public async Task<IActionResult> AtualizarLimite(string cpf, string numeroConta, [FromBody] decimal novoLimite)
    {
        // Atualiza apenas o limite da conta.
        var sucesso = await _contaService.AtualizarLimiteAsync(cpf, numeroConta, novoLimite);
        if (!sucesso)
        {
            // Se a conta não existir, retorna 404.
            return NotFound("Conta não encontrada.");
        }

        // Retorna 204 No Content quando a atualização foi bem-sucedida.
        return NoContent();
    }

    /// <summary>
    /// Remove uma conta.
    /// </summary>
    [HttpDelete("{cpf}/{numeroConta}")]
    public async Task<IActionResult> RemoverConta(string cpf, string numeroConta)
    {
        // Remove a conta do armazenamento.
        var sucesso = await _contaService.RemoverContaAsync(cpf, numeroConta);
        if (!sucesso)
        {
            // Se a conta não existir, retorna 404.
            return NotFound("Conta não encontrada.");
        }

        // Retorna 204 No Content para indicar remoção bem-sucedida.
        return NoContent();
    }
}