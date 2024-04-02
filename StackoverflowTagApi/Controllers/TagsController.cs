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
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            List<Tag> tags = (await _stackOverflowService.GetTagsAsync()).ToList();

            if (tags.Count < 1000)
            {
                var additionalTags = await _stackOverflowService.GetTagsAsync();
                _tagRepository.AddRange(additionalTags);
                tags.AddRange(additionalTags);
            }

            return tags.Take(1000).ToList();
        }
    }
}
