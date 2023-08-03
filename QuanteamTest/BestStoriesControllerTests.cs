using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Moq.Protected;
using QuanteamAPI.Constants;
using QuanteamAPI.Controllers;
using QuanteamAPI.Models;
using System.Net;
using System.Text.Json;

namespace QuanteamTest
{

    public class BestStoriesControllerTests
    {
        [Fact]
        public async void Get_ReturnsOk_Best_Stories()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var configuration = new TestConfiguration();
            var loggerMock = new Mock<ILogger<BestStoriesController>>();

            var n = 5;

            var fakeBestStories = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var fakeStories = new List<StoryResponseObject>
            {
                new StoryResponseObject { Title = "Fake Story 1", Id = 1 },
                new StoryResponseObject { Title = "Fake Story 2", Id = 2 },
                new StoryResponseObject { Title = "Fake Story 3", Id = 3 },
                new StoryResponseObject { Title = "Fake Story 4", Id = 4 },
                new StoryResponseObject { Title = "Fake Story 5", Id = 5 },
                // Add more fake stories as needed
            };

            // Mock the HTTP client and its behavior
            var responseContent = JsonSerializer.Serialize(fakeBestStories);
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);


      


            var controller = new BestStoriesController(httpClientFactoryMock.Object, memoryCacheMock.Object, configuration, loggerMock.Object);

            // Act
            var result = await controller.Get(n);

            // Assert
            Assert.NotNull(result);

            var actionResult = result as OkObjectResult;
            Assert.NotNull(actionResult);

            var stories = actionResult.Value as List<StoryResponseObject>;
            Assert.Equal(n, stories!.Count);
            // Add additional assertions based on your fakeStories data

            // Optionally, verify that the HttpClientFactory.CreateClient method was called once
            httpClientFactoryMock.Verify(factory => factory.CreateClient(It.IsAny<string>()), Times.Once);

        }

        [Fact]
        public async Task Get_Returns_BadRequest_When_N_Is_Negative()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<BestStoriesController>>();

            var controller = new BestStoriesController(httpClientFactoryMock.Object, memoryCacheMock.Object, configurationMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Get(-1);

            // Assert
            Assert.NotNull(result);

            var actionResult = result as BadRequestObjectResult;
            Assert.NotNull(actionResult);
            Assert.Equal(ErrorMessages.ERROR_NO_POSITIVE_VALUE_FOR_N, actionResult.Value);
        }


        private class TestConfiguration : IConfiguration
        {
            private readonly IConfigurationSection _section;

            public TestConfiguration()
            {
                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "BaseUrls:StoryUrlFormat", "https://hacker-news.firebaseio.com/v0/item/{0}.json" },
            { "BaseUrls:BestStoriesUrl", "https://hacker-news.firebaseio.com/v0/beststories.json" }
        });
                _section = builder.Build().GetSection("BaseUrls");
            }

            public string this[string key] { get => _section[key]; }
            string IConfiguration.this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IEnumerable<IConfigurationSection> GetChildren() => _section.GetChildren();

            public IChangeToken GetReloadToken() => _section.GetReloadToken();

            public IConfigurationSection GetSection(string key) => _section.GetSection(key);
        }



    }
}