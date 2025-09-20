using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DurbanMunicipalApp.Models
{
    public partial class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

     

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool? IsActive { get; set; }

        public string? Otp { get; set; }

        public DateTime? OtpExpiry { get; set; }

        public string? UserType { get; set; }  // "Admin", "Customer", "Team"

        // Optional: navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual Admin? Admin { get; set; }
        public virtual Team? Team { get; set; }
    }



}
