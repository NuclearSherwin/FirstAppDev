using System.ComponentModel.DataAnnotations;
using AppdevPhong.Utility;

namespace AppdevPhong.Models
{
    public class Trainer:ApplicationUser
    {
        [Required]
        public TypeOfTrainer Type { get; set; }
        [Required]
        public string WorkingPlace { get; set; }
    }
}