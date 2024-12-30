using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TesteSize.API.InvoiceService.Domain.Entities;

namespace TesteSize.API.CartService.Domain.Entities
{
    public class CartInvoice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Carrinho))]       
        [Column("CarrinhoId")]
        public Guid CarrinhoId { get; set; }
        [JsonIgnore]
        public Cart Carrinho { get; set; }

        [ForeignKey(nameof(NotaFiscal))]
        [Column("NotaFiscalId")]
        public Guid NotaFiscalId { get; set; }
        [JsonIgnore]
        public Invoice NotaFiscal { get; set; }
    }
}
