using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using QuanteamAPI.Constants;
using QuanteamAPI.Models;

namespace QuanteamAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BestStoriesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BestStoriesController> _logger;
        public BestStoriesController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<BestStoriesController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet(Name = "GetBestStories")]
        public async Task<IActionResult> Get(int n)
        {
            try
            {
                if (n <= 0)
                {
                    return BadRequest(ErrorMessages.ERROR_NO_POSITIVE_VALUE_FOR_N);
                }

                var client = _httpClientFactory.CreateClient();
                 string storyUrlFormat = _configuration.GetValue<string>("BaseUrls:StoryUrlFormat");
                string bestStoriesUrl = _configuration.GetValue<string>("BaseUrls:BestStoriesUrl");
                var bestStoriesResponse =  await client.GetAsync(bestStoriesUrl);


                if (!bestStoriesResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500);
                }

                var bestStories = await bestStoriesResponse.Content.ReadFromJsonAsync<List<int>>();
                if (bestStories == null)
                {
                    return StatusCode(500, ErrorMessages.FAIL_TO_RETRIEVE);
                }

                var stories = new List<StoryResponseObject>();
                foreach (var storyId in bestStories!.Take(10))
                {
                    var story = await _memoryCache.GetOrCreateAsync(storyId.ToString(), async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // Cache for 10 minutes
                        var storyResponse = await client.GetAsync(string.Format(storyUrlFormat!, storyId));
                        if (!storyResponse.IsSuccessStatusCode)
                        {
                            return null;
                        }
                        return await storyResponse.Content.ReadFromJsonAsync<StoryResponseObject>();
                    });
                    if (story != null)
                    {
                        stories.Add(story!);
                    }


                    if (stories.Count == n)
                    {
                        break;
                    }
                }
                return Ok(stories);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error while getting best stories");
                return StatusCode(500);
            }

        }
    }
}