using Microsoft.EntityFrameworkCore;
using TesteSize.API.InvoiceService.Application.Interfaces;
using TesteSize.API.InvoiceService.Application.Services;
using TesteSize.API.InvoiceService.Infrastructure.Data;
using TesteSize.API.InvoiceService.Infrastructure.Repositories;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Invoice API",
        Version = "v1",
        Description = "API para gerenciar notas fiscais.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ivan",
            Email = "ivanlempek@hotmail.com"
        }
    });
});

builder.Services.AddHttpClient<ICompanyService, CompanyService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135/api/company/");
});

builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<IInvoiceService, InvoiceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
