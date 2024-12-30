using TesteSize.BaseDTOs.Enums;

namespace TesteSize.BaseDTOs.Company
{
    public class CompanyDto
    {
        public Guid? Id { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public decimal FaturamentoMensal { get; set; }
        public Ramo Ramo { get; set; }
    }
}
