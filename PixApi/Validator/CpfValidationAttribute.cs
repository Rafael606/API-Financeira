using System.ComponentModel.DataAnnotations;

namespace PixApi.Validations
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("CPF é obrigatório.");

            var cpf = value.ToString()!.Trim();

            if (cpf.Length != 11)
                return new ValidationResult("CPF deve ter 11 dígitos.");

            if (!cpf.All(char.IsDigit))
                return new ValidationResult("CPF deve conter apenas números.");

            if (cpf.Distinct().Count() == 1)
                return new ValidationResult("CPF inválido.");

            // Validação do primeiro dígito verificador
            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            var resto = soma % 11;
            var primeiroDigito = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != primeiroDigito)
                return new ValidationResult("CPF inválido.");

            // Validação do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
 
            resto = soma % 11;
            var segundoDigito = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[10].ToString()) != segundoDigito)
                return new ValidationResult("CPF inválido.");

            return ValidationResult.Success;
        }
    }
}