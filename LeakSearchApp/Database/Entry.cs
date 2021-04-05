namespace LeakSearchApp.Database
{
    public class Entry
    {
        public long Id { get; set; }
        public byte[] PhoneHash { get; set; }
        public byte[] IdHash { get; set; }
        public bool HasName { get; set; }
        public bool HasGender { get; set; }
        public bool HasLivingPlace { get; set; }
        public bool HasComingPlace { get; set; }
        public bool HasRelationshipStatus { get; set; }
        public bool HasWorkplace { get; set; }
        public bool HasEmail { get; set; }
        public bool HasBirthdate { get; set; }

        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }
    }
}