using Xunit;
using CommonApi.Controllers;
using Comm.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CommonApi.Tests
{
    public class ApiTests
    {
        private readonly AuthController _authController;
        private readonly WeatherForecastController _weatherForecastController;

        public ApiTests()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var jwtHelper = new JwtHelper(configuration);
            _authController = new AuthController(configuration, jwtHelper);
            _weatherForecastController = new WeatherForecastController(null);
        }

        [Fact]
        public void Register_User_ShouldReturnSuccess()
        {
            var request = new UserRegistrationRequest
            {
                Username = "testuser",
                Password = "password"
            };

            var result = _authController.Register(request) as OkObjectResult;
            var apiResult = result.Value as APIResult;

            Assert.True(apiResult.IsSuccess);
            Assert.Equal("User registered successfully", apiResult.Message);
        }

        [Fact]
        public void Login_User_ShouldReturnToken()
        {
            var request = new UserLoginRequest
            {
                Username = "testuser",
                Password = "password"
            };

            var result = _authController.Login(request) as OkObjectResult;
            var apiResult = result.Value as APIResult;

            Assert.True(apiResult.IsSuccess);
            Assert.Equal("Login successful", apiResult.Message);
            Assert.NotNull(apiResult.Data);
        }

        [Fact]
        public void GetWeatherForecast_ShouldReturnForecasts()
        {
            var result = _weatherForecastController.Get() as IEnumerable<WeatherForecast>;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetWeatherForecastByDateRange_ShouldReturnForecasts()
        {
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);

            var result = _weatherForecastController.GetByDateRange(startDate, endDate) as IEnumerable<WeatherForecast>;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetWeatherForecastByTemperatureRange_ShouldReturnForecasts()
        {
            var minTemp = -10;
            var maxTemp = 30;

            var result = _weatherForecastController.GetByTemperatureRange(minTemp, maxTemp) as IEnumerable<WeatherForecast>;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
