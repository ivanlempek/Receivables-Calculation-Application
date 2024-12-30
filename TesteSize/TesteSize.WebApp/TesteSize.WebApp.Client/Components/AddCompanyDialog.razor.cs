using Microsoft.AspNetCore.Components;
using MudBlazor;
using TesteSize.BaseDTOs.Company;

namespace TesteSize.WebApp.Client.Components
{
    public partial class AddCompanyDialog
    {
        private CompanyDto newCompany = new CompanyDto();
        [Parameter]
        public CompanyDto? Company { get; set; } 

        [CascadingParameter]
        private MudDialogInstance? MudDialog { get; set; }

        private string buttonText => Company?.Id != Guid.Empty && Company != null ? "Atualizar" : "Adicionar";

        private IEnumerable<CompanyDto> _companies = new List<CompanyDto>();

        private string _comboBoxLabel = "Ramos";

        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            if (Company?.Id != Guid.Empty && Company != null)
            {
                newCompany = await CompanyService.GetCompanyById(Company.Id);
            }
            else
            {
                newCompany = new CompanyDto();
            }
            _isLoading = false;
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                if (await CompanyAlreadyExists() && newCompany.Id == null)
                {
                    Snackbar.Add("Já existe uma  empresa com esse CNPJ.", Severity.Warning);
                    return;
                }

                if (newCompany.Nome.Length <= 3)
                {
                    Snackbar.Add("O nome da empresa precisa ter mais do que 3 caracteres.", Severity.Warning);
                    return;
                }

                if (newCompany.Id == null)
                {
                    var addCompany = new CompanyDto
                    {
                        Nome = newCompany.Nome,
                        CNPJ = newCompany.CNPJ,
                        FaturamentoMensal = newCompany.FaturamentoMensal,
                        Ramo = newCompany.Ramo
                    };

                    await CompanyService.CreateAsync(addCompany);
                    Snackbar.Add("Empresa adicionada com sucesso.", Severity.Success);

                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    await CompanyService.UpdateAsync(newCompany);
                    Snackbar.Add("Empresa atualizada com sucesso.", Severity.Success);

                    MudDialog.Close(DialogResult.Ok(true));
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Warning);
            }
        }

        private void Cancel()
        {
            MudDialog.Close(DialogResult.Cancel());
        }

        private async Task<IEnumerable<string?>> Search(string value, CancellationToken token)
        {
            _companies = await CompanyService.GetAllAsync();

            var resultWithoutStringEmpty = _companies.Where(c => !string.IsNullOrEmpty(c.Nome));

            return resultWithoutStringEmpty.Select(c => c.Nome);
        }

        private async Task<bool> CompanyAlreadyExists()
        {
            var companies = await CompanyService.GetAllAsync();

            foreach (var company in companies)
            {
                if (newCompany.CNPJ == company.CNPJ)
                    return true;
            }

            return false;
        }
    }
}
