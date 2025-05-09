using Hangfire;
using HangfireBasicAuthenticationFilter;
using Salla7ly.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, services, configuration) =>
{
    // Reads Serilog settings from appsettings.json (or other configuration sources)
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = 
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
            Pass =app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "Salla7ly Dashboard"
});

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
