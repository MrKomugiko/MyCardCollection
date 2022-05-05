// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyCardCollection.Models;
using MyCardCollection.Repository;

namespace MyCardCollection.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUsersRepository _usersRepository;


        public IndexModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUsersRepository usersRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usersRepository = usersRepository;
        }



        [TempData]
        public string StatusMessage { get; set; }
        public PrivacySettings Privacy { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required] public string UserName { get; set; }
            public string? Name { get; set; }
            [Phone] [Display(Name = "Phone number")] public string PhoneNumber { get; set; }
            public string? Lastname { get; set; }
            public string? City { get; set; }
            public string? CountryCode { get; set; }
            public DateTime? Birthday { get; set; }
            public string? Description { get; set; }
            public string? Website { get; set; }
            public string? AvatarImage { get; set; }



            public PrivacySettings Privacy { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            PrivacySettings _privacySettings = await _usersRepository.GetPrivacyDataByUser(user);

            Input = new InputModel
            {   
                UserName = userName, 
                PhoneNumber = phoneNumber,
                Name = user.Name,
                Lastname = user.Lastname,
                City = user.City,
                CountryCode = user.CountryCode,
                Birthday = ((DateTime)(user.Birthday)).ToLocalTime(),
                Description = user.Description ,
                Website = user.Website,
                AvatarImage = user.AvatarImage,
                Privacy = _privacySettings
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _usersRepository.UpdateUserData(user, Input);

            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
