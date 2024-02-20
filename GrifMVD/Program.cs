using GrifMVD.NewsFolder.Data;
using GrifMVD.NewsFolder.Interfaces;
using GrifMVD.NewsFolder.Mapping;
using GrifMVD.NewsFolder.Services;
using Microsoft.EntityFrameworkCore;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddAutoMapper(typeof(MapOptions));
        builder.Services.AddScoped<IScraping, ScrapingService>();
        builder.Services.AddScoped<IHandleDate, DateTreatmentService>();
        builder.Services.AddScoped<IHandleNews, NewsService>();
        builder.Services.AddScoped<DateTreatmentService>();
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

}
