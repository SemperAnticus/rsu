using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [NotMapped] // Does not effect with your database
        [Compare("Password", ErrorMessage = "Пароли не совпадают. Попробуйте снова.")]
        public string ConfirmPassword { get; set; }
    }
}