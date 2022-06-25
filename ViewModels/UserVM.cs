using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppdevPhong.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppdevPhong.ViewModels
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }

        [Required] public string Role { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}