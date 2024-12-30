using TesteSize.BaseDTOs.Company;

namespace TesteSize.ExternalServices.Interfaces
{
    public interface IESCompanyService
    {
        Task<decimal> GetCreditLimitAsync(Guid companyId);
        Task<CompanyDto> GetCompanyById(Guid? companyId);
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<CompanyDto> CreateAsync(CompanyDto company);
        Task<bool> UpdateAsync(CompanyDto company);
        Task<bool> DeleteAsync(Guid? id);
    }
}
