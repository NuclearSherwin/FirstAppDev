using System;
using System.Linq;
using AppdevPhong.Data;
using AppdevPhong.Models;
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
            if(_db.Roles.Any(r=>r.Name == "Admin")) return;
            if(_db.Roles.Any(r=>r.Name == "Staff")) return;
            if(_db.Roles.Any(r=>r.Name == "Trainer")) return;
            if(_db.Roles.Any(r=>r.Name == "Trainee")) return;
            
            //create new role
            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Staff")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Trainer")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Trainee")).GetAwaiter().GetResult();

            //create new user
            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                EmailConfirmed = true
            }, "Admin@gmail.com").GetAwaiter().GetResult();

            
            //Add user to role
            var userAdmin = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@gmail.com");
            _userManager.AddToRoleAsync(userAdmin, "Admin").GetAwaiter().GetResult();
            
        }
    }
}