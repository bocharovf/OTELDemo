using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OTELDemo.WebAPI;
using OTELDemo.WebAPI.Data;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

var meter = new Meter(Configuration.ServiceName);
meter.CreateObservableGauge("ThreadCount", () => new[] { new Measurement<int>(ThreadPool.ThreadCount) });

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
    .AddSource(Configuration.ServiceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(Configuration.ServiceName, Configuration.ServiceVersion))
    // Инструментируем ASP.NET Core
    .AddAspNetCoreInstrumentation((options) => options.Filter =
        (context) =>
        {
            // Исключаем трассировку запросов к ресурсам
            var isPrometheusRequest = context.Request.Path.ToString().Contains("/metrics");
            var fileExtension = System.IO.Path.GetExtension(context.Request.Path.ToString()).ToLower();
            return !new[] { ".js", ".ico", ".json", ".html" }.Contains(fileExtension) && !isPrometheusRequest;
        })
    // Инструментируем запросы к Postgres
    .AddNpgsql()
    // Отправляем данные в OTEL Collector
    .AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri(OTELDemo.WebAPI.Configuration.OtlpEndpoint);
        opt.Protocol = OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(Configuration.ServiceName, Configuration.ServiceVersion))
    .AddMeter(Configuration.ServiceName)
    .AddAspNetCoreInstrumentation()
    .AddPrometheusExporter();
});

builder.Logging.AddOpenTelemetry(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(Configuration.ServiceName, Configuration.ServiceVersion));
    b.IncludeFormattedMessage = true;
    b.IncludeScopes = true;
    b.ParseStateValues = true;
    b.AddOtlpExporter(options => {
        options.Endpoint = new Uri(OTELDemo.WebAPI.Configuration.OtlpEndpoint);
        options.Protocol = OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddDbContext<ApplicationContext>();

builder.Services.AddControllers();
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
