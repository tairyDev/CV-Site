using GitHubService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace GitHubPortfolioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // שליפת רשימת ה-repositories של המשתמש כולל המידע הנדרש
        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            var portfolio = await _gitHubService.GetPortfolioAsync();
            return Ok(portfolio);
        }

        // חיפוש repositories ציבוריים לפי קריטריונים (שם, שפה, משתמש)
        [HttpGet("search")]
        public async Task<IActionResult> SearchRepositories(
            [FromQuery] string query,
            [FromQuery] string language,
            [FromQuery] string user)
        {
            var repositories = await _gitHubService.SearchRepositoriesAsync(query, language, user);
            return Ok(repositories);
        }

        // קבלת רשימת repositories של המשתמש
        [HttpGet("repos")]
        public async Task<IActionResult> GetUserRepositories()
        {
            var repositories = await _gitHubService.GetUserRepositoriesAsync();
            return Ok(repositories);
        }

        // קבלת כמות עוקבים של משתמש
        [HttpGet("followers/{userName}")]
        public async Task<IActionResult> GetUserFollowers(string userName)
        {
            var followers = await _gitHubService.GetUserFollowersAsync(userName);
            return Ok(new { Followers = followers });
        }

    }
}


