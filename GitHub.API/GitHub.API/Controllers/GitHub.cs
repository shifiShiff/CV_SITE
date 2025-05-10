using GitHub.Service;
using GitHub.Service.Modals;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GitHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class GitHub : ControllerBase
    {
      
        private readonly IGitHubService _gitHubService;

        public GitHub(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }


        [HttpGet("portfolio")]
        public async Task<ActionResult<List<Portfolio>>> Get()
        {
            return await _gitHubService.GetPortfolioAsync();

        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Portfolio>>> Search([FromQuery] string? name, [FromQuery] string? language, [FromQuery] string? user)
        {
            var results = await _gitHubService.SearchRepositoriesAsync(name, language, user);
            return Ok(results);
        }




    }
}
