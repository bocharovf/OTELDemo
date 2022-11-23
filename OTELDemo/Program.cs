using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var serviceName = "OTELDemo.Front";
var serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
    .AddConsoleExporter()
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation((options) => options.Filter = 
        (context) => {
            var fileExtension = System.IO.Path.GetExtension(context.Request.Path.ToString()).ToLower();
            return ! new[] { ".js", ".css" }.Contains(fileExtension);
        })
    .AddSqlClientInstrumentation();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
