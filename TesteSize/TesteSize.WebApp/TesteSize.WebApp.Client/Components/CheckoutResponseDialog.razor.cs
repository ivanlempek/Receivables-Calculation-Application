using Microsoft.AspNetCore.Components;
using MudBlazor;
using TesteSize.BaseDTOs.Checkout;

namespace TesteSize.WebApp.Client.Components
{
    public partial class CheckoutResponseDialog
    {
        [Parameter]
        public CheckoutResponseDto? Result { get; set; }

        [CascadingParameter]
        private MudDialogInstance? MudDialog { get; set; }
    }
}
