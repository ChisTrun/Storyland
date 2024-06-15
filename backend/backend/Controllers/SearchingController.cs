﻿using Backend.Application.DTO;
using Backend.Application.Services.Abstract;
using Backend.Domain.Objects;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers
{
    [Route("api/search")]
    public class SearchingController : Controller
    {
        private readonly ICrawlingService _crawlingService;

        public SearchingController(ICrawlingService crawlingService) => _crawlingService = crawlingService;

        /// <summary>
        /// Get all stories of an Author.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="authorId" example="vainy">Author's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(StoryDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/tacgia/{authorId}/all")]
        public IActionResult GetStoriesOfAuthor(string serverId, string authorId)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesOfAuthor(serverId, authorId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories of the author with id {authorId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories of an Author with paging.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="authorId" example="vainy">Author's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<StoryDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/tacgia/{authorId}")]
        public IActionResult GetStoriesOfAuthor(string serverId, string authorId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesOfAuthor(serverId, authorId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} of the author with id  {authorId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all stories by searching.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        [ProducesResponseType(typeof(StoryDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/truyen/{storyName}/all")]
        public IActionResult GetStoriesBySearchName(string serverId, string storyName)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesBySearchName(serverId, storyName));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories by searching with keyword {storyName}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<StoryDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/truyen/{storyName}")]
        public IActionResult GetStoriesBySearchName(string serverId, string storyName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesBySearchName(serverId, storyName, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} by searching with keyword {storyName}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="minChapNum" example="1">Min number of chapter (-1 for no minimum)</param>
        /// <param name="maxChapNum" example="100">Max number of chapter (-1 for no maximum)</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<StoryDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/truyen/{storyName}/{minChapNum}/{maxChapNum}")]
        public IActionResult GetStoriesBySearchNameWithFilter(string serverId, string storyName, int minChapNum, int maxChapNum, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesBySearchNameWithFilter(serverId, storyName, minChapNum, maxChapNum, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} by searching with keyword {storyName}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all stories by category.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="categoryId" example="tien-hiep">Category's ID</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(StoryDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/{categoryId}/all")]
        public IActionResult GetAllStoriesOfCategory(string serverId, string categoryId)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesOfCategory(serverId, categoryId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories of the category with id {categoryId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories by category with paging.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="categoryId" example="tien-hiep">Category's ID</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<StoryDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/{categoryId}")]
        public IActionResult GetStoriesOfCategory(string serverId, string categoryId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesOfCategory(serverId, categoryId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} of the category with id {categoryId}: {e.Message}.");
            }
        }


        /// <summary>
        /// Get all stories with server priority.
        /// </summary>
        /// <param name="idsWithPriority"> List of string ids.
        /// <exmaple>["1", "2", "3"]</exmaple>
        /// </param>
        /// <param name="storyName" example="kiem lai">Story name's searching keyword.</param>
        /// <param name="minChapNum" example="1">Min number of chapter (-1 for no minimum)</param>
        /// <param name="maxChapNum" example="100">Max number of chapter (-1 for no maximum)</param>
        [ProducesResponseType(typeof(StoryDTO[]), 200)]
        [HttpPost]
        [Route("all/truyen/{storyName}/{minChapNum}/{maxChapNum}/all")]
        public IActionResult GetStoriesBySearchName([FromBody] IEnumerable<string> idsWithPriority, string storyName, int minChapNum, int maxChapNum)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesWithPriority(idsWithPriority, storyName, minChapNum, maxChapNum));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories by searching with keyword {storyName}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all stories by category with priority.
        /// </summary>
        /// <param name="idsWithPriority"> List of string ids.
        /// <exmaple>["1", "2", "3"]</exmaple>
        /// </param>
        /// <param name="categoryId">ID of a category</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(StoryDTO[]), 200)]
        [HttpPost]
        [Route("all/{categoryId}/all")]
        public IActionResult GetAllStoriesOfCategory([FromBody] IEnumerable<string> idsWithPriority, string categoryId)
        {
            try
            {
                return Ok(_crawlingService.GetStoriesOfCategoryWithPriority(idsWithPriority, categoryId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories of the category with id {categoryId}: {e.Message}.");
            }
        }
    }
}
