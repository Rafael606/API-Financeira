using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace PixApi.Models;

[DynamoDBTable("Conta")]

public class Conta
{
    [DynamoDBHashKey]
    [Required(ErrorMessage = "CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números.")]
    public string Cpf { get; set; } = string.Empty;

    [RegularExpression(@"^\d+$", ErrorMessage = "Agência deve conter apenas números.")]
    public string AgenciaConta { get; set; } = string.Empty;

    [DynamoDBRangeKey]
    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Número da conta deve ter entre 4 e 10 dígitos.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Número da conta deve conter apenas números.")]
    public string NumeroConta { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Limite PIX deve estar entre 0 e 1.000.000.")]
    public decimal LimitePIX { get; set; }
}