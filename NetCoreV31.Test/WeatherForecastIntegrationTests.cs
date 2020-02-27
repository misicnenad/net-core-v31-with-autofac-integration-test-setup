using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using NetCoreV31.Interfaces;
using NetCoreV31.Services;
using System;
using System.Net;
using System.Threading.Tasks;

using Xunit;

namespace NetCoreV31.Test
{
    public class WeatherForecastIntegrationTests
    {
        private const string _apiUrl = "api/weatherforecast";

        private readonly IHostBuilder _hostBuilder;

        public WeatherForecastIntegrationTests()
        {
            _hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule<ApiModule>();
                })
                .ConfigureWebHost(conf =>
                {
                    conf.UseTestServer();
                    conf.UseStartup<Startup>();
                });
        }

        [Fact]
        public async Task Get_Returns_Weather_Forecast()
        {
            // Arrange
            _hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                var mockRequestValidationService = new Mock<IRequestValidationService>();
                mockRequestValidationService.Setup(service =>
                        service.RequestCanBeProcessed())
                    .Returns(true);

                builder.Register(c => mockRequestValidationService.Object)
                    .As<IRequestValidationService>();
            });

            var host = _hostBuilder.Start();
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync(_apiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_Returns_BadRequest()
        {
            // Arrange
            _hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                var mockRequestValidationService = new Mock<IRequestValidationService>();
                mockRequestValidationService.Setup(service =>
                        service.RequestCanBeProcessed())
                    .Returns(false);

                builder.Register(c => mockRequestValidationService.Object)
                    .As<IRequestValidationService>();
            });

            var host = _hostBuilder.Start();
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync(_apiUrl);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}