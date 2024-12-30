using Microsoft.EntityFrameworkCore;
using TesteSize.API.InvoiceService.Domain.Entities;

namespace TesteSize.API.InvoiceService.Infrastructure.Data
{
    public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options)
    {
        public DbSet<Invoice> NotasFiscais { get; set; }
    }
}
