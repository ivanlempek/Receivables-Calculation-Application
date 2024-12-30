using Helpers.Exceptions;

namespace TesteSize.API.InvoiceService.Domain.Entities
{
    public class Invoice
    {
        public Guid? Id { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public Guid EmpresaId { get; set; } 

        public Invoice() { }

        public Invoice(string numero, decimal valor, DateTime dataVencimento, Guid empresaId, Guid? id = null)
        {
            Id = id == null || id == Guid.Empty ? Guid.NewGuid() : id.Value;
            Numero = numero;
            Valor = valor;
            DataVencimento = dataVencimento;
            EmpresaId = empresaId;
        }

        public ValidationHelper Validate()
        {
            if (EmpresaId == Guid.Empty)
                return new ValidationHelper(false, "O ID da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(Numero))
                return new ValidationHelper(false, "O número da nota fiscal é obrigatório.");

            if (Valor <= 0)
                return new ValidationHelper(false, "O valor da nota fiscal deve ser maior que zero.");

            if (DataVencimento <= DateTime.UtcNow.Date)
                return new ValidationHelper(false, "A data de vencimento deve ser maior que a data atual.");

            return new ValidationHelper(true);
        }
    }
}
