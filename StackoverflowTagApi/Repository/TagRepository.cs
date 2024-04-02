using StackoverflowTagApi.Data;
using StackoverflowTagApi.Models;
using Microsoft.EntityFrameworkCore;
using StackoverflowTagApi.Interfaces;

namespace StackoverflowTagApi.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Tag tag)
        {
            _context.Tags.Add(tag);
            return Save();
        }

        public bool AddRange(IEnumerable<Tag> tags)
        {
            _context.Tags.AddRange(tags);
            return Save();
        }

        public bool Delete(string name)
        {
            _context.Tags.Remove(new Tag { Name = name });
            return Save();
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            return await _context.Tags.ToListAsync();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public bool Update(Tag tag)
        {
            _context.Tags.Update(tag);
            return Save();
        }
    }
}
