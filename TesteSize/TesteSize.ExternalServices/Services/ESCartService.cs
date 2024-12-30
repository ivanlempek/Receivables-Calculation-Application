using System.Net.Http.Json;
using TesteSize.BaseDTOs.Cart;
using TesteSize.BaseDTOs.Checkout;
using TesteSize.ExternalServices.Interfaces;

namespace TesteSize.ExternalServices.Services
{
    /// <summary>
    /// Serviço externo para consumir a API do carrinho de notas fiscais.
    /// </summary>
    public class ESCartService : IESCartService
    {
        private readonly HttpClient _httpClient;

        public ESCartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Faz uma chamada à API de carrinho para obter o carrinho de uma empresa pelo ID.
        /// </summary>
        /// <param name="companyId">ID da empresa cujo carrinho será obtido.</param>
        /// <returns>Objeto <see cref="CartDto"/> representando o carrinho retornado pela API.</returns>
        /// <exception cref="ArgumentException">Lançada se o ID da empresa for inválido.</exception>
        /// <exception cref="Exception">Lançada se ocorrer um erro na comunicação com a API.</exception>
        public async Task<CartDto> GetCartByCompanyId(Guid? companyId)
        {
            if (companyId == null || companyId == Guid.Empty)
                throw new ArgumentException("O ID da empresa não pode ser vazio ou nulo.");

            try
            {
                var response = await _httpClient.GetAsync($"cart/{companyId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar carrinho: {errorMsg}");
                }

                var cart = await response.Content.ReadFromJsonAsync<CartDto>();

                return cart;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API de carrinho para adicionar uma nota fiscal ao carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal a ser adicionada.</param>
        /// <returns><c>true</c> se a nota fiscal for adicionada com sucesso.</returns>
        /// <exception cref="ArgumentException">Lançada se os IDs forem inválidos.</exception>
        /// <exception cref="Exception">Lançada se ocorrer um erro na comunicação com a API.</exception>
        public async Task<bool> AddInvoiceToCartAsync(Guid? companyId, Guid invoiceId)
        {
            if (companyId == Guid.Empty || invoiceId == Guid.Empty)
                throw new ArgumentException("IDs de empresa ou nota fiscal são inválidos.");

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"cart/{companyId}/add-invoice/{invoiceId}", new { });

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao adicionar nota fiscal ao carrinho: {errorMsg}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API do carrinho para remover uma nota fiscal do carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal a ser removida.</param>
        /// <returns><c>true</c> se a nota fiscal for removida com sucesso.</returns>
        /// <exception cref="ArgumentException">Lançada se os IDs forem inválidos.</exception>
        /// <exception cref="Exception">Lançada se ocorrer um erro na comunicação com a API.</exception>
        public async Task<bool> RemoveInvoiceFromCart(Guid? companyId, Guid invoiceId)
        {
            if (companyId == Guid.Empty || invoiceId == Guid.Empty)
                throw new ArgumentException("IDs de empresa ou nota fiscal são inválidos.");

            try
            {
                var response = await _httpClient.DeleteAsync($"cart/{companyId}/remove-invoice/{invoiceId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao remover nota fiscal do carrinho: {errorMsg}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API do carrinho para calcular o checkout de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Objeto <see cref="CheckoutResponseDto"/> com os detalhes do checkout retornado pela API.</returns>
        /// <exception cref="ArgumentException">Lançada se o ID da empresa for inválido.</exception>
        /// <exception cref="Exception">Lançada se ocorrer um erro na comunicação com a API.</exception>
        public async Task<CheckoutResponseDto> CalculateCheckout(Guid? companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("ID da empresa é inválido.");

            try
            {
                var response = await _httpClient.GetAsync($"cart/checkout/{companyId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao calcular checkout: {errorMsg}");
                }

                var checkoutResponse = await response.Content.ReadFromJsonAsync<CheckoutResponseDto>();

                return checkoutResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
