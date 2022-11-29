using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
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
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation((options) => options.Filter =
        (context) =>
        {
            var isPrometheusRequest = context.Request.Path.ToString().Contains("/metrics");
            var fileExtension = System.IO.Path.GetExtension(context.Request.Path.ToString()).ToLower();
            return !new[] { ".js", ".ico", ".css" }.Contains(fileExtension) && !isPrometheusRequest;
        })
    .AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri(OTELDemo.Configuration.OtlpEndpoint);
        opt.Protocol = OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation()
    .AddPrometheusExporter();
});

builder.Logging.AddOpenTelemetry(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion));
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
