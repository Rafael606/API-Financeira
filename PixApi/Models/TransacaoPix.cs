using System.ComponentModel.DataAnnotations;

namespace PixApi.Models;

/// <summary>
/// Representa uma transação PIX.
/// </summary>
public class TransacaoPix
{
    /// <summary>
    /// CPF do titular da conta.
    /// </summary>
    [Required(ErrorMessage = "CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos.")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta.
    /// </summary>
    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    public string NumeroConta { get; set; } = string.Empty;

    /// <summary>
    /// Valor da transação.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

/// <summary>
/// Resultado da transação PIX.
/// </summary>
public class ResultadoTransacao
{
    /// <summary>
    /// Indica se a transação foi aprovada.
    /// </summary>
    public bool Aprovada { get; set; }

    /// <summary>
    /// Mensagem explicativa.
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;
}

// Esses modelos representam o request e response da transação.
// O controller usa TransacaoPix para receber dados e ResultadoTransacao para retornar o resultado.