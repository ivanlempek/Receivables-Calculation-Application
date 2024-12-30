namespace TesteSize.BaseDTOs.Invoice
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public Guid? EmpresaId { get; set; }
    }
}
