namespace OTELDemo.WebAPI
{
    public static class Configuration
    {
        public static readonly string OtlpEndpoint = Environment.GetEnvironmentVariable("OTELDEMO_OTLP_ENPOINT") ?? String.Empty;
    }
}
