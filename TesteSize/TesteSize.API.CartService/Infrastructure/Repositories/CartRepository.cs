using Microsoft.EntityFrameworkCore;
using TesteSize.API.CartService.Application.Interfaces;
using TesteSize.API.CartService.Domain.Entities;
using TesteSize.API.CartService.Infrastructure.Data;

namespace TesteSize.API.CartService.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório para gerenciamento de operações relacionadas ao carrinho de notas fiscais.
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;

        public CartRepository(CartDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém o carrinho de uma empresa pelo seu ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>O carrinho da empresa, incluindo as notas fiscais associadas.</returns>
        public async Task<Cart?> GetCartByCompanyIdAsync(Guid companyId)
        {
            return await _context.Carrinhos.Include(c => c.NotasFiscais).FirstOrDefaultAsync(x => x.EmpresaId == companyId);
        }

        /// <summary>
        /// Adiciona um novo carrinho ao banco de dados.
        /// </summary>
        /// <param name="cart">Entidade do carrinho a ser adicionada.</param>
        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carrinhos.AddAsync(cart);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza os dados de um carrinho existente.
        /// </summary>
        /// <param name="cart">Entidade do carrinho com os dados atualizados.</param>
        public async Task UpdateAsync(Cart cart)
        {
            _context.Carrinhos.Update(cart);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adiciona uma nota fiscal a um carrinho.
        /// </summary>
        /// <param name="cartId">ID do carrinho.</param>
        /// <param name="invoiceId">ID da nota fiscal a ser adicionada.</param>
        /// <param name="invoiceValue">Valor da nota fiscal.</param>
        public async Task AddInvoiceToCartAsync(Guid cartId, Guid invoiceId, decimal invoiceValue)
        {
            var cart = await _context.Carrinhos.Include(c => c.NotasFiscais).FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                cart.ValorTotal += invoiceValue;

                _context.NotasFiscaisCarrinho.Add(new CartInvoice
                {
                    CarrinhoId = cartId,
                    NotaFiscalId = invoiceId
                });

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Remove uma nota fiscal de um carrinho.
        /// </summary>
        /// <param name="cartId">ID do carrinho.</param>
        /// <param name="invoiceId">ID da nota fiscal a ser removida.</param>
        /// <param name="invoiceValue">Valor da nota fiscal a ser subtraído do total do carrinho.</param>
        /// <exception cref="Exception">Lançada quando a nota fiscal não está associada ao carrinho.</exception>
        public async Task RemoveInvoiceFromCartAsync(Guid cartId, Guid invoiceId, decimal invoiceValue)
        {
            var cart = await _context.Carrinhos.Include(c => c.NotasFiscais).FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                var cartInvoice = await _context.NotasFiscaisCarrinho
                    .FirstOrDefaultAsync(ci => ci.CarrinhoId == cartId && ci.NotaFiscalId == invoiceId);

                if (cartInvoice == null)
                {
                    throw new Exception("Essa nota fiscal não está adicionada a esse carrinho!");
                }
                else 
                {
                    cart.ValorTotal -= invoiceValue;

                    _context.NotasFiscaisCarrinho.Remove(cartInvoice);

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
