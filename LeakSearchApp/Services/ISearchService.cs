using System.Collections.Generic;
using LeakSearchApp.Models;

namespace LeakSearchApp.Services
{
    public interface ISearchService
    {
        SearchResultModel SearchByPhoneNumber(string hash);
        SearchResultModel SearchByFacebookId(string hash);
        BatchSearchResultModel BatchSearchByPhoneNumber(List<SearchQueryEntry> queries);
        BatchSearchResultModel BatchSearchByFacebookId(List<SearchQueryEntry> queries);
    }
}