using Microsoft.AspNetCore.Components;
using MudBlazor;
using TesteSize.BaseDTOs.Company;
using InvoiceDto = TesteSize.BaseDTOs.Invoice.InvoiceDto;

namespace TesteSize.WebApp.Client.Components
{
    public partial class AddInvoiceDialog
    {
        private InvoiceDto newInvoice = new InvoiceDto();
        [Parameter]
        public InvoiceDto? Invoice { get; set; }

        [CascadingParameter]
        private MudDialogInstance? MudDialog { get; set; }

        private string buttonText => Invoice?.Id != Guid.Empty && Invoice != null ? "Atualizar" : "Adicionar";

        private IEnumerable<InvoiceDto> _invoices = new List<InvoiceDto>();
        private IEnumerable<CompanyDto> _companies = new List<CompanyDto>();

        private string _company;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            if (Invoice?.Id != Guid.Empty && Invoice != null)
            {
                newInvoice = await InvoiceService.GetByIdAsync(Invoice.Id);

                var getCompany = await CompanyService.GetCompanyById(Invoice.EmpresaId);

                _company = getCompany.Nome;
            }
            else
            {
                newInvoice = new InvoiceDto();
            }

            _isLoading = false;
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                if (await InvoiceAlreadyExists() && newInvoice.Id == Guid.Empty)
                {
                    Snackbar.Add("Já existe uma  nota fiscal com esse número.", Severity.Warning);
                    return;
                }

                if (newInvoice.Id == Guid.Empty)
                {
                    var addInvoice = new InvoiceDto
                    {
                        Id = Guid.NewGuid(),
                        Numero = newInvoice.Numero,
                        Valor = newInvoice.Valor,
                        DataVencimento = newInvoice.DataVencimento,
                        EmpresaId = await CheckCompany(_company),
                    };

                    await InvoiceService.CreateAsync(addInvoice);
                    Snackbar.Add("Nota fiscal adicionada com sucesso.", Severity.Success);

                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    newInvoice.EmpresaId = await CheckCompany(_company);
                    await InvoiceService.UpdateAsync(newInvoice);
                    Snackbar.Add("Nota fiscal atualizada com sucesso.", Severity.Success);

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

        private async Task<bool> InvoiceAlreadyExists()
        {
            var invoices = await InvoiceService.GetAllAsync();

            foreach (var invoice in invoices)
            {
                if (newInvoice.Numero == invoice.Numero)
                    return true;
            }

            return false;
        }

        private async Task<Guid?> CheckCompany(string companyName)
        {
            var companyGuid = new Guid?();

            var companies = await CompanyService.GetAllAsync();

            foreach (var company in companies)
            {
                if (company.Nome == companyName)
                {
                    companyGuid = company.Id;

                    return companyGuid;
                }
            }
            return Guid.Empty;
        }
    }
}

