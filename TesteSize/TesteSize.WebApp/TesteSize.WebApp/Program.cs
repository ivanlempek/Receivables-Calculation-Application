using MudBlazor.Services;
using TesteSize.ExternalServices.Interfaces;
using TesteSize.ExternalServices.Services;
using TesteSize.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TesteSize.WebApp.Client._Imports).Assembly);

app.Run();
