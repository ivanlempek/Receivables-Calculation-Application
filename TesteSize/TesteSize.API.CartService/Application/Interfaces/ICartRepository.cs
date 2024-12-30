using TesteSize.API.CartService.Domain.Entities;

namespace TesteSize.API.CartService.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByCompanyIdAsync(Guid companyId);
        Task AddCartAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task AddInvoiceToCartAsync(Guid cartId, Guid invoiceId, decimal invoiceValue);
        Task RemoveInvoiceFromCartAsync(Guid cartId, Guid invoiceId, decimal invoiceValue);
    }
}
