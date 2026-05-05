using System.ComponentModel.DataAnnotations;

namespace PixApi.Models;

/// <summary>
/// Representa uma conta bancária para controle de limite PIX.
/// </summary>
public class Conta
{
    /// <summary>
    /// CPF do titular da conta.
    /// </summary>
    [Required(ErrorMessage = "CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos.")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Agência da conta.
    /// </summary>
    [Required(ErrorMessage = "Agência é obrigatória.")]
    public string Agencia { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta.
    /// </summary>
    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    public string NumeroConta { get; set; } = string.Empty;

    /// <summary>
    /// Limite disponível para transações PIX.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Limite deve ser maior ou igual a zero.")]
    public decimal LimiteDisponivel { get; set; }
}

// Nota: os atributos de validação ajudam a garantir que a conta criada no controller esteja correta.