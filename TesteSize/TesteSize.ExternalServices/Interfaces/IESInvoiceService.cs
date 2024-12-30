using TesteSize.BaseDTOs.Invoice;

namespace TesteSize.ExternalServices.Interfaces
{
    public interface IESInvoiceService
    {
        Task<InvoiceDto> GetByIdAsync(Guid? invoiceId);
        Task<IEnumerable<InvoiceDto>> GetAllAsync();
        Task<InvoiceDto> CreateAsync(InvoiceDto company);
        Task<bool> UpdateAsync(InvoiceDto company);
        Task<bool> DeleteAsync(Guid? id);
        Task<IEnumerable<InvoiceDto>> GetCartInvoices(List<Guid> invoicesId);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByCompany(Guid? companyId);
    }
}
