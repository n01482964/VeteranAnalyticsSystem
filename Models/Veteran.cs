namespace VeteranAnalyticsSystem.Models
{
    public class Veteran
    {
        public int VeteranId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; } // Male, Female, etc.
        public DateTime DateOfBirth { get; set; }
        public string BranchOfService { get; set; } // Military branch
        public string Email { get; set; }
        public string Condition { get; set; } // e.g., PTSD, Amputee, etc.
        public DateTime DateOfEntry { get; set; } // Date added to the system
    }
}
