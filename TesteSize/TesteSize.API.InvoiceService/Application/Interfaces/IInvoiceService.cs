using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteSize.API.InvoiceService.Domain.Entities;

namespace TesteSize.API.InvoiceService.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> GetByIdAsync(Guid id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Invoice>> GetCartInvoices(List<Guid?> invoiceIds);
        Task<IEnumerable<Invoice>> GetInvoicesByCompany(Guid? companyId);
    }
}
