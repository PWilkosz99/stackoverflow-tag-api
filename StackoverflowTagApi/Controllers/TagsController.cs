using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackoverflowTagApi.Data;
using StackoverflowTagApi.Interfaces;
using StackoverflowTagApi.Models;
using StackoverflowTagApi.Services;

namespace StackoverflowTagApi.Controllers
{
    /// <summary>
    /// Controller for managing tags.
    /// </summary>
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

        /// <summary>
        /// Get a list of tags.
        /// </summary>
        /// <param name="sortBy">Sort field.</param>
        /// <param name="ascending">Sort direction.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>A list of tags.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Tag>), 200)]
        [Produces("application/json")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Tag>>> GetTags(string sortBy = "name", bool ascending = true, int page = 1, int pageSize = 10)
        {
            try
            {
                if (await _tagRepository.GetTotalTagCountAsync() < 1000)
                {
                    var savedTags = await _stackOverflowService.GetTagsAsync();
                    _tagRepository.AddRange(savedTags);
                    await _tagRepository.UpdatePercentageShareAsync();
                }

                var tags = await _tagRepository.GetAllSortedAsync(sortBy, ascending);

                var paginatedTags = tags.Skip((page - 1) * pageSize).Take(pageSize);

                return paginatedTags.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tags: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching tags.");
            }
        }

        /// <summary>
        /// Refreshes the list of tags by fetching them from the Stack Overflow service.
        /// </summary>
        /// <remarks>
        /// This action clears the existing tags in the repository and retrieves the latest tags from the Stack Overflow service.
        /// </remarks>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RefreshTags()
        {
            try
            {
                var savedTags = await _stackOverflowService.GetTagsAsync();
                _tagRepository.DeleteAll();
                _tagRepository.AddRange(savedTags);
                await _tagRepository.UpdatePercentageShareAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while refreshing tags: {ex.Message}");
                return StatusCode(500, "An error occurred while refreshing tags.");
            }
        }
    }
}
