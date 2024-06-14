using backend.Application.DTO;
using backend.Application.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/category")]
    public class CategoryController : Controller
    {
        private readonly ICrawlingService _crawlingService;

        public CategoryController(ICrawlingService crawlingService) => _crawlingService = crawlingService;

        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        [ProducesResponseType(typeof(DisplayDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/")]
        public IActionResult GetCategories(string serverId)
        {
            try
            {
                return Ok(_crawlingService.GetCategories(serverId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get all categories: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all categories with priority.
        /// </summary>
        /// <param name="idsWithPriority"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DisplayDTO[]), 200)]
        [HttpPost]
        [Route("all/")]
        public IActionResult GetCategoriesWithPriority([FromBody] IEnumerable<string> idsWithPriority)
        {
            try
            {
                return Ok(_crawlingService.GetCategoriesWithPriority(idsWithPriority));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get all categories: {e.Message}.");
            }
        }
    }
}
