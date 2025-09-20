using System.ComponentModel.DataAnnotations;

namespace DurbanMunicipalApp.ViewModels
{
    public class CustomerRegistrationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string CustomerFirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        public string CustomerLastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
