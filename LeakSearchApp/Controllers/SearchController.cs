using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeakSearchApp.Config;
using LeakSearchApp.Database;
using LeakSearchApp.Models;
using LeakSearchApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LeakSearchApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController
    {
        private readonly LeakContext _cx;
        private readonly AppSecrets _appSecrets;
        private readonly ISearchService _searchService;
        private readonly IHashService hashService;

        public SearchController(
            LeakContext context,
            ISearchService searchService,
            IOptions<AppSecrets> appSecrets, IHashService hashService)
        {
            _appSecrets = appSecrets.Value;
            _cx = context;
            _searchService = searchService;
            this.hashService = hashService;
        }

        [HttpGet("publicSalt")]
        public string GetPublicSalt()
        {
            return _appSecrets.PublicSalt;
        }

        [HttpGet("collections")]
        public IEnumerable<string> GetCollections()
        {
            var collections = _cx.Collections.ToList();
            return collections.Select(c => c.Name);
        }

        [HttpPost("byPhone")]
        public SearchResultModel SearchByPhoneNumber([FromBody] string hash)
        {
            if(string.IsNullOrWhiteSpace(hash))
                return null;

            return _searchService.SearchByPhoneNumber(hash);
        }

        [HttpPost("byFacebookId")]
        public SearchResultModel SearchByFacebookId([FromBody] string hash)
        {
            if(string.IsNullOrWhiteSpace(hash))
                return null;

            return _searchService.SearchByFacebookId(hash);
        }

        [HttpPost("batchByPhone")]
        public BatchSearchResultModel BatchSearchByPhoneNumber(List<SearchQueryEntry> queries)
        {
            if (queries == null || queries.Count == 0 || queries.Count > 500)
                return null;

            return _searchService.BatchSearchByPhoneNumber(queries);
        }

        [HttpPost("batchByFacebookId")]
        public BatchSearchResultModel BatchSearchByFacebookId(List<SearchQueryEntry> queries)
        {
            if (queries == null || queries.Count == 0 || queries.Count > 500)
                return null;

            return _searchService.BatchSearchByFacebookId(queries);
        }
    }
}