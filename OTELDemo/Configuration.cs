namespace OTELDemo
{
    public static class Configuration
    {

        public static readonly string ServiceName = "OTELDemo.Front";
        public static readonly string ServiceVersion = "1.0.0";

        public static readonly string WebApiUrl = Environment.GetEnvironmentVariable("OTELDEMO_WEBAPI_URL") ?? "http://localhost:5002";

        public static readonly string OtlpEndpoint = Environment.GetEnvironmentVariable("OTELDEMO_OTLP_ENPOINT") ?? "http://localhost:4317";

    }
}
