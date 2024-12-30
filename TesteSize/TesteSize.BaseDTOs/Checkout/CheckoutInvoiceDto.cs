namespace TesteSize.BaseDTOs.Checkout
{
    public class CheckoutInvoiceDto
    {
        public string Numero { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorLiquido { get; set; }
    }
}
