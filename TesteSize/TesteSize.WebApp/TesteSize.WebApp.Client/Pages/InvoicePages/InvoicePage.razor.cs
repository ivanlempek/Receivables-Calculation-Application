using MudBlazor;
using TesteSize.BaseDTOs.Invoice;
using TesteSize.WebApp.Client.Components;

namespace TesteSize.WebApp.Client.Pages.InvoicePages
{
    public partial class InvoicePage
    {
        private IEnumerable<InvoiceDto> _invoices = new List<InvoiceDto>();

        private string? _searchString;

        private bool _sortNameByLength;

        private bool _isLoading;

        private List<string> _events = new();

        private InvoiceDto _selectedItem;

        private MudDataGrid<InvoiceDto> _dataGrid;

        private Func<InvoiceDto, object?> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.Numero!.Length;
            else
                return x.Numero;
        };

        private Func<InvoiceDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            return Task.Run(() =>
            {
                if ($"{x.Id}".Contains(_searchString))
                    return true;

                if (x.Numero!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).GetAwaiter().GetResult();
        };

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            var result = await InvoiceService.GetAllAsync();

            _invoices = result;

            _isLoading = false;
        }

        private async Task OpenAddInvoiceDialog()
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<AddInvoiceDialog>("Adicionar Nota fiscal", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _invoices = await InvoiceService.GetAllAsync();
            }
        }

        private async Task OpenUpdateCompanyDialog(InvoiceDto selectedInvoice)
        {
            var parameters = new DialogParameters
        {
            { "Invoice", selectedInvoice }
        };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<AddInvoiceDialog>("Atualizar Nota fiscal", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await RefreshPage();
            }
        }

        private async Task OpenUpdateDialog()
        {
            if (_selectedItem == null || _selectedItem.Id == null || _selectedItem.Id == Guid.Empty)
            {
                Snackbar.Add("Selecione uma nota fiscal para atualizar.", Severity.Warning);
                return;
            }

            await OpenUpdateCompanyDialog(_selectedItem);
        }

        private async Task DeleteInvoice()
        {
            if (_selectedItem == null || _selectedItem.Id == null || _selectedItem.Id == Guid.Empty)
            {
                Snackbar.Add("Selecione uma nota fiscal para deletar.", Severity.Warning);
                return;
            }

            await InvoiceService.DeleteAsync(_selectedItem.Id);

            Snackbar.Add("Nota fiscal excluída com sucesso.", Severity.Success);
            await RefreshPage();
        }

        // events
        void RowClicked(DataGridRowClickEventArgs<InvoiceDto> args)
        {
            _events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");

            _selectedItem = args.Item;
        }

        void RowRightClicked(DataGridRowClickEventArgs<InvoiceDto> args)
        {
            _events.Insert(0, $"Event = RowRightClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
        }

        void SelectedItemsChanged(HashSet<InvoiceDto> items)
        {
            _events.Insert(0, $"Event = SelectedItemsChanged, Data = {System.Text.Json.JsonSerializer.Serialize(items)}");
            _selectedItem = items.Any() ? items.First() : new InvoiceDto();
        }

        void ClearSelectedItems()
        {
            if (_dataGrid != null)
            {
                _dataGrid.SelectedItems = new HashSet<InvoiceDto>();
            }
        }

        async Task RefreshPage()
        {
            ClearSelectedItems();

            _invoices = await InvoiceService.GetAllAsync();
            StateHasChanged();
        }
    }
}
