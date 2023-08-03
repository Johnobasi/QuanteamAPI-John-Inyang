using Microsoft.Extensions.Caching.Memory;
using QuanteamAPI.Constants;
using QuanteamAPI.Controllers;
using QuanteamAPI.ExceptionMiddleware;
using QuanteamAPI.Models;
using System.Text.Json;

namespace QuanteamAPI.Abstracts
{
    public class BestStoryService : IBestStory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BestStoriesController> _logger;
        public BestStoryService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<BestStoriesController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<List<StoryResponseObject>> GetStory(int n)
        {

            try
            {
                if (n <= 0)
                {
                    throw new UserFriendlyException(ErrorMessages.ERROR_NO_POSITIVE_VALUE_FOR_N);
                }

                var client = _httpClientFactory.CreateClient();
                string storyUrlFormat = _configuration.GetValue<string>("BaseUrls:StoryUrlFormat");
                string bestStoriesUrl = _configuration.GetValue<string>("BaseUrls:BestStoriesUrl");
                var bestStoriesResponse = await client.GetAsync(bestStoriesUrl);
                
                _logger.LogInformation($"{JsonSerializer.Serialize(bestStoriesResponse)}");

                if (!bestStoriesResponse.IsSuccessStatusCode)
                {
                    return default!;  
                }

               
                var bestStories = await bestStoriesResponse.Content.ReadFromJsonAsync<List<int>>();
                if (bestStories == null)
                {
                    return default!;
                }
                _logger.LogInformation($"{JsonSerializer.Serialize(bestStories)}");
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
                return stories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting best stories");
                throw new UserFriendlyException(ErrorMessages.FAIL_TO_RETRIEVE);
            }

        }
    }
}
