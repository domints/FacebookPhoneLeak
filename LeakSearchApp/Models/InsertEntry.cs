namespace LeakSearchApp.Models
{
    public class InsertEntry
    {
        public string PhoneHash { get; set; }
        public string IdHash { get; set; }
        public bool HasName { get; set; }
        public bool HasGender { get; set; }
        public bool HasLivingPlace { get; set; }
        public bool HasComingPlace { get; set; }
        public bool HasRelationshipStatus { get; set; }
        public bool HasWorkplace { get; set; }
        public bool HasEmail { get; set; }
        public bool HasBirthdate { get; set; }
    }
}