using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

using Moq;

using NetCoreV31.Interfaces;

using System.Net;
using System.Threading.Tasks;

using Xunit;

namespace NetCoreV31.Test
{
    public class WeatherForecastIntegrationTests
    {
        private const string _apiUrl = "api/weatherforecast";

        [Fact]
        public async Task Get_Returns_Weather_Forecast()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHost(conf =>
                {
                    conf.UseTestServer();
                    conf.UseStartup<Startup>();
                    conf.ConfigureTestContainer<ContainerBuilder>(builder =>
                    {
                        var mockRequestValidationService = new Mock<IRequestValidationService>();
                        mockRequestValidationService.Setup(service =>
                                service.RequestCanBeProcessed())
                            .Returns(true);

                        builder.Register(c => mockRequestValidationService.Object)
                            .As<IRequestValidationService>();
                    });
                });

            var host = hostBuilder.Start();
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
            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHost(conf =>
                {
                    conf.UseTestServer();
                    conf.UseStartup<Startup>();
                    conf.ConfigureTestContainer<ContainerBuilder>(builder =>
                    {
                        var mockRequestValidationService = new Mock<IRequestValidationService>();
                        mockRequestValidationService.Setup(service =>
                                service.RequestCanBeProcessed())
                            .Returns(false);

                        builder.Register(c => mockRequestValidationService.Object)
                            .As<IRequestValidationService>();
                    });
                });

            var host = hostBuilder.Start();
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync(_apiUrl);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}