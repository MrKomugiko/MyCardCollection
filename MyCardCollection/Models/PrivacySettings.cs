using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCardCollection.Models
{
    public class PrivacySettings
    {
        [Key] public int Id { get; set; }
        public bool AllowEmail { get; set; } = true;
        public bool AllowFirstName { get; set; } = false;
        public bool AllowLastName { get; set; } = false;
        public bool AllowCity { get; set; } = false;
        public bool AllowCountry { get; set; } = true;
        public bool AllowWebsite { get; set; } = true;
        public bool AllowBirthday { get; set; } = false;
        
        public string? UserId { get; set; }
        [ForeignKey("UserId")] public virtual AppUser? User { get; set; }
    }
}
