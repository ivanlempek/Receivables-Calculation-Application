using System.Net.Http.Json;
using TesteSize.BaseDTOs.Invoice;
using TesteSize.ExternalServices.Interfaces;

namespace TesteSize.ExternalServices.Services
{
    /// <summary>
    /// Serviço responsável por consumir a API de notas fiscais.
    /// </summary>
    public class ESInvoiceService : IESInvoiceService
    {
        private readonly HttpClient _httpClient;

        public ESInvoiceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Faz uma chamada à API para buscar uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="invoiceId">ID da nota fiscal.</param>
        /// <returns>Um objeto <see cref="InvoiceDto"/> representando a nota fiscal.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<InvoiceDto> GetByIdAsync(Guid? invoiceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"invoice/{invoiceId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar a nota fiscal: {errorMsg}");
                }

                return await response.Content.ReadFromJsonAsync<InvoiceDto>();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            } 
        }

        /// <summary>
        /// Faz uma chamada à API para buscar todas as notas fiscais.
        /// </summary>
        /// <returns>Uma lista de <see cref="InvoiceDto"/> representando todas as notas fiscais.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetAsync($"invoice");

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao obter a lista de notas fiscais: {errorMsg}");
                }
                    

                var invoices = await result.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>();

                return invoices!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para criar uma nova nota fiscal.
        /// </summary>
        /// <param name="invoice">Objeto com os dados da nota fiscal a ser criada.</param>
        /// <returns>Um objeto <see cref="InvoiceDto"/> representando a nota fiscal criada.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<InvoiceDto> CreateAsync(InvoiceDto invoice)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"invoice", invoice);

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao adicionar nota fiscal: {errorMsg}");
                }
                    

                var createdInvoice = await result.Content.ReadFromJsonAsync<InvoiceDto>();

                if (createdInvoice == null)
                    throw new Exception("Não foi possível criar a nota fiscal.");

                return createdInvoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para atualizar uma nota fiscal.
        /// </summary>
        /// <param name="invoice">Objeto com os dados atualizados da nota fiscal.</param>
        /// <returns><c>true</c> se a nota fiscal for atualizada com sucesso.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<bool> UpdateAsync(InvoiceDto invoice)
        {
            try
            {
                var result = await _httpClient.PutAsJsonAsync($"invoice/{invoice.Id}", invoice);

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao atualizar a nota fiscal: {errorMsg}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para deletar uma nota fiscal pelo ID.
        /// </summary>
        /// <param name="id">ID da nota fiscal a ser deletada.</param>
        /// <returns><c>true</c> se a nota fiscal for deletada com sucesso.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<bool> DeleteAsync(Guid? id)
        {
            try
            {
                var result = await _httpClient.DeleteAsync($"invoice/{id}");

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao deletar nota fiscal: {errorMsg}");
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para buscar notas fiscais associadas a um carrinho.
        /// </summary>
        /// <param name="invoicesId">Lista de IDs das notas fiscais.</param>
        /// <returns>Uma lista de <see cref="InvoiceDto"/> representando as notas fiscais do carrinho.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<IEnumerable<InvoiceDto>> GetCartInvoices(List<Guid> invoicesId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("invoice/get-cart-invoices", invoicesId);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar as notas do carrinho: {errorMsg}");
                }

                var invoices = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>();

                return invoices ?? Array.Empty<InvoiceDto>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }   
        }

        /// <summary>
        /// Faz uma chamada à API para buscar notas fiscais de uma empresa pelo ID da empresa.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Uma lista de <see cref="InvoiceDto"/> representando as notas fiscais da empresa.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<IEnumerable<InvoiceDto>> GetInvoicesByCompany(Guid? companyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"invoice/get-company-invoices/{companyId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar as notas da empresa: {errorMsg}");
                }

                var invoices = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>();

                return invoices ?? Array.Empty<InvoiceDto>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
