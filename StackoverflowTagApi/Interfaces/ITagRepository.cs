using StackoverflowTagApi.Models;

namespace StackoverflowTagApi.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAll();
        bool Add(Tag tag);
        bool AddRange(IEnumerable<Tag> tag);
        bool Update(Tag tag);
        bool Delete(string name);
        bool Save();
    }
}
