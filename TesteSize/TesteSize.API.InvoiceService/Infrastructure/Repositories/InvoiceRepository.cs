using Microsoft.EntityFrameworkCore;
using TesteSize.API.InvoiceService.Application.Interfaces;
using TesteSize.API.InvoiceService.Domain.Entities;
using TesteSize.API.InvoiceService.Infrastructure.Data;

namespace TesteSize.API.InvoiceService.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório para operações de persistência e recuperação de notas fiscais.
    /// </summary>
    public class InvoiceRepository : IInvoiceService
    {
        private readonly InvoiceDbContext _context;

        private readonly ICompanyService _companyService;

        public InvoiceRepository(InvoiceDbContext context, ICompanyService companyService)
        {
            _context = context;
            _companyService = companyService;
        }

        /// <summary>
        /// Adiciona uma nova nota fiscal ao banco de dados.
        /// </summary>
        /// <param name="invoice">Dados da nota fiscal a ser adicionada.</param>
        /// <exception cref="ArgumentException">Lançada se a nota fiscal for inválida.</exception>
        /// <exception cref="KeyNotFoundException">Lançada se a empresa associada à nota não for encontrada.</exception>
        public async Task AddAsync(Invoice invoice)
        {
            if (invoice == null) 
                throw new ArgumentException("Nota fiscal inválida.");

            var companyExists = await _companyService.CheckIfCompanyExistsAsync(invoice.EmpresaId);

            if (!companyExists)
                throw new KeyNotFoundException("Empresa não encontrada para associar à nota fiscal.");

            var checkInvoice = await CheckIfInvoiceExists(invoice);

            if (checkInvoice) 
                throw new Exception("Já existe uma empresa com esse número.");

            await _context.NotasFiscais.AddAsync(invoice);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="id">ID da nota fiscal.</param>
        /// <returns>Nota fiscal correspondente ao ID.</returns>
        /// <exception cref="ArgumentException">Lançada se o ID for inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançada se a nota fiscal não for encontrada.</exception>
        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID inválido.");

            var invoice = await _context.NotasFiscais.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                throw new KeyNotFoundException("Não existe uma nota fiscal com esse ID.");

            return invoice;
        }

        /// <summary>
        /// Obtém todas as notas fiscais.
        /// </summary>
        /// <returns>Lista de todas as notas fiscais.</returns>
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.NotasFiscais.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Atualiza uma nota fiscal no banco de dados.
        /// </summary>
        /// <param name="invoice">Dados da nota fiscal a ser atualizada.</param>
        /// <exception cref="ArgumentException">Lançada se a nota fiscal for inválida.</exception>
        public async Task UpdateAsync(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentException("Nota fiscal inválida.");

            _context.NotasFiscais.Update(invoice);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="id">ID da nota fiscal a ser removida.</param>
        /// <exception cref="ArgumentException">Lançada se o ID for inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançada se a nota fiscal não for encontrada.</exception>
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("ID inválido.");

            var invoice = await _context.NotasFiscais.FindAsync(id);

            if (invoice == null)
                throw new KeyNotFoundException("Não foi encontrado uma nota fiscal com esse ID.");

            _context.NotasFiscais.Remove(invoice);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica se uma nota fiscal com as mesmas características já existe.
        /// </summary>
        /// <param name="invoice">Dados da nota fiscal.</param>
        /// <returns><c>true</c> se a nota fiscal existir; caso contrário, <c>false</c>.</returns>
        public async Task<bool> CheckIfInvoiceExists(Invoice invoice)
        {
            bool invoiceCheck = false;

            var result = await _context.NotasFiscais.FirstOrDefaultAsync(x => x.Numero == invoice.Numero);

            if (result != null) invoiceCheck = true;

            return invoiceCheck;
        }

        /// <summary>
        /// Obtém as notas fiscais associadas a um carrinho.
        /// </summary>
        /// <param name="invoiceIds">Lista de IDs das notas fiscais.</param>
        /// <returns>Lista de notas fiscais associadas.</returns>
        /// <exception cref="ArgumentException">Lançada se nenhum ID for fornecido.</exception>
        public async Task<IEnumerable<Invoice>> GetCartInvoices(List<Guid?> invoiceIds)
        {
            if (invoiceIds == null || !invoiceIds.Any())
                throw new ArgumentException("Informe algum ID de nota fiscal válido.");

            var invoices = await _context.NotasFiscais.Where(i => invoiceIds.Contains(i.Id)).ToListAsync();

            return invoices;
        }

        /// <summary>
        /// Obtém as notas fiscais associadas a uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Lista de notas fiscais da empresa.</returns>
        /// <exception cref="ArgumentException">Lançada se o ID da empresa for inválido.</exception>
        public async Task<IEnumerable<Invoice>> GetInvoicesByCompany(Guid? companyId)
        {
            if (companyId == null || companyId == Guid.Empty)
                throw new ArgumentException("Informe um ID de empresa válido.");

            var companyInvoices = await _context.NotasFiscais.Where(x => x.EmpresaId == companyId).ToListAsync(); 

            return companyInvoices;
        }
    }
}
