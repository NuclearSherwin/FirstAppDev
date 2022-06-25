using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AppdevPhong.Models;
using AppDevPhong.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace AppdevKhanhPhong.Areas.Identity.Pages.Account
{
    [Area(SD.Authenticated_Area)]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Staff)]
    public class RegisterModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            GetRole();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser();
                var trainee = new Trainee();
                var trainer = new Trainer();
                var result = new IdentityResult();
                if (Input.Role == SD.Role_Trainee)
                {
                    trainee = new Trainee
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        Role = Input.Role,
                        Name = Input.Name
                    };
                    result = await _userManager.CreateAsync(trainee, Input.Password);
                }
                else if (Input.Role == SD.Role_Trainer)
                {
                    trainer = new Trainer
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        Role = Input.Role,
                        Name = Input.Name
                    };
                    result = await _userManager.CreateAsync(trainer, Input.Password);
                }
                else
                {
                    applicationUser = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        Role = Input.Role,
                        Name = Input.Name
                    };
                    result = await _userManager.CreateAsync(applicationUser, Input.Password);
                }

                if (result.Succeeded)
                {
                    if (Input.Role == SD.Role_Trainee)
                        await _userManager.AddToRoleAsync(trainee, trainee.Role);
                    else if (Input.Role == SD.Role_Trainer)
                        await _userManager.AddToRoleAsync(trainer, trainer.Role);
                    else
                        await _userManager.AddToRoleAsync(applicationUser, applicationUser.Role);
                    _logger.LogInformation("User created a new account with password.");

                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    // var callbackUrl = Url.Page(
                    //     "/Account/ConfirmEmail",
                    //     pageHandler: null,
                    //     values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //     protocol: Request.Scheme);
                    //
                    // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    return RedirectToAction("Index", "Users", new { Area = "Authenticated" });
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            GetRole();
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void GetRole()
        {
            Input = new InputModel
            {
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_Trainee).Select(x => x.Name).Select(i =>
                    new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
            };
            if (User.IsInRole(SD.Role_Staff))
                Input = new InputModel
                {
                    RoleList = _roleManager.Roles.Where(u => u.Name == SD.Role_Trainee).Select(x => x.Name).Select(i =>
                        new SelectListItem
                        {
                            Text = i,
                            Value = i
                        })
                };
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Required] public string Name { get; set; }

            [Required] public string PhoneNumber { get; set; }

            //drop down
            [Required] public string Role { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }
        }
    }
}