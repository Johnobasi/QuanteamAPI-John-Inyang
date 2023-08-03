using Microsoft.AspNetCore.Mvc;
using QuanteamAPI.Abstracts;

namespace QuanteamAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BestStoriesController : ControllerBase
    {
        private readonly IBestStory _bestStory;
        private readonly ILogger<BestStoriesController> _logger;
        public BestStoriesController(IBestStory bestStory, ILogger<BestStoriesController> logger)
        {
            _bestStory = bestStory;
            _logger = logger;
        }

        [HttpGet(Name = "GetBestStories")]
        public async Task<IActionResult> Get(int n)
        {
            try
            {
                _logger.LogInformation($"Getting best stories for {n}");
                var response = await _bestStory.GetStory(n);
                return Ok(response);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error while getting best stories");
                return StatusCode(500);
            }

        }
    }
}