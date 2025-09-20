using System.ComponentModel.DataAnnotations;

namespace DurbanMunicipalApp.ViewModels
{
    public class TeamRegistrationViewModel
    {
        [Required]
        public string StaffName { get; set; } = null!;

        [Required]
        public string StaffRole { get; set; } = null!; // Electrician, Plumber, etc.

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
