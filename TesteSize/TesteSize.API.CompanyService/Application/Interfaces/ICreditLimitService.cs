using TesteSize.API.CompanyService.Domain.Enums;

namespace TesteSize.API.CompanyService.Application.Interfaces;

public interface ICreditLimitService
{
    decimal CalculateLimit(decimal faturamentoMensal, Ramo ramo);
}
