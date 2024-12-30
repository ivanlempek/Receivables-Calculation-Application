namespace TesteSize.API.CartService.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid EmpresaId { get; set; }
        public decimal ValorTotal { get; set; }
        public ICollection<CartInvoice> NotasFiscais { get; set; }

        protected Cart() { }

        public Cart(Guid empresaId)
        {
            Id = Guid.NewGuid();
            EmpresaId = empresaId;
        }
    }
}
