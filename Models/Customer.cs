using DurbanMunicipalApp.Models;
using System.ComponentModel.DataAnnotations;

public partial class Customer
{
    [Key] // Add a PK
    public int UserId { get; set; } // or you can use an Id column

    public string CustomerId { get; set; } = null!;
    public string CustomerFirstName { get; set; } = null!;
    public string CustomerLastName { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
