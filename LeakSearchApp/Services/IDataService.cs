using System.Collections.Generic;
using System.Threading.Tasks;
using LeakSearchApp.Models;

namespace LeakSearchApp.Services
{
    public interface IDataService
    {
        Task<int> AddCollection(string name);
        Task AddEntries(int collectionId, IEnumerable<InsertEntry> entries);
    }
}