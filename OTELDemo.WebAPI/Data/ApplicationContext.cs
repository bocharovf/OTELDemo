using Microsoft.EntityFrameworkCore;
using OTELDemo.Model;

namespace OTELDemo.WebAPI.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.PostgresConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>().ToTable("weatherforecast").HasNoKey();
            modelBuilder.Entity<WeatherForecast>().Property("Date").HasColumnName("date");
            modelBuilder.Entity<WeatherForecast>().Property("TemperatureC").HasColumnName("temperaturec");
            modelBuilder.Entity<WeatherForecast>().Property("Summary").HasColumnName("summary");
        }
    }
}
