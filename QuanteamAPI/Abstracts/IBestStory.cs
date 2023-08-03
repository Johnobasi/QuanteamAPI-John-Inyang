using QuanteamAPI.Models;

namespace QuanteamAPI.Abstracts
{
    public interface IBestStory
    {
        Task<List<StoryResponseObject>> GetStory(int n);
    }
}
