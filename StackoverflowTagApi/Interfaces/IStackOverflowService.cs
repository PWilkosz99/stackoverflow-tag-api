using StackoverflowTagApi.Models;

namespace StackoverflowTagApi.Interfaces
{
    public interface IStackOverflowService
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
    }
}
