using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Data.Models
{
    public class BMIProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 

        [Required]
        public double BMIValue { get; set; } 

        [Required]
        public DateTime RecordedAt { get; set; } 
    }
}
