using GitHubService.Services;
using GitHubService.Services.DataEntities;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace GitHubPortfolioAPI.CachedServises
{
    public class CachedGitHubSerivice : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string User = "UserPortFolio";

        public CachedGitHubSerivice(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public async Task<List<RepositoryDetails>> GetPortfolioAsync()
        {
            if (_memoryCache.TryGetValue(User, out List<RepositoryDetails> portfolio))
            {
                return portfolio;
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            portfolio = await _gitHubService.GetPortfolioAsync();
            _memoryCache.Set(User, portfolio, cacheOptions);

            return portfolio ?? new List<RepositoryDetails>(); // לא מחזירים null
        }

        public Task<int> GetUserFollowersAsync(string userName)
        {
           return  _gitHubService.GetUserFollowersAsync(userName);
        }

        public Task<List<Repository>> GetUserRepositoriesAsync()
        {
          return  _gitHubService.GetUserRepositoriesAsync();
        }

        public Task<List<Repository>> SearchRepositoriesAsync(string query, string language, string user)
        {
            return _gitHubService.SearchRepositoriesAsync(query, language, user);
        }
    }
}
