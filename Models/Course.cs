using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppdevPhong.Models
{
    public class Course
    {
        [Key] public int Id { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required] public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] public Category Category { get; set; }
    }
}