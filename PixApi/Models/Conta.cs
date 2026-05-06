using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace PixApi.Models;

/// <summary>
/// Representa uma conta bancária para controle de limite PIX.
/// </summary>
[DynamoDBTable("Conta")]
public class Conta
{
    [Required]
    [DynamoDBHashKey] // PK
    public string Cpf { get; set; } = string.Empty;

    // [Required]
    public string AgenciaConta { get; set; } = string.Empty;

    [Required]
    public string NumeroConta { get; set; } = string.Empty;


    [Range(0, double.MaxValue)]
    public decimal LimitePIX { get; set; }
}