using TesteSize.API.CompanyService.Domain.Entities;

namespace TesteSize.API.CompanyService.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetByIdAsync(Guid id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Guid id);
        bool Exists(Guid id);
    }
}
