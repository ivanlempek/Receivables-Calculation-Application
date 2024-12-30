using Microsoft.AspNetCore.Mvc;
using TesteSize.API.InvoiceService.Application.Interfaces;
using TesteSize.API.InvoiceService.Domain.Entities;

namespace TesteSize.API.InvoiceService.API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar as operações relacionadas às notas fiscais.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceRepository;

        public InvoiceController(IInvoiceService invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        /// <summary>
        /// Cria uma nova nota fiscal.
        /// </summary>
        /// <param name="invoice">Dados da nota fiscal a ser criada.</param>
        /// <returns>Detalhes da nota fiscal criada.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            var validationResult = invoice.Validate();

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ErrorMessage);

            try
            {
                await _invoiceRepository.AddAsync(invoice);

                return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        /// <summary>
        /// Obtém uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="id">ID da nota fiscal.</param>
        /// <returns>Detalhes da nota fiscal.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            return Ok(invoice);
        }

        /// <summary>
        /// Obtém todas as notas fiscais.
        /// </summary>
        /// <returns>Lista de notas fiscais.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            return Ok(invoices);
        }

        /// <summary>
        /// Atualiza uma nota fiscal existente.
        /// </summary>
        /// <param name="id">ID da nota fiscal a ser atualizada.</param>
        /// <param name="invoice">Dados atualizados da nota fiscal.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] Invoice invoice)
        {
            if (id == Guid.Empty || invoice == null)
                return BadRequest("ID ou dados da nota fiscal são inválidos.");
            
            var validationResult = invoice.Validate();

            if (!validationResult.IsValid)
                return BadRequest (validationResult.ErrorMessage);

            try
            {
                var existingInvoice = await _invoiceRepository.GetByIdAsync(id);

                if (existingInvoice == null)
                    return NotFound("Empresa não encontrada.");

                await _invoiceRepository.UpdateAsync(invoice);

                return Ok("Nota fiscal atualizada com sucesso!");

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Deleta uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="id">ID da nota fiscal a ser deletada.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            await _invoiceRepository.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Obtém notas fiscais específicas para um carrinho, baseado em uma lista de IDs.
        /// </summary>
        /// <param name="invoiceIds">Lista de IDs de notas fiscais.</param>
        /// <returns>Lista de notas fiscais encontradas.</returns>
        [HttpPost("get-cart-invoices")]
        public async Task<IActionResult> GetCartInvoices([FromBody] List<Guid?> invoiceIds)
        {
            if (invoiceIds == null)
                return BadRequest("Nenhum ID de nota fornecido.");

            try
            {
                var invoices = await _invoiceRepository.GetCartInvoices(invoiceIds);

                if (invoices == null || !invoices.Any())
                    return NotFound("Nenhuma nota fiscal encontrada para os IDs informados.");

                var invoicesList = invoices.Select(i => new Invoice
                {
                    Id = i.Id,
                    Numero = i.Numero,
                    Valor = i.Valor,
                    DataVencimento = i.DataVencimento
                })
                .ToList();

                return Ok(invoicesList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém notas fiscais de uma empresa específica.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Lista de notas fiscais da empresa.</returns>
        [HttpGet("get-company-invoices/{companyId:guid}")]
        public async Task<IActionResult> GetInvoicesByCompany(Guid companyId)
        {
            if (companyId == null || companyId == Guid.Empty)
                return BadRequest("Nenhum ID de nota fornecido.");

            try
            {
                var companyInvoices = await _invoiceRepository.GetInvoicesByCompany(companyId);

                if (companyInvoices == null)
                    return NotFound("Nenhuma nota fiscal encontrada para a empresa informada.");

                return Ok(companyInvoices);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
