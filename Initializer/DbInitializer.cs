using System;
using System.Linq;
using AppdevPhong.Data;
using AppdevPhong.Models;
using AppdevPhong.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppdevPhong.Initializer
{
    public class DbInitializer:IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            // check role existed
            if(_db.Roles.Any(r=>r.Name == SD.Role_Admin)) return;
            if(_db.Roles.Any(r=>r.Name == SD.Role_Staff)) return;
            if(_db.Roles.Any(r=>r.Name == SD.Role_Trainer)) return;
            if(_db.Roles.Any(r=>r.Name == SD.Role_Trainee)) return;
            
            //create new role
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Staff)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Trainer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Trainee)).GetAwaiter().GetResult();

            //create new Admin user
            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                EmailConfirmed = true,
                Name = "Admin"
            }, "Admin123@").GetAwaiter().GetResult();
            
            //Add user to Admin role
            var userAdmin = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@gmail.com");
            _userManager.AddToRoleAsync(userAdmin, "Admin").GetAwaiter().GetResult();
            
            
            //create new Staff user
            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "Staff@gmail.com",
                Email = "Staff@gmail.com",
                EmailConfirmed = true,
                Name = "Staff"
            }, "Staff123@").GetAwaiter().GetResult(); 
            
            //Add user to Staff role
            var userStaff = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Staff@gmail.com");
            _userManager.AddToRoleAsync(userStaff, "Staff").GetAwaiter().GetResult();
            
            
            //Add user to Trainer Role
            _userManager.CreateAsync(new Trainer()
            {
                UserName = "Trainer@gmail.com",
                Email = "Trainer@gmail.com",
                EmailConfirmed = true,
                Name = "Trainer",
                Type = TypeOfTrainer.External,
                WorkingPlace = "NY"
            }, "Trainer123@").GetAwaiter().GetResult();
            
            //Add user to Trainer role
            var userTrainer = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Trainer@gmail.com");
            _userManager.AddToRoleAsync(userTrainer, "Trainer").GetAwaiter().GetResult();
            
            
            //Add user to Trainee Role
            _userManager.CreateAsync(new Trainee()
            {
                UserName = "Trainee@gmail.com",
                Email = "Trainee@gmail.com",
                EmailConfirmed = true,
                Name = "Trainee",
                Age = 20,
                Education = "Fresher",
                MainProgrammingLanguage = "JavaScript",
                ToeicScore = 700,
                Department = Department.Development,
                Location = "NY",
                DateOfBirth = DateTime.Now
            }, "Trainee123@").GetAwaiter().GetResult();
            
            //Add user to Trainer role
            var userTrainee = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Trainee@gmail.com");
            _userManager.AddToRoleAsync(userTrainee, "Trainee").GetAwaiter().GetResult();
        }
    }
}