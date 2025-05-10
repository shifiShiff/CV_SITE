using GitHub.Service.Modals;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GitHub.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options.Value;
            _client = new GitHubClient(new ProductHeaderValue("My-GitHub-App"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<List<Portfolio>> GetPortfolioAsync()
        {
            var repos = await _client.Repository.GetAllForUser(_options.UserName);
            var result = new List<Portfolio>();

            foreach (var repo in repos)
            {
                var languages = await _client.Repository.GetAllLanguages(repo.Id);

                var commits = await _client.Repository.Commit.GetAll(repo.Id);
                var lastCommit = commits.FirstOrDefault()?.Commit.Author.Date;

                var stars = repo.StargazersCount;

                var pulls = await _client.PullRequest.GetAllForRepository(repo.Id);
                var pullCount = pulls.Count;

                result.Add(new Portfolio
                {
                    Name = repo.Name,
                    Url = repo.HtmlUrl,
                    Languages = languages.Select(l => l.Name).ToList(),
                    LastCommitDate = lastCommit,
                    Stars = stars,
                    PullRequests = pullCount
                });
            }

            return result;
        }



        public async Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null)
        {
            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
                queryParts.Add($"{name} in:name");

            if (!string.IsNullOrWhiteSpace(language))
                queryParts.Add($"language:{language}");

            if (!string.IsNullOrWhiteSpace(user))
                queryParts.Add($"user:{user}");

            var query = string.Join(" ", queryParts);

            var request = new SearchRepositoriesRequest(query)
            {
                SortField = RepoSearchSort.Updated,
                Order = SortDirection.Descending
            };

            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }

        public async Task<DateTimeOffset?> GetLastUserActivityDateAsync()
        {
            var events = await _client.Activity.Events.GetAllUserPerformed(_options.UserName);

            return events.FirstOrDefault()?.CreatedAt;
        }

    }

}
