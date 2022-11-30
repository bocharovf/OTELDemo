using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OTELDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
    .AddSource(Configuration.ServiceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(Configuration.ServiceName, Configuration.ServiceVersion))
    // Инструментируем HTTP Client для проброса контекста между сервисами
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation((options) => options.Filter =
        (context) =>
        {
            // Исключаем трассировку запросов к ресурсам
            var isPrometheusRequest = context.Request.Path.ToString().Contains("/metrics");
            var fileExtension = System.IO.Path.GetExtension(context.Request.Path.ToString()).ToLower();
            return !new[] { ".js", ".ico", ".css" }.Contains(fileExtension) && !isPrometheusRequest;
        })
    // Отправляем данные в OTEL Collector
    .AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri(OTELDemo.Configuration.OtlpEndpoint);
        opt.Protocol = OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(Configuration.ServiceName, Configuration.ServiceVersion))
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
        options.Endpoint = new Uri(OTELDemo.Configuration.OtlpEndpoint);
        options.Protocol = OtlpExportProtocol.Grpc;
    });
});

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint(context => context.Request.Path == "/metrics"
            && context.Connection.LocalPort == (app.Environment.IsDevelopment() ? 7217 : 80));

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
