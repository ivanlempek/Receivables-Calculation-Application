using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TesteSize.BaseDTOs.Invoice;

namespace TesteSize.BaseDTOs.Cart
{
    public class CartInvoiceDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Carrinho))]
        [Column("CarrinhoId")]
        public Guid CarrinhoId { get; set; }
        [JsonIgnore]
        public CartDto Carrinho { get; set; }

        [ForeignKey(nameof(NotaFiscal))]
        [Column("NotaFiscalId")]
        public Guid NotaFiscalId { get; set; }
        [JsonIgnore]
        public InvoiceDto NotaFiscal { get; set; }
    }
}
