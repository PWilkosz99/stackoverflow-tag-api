﻿using StackoverflowTagApi.Models;

namespace StackoverflowTagApi.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAll();
        Task<int> GetTotalTagCountAsync();
        Task<IEnumerable<Tag>> GetAllSortedAsync(string sortBy, bool ascending);
        bool Add(Tag tag);
        bool AddRange(IEnumerable<Tag> tag);
        bool Update(Tag tag);
        Task<bool> UpdatePercentageShareAsync();
        bool Delete(string name);
        bool DeleteAll();
        bool Save();
    }
}
