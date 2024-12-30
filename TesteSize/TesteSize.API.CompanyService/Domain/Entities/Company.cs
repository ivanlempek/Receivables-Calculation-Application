using Helpers.Exceptions;
using TesteSize.API.CompanyService.Domain.Enums;

namespace TesteSize.API.CompanyService.Domain.Entities
{
    public class Company
    {
        public Guid? Id { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public decimal FaturamentoMensal { get; set; }
        public Ramo Ramo { get; set; }

        protected Company() { }

        public Company(Guid? id, string cnpj, string nome, decimal faturamentoMensal, Ramo ramo)
        {
            Id = id == null || id == Guid.Empty ? Guid.NewGuid() : id.Value;
            CNPJ = cnpj;
            Nome = nome;
            FaturamentoMensal = faturamentoMensal;
            Ramo = ramo;
        }

        public ValidationHelper Validate()
        {
            if (string.IsNullOrWhiteSpace(CNPJ))
                return new ValidationHelper(false, "CNPJ é obrigatório.");

            if (CNPJ.Length < 14 || CNPJ.Length > 18)
                return new ValidationHelper(false, "CNPJ inválido.");

            if (string.IsNullOrWhiteSpace(Nome))
                return new ValidationHelper(false, "Nome é obrigatório.");

            if (Nome.Length <= 3)
                return new ValidationHelper(false, "O Nome precisa ter mais de 3 caracteres.");

            if (FaturamentoMensal <= 0)
                return new ValidationHelper(false, "Faturamento Mensal deve ser maior que zero.");

            if (!Enum.IsDefined(typeof(Ramo), Ramo))
                return new ValidationHelper(false, "Ramo inválido. Deve ser 'Serviços(1)' ou 'Produtos(2)'.");

            return new ValidationHelper(true); 
        }
    } 
}
