using System.ComponentModel.DataAnnotations;

namespace PixApi.Models;

/// <summary>
/// Representa uma transação PIX.
/// </summary>
 

public class TransacaoPix
{
    /// CPF do titular da conta.
    [Required(ErrorMessage = "CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos.")]
    public string Cpf { get; set; } = string.Empty;

    /// Número da conta.
    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    public string NumeroConta { get; set; } = string.Empty;

    /// Valor da transação.
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

/// <summary>
/// Resultado da transação PIX.
/// </summary>
public class ResultadoTransacao
{
    /// Indica se a transação foi aprovada.
    public bool Aprovada { get; set; }


    /// Mensagem explicativa.
    public string Mensagem { get; set; } = string.Empty;
}
