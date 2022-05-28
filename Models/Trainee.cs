using System;
using System.ComponentModel.DataAnnotations;
using AppdevPhong.Utility;

namespace AppdevPhong.Models
{
    public class Trainee:ApplicationUser
    {
        [Required]
        public int Age { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string MainProgrammingLanguage { get; set; }
        [Required]
        public float ToeicScore { get; set; }
        [Required]
        public Department Department { get; set; }
        [Required]
        public string Location { get; set; }    
    }
}