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
    public interface IGitHubService
    {
         Task<List<Portfolio>> GetPortfolioAsync();

        Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null);

        Task<DateTimeOffset?> GetLastUserActivityDateAsync();

    }
}
