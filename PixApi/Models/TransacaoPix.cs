using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace PixApi.Models;
 
public class TransacaoPix
{
    [DynamoDBHashKey]
    [Required(ErrorMessage = "CPF é obrigatório.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número da conta é obrigatório.")]
    public string NumeroConta { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Limite PIX deve estar entre 0 e 1.000.000.")]
    public decimal Valor { get; set; }
}

public class ResultadoTransacao
{
    public bool Aprovada { get; set; }

    public string Mensagem { get; set; } = string.Empty;
}
