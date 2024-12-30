using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TesteSize.ExternalServices.Interfaces;
using TesteSize.ExternalServices.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddHttpClient<IESCompanyService, ESCompanyService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135/api/company");
});

builder.Services.AddHttpClient<IESInvoiceService, ESInvoiceService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7003/api/invoice");
});

builder.Services.AddHttpClient<IESCartService, ESCartService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7179/api/cart");
});

await builder.Build().RunAsync();
