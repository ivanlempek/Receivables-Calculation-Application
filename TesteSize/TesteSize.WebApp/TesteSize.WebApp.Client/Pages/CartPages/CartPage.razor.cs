using Microsoft.AspNetCore.Components;
using MudBlazor;
using TesteSize.BaseDTOs.Cart;
using TesteSize.BaseDTOs.Checkout;
using TesteSize.BaseDTOs.Company;
using TesteSize.BaseDTOs.Invoice;
using TesteSize.WebApp.Client.Components;

namespace TesteSize.WebApp.Client.Pages.CartPages
{
    public partial class CartPage
    {
        private IEnumerable<InvoiceDto> _invoices = new List<InvoiceDto>();
        private IEnumerable<CompanyDto> _companies = new List<CompanyDto>();
        private IEnumerable<CartDto> _carts = new List<CartDto>();
        private List<Guid> _cartInvoicesIds;

        private CartDto? _cart;

        private string? _searchString;

        private bool _sortNameByLength;

        private bool _isLoading;

        private List<string> _events = new();

        private InvoiceDto _selectedItem;
        private CompanyDto _selectedCompany;

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

            var companies = await CompanyService.GetAllAsync();

            _companies = companies;

            _isLoading = false;
        }

        private async Task OpenAddInvoiceDialog()
        {
            if (_selectedCompany == null || _selectedCompany.Id == null || _selectedCompany.Id == Guid.Empty)
            {
                Snackbar.Add("Selecione uma empresa antes de adicionar uma nota ao carrinho.", Severity.Warning);
                return;
            }

            var checkCompanyInvoices = await InvoiceService.GetInvoicesByCompany(_selectedCompany.Id);

            if (!checkCompanyInvoices.Any())
            {
                Snackbar.Add("Essa empresa não possui notas fiscais. Adicione uma antes de continuar.", Severity.Warning);
                return;
            }

            var parameters = new DialogParameters
            {
                ["CartInvoices"] = _invoices,
                ["CompanyId"] = _selectedCompany.Id
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<AddInvoiceToCartDialog>("Adicionar Nota fiscal ao carrinho", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await RefreshPage();
            }
        }

        private async Task OpenCheckoutModal(CheckoutResponseDto checkoutResult)
        {
            var parameters = new DialogParameters
            {
                ["Result"] = checkoutResult
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };
            var dialog = DialogService.Show<CheckoutResponseDialog>("Resultado do Checkout", parameters);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await RefreshPage();
            }
        }

        private async Task DeleteInvoiceFromCart()
        {
            if (_selectedItem == null || _selectedItem.Id == null || _selectedItem.Id == Guid.Empty)
            {
                Snackbar.Add("Selecione uma nota fiscal do carrinho para deletar.", Severity.Warning);
                return;
            }

            await CartService.RemoveInvoiceFromCart(_selectedCompany.Id, _selectedItem.Id);

            Snackbar.Add("Nota fiscal excluída do carrinho com sucesso.", Severity.Success);
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

        async Task RefreshPage()
        {
            ClearSelectedItems();

            await UpdateCartInvoices();
            StateHasChanged();
        }

        void ClearSelectedItems()
        {
            if (_dataGrid != null)
            {
                _dataGrid.SelectedItems = new HashSet<InvoiceDto>();
            }
        }

        private async Task OnCompanyChanged(CompanyDto selected)
        {
            _selectedCompany = selected;
            _isLoading = true;

            try
            {
                if (_selectedCompany != null)
                {
                    await UpdateCartInvoices();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Warning);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task UpdateCartInvoices()
        {
            _cart = await CartService.GetCartByCompanyId(_selectedCompany.Id);

            if (_cart == null || _cart.NotasFiscais?.Count == 0)
            {
                _invoices = new List<InvoiceDto>();
                return;
            }
            else
            {
                _cartInvoicesIds = _cart.NotasFiscais!.Select(x => x.NotaFiscalId).ToList();

                _invoices = await InvoiceService.GetCartInvoices(_cartInvoicesIds);
            }
        }
        private async Task HandleCheckout()
        {
            if (_selectedCompany == null || _selectedCompany.Id == null || _selectedCompany.Id == Guid.Empty)
            {
                Snackbar.Add("Selecione uma empresa antes de iniciar o checkout.", Severity.Warning);
                return;
            }

            if (_invoices == null || !_invoices.Any())
            {
                Snackbar.Add("Adicione alguma nota fiscal ao carrinho antes de iniciar o checkout.", Severity.Warning);
                return;
            }

            try
            {
                var result = await CartService.CalculateCheckout(_selectedCompany.Id);

                await OpenCheckoutModal(result);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Erro ao calcular antecipação: {ex.Message}", Severity.Error);
            }
        }
    }
}
