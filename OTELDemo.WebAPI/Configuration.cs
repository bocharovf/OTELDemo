namespace OTELDemo.WebAPI
{
    public static class Configuration
    {

        public static readonly string ServiceName = "OTELDemo.WebAPI";
        public static readonly string ServiceVersion = "1.0.0";

        public static readonly string OtlpEndpoint = Environment.GetEnvironmentVariable("OTELDEMO_OTLP_ENPOINT") ?? "http://localhost:4317";

        public static readonly string PostgresConnectionString = Environment.GetEnvironmentVariable("OTELDEMO_POSTGRES_CONSTR") ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";
    }
}
