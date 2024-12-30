using System.Text.Json;
using TesteSize.API.InvoiceService.Application.Interfaces;

namespace TesteSize.API.InvoiceService.Application.Services
{
    /// <summary>
    /// Serviço responsável por interagir com a API de empresas para verificar informações relacionadas às empresas.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;

        public CompanyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Verifica se uma empresa existe na base de dados.
        /// </summary>
        /// <param name="companyId">ID da empresa a ser verificada.</param>
        /// <returns>Retorna <c>true</c> se a empresa existir; caso contrário, <c>false</c>.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro na comunicação com a API de empresas.</exception>
        public async Task<bool> CheckIfCompanyExistsAsync(Guid companyId)
        {
            var response = await _httpClient.GetAsync($"{companyId}/exists");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao verificar a empresa no serviço de Company.");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var exists = doc.RootElement.GetProperty("exists").GetBoolean();

            return exists;
        }
    }
}
