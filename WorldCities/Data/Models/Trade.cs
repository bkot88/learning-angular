using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Data.Models
{
    public class Trade
    {
        public ulong Id { get; set; }
        
        public string Symbol { get; set; }

        public string Direction { get; set; }

        public string Status { get; set; }

        public DateTime? OpenDateTime { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal? PxOpen { get; set; }

        public DateTime? CloseDateTime { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal? PxClose { get; set; }

        public string Comment { get; set; }
    }
}
