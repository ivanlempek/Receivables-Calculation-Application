using Microsoft.EntityFrameworkCore;
using TesteSize.API.CompanyService.Application.Interfaces;
using TesteSize.API.CompanyService.Domain.Entities;
using TesteSize.API.CompanyService.Infrastructure.Data;

namespace TesteSize.API.CompanyService.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de persistência relacionadas às empresas.
    /// </summary>
    public class CompanyRepository : ICompanyService
    {
        private readonly CompanyDbContext _context;

        public CompanyRepository(CompanyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma empresa pelo ID.
        /// </summary>
        /// <param name="id">ID da empresa.</param>
        /// <returns>Instância da empresa encontrada.</returns>
        /// <exception cref="ArgumentException">Lançado se o ID for inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançado se nenhuma empresa for encontrada com o ID informado.</exception>
        public async Task<Company> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("ID inválido.");

            var company = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (company == null) 
                throw new KeyNotFoundException("Não existe uma empresa com esse ID.");

            return company;
        }

        /// <summary>
        /// Obtém todas as empresas.
        /// </summary>
        /// <returns>Lista de empresas.</returns>
        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Empresas.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Adiciona uma nova empresa.
        /// </summary>
        /// <param name="company">Entidade da empresa.</param>
        /// <exception cref="ArgumentException">Lançado se a entidade da empresa for inválida.</exception>
        /// <exception cref="Exception">Lançado se já existir uma empresa com o mesmo Nome, CNPJ ou ID.</exception>
        public async Task AddAsync(Company company)
        {
            if (company == null) 
                throw new ArgumentException("Empresa inválida.");

            var checkCompany = await CheckIfCompanyExists(company);

            if (checkCompany) 
                throw new Exception("Já existe uma empresa com esse Nome ou CNPJ ou ID.");

            await _context.Empresas.AddAsync(company);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza os dados de uma empresa existente.
        /// </summary>
        /// <param name="company">Entidade da empresa com os dados atualizados.</param>
        /// <exception cref="ArgumentException">Lançado se a entidade da empresa for inválida.</exception>
        public async Task UpdateAsync(Company company)
        {
            if (company == null) 
                throw new ArgumentException("Empresa inválida.");

            _context.Empresas.Update(company);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Exclui uma empresa pelo ID.
        /// </summary>
        /// <param name="id">ID da empresa.</param>
        /// <exception cref="ArgumentException">Lançado se o ID for inválido ou se nenhuma empresa for encontrada com o ID informado.</exception>
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("ID inválido.");

            var company = await _context.Empresas.FindAsync(id);

            if (company == null)
            {
                throw new ArgumentException("Não foi encontrado uma empresa com esse ID.");
            }
            else
            {
                _context.Empresas.Remove(company);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifica se uma empresa já existe com base no Nome, CNPJ ou ID.
        /// </summary>
        /// <param name="company">Entidade da empresa para verificação.</param>
        /// <returns>Retorna <c>true</c> se a empresa existir; caso contrário, <c>false</c>.</returns>
        public async Task<bool> CheckIfCompanyExists(Company company)
        {
            bool companyCheck = false;

            var result = await _context.Empresas.FirstOrDefaultAsync(x => x.Nome == company.Nome || x.CNPJ == company.CNPJ || x.Id == company.Id);

            if (result != null) companyCheck = true;

            return companyCheck;
        }

        /// <summary>
        /// Verifica se uma empresa existe pelo ID.
        /// </summary>
        /// <param name="id">ID da empresa.</param>
        /// <returns>Retorna <c>true</c> se a empresa existir; caso contrário, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Lançado se o ID for inválido.</exception>
        public bool Exists(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID inválido.");

            var check = _context.Empresas.Any(c => c.Id == id);

            return check;
           
        }
    }
}

