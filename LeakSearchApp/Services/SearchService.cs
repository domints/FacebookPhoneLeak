using System.Collections.Generic;
using System.Linq;
using LeakSearchApp.Database;
using LeakSearchApp.Models;

namespace LeakSearchApp.Services
{
    public class SearchService : ISearchService
    {
        private readonly IHashService _hashService;
        private readonly LeakContext _cx;

        public SearchService(IHashService hashService, LeakContext context)
        {
            _hashService = hashService;
            _cx = context;
        }

        public BatchSearchResultModel BatchSearchByFacebookId(List<SearchQueryEntry> queries)
        {
            throw new System.NotImplementedException();
        }

        public BatchSearchResultModel BatchSearchByPhoneNumber(List<SearchQueryEntry> queries)
        {
            throw new System.NotImplementedException();
        }

        public SearchResultModel SearchByFacebookId(string hash)
        {
            var privateHash = _hashService.GeneratePrivateHash(hash);
            var entry = _cx.Entries.FirstOrDefault(e => e.IdHash == privateHash);
            if(entry == null)
                return new SearchResultModel { EntryExists = false };
            
            return GetResultModel(entry);
        }

        public SearchResultModel SearchByPhoneNumber(string hash)
        {
            var privateHash = _hashService.GeneratePrivateHash(hash);
            var entry = _cx.Entries.FirstOrDefault(e => e.PhoneHash == privateHash);
            if(entry == null)
                return new SearchResultModel { EntryExists = false };
            
            return GetResultModel(entry);
        }

        private BatchSearchResultModel GetResultModel(Entry entry, string name = null)
        {
            return new BatchSearchResultModel
            {
                Name = name,
                EntryExists = true,
                HasName = entry.HasName,
                HasBirthdate = entry.HasBirthdate,
                HasComingPlace = entry.HasComingPlace,
                HasEmail = entry.HasEmail,
                HasGender = entry.HasGender,
                HasLivingPlace = entry.HasLivingPlace,
                HasRelationshipStatus = entry.HasRelationshipStatus,
                HasWorkplace = entry.HasWorkplace
            };
        }
    }
}