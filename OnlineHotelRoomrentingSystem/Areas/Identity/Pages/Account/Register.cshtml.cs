﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using OnlineHotelRoomrentingSystem.Commons;
using OnlineHotelRoomrentingSystem.Models;
using System.ComponentModel.DataAnnotations;
using static OnlineHotelRoomrentingSystem.Commons.EntityValidationConstants.User;


namespace OnlineHotelRoomrentingSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMemoryCache cache;

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             ILogger<RegisterModel> logger,IMemoryCache _cache)
        {
            _userManager = userManager; 
            _signInManager = signInManager;
            _logger = logger;
            cache = _cache;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required]
            [Display(Name = "First Name")]
            [StringLength(UserFirstNameMaxLength,MinimumLength = UserFirstNameMinLength)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            [StringLength(UserLastNameMaxLength,MinimumLength = UserLastNameMinLength)]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = Input.Email,
                    UserName = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                };

                var result = await _userManager.CreateAsync(user,Input.Password);

                if(result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    cache.Remove(GeneralApplicationConstants.UsersCacheKey);
                    
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
