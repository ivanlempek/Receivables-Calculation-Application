using MudBlazor;
using TesteSize.BaseDTOs.Company;
using TesteSize.WebApp.Client.Components;

namespace TesteSize.WebApp.Client.Pages.CompanyPages
{
    public partial class CompanyPage
    {
        private IEnumerable<CompanyDto> _companies = new List<CompanyDto>();

        private string? _searchString;

        private bool _sortNameByLength;

        private bool _isLoading;

        private List<string> _events = new();

        private CompanyDto _selectedItem = new();

        private MudDataGrid<CompanyDto> _dataGrid;

        // custom sort by name length
        private Func<CompanyDto, object?> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.Nome!.Length;
            else
                return x.Nome;
        };

        // quick filter - filter globally across multiple columns with the same input
        private Func<CompanyDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            return Task.Run(() =>
            {
                if ($"{x.Id}".Contains(_searchString))
                    return true;

                if (x.Nome!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).GetAwaiter().GetResult();
        };

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            var result = await CompanyService.GetAllAsync();
            _companies = result;

            _isLoading = false;
        }

        private async Task OpenAddCompanyDialog()
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<AddCompanyDialog>("Adicionar Empresa", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _companies = await CompanyService.GetAllAsync();
            }
        }

        private async Task OpenUpdateCompanyDialog(CompanyDto selectedCompany)
        {
            var parameters = new DialogParameters
        {
            { "Company", selectedCompany }
        };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<AddCompanyDialog>("Atualizar Empresa", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await RefreshPage();
            }
        }

        private async Task OpenUpdateDialog()
        {
            if (_selectedItem == null || _selectedItem.Id == null)
            {
                Snackbar.Add("Selecione uma empresa para atualizar.", Severity.Warning);
                return;
            }

            await OpenUpdateCompanyDialog(_selectedItem);
        }

        private async Task DeleteCompany()
        {
            if (_selectedItem == null || _selectedItem.Id == null)
            {
                Snackbar.Add("Selecione uma empresa para deletar.", Severity.Warning);
                return;
            }

            await CompanyService.DeleteAsync(_selectedItem.Id);

            Snackbar.Add("Empresa excluída com sucesso.", Severity.Success);
            await RefreshPage();
        }

        // events
        void RowClicked(DataGridRowClickEventArgs<CompanyDto> args)
        {
            _events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");

            _selectedItem = args.Item;
        }

        void RowRightClicked(DataGridRowClickEventArgs<CompanyDto> args)
        {
            _events.Insert(0, $"Event = RowRightClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
        }

        void SelectedItemsChanged(HashSet<CompanyDto> items)
        {
            _events.Insert(0, $"Event = SelectedItemsChanged, Data = {System.Text.Json.JsonSerializer.Serialize(items)}");
            _selectedItem = items.Any() ? items.First() : new CompanyDto();
        }

        async Task RefreshPage()
        {
            ClearSelectedItems();

            _companies = await CompanyService.GetAllAsync();
            StateHasChanged();
        }

        void ClearSelectedItems()
        {
            if (_dataGrid != null)
            {
                _dataGrid.SelectedItems = new HashSet<CompanyDto>();
            }
        }
    }
}
