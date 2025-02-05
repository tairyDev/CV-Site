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

        public async Task<List<RepositoryDetails>> GetPortfolioAsync()
        {
            
                var repositories = await _client.Repository.GetAllForUser(_options.Username);
                var result = new List<RepositoryDetails>();

                foreach (var repo in repositories)
                {
                    var languages = await _client.Repository.GetAllLanguages(_options.Username, repo.Name);
                    var languageNames = languages.Select(l => l.Name).ToList();

                    var lastCommit = await GetLastCommitDateAsync(_options.Username, repo.Name);
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

        private async Task<DateTime?> GetLastCommitDateAsync(string owner, string repoName)
        {        
                var commits = await _client.Repository.Commit.GetAll(owner, repoName);
                return commits.FirstOrDefault()?.Commit.Committer.Date.UtcDateTime;      
        }

        public async Task<int> GetPullRequestsCountAsync(string owner, string repoName)
        {
                var pulls = await _client.PullRequest.GetAllForRepository(owner, repoName, new PullRequestRequest
                {
                    State = ItemStateFilter.Open
                });
                return pulls.Count;                
        }
        public async Task<List<Repository>> SearchRepositoriesAsync(string? query = null, string? language = null, string? user = null)
        {
               var searchQuery = new StringBuilder();

                // הוספת query אם קיים
                if (!string.IsNullOrEmpty(query))
                {
                    searchQuery.Append(query);
                }

                // הוספת קריטריון משתמש אם קיים
                if (!string.IsNullOrEmpty(user))
                {
                    if (searchQuery.Length > 0)
                        searchQuery.Append(" ");
                    searchQuery.Append($"user:{user}");
                }

                // אם לא נבנה קריטריון כלשהו, משתמשים בקריטריון כללי להחזרת תוצאות
                if (searchQuery.Length == 0)
                {
                    searchQuery.Append("stars:>=0");
                }

                // חשוב: במקרה ויש גם פרמטר שפה, לא נצרף אותו לשאילתת החיפוש הראשונית,
                // אלא נבצע סינון מקומי לאחר קבלת התוצאות.
                Console.WriteLine($"Search query: {searchQuery}");

                var request = new SearchRepositoriesRequest(searchQuery.ToString());
                var result = await _client.Search.SearchRepo(request);
                var repositories = result.Items.ToList();

                // סינון לפי שפות אם נמסר פרמטר language
                if (!string.IsNullOrEmpty(language))
                {
                    repositories = await FilterRepositoriesByLanguage(repositories, language);
                }

                return repositories;
        }

        private async Task<List<Repository>> FilterRepositoriesByLanguage(List<Repository> repositories, string languagesInput)
        {
            var filteredRepositories = new List<Repository>();

            // מפצלים את המחרוזת למערך של שפות (במקרה של שפה אחת, המערך יכיל רק פריט אחד)
            var languagesToSearch = languagesInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var repo in repositories)
            {
           
                    // GetAllLanguages מחזירה רשימה של RepositoryLanguage עבור הריפוזיטורי הנתון
                    var repoLanguages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);

                    // בודקים אם לכל שפת חיפוש יש התאמה ברשימת השפות של הריפוזיטורי
                    bool containsAllLanguages = languagesToSearch.All(searchLang =>
                        repoLanguages.Any(rl => string.Equals(rl.Name, searchLang, StringComparison.OrdinalIgnoreCase))
                    );

                    if (containsAllLanguages)
                    {
                        filteredRepositories.Add(repo);
                    }   
            }
            return filteredRepositories;
        }
    }
}




