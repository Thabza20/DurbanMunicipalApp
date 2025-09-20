using DurbanMunicipalApp.Models;
using System.ComponentModel.DataAnnotations;

public partial class Team
{
    [Key]
    public int UserId { get; set; }

    public string StaffName { get; set; } = null!;
    public string StaffRole { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public virtual UserProfile User { get; set; } = null!;
}
