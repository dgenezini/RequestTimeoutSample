
using RequestTimeoutSample.Interfaces;
using RequestTimeoutSample.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Threading;

namespace RequestTimeoutSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IDogImageUseCase, DogImageUseCase>();

        builder.Services.AddRefitClient<IDogApi>()
            .ConfigureHttpClient((provider, c) =>
            {
                c.BaseAddress = new Uri(configuration["DogApiUrl"]);
                c.Timeout = TimeSpan.FromSeconds(double.Parse(configuration["HttpClientTimeoutSeconds"]!));
            });

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

        var requestTimeoutSeconds = int.Parse(configuration["RequestTimeoutSeconds"]!);
        app.UseTimeoutCancellationToken(TimeSpan.FromSeconds(requestTimeoutSeconds));

        app.Run();
    }
}
