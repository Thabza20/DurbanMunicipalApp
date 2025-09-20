using System.ComponentModel.DataAnnotations;

namespace DurbanMunicipalApp.Models
{
    public partial class Admin
    {
        [Key] // <-- tells EF this is the PK
        public int UserId { get; set; }

        public string AdminName { get; set; } = null!;

        public virtual UserProfile User { get; set; } = null!;
    }
}
