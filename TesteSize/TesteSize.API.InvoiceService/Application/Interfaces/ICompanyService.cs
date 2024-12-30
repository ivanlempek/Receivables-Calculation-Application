namespace TesteSize.API.InvoiceService.Application.Interfaces;

public interface ICompanyService
{
    Task<bool> CheckIfCompanyExistsAsync(Guid companyId);
}
