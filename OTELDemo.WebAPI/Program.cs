using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OTELDemo.WebAPI.Data;

var serviceName = "OTELDemo.WebAPI";
var serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation((options) => options.Filter =
        (context) =>
        {
            var isPrometheusRequest = context.Request.Path.ToString().Contains("/metrics");
            var fileExtension = System.IO.Path.GetExtension(context.Request.Path.ToString()).ToLower();
            return !new[] { ".js", ".ico", ".json", ".html" }.Contains(fileExtension) && !isPrometheusRequest;
        })
    .AddNpgsql()
    .AddConsoleExporter()
    .AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri(OTELDemo.WebAPI.Configuration.OtlpEndpoint);
        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation()
    .AddConsoleExporter()
    .AddPrometheusExporter();
});

builder.Services.AddDbContext<ApplicationContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context => context.Request.Path == "/metrics"
            && context.Connection.LocalPort == (app.Environment.IsDevelopment() ? 5002 : 80));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
