using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Yomoney.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Cell Number e.g 263777999999")]
        [DataType(DataType.PhoneNumber)]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public string redirect { get; set; }
       
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Cellphone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Cell { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "School")]
        public string School { get; set; }

        //[Required]
        [Display(Name = "Date of Birth")]
        public string DOB { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

       
        [Display(Name = "Security Question")]
        public string question { get; set; }

      
        [Display(Name = "Select your syllabus")]
        public string Answer { get; set; }

        public SelectList syllabus { get; set;}

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool Disclamer { get; set; }

       // [Required]
        public string Gender { get; set; }

        [Required]
        public bool Sex { get; set; }

        [Required]
        [Display(Name = "Date")]
        public int Date { get; set; }
        [Required]
        [Display(Name = "Month")]
        public int month { get; set; }
        [Required]
        [Display(Name = "Year")]
        public int year { get; set; }

    }
}
