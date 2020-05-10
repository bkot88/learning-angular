using System.ComponentModel.DataAnnotations;

namespace WorldCities.Data.Models
{
    public class Quote
    {
        [Required]
        public string Symbol { get; set; }

        [Required]
        public string Date { get; set; }

        public decimal? PxOpen { get; set; }
        
        public decimal? PxClose { get; set; }
    }
}
