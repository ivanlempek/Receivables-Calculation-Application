namespace TesteSize.BaseDTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid EmpresaId { get; set; }
        public decimal ValorTotal { get; set; }
        public ICollection<CartInvoiceDto>? NotasFiscais { get; set; }
    }
}
