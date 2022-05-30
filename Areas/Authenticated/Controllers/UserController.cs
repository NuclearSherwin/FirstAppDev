using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppdevPhong.Data;
using AppdevPhong.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppdevPhong.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppdevPhong.Areas.Authenticated.Controllers
{
    [Authorize(SD.Authenticated_Area)]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Staff)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager
            , ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // take current login user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            var userList = _db.ApplicationUsers.Where(u => u.Id != claims.Value);
            
            foreach (var user in userList)
            {
                var userTemp = await _userManager.FindByIdAsync(user.Id);
                var roleTemp = await _userManager.GetRolesAsync(userTemp);
                user.Role = roleTemp.FirstOrDefault();
            }

            if (User.IsInRole(SD.Role_Staff))
            {
                return View(userList.ToList().Where(u => u.Role != SD.Role_Admin  && u.Role != SD.Role_Staff));
            }
            
            return View(userList.ToList().Where(U => U.Role != SD.Role_Trainer));
        }

        // [HttpGet]
        // public IActionResult Update(string id)
        // {
        //     if (id != null)
        //     {
        //         UserVM userVm = new UserVM();
        //         var user = _db.ApplicationUsers.Find(id);
        //         userVm.ApplicationUser = user;
        //
        //         userVm.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //         {
        //             Text = i,
        //             Value = i
        //         });
        //         return View(userVm);
        //     }
        //
        //     return NotFound();
        // }
        //
        // [HttpPost]
        // public async Task<IActionResult> Update(UserVM userVm )
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = _db.ApplicationUsers.Find(userVm.ApplicationUser.Id);
        //         user.FullName = userVm.ApplicationUser.FullName;
        //         user.DateOfBirth = userVm.ApplicationUser.DateOfBirth;
        //         user.HealthCareId = userVm.ApplicationUser.HealthCareId;
        //         user.CredentialId = userVm.ApplicationUser.CredentialId;
        //
        //         var roleOld = await _userManager.GetRolesAsync(user);
        //         await _userManager.RemoveFromRoleAsync(user, roleOld.First());
        //         await _userManager.AddToRoleAsync(user, userVm.Role);
        //
        //         _db.ApplicationUsers.Update(user);
        //         _db.SaveChanges();
        //         
        //         return RedirectToAction(nameof(Index));
        //     }
        //     
        //     userVm.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //     {
        //         Text = i,
        //         Value = i
        //     });
        //     
        //     return View(userVm);
        // }
        
        [HttpGet]
        public async Task<IActionResult> LockUnlock(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userNeedToLock = _db.ApplicationUsers.Where(u => u.Id == id).First();

            if (userNeedToLock.Id == claims.Value)
            {
                // hien ra loi ban dang lock account cua chinh minh
            }

            if (userNeedToLock.LockoutEnd != null && userNeedToLock.LockoutEnd > DateTime.Now)
            {
                userNeedToLock.LockoutEnd = DateTime.Now;
            }
            else
            {
                userNeedToLock.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (User.IsInRole(SD.Role_Staff))
            {
                var roleTemp = await _userManager.GetRolesAsync(user);
                var role = roleTemp.FirstOrDefault();
                if (role == SD.Role_Trainer)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            await _userManager.DeleteAsync(user);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            var user = _db.ApplicationUsers.Find(id);

            if (user == null)
            {
                return View();
            }

            ConfirmEmailVM confirmEmailVm = new ConfirmEmailVM()
            {
                Email = user.Email
            };

            return View(confirmEmailVm);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailVm.Email);
                if ( user!= null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    return RedirectToAction("ResetPassword", "User", new { token = token, email = user.Email });
                }
            }
            return View(confirmEmailVm);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {

            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel()
            {
                Email = email,
                Token = token
            };

            return View(resetPasswordViewModel);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token,
                        resetPasswordViewModel.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(resetPasswordViewModel);
        }
    }
}