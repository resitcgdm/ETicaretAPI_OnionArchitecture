using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Infrastructure.Services.Storage.Local;

var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.           //AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() herhangi bir siteden gelen her isteðe izin verir.

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
//builder.Services.AddStorage(StorageType.Azure);
builder.Services.AddStorage<LocalStorage>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("https://localhost:4200", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod()
    )); 


builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>()).AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options=>options.SuppressModelStateInvalidFilter= true);    //kendi filtreleme iþlemlerimizi yapabiliriz, default olaný yok saymýþ oluruz.


    //builder.Services.AddControllers().AddFluentValidation(configuration=> configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()); default validation iþlemlerini yapar.



    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

