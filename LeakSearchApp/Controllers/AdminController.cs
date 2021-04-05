using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeakSearchApp.Attributes;
using LeakSearchApp.Database;
using LeakSearchApp.Models;
using LeakSearchApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeakSearchApp.Controllers
{
    [ApiKey]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController
    {
        private readonly LeakContext _cx;
        private readonly IDataService _dataService;

        public AdminController(
            LeakContext context,
            IDataService dataService)
        {
            _cx = context;
            _dataService = dataService;
        }

        [HttpGet("collections")]
        public IEnumerable<Collection> GetCollections()
        {
            return _cx.Collections.ToList();
        }

        [HttpPost("collection")]
        public Task<int> CreateCollection([FromBody] string name)
        {
            System.Console.WriteLine($"Trying to add name: <<{name}>>");
            return _dataService.AddCollection(name);
        }

        [HttpPost("collection/{id}/entries")]
        public Task AddEntries(int id, List<InsertEntry> entries)
        {
            return _dataService.AddEntries(id, entries);
        }
    }
}