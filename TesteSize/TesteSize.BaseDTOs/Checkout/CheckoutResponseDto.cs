namespace TesteSize.BaseDTOs.Checkout
{
    public class CheckoutResponseDto
    {
        public string Empresa { get; set; }
        public string CNPJ { get; set; }
        public decimal Limite { get; set; }
        public List<CheckoutInvoiceDto> NotasFiscais { get; set; } = new();
        public decimal TotalLiquido { get; set; }
        public decimal TotalBruto { get; set; }
    }
}
