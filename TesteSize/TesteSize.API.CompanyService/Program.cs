using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using TesteSize.API.CompanyService.Application.Interfaces;
using TesteSize.API.CompanyService.Application.Services;
using TesteSize.API.CompanyService.Infrastructure.Data;
using TesteSize.API.CompanyService.Infrastructure.Repositories;

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
        Title = "Company API",
        Version = "v1",
        Description = "API para gerenciar empresas.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ivan",
            Email = "ivanlempek@hotmail.com"
        }
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.ActionDescriptor is ControllerActionDescriptor descriptor &&
               descriptor.ControllerTypeInfo.Namespace!.Contains("Company");
    });
});

builder.Services.AddDbContext<CompanyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<ICompanyService, CompanyRepository>();
builder.Services.AddScoped<ICreditLimitService, CreditLimitService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
