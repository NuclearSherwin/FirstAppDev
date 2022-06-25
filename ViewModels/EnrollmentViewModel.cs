using System.Collections.Generic;
using AppdevPhong.Models;

namespace AppdevPhong.ViewModels
{
    public class EnrollmentViewModel
    {
        public int CourseId { get; set; }
        public IEnumerable<Enrollment> EnrollmentsList { get; set; }
        public IEnumerable<Trainee> Trainees { get; set; }
    }
}