using System.Collections.Generic;
using AppdevPhong.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppdevPhong.ViewModels
{
    public class CourseVm
    {
        public Course Course { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}