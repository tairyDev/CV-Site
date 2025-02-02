using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
namespace GitHubService.Services
{
    public interface IGitHubService
    {
        Task<List<Repository>> GetUserRepositoriesAsync();
        Task<int> GetUserFollowersAsync(string userName);
        Task<List<Repository>> SearchRepositoriesAsync(string query, string language, string user);
        Task<List<RepositoryDetails>> GetPortfolioAsync();
    }


}



