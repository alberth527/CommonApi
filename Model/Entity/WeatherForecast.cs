namespace CommonApi.Model.Entity
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public int Humidity { get; set; } // P3600

        public int WindSpeed { get; set; } // Pefbf
    }
}
