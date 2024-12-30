using TesteSize.API.CompanyService.Application.Interfaces;
using TesteSize.API.CompanyService.Domain.Enums;

namespace TesteSize.API.CompanyService.Application.Services
{
    /// <summary>
    /// Serviço responsável pelo cálculo do limite de crédito de empresas com base no faturamento mensal e no ramo de atuação.
    /// </summary>
    public class CreditLimitService : ICreditLimitService
    {
        /// <summary>
        /// Calcula o limite de crédito para uma empresa com base no faturamento mensal e no ramo de atuação.
        /// </summary>
        /// <param name="faturamentoMensal">O faturamento mensal da empresa.</param>
        /// <param name="ramo">O ramo de atuação da empresa.</param>
        /// <returns>O valor do limite de crédito calculado.</returns>
        /// <exception cref="ArgumentException">
        /// Lançado quando o faturamento mensal é menor ou igual a zero ou quando o faturamento não é suportado.
        /// </exception>
        public decimal CalculateLimit(decimal faturamentoMensal, Ramo ramo)
        {
            if (faturamentoMensal <= 0)
                throw new ArgumentException("Faturamento mensal deve ser maior que zero.");

            if (faturamentoMensal < 10_000)
            {
                return 0;
            }

            if (faturamentoMensal >= 10_000 && faturamentoMensal <= 50_000)
            {
                return faturamentoMensal * 0.50m;
            }

            if (faturamentoMensal > 50_000 && faturamentoMensal <= 100_000)
            {
                return ramo == Ramo.Servicos ? faturamentoMensal * 0.55m : faturamentoMensal * 0.60m;
            }

            if (faturamentoMensal > 100_000)
            {
                return ramo == Ramo.Servicos ? faturamentoMensal * 0.60m : faturamentoMensal * 0.65m;
            }

            throw new ArgumentException("O faturamento dessa empresa não é suportado.");
        }
    }
}
