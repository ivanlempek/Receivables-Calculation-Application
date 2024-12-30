using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TesteSize.API.CompanyService.Domain.Entities;

namespace TesteSize.API.CompanyService.Infrastructure.Data
{
    public class CompanyDbContext(DbContextOptions<CompanyDbContext> options) : DbContext(options)
    {
        public DbSet<Company> Empresas { get; set; }
    }
}

