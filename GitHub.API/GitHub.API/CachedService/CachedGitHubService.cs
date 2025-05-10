using GitHub.Service;
using GitHub.Service.Modals;
using Microsoft.Extensions.Caching.Memory;
using Octokit;
using System.Reflection;

namespace GitHub.API.CachedService
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string UserPortfolioKey = "UserPortfolioKey";
        private const string LastEventKey = "LastEventKey";

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }


        public async Task<List<Portfolio>> GetPortfolioAsync()
        {
            var latestEventDate = await GetLastUserActivityDateAsync();

            if (_memoryCache.TryGetValue(LastEventKey,out DateTimeOffset cachedEventDate)&&
                _memoryCache.TryGetValue(UserPortfolioKey, out List<Portfolio> cachedPortfolio))
            {
                if( latestEventDate <= cachedEventDate)
                {
                    return cachedPortfolio;
                }
            }

            cachedPortfolio = await _gitHubService.GetPortfolioAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // או כל זמן שתבחרי
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(UserPortfolioKey, cachedPortfolio, cacheOptions);
            _memoryCache.Set(LastEventKey, latestEventDate ?? DateTimeOffset.UtcNow, cacheOptions);


            if (_memoryCache.TryGetValue(UserPortfolioKey, out  cachedPortfolio))
            {
                Console.WriteLine($"Portfolio found in cache: {cachedPortfolio.Count} items");
            }
            else
            {
                Console.WriteLine("Portfolio not found in cache.");
            }

            return cachedPortfolio;


            //if (_memoryCache.TryGetValue(UserPortfolioKey, out List<Portfolio> portfolio))
            //    return portfolio;

            //var cacheOption = new MemoryCacheEntryOptions()
            //    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //    .SetSlidingExpiration(TimeSpan.FromMinutes(7));

            //portfolio= await _gitHubService.GetPortfolioAsync();
            //_memoryCache.Set(UserPortfolioKey, portfolio, cacheOption);

            //return portfolio;
        }

        public async Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null)
        {
            return await _gitHubService.SearchRepositoriesAsync(name, language, user);
        }

        public async Task<DateTimeOffset?> GetLastUserActivityDateAsync()
        {
            return await _gitHubService.GetLastUserActivityDateAsync();
        }

    
    }
}
