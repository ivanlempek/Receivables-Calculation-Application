using System.Net.Http.Json;
using TesteSize.BaseDTOs.Company;
using TesteSize.ExternalServices.Interfaces;

namespace TesteSize.ExternalServices.Services
{
    /// <summary>
    /// Serviço responsável por consumir a API de empresas.
    /// </summary>
    public class ESCompanyService : IESCompanyService
    {
        private readonly HttpClient _httpClient;

        public ESCompanyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Faz uma chamada à API para obter o limite de crédito de uma empresa com base no seu ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>O limite de crédito da empresa.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<decimal> GetCreditLimitAsync(Guid companyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"company/{companyId}/credit-limit");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar o limite de crédito: {errorMsg}");
                }

                return await response.Content.ReadFromJsonAsync<decimal>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para buscar uma empresa pelo ID.
        /// </summary>
        /// <param name="companyId">ID da empresa.</param>
        /// <returns>Um objeto <see cref="CompanyDto"/> representando a empresa.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<CompanyDto> GetCompanyById(Guid? companyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"company/{companyId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar empresa: {errorMsg}");
                }

                var company = await response.Content.ReadFromJsonAsync<CompanyDto>();

                if (company == null)
                    throw new Exception("Empresa não encontrada.");

                return company;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para buscar todas as empresas.
        /// </summary>
        /// <returns>Uma lista de <see cref="CompanyDto"/> representando as empresas.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetAsync($"company");

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao obter a lista de empresas: {errorMsg}");
                }

                var companies = await result.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>();

                return companies!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para criar uma nova empresa.
        /// </summary>
        /// <param name="company">Dados da empresa a ser criada.</param>
        /// <returns>Um objeto <see cref="CompanyDto"/> representando a empresa criada.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<CompanyDto> CreateAsync(CompanyDto company)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"company", company);

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao registrar empresa: {errorMsg}");
                }

                var createdCompany = await result.Content.ReadFromJsonAsync<CompanyDto>();

                if (createdCompany == null)
                    throw new Exception("Não foi possível criar a empresa");

                return createdCompany;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para atualizar os dados de uma empresa.
        /// </summary>
        /// <param name="company">Dados atualizados da empresa.</param>
        /// <returns><c>true</c> se a empresa for atualizada com sucesso.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<bool> UpdateAsync(CompanyDto company)
        {
            try
            {
                var result = await _httpClient.PutAsJsonAsync($"company/{company.Id}", company);

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao atualizar a empresa: {errorMsg}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Faz uma chamada à API para deletar uma empresa com base no ID.
        /// </summary>
        /// <param name="id">ID da empresa a ser deletada.</param>
        /// <returns><c>true</c> se a empresa for deletada com sucesso.</returns>
        /// <exception cref="Exception">Lançada se ocorrer um erro ao consumir a API.</exception>
        public async Task<bool> DeleteAsync(Guid? id)
        {
            try
            {
                var result = await _httpClient.DeleteAsync($"company/{id}");

                if (!result.IsSuccessStatusCode)
                {
                    var errorMsg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao deletar empresa: {errorMsg}");
                }

                return true;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
