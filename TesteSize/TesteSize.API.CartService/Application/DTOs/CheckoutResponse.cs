namespace TesteSize.API.CartService.Application.DTOs
{
    public class CheckoutResponse
    {
        public string Empresa { get; set; }
        public string CNPJ { get; set; }
        public decimal Limite { get; set; }
        public List<CheckoutNotaFiscal> NotasFiscais { get; set; } = new();
        public decimal TotalLiquido { get; set; }
        public decimal TotalBruto { get; set; }
    }
}
