using API.Service;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Env.Load();

        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Ravelry Reimagined API",
                Description = "An ASP.NET Core Web API for connecting to Ravelry's API",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddHttpClient<IAuth2Service, Auth2Service>();
        builder.Services.AddSingleton<IAuth2Service>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new Auth2Service(httpClient, config);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod());
        });
        builder.Services.AddDistributedMemoryCache();  
        builder.Services.AddSession();
        builder.Services.AddHttpClient();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowAll");
        app.UseSession();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
