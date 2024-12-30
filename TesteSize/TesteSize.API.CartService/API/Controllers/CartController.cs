using Microsoft.AspNetCore.Mvc;
using TesteSize.API.CartService.Application.Interfaces;

namespace TesteSize.API.CartService.API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar as operações relacionadas ao carrinho de notas fiscais.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Adiciona uma nota fiscal ao carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal.</param>
        /// <returns>Retorna uma mensagem de sucesso ou erro.</returns>
        [HttpPost("{companyId}/add-invoice/{invoiceId}")]
        public async Task<IActionResult> AddInvoiceToCart(Guid companyId, Guid invoiceId)
        {
            if (companyId == Guid.Empty || invoiceId == Guid.Empty)
                return BadRequest("IDs de empresa ou nota fiscal são inválidos.");

            try
            {
                var result = await _cartService.AddInvoiceToCart(companyId, invoiceId);

                if (!result.IsValid)
                    return BadRequest(result.ErrorMessage);

                return Ok("Nota fiscal adicionada ao carrinho com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove uma nota fiscal do carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <param name="invoiceId">ID da nota fiscal.</param>
        /// <returns>Retorna uma mensagem de sucesso ou erro.</returns>
        [HttpDelete("{companyId}/remove-invoice/{invoiceId}")]
        public async Task<IActionResult> RemoveInvoiceFromCart(Guid companyId, Guid invoiceId)
        {
            if (companyId == Guid.Empty || invoiceId == Guid.Empty)
                return BadRequest("IDs de empresa ou nota fiscal são inválidos.");

            try
            {
                var result = await _cartService.RemoveInvoiceFromCart(companyId, invoiceId);

                if (!result.IsValid)
                    return BadRequest("Erro ao remover nota fiscal do carrinho.");

                return Ok("Nota fiscal removida do carrinho com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém o carrinho de uma empresa pelo ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Retorna o carrinho da empresa ou uma mensagem de erro.</returns>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCartByCompanyId(Guid companyId)
        {
            if (companyId == Guid.Empty)
                return BadRequest("ID de empresa é inválido.");

            try
            {
                var cart = await _cartService.GetCartByCompanyId(companyId);

                if (cart == null)
                    return NotFound("Carrinho não encontrado para a empresa.");

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Calcula o valor total de antecipação para o carrinho de uma empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Retorna o cálculo de antecipação do carrinho.</returns>
        [HttpGet("checkout/{companyId:guid}")]
        public async Task<IActionResult> CalcularCheckout(Guid companyId)
        {
            var checkCart = await _cartService.GetCartByCompanyId(companyId);

            if (checkCart == null)
                return NotFound("Carrinho não encontrado.");

            try
            {
                var result = await _cartService.CalculateAnticipationAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
