using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using TesteSize.API.CartService.Application.Interfaces;
using TesteSize.API.CartService.Application.Services;
using TesteSize.API.CartService.Infrastructure.Data;
using TesteSize.API.CartService.Infrastructure.Repositories;
using TesteSize.ExternalServices.Interfaces;
using TesteSize.ExternalServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1",
        Description = "API para gerenciar o carrinho de notas fiscais.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ivan",
            Email = "ivanlempek@hotmail.com"
        }
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.ActionDescriptor is ControllerActionDescriptor descriptor &&
               descriptor.ControllerTypeInfo.Namespace!.Contains("Cart");
    });
});

builder.Services.AddHttpClient<IESInvoiceService, ESInvoiceService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7003/api/invoice");
});

builder.Services.AddHttpClient<IESCompanyService, ESCompanyService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135/api/company");
});

builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
