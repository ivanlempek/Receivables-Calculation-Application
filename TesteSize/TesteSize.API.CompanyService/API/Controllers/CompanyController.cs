using Microsoft.AspNetCore.Mvc;
using TesteSize.API.CompanyService.Application.Interfaces;
using TesteSize.API.CompanyService.Domain.Entities;

namespace TesteSize.API.CompanyService.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar as operações relacionadas às empresas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyRepository;
    private readonly ICreditLimitService _creditLimitService;

    public CompanyController(ICompanyService companyRepository, ICreditLimitService creditLimitService)
    {
        _companyRepository = companyRepository;
        _creditLimitService = creditLimitService;
    }

    /// <summary>
    /// Obtém os detalhes de uma empresa pelo ID.
    /// </summary>
    /// <param name="id">ID da empresa.</param>
    /// <returns>Os dados da empresa, se encontrada.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido.");

        try
        {
            var company = await _companyRepository.GetByIdAsync(id);

            return Ok(company);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); 
        }
    }

    /// <summary>
    /// Obtém a lista de todas as empresas.
    /// </summary>
    /// <returns>Lista de empresas.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var companies = await _companyRepository.GetAllAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova empresa.
    /// </summary>
    /// <param name="company">Objeto contendo os dados da empresa.</param>
    /// <returns>Empresa criada.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] Company company)
    {
        if (company == null)
            return BadRequest("Os dados da empresa são obrigatórios.");

        var validationResult = company.Validate();

        if (!validationResult.IsValid)
            return BadRequest(validationResult.ErrorMessage);

        try
        {
            await _companyRepository.AddAsync(company);

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }

    /// <summary>
    /// Atualiza os dados de uma empresa existente.
    /// </summary>
    /// <param name="id">ID da empresa a ser atualizada.</param>
    /// <param name="company">Dados atualizados da empresa.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] Company company)
    {
        if (id == Guid.Empty || company == null)
            return BadRequest("ID ou dados da empresa são inválidos.");

        var validationResult = company.Validate();

        if (!validationResult.IsValid)
            return BadRequest(validationResult.ErrorMessage);

        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(id);

            if (existingCompany == null)
                return NotFound("Empresa não encontrada.");

            var updatedCompany = new Company(
                id, 
                company.CNPJ,
                company.Nome,
                company.FaturamentoMensal,
                company.Ramo
            );

            await _companyRepository.UpdateAsync(updatedCompany);

            return Ok("Empresa atualizada com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }

    /// <summary>
    /// Deleta uma empresa pelo ID.
    /// </summary>
    /// <param name="id">ID da empresa a ser deletada.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido.");

        try
        {
            await _companyRepository.DeleteAsync(id);

            return Ok("Empresa deletada com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }

    /// <summary>
    /// Verifica se uma empresa existe pelo ID.
    /// </summary>
    /// <param name="id">ID da empresa.</param>
    /// <returns>Objeto indicando se a empresa existe.</returns>
    [HttpGet("{id:guid}/exists")]
    public IActionResult CheckIfExists(Guid id)
    {
        try
        {
            var exists = _companyRepository.Exists(id);

            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        } 
    }

    /// <summary>
    /// Obtém o limite de crédito de uma empresa com base no faturamento e ramo.
    /// </summary>
    /// <param name="id">ID da empresa.</param>
    /// <returns>Limite de crédito calculado.</returns>
    [HttpGet("{id:guid}/credit-limit")]
    public async Task<decimal> GetCreditLimit(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            throw new Exception("Empresa não encontrada.");

        var limit = _creditLimitService.CalculateLimit(company.FaturamentoMensal, company.Ramo);
        var limitAdjust = Math.Round(limit, 2);

        return limitAdjust;
    }
}
