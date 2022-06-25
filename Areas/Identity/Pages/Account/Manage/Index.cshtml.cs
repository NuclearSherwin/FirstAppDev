using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppdevPhong.Data;
using AppdevPhong.Utility;
using AppDevPhong.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppdevKhanhPhong.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                ApplicationDbContext db)
            // ApplicationDbContext db
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; }


        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var usertemp = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claims.Value);
            var role = await _userManager.GetRolesAsync(usertemp);

            if (role.FirstOrDefault() == SD.Role_Trainee)
            {
                var userFromDb = _db.Trainees.Find(user.Id);
                Username = userName;
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Name = userFromDb.Name,
                    Age = userFromDb.Age,
                    Education = userFromDb.Education,
                    MainProgrammingLanguage = userFromDb.MainProgrammingLanguage,
                    ToeicScore = userFromDb.ToeicScore,
                    DateOfBirth = userFromDb.DateOfBirth,
                    Location = userFromDb.Location,
                    department = userFromDb.Department
                };
            }
            else if (role.FirstOrDefault() == SD.Role_Trainer)
            {
                var userFromDb = _db.Trainers.Find(user.Id);
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Name = userFromDb.Name,
                    TypeOfTrainer = userFromDb.Type,
                    WorkingPlace = userFromDb.WorkingPlace
                };
            }
            else
            {
                var userFromDb = _db.ApplicationUsers.Find(user.Id);
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Name = userFromDb.Name
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            // if (!ModelState.IsValid)
            // {
            //     await LoadAsync(user);
            //     return Page();
            // }
            //
            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            // if (Input.PhoneNumber != phoneNumber)
            // {
            //     var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //     if (!setPhoneResult.Succeeded)
            //     {
            //         StatusMessage = "Unexpected error when trying to set phone number.";
            //         return RedirectToPage();
            //     }
            // }

            if (User.IsInRole(SD.Role_Trainee))
            {
                var profile = _db.Trainees.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber) profile.PhoneNumber = Input.PhoneNumber;

                if (Input.Name != profile.Name) profile.Name = Input.Name;

                if (Input.Education != profile.Education) profile.Education = Input.Education;

                if (Input.DateOfBirth != profile.DateOfBirth) profile.DateOfBirth = Input.DateOfBirth;

                if (Input.MainProgrammingLanguage != profile.MainProgrammingLanguage)
                    profile.MainProgrammingLanguage = Input.MainProgrammingLanguage;

                if (Input.ToeicScore != profile.ToeicScore) profile.ToeicScore = Input.ToeicScore;

                if (Input.department != profile.Department) profile.Department = Input.department;

                if (Input.Location != profile.Location) profile.Location = Input.Location;

                _db.Trainees.Update(profile);
                _db.SaveChanges();
            }
            else if (User.IsInRole(SD.Role_Trainer))
            {
                var profile = _db.Trainers.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber) profile.PhoneNumber = Input.PhoneNumber;

                if (Input.Name != profile.Name) profile.Name = Input.Name;

                if (Input.WorkingPlace != profile.WorkingPlace) profile.WorkingPlace = Input.WorkingPlace;

                if (Input.TypeOfTrainer != profile.Type) profile.Type = Input.TypeOfTrainer;

                _db.Trainers.Update(profile);
                _db.SaveChanges();
            }
            else
            {
                var profile = _db.ApplicationUsers.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber) profile.PhoneNumber = Input.PhoneNumber;

                if (profile.Name != Input.Name) profile.Name = Input.Name;

                _db.ApplicationUsers.Update(profile);
                _db.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string Name { get; set; }
            public int Age { get; set; }
            public string Education { get; set; }
            public string MainProgrammingLanguage { get; set; }
            public float ToeicScore { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Location { get; set; }
            public Department department { get; set; }
            public TypeOfTrainer TypeOfTrainer { get; set; }
            public string WorkingPlace { get; set; }
        }
    }
}