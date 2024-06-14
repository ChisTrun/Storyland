using backend.Application.DTO;
using backend.Application.Services.Abstract;
using backend.Domain.Objects;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/author")]
    public class AuthorController : Controller
    {
        private readonly ICrawlingService _crawlingService;

        public AuthorController(ICrawlingService crawlingService) => _crawlingService = crawlingService;

        /// <summary>
        /// Get all authors by search.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="authorName" example="Đỉnh">Author's name searching keyword.</param>
        [ProducesResponseType(typeof(DisplayDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/author/{authorName}/all")]
        public IActionResult SearchAuthorsByName(string serverId, string authorName)
        {
            try
            {
                return Ok(_crawlingService.GetAuthorsBySearchName(serverId, authorName));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories by searching with keyword {authorName}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get authors with paging by search.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="authorName" example="Đỉnh">Author's name searching keyword.</param>
        /// <param name="page" example="1">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<DisplayDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/author/{authorName}")]
        public IActionResult SearchAuthorsByName(string serverId, string authorName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetAuthorBySearchName(serverId, authorName, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} by searching with keyword {authorName}: {e.Message}.");
            }
        }
    }
}
