namespace LeakSearchApp.Models
{
    public class SearchResultModel
    {
        public bool EntryExists { get; set; }
        public bool HasName { get; set; }
        public bool HasGender { get; set; }
        public bool HasLivingPlace { get; set; }
        public bool HasComingPlace { get; set; }
        public bool HasRelationshipStatus { get; set; }
        public bool HasWorkplace { get; set; }
        public bool HasEmail { get; set; }
        public bool HasBirthdate { get; set; }
    }

    public class BatchSearchResultModel : SearchResultModel
    {
        public string Name { get; set; }
    }
}