using Helpers.Exceptions;
using TesteSize.API.CartService.Application.DTOs;
using TesteSize.API.CartService.Domain.Entities;

namespace TesteSize.API.CartService.Application.Interfaces
{
    public interface ICartService
    {
        Task<ValidationHelper> AddInvoiceToCart(Guid companyId, Guid invoiceId);
        Task<ValidationHelper> RemoveInvoiceFromCart(Guid companyId, Guid invoiceId);
        Task<Cart> GetCartByCompanyId(Guid companyId);
        Task<CheckoutResponse> CalculateAnticipationAsync(Guid companyId);
    }
}
