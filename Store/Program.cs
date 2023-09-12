using Microsoft.EntityFrameworkCore;
using Shopping.Core.IRepository;
using Shopping.EF.Context;
using Shopping.EF.Migrations;
using Shopping.EF.RepositoryImplementation;
using Shopping.Services.IServices;
using Shopping.Services.Services;

var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IBookServices, BookServices>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IStatisticsServices, StatisticsServices>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
