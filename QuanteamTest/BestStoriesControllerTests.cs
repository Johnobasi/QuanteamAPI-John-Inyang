using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QuanteamAPI.Abstracts;
using QuanteamAPI.Constants;
using QuanteamAPI.Controllers;
using QuanteamAPI.Models;
using Xunit;

namespace QuanteamTest
{

    public class BestStoriesControllerTests
    {
        [Fact]
        public async Task Get_Returns_OkResult_With_Valid_N()
        {
            // Arrange
            int validN = 5;
            var bestStoryMock = new Mock<IBestStory>();
            bestStoryMock.Setup(x => x.GetStory(It.IsAny<int>())).ReturnsAsync(new List<StoryResponseObject>());
            var loggerMock = new Mock<ILogger<BestStoriesController>>();
            var controller = new BestStoriesController(bestStoryMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Get(validN);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<List<StoryResponseObject>>(okResult.Value);
            Assert.Equal(0, response!.Count!);
        }


        [Fact]
        public async Task Get_Returns_BadRequest_With_Invalid_N()
        {
            // Arrange
            int invalidN = 0;
            var bestStoryMock = new Mock<IBestStory>();
            var loggerMock = new Mock<ILogger<BestStoriesController>>();
            var controller = new BestStoriesController(bestStoryMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Get(invalidN);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.REQUIRED_PARAMETER_FAILED, badRequestResult.Value);
        }
    }
}