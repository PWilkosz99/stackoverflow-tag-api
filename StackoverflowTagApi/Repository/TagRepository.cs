using StackoverflowTagApi.Data;
using StackoverflowTagApi.Models;
using Microsoft.EntityFrameworkCore;
using StackoverflowTagApi.Interfaces;
using Microsoft.Extensions.Logging;

namespace StackoverflowTagApi.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(ApplicationDbContext context, ILogger<TagRepository> logger)
        {
            _context = context;
            _logger = logger;
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

        public bool DeleteAll()
        {
            _context.Tags.RemoveRange(_context.Tags);
            return Save();
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<int> GetTotalTagCountAsync()
        {
            return await _context.Tags.CountAsync();
        }

        public async Task<IEnumerable<Tag>> GetAllSortedAsync(string sortBy, bool ascending)
        {
            IQueryable<Tag> query = _context.Tags;

            switch (sortBy.ToLower())
            {
                case "name":
                    query = ascending ? query.OrderBy(tag => tag.Name) : query.OrderByDescending(tag => tag.Name);
                    break;
                case "count":
                    query = ascending ? query.OrderBy(tag => tag.Count) : query.OrderByDescending(tag => tag.Count);
                    break;
                case "percentage":
                    query = ascending ? query.OrderBy(tag => tag.PercentageShare) : query.OrderByDescending(tag => tag.PercentageShare);
                    break;
                default:
                    _logger.LogWarning($"Invalid sorting option '{sortBy}'. Defaulting to sorting by name.");
                    query = query.OrderBy(tag => tag.Name);
                    break;
            }

            return await query.ToListAsync();
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

        public async Task<bool> UpdatePercentageShareAsync()
        {
            try
            {
                var tags = await _context.Tags.ToListAsync();

                int totalTagCount = tags.Sum(tag => tag.Count);

                foreach (var tag in tags)
                {
                    tag.PercentageShare = (double)tag.Count / totalTagCount * 100;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Percentage share updated successfully.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating percentage share: {ex.Message}");
                return false;
            }
        }
    }
}
