namespace OTELDemo
{
    public static class Configuration
    {
        public static readonly string WebApiUrl = Environment.GetEnvironmentVariable("OTELDEMO_WEBAPI_URL") ?? "http://localhost:5002";
    }
}
