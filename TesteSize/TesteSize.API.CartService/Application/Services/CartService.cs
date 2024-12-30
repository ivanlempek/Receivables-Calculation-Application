using Helpers.Exceptions;
using TesteSize.API.CartService.Application.Interfaces;
using TesteSize.API.CartService.Domain.Entities;
using TesteSize.ExternalServices.Interfaces;
using TesteSize.API.CartService.Application.DTOs;

namespace TesteSize.API.CartService.Application.Services
{
    /// <summary>
    /// Serviço responsável por gerenciar o carrinho de notas fiscais, incluindo operações como adição, remoção e cálculo de antecipação.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IESInvoiceService _invoiceService;
        private readonly IESCompanyService _companyService;

        public CartService(ICartRepository cartRepository, IESInvoiceService invoiceService, IESCompanyService companyService)
        {
            _cartRepository = cartRepository;
            _invoiceService = invoiceService;
            _companyService = companyService;
        }

        /// <summary>
        /// Adiciona uma nota fiscal ao carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal.</param>
        /// <returns>Validação contendo o status e a mensagem da operação.</returns>
        public async Task<ValidationHelper> AddInvoiceToCart(Guid companyId, Guid invoiceId)
        {
            var cart = await _cartRepository.GetCartByCompanyIdAsync(companyId);

            if (cart == null)
            {
                cart = new Cart(companyId);
                await _cartRepository.AddCartAsync(cart);
            }

            var invoice = await _invoiceService.GetByIdAsync(invoiceId);

            if (invoice == null)
                return new ValidationHelper(false, "Nota fiscal não encontrada.");

            var companyCreditLimit = await GetCompanyCreditLimit(cart.EmpresaId);

            if (companyCreditLimit == 0)
                return new ValidationHelper(false, "Empresa com faturamento inferior à R$10.000.");

            if (cart.ValorTotal + invoice.Valor > companyCreditLimit)
                return new ValidationHelper(false, "O limite de crédito da empresa foi ultrapassado.");

            await _cartRepository.AddInvoiceToCartAsync(cart.Id, invoiceId, invoice.Valor);

            return new ValidationHelper(true, "Nota fiscal adicionada ao carrinho com sucesso.");
        }

        /// <summary>
        /// Remove uma nota fiscal do carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal.</param>
        /// <returns>Validação contendo o status e a mensagem da operação.</returns>
        public async Task<ValidationHelper> RemoveInvoiceFromCart(Guid companyId, Guid invoiceId)
        {
            var cart = await _cartRepository.GetCartByCompanyIdAsync(companyId);

            if (cart == null)
                return new ValidationHelper(false, "Carrinho não encontrado.");

            var invoice = await _invoiceService.GetByIdAsync(invoiceId);

            await _cartRepository.RemoveInvoiceFromCartAsync(cart.Id, invoiceId, invoice.Valor);

            await _cartRepository.UpdateAsync(cart);

            return new ValidationHelper(true, "Nota fiscal removida com sucesso.");
        }

        /// <summary>
        /// Obtém o carrinho de uma empresa pelo seu ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>O carrinho correspondente à empresa.</returns>
        /// <exception cref="ArgumentException">Lançado se o ID for inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançado se o carrinho não for encontrado.</exception>
        public async Task<Cart> GetCartByCompanyId(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("ID de empresa inválido.");

            var cart = await _cartRepository.GetCartByCompanyIdAsync(companyId);

            if (cart == null)
                throw new KeyNotFoundException("Carrinho não encontrado para a empresa.");

            return cart;
        }

        /// <summary>
        /// Calcula a antecipação de valores para o carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Resumo do checkout contendo valores brutos, líquidos e informações das notas fiscais.</returns>
        /// <exception cref="Exception">Lançado se o carrinho não for encontrado.</exception>
        public async Task<CheckoutResponse> CalculateAnticipationAsync(Guid companyId)
        {
            var cart = await _cartRepository.GetCartByCompanyIdAsync(companyId);

            if (cart == null)
                throw new Exception("Carrinho não encontrado.");

            var empresa = await _companyService.GetCompanyById(cart.EmpresaId);

            var response = new CheckoutResponse
            {
                Empresa = empresa.Nome,
                CNPJ = empresa.CNPJ,
                Limite = await GetCompanyCreditLimit(cart.EmpresaId) 
            };

            decimal totalBruto = 0;
            double totalLiquido = 0;

            foreach (var cartInvoice in cart.NotasFiscais)
            {

                var invoice = await _invoiceService.GetByIdAsync(cartInvoice.NotaFiscalId);
                decimal valorBruto = invoice.Valor;  

                int prazoDias = (invoice.DataVencimento - DateTime.Today).Days;

                if (prazoDias < 0) prazoDias = 0; 

                double taxa = 0.0465; 
                double baseExp = prazoDias / 30.0;

                double valorLiquido = (double)valorBruto / Math.Pow(1 + taxa, baseExp);
                valorLiquido = double.Round(valorLiquido, 2);

                totalBruto += valorBruto;
                totalLiquido += valorLiquido;

                response.NotasFiscais.Add(new CheckoutNotaFiscal
                {
                    Numero = invoice.Numero,
                    ValorBruto = valorBruto,
                    ValorLiquido = (decimal)valorLiquido
                });
            }

            response.TotalBruto = totalBruto;
            response.TotalLiquido = (decimal)totalLiquido;

            return response;
        }

        /// <summary>
        /// Obtém o limite de crédito de uma empresa pelo seu ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>O limite de crédito da empresa.</returns>
        public async Task<decimal> GetCompanyCreditLimit(Guid companyId)
        {
            return await _companyService.GetCreditLimitAsync(companyId);
        }
    }
}
