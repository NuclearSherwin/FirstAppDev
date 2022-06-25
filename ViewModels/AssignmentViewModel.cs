using System.Collections.Generic;
using AppdevPhong.Models;

namespace AppdevPhong.ViewModels
{
    public class AssignmentViewModel
    {
        public int CourseId { get; set; }
        public IEnumerable<Assigment> AssignmentList { get; set; }
        public IEnumerable<Trainer> TrainerList { get; set; }
    }
}