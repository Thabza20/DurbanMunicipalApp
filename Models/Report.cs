using System.ComponentModel.DataAnnotations;

namespace DurbanMunicipalApp.Models
{
    public enum ProblemType
    {
        [Display(Name = "Water Leak")]
        WaterLeak,
        [Display(Name = "No Water")]
        NoWater,
        [Display(Name = "Water Quality")]
        WaterQuality,
        [Display(Name = "Sewer Issue")]
        SewerIssue,
        [Display(Name = "Other")]
        Other
    }

    public enum ReportStatus
    {
        [Display(Name = "Submitted")]
        Submitted,
        [Display(Name = "Under Review")]
        UnderReview,
        [Display(Name = "In Progress")]
        InProgress,
        [Display(Name = "Resolved")]
        Resolved,
        [Display(Name = "Closed")]
        Closed
    }

    public class Report
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Problem type is required")]
        [Display(Name = "Problem Type")]
        public ProblemType? ProblemType { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Picture")]
        public string? PicturePath { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public ReportStatus Status { get; set; } = ReportStatus.Submitted;

        [Display(Name = "Submitted Date")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
