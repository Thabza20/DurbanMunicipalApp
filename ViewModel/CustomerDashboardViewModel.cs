namespace DurbanMunicipalApp.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string Email { get; set; }

        public int ReportsSubmitted { get; set; }
        public int ReportsResolved { get; set; }
    }

}
