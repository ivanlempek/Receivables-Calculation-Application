using Microsoft.AspNetCore.Components;
using MudBlazor;
using TesteSize.BaseDTOs.Company;
using TesteSize.BaseDTOs.Invoice;

namespace TesteSize.WebApp.Client.Components
{
    public partial class AddInvoiceToCartDialog
    {
        private InvoiceDto newCartInvoice = new InvoiceDto();

        [Parameter]
        public Guid? CompanyId { get; set; }

        [Parameter]
        public IEnumerable<InvoiceDto> CartInvoices { get; set; }


        [CascadingParameter]
        private MudDialogInstance? MudDialog { get; set; }

        private string buttonText => "Adicionar";

        private IEnumerable<InvoiceDto> _invoices = new List<InvoiceDto>();

        private string? _company;
        private InvoiceDto? _selectedInvoice;

        protected override async Task OnInitializedAsync()
        {
            _invoices = await InvoiceService.GetInvoicesByCompany(CompanyId);

            if (CartInvoices != null && CartInvoices.Any())
            {
                _invoices = _invoices.Where(inv => !CartInvoices.Any(cartInv => cartInv.Id == inv.Id)).ToList();
            }
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    Snackbar.Add("Selecione uma nota fiscal para adicionar ao carrinho.", Severity.Warning);
                    return;
                }

                if (_selectedInvoice.Id != Guid.Empty)
                {

                    await CartService.AddInvoiceToCartAsync(_selectedInvoice.EmpresaId, _selectedInvoice.Id);

                    Snackbar.Add("Nota fiscal adicionada ao carrinho com sucesso.", Severity.Success);

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
    }
}
