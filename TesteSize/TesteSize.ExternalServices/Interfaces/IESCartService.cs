using TesteSize.BaseDTOs.Cart;
using TesteSize.BaseDTOs.Checkout;

namespace TesteSize.ExternalServices.Interfaces
{
    public interface IESCartService
    {
        Task<CartDto> GetCartByCompanyId(Guid? companyId);
        Task<bool> AddInvoiceToCartAsync(Guid? companyId, Guid invoiceId);
        Task<bool> RemoveInvoiceFromCart(Guid? companyId, Guid invoiceId);
        Task<CheckoutResponseDto> CalculateCheckout(Guid? companyId);
    }
}
