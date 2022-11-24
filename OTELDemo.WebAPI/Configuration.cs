namespace OTELDemo.WebAPI
{
    public static class Configuration
    {
        public static readonly string OtlpEndpoint = Environment.GetEnvironmentVariable("OTELDEMO_OTLP_ENPOINT") ?? "http://localhost:4318/v1/traces";

        public static readonly string PostgresConnectionString = Environment.GetEnvironmentVariable("OTELDEMO_POSTGRES_CONSTR") ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";
    }
}
