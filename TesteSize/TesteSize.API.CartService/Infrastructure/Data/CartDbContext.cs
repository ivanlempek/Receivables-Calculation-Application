using Microsoft.EntityFrameworkCore;
using TesteSize.API.CartService.Domain.Entities;

namespace TesteSize.API.CartService.Infrastructure.Data
{
    public class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options)
    {
        public DbSet<Cart> Carrinhos { get; set; }
        public DbSet<CartInvoice> NotasFiscaisCarrinho { get; set; }
    }
}
