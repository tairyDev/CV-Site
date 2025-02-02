using GitHubService.Services;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace GitHubService.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegretionOption _options;

        public GitHubService(IOptions<GitHubIntegretionOption> options)
        {
            _options = options.Value;

            _client = new GitHubClient(new ProductHeaderValue("GitHubPortfolio"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<List<RepositoryDetails>> GetPortfolioAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_options.Username);
            var result = new List<RepositoryDetails>();

            foreach (var repo in repositories)
            {
                var languages = await _client.Repository.GetAllLanguages(_options.Username, repo.Name);
                var languageNames = languages.Select(l => l.Name).ToList();
                var lastCommit = repo.PushedAt?.UtcDateTime;

                int pullRequestsCount = await GetPullRequestsCountAsync(_options.Username, repo.Name);

                result.Add(new RepositoryDetails
                {
                    Name = repo.Name,
                    Description = repo.Description,
                    Languages = languageNames,
                    LastCommit = lastCommit,
                    Stars = repo.StargazersCount,
                    OpenPullRequests = pullRequestsCount,
                    RepoUrl = repo.HtmlUrl
                });
            }

            return result;
        }


        public async Task<int> GetPullRequestsCountAsync(string owner, string repoName)
        {
            var pulls = await _client.PullRequest.GetAllForRepository(owner, repoName, new PullRequestRequest
            {
                State = ItemStateFilter.Open
            });

            return pulls.Count;
        }


        public async Task<List<Repository>> GetUserRepositoriesAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_options.Username);
            return repositories.ToList();
        }


        public async Task<int> GetUserFollowersAsync(string userName)
        {
            var user = await _client.User.Get(userName);
            return user.Followers;
        }


        public async Task<List<Repository>> SearchRepositoriesAsync(string? query = null, string? language = null, string? user = null)
        {
            var searchQuery = new StringBuilder();

            if (!string.IsNullOrEmpty(query))
            {
                searchQuery.Append(query);
            }

            if (!string.IsNullOrEmpty(language))
            {
                searchQuery.Append($" language:{language}");
            }

            if (!string.IsNullOrEmpty(user))
            {
                searchQuery.Append($" user:{user}");
            }


            if (searchQuery.Length == 0)
            {
                searchQuery.Append("stars:>100");
            }

            var request = new SearchRepositoriesRequest(searchQuery.ToString());
            var result = await _client.Search.SearchRepo(request);

            return result.Items.ToList();
        }
    }
}




