using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackoverflowTagApi.Data;
using StackoverflowTagApi.Interfaces;
using StackoverflowTagApi.Models;
using StackoverflowTagApi.Services;

namespace StackoverflowTagApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : Controller
    {
        private readonly StackOverflowService _stackOverflowService;
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<StackOverflowService> _logger;

        public TagsController(StackOverflowService stackOverflowService, ITagRepository tagRepository, ILogger<StackOverflowService> logger)
        {
            _stackOverflowService = stackOverflowService;
            _tagRepository = tagRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tag>>> GetTags(string sortBy = "name", bool ascending = true)
        {
            if(await _tagRepository.GetTotalTagCountAsync() < 1000)
            {
                var savedTags = await _stackOverflowService.GetTagsAsync();
                _tagRepository.AddRange(savedTags);
                await _tagRepository.UpdatePercentageShareAsync();
            }

            var tags = await _tagRepository.GetAllSortedAsync(sortBy, ascending);

            return tags.ToList();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTags()
        {
            var savedTags = await _stackOverflowService.GetTagsAsync();
            _tagRepository.DeleteAll();
            _tagRepository.AddRange(savedTags);
            await _tagRepository.UpdatePercentageShareAsync();
            return Ok();
        }
    }
}
