using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Models.DTO
{
    public class AuthDTO
    {
        [Required(ErrorMessage = "em_Login")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "em_Email")]
        public string login { get; set; }
        [Required(ErrorMessage = "em_Pass")]
        public string password { get; set; }
    }

    public class RegDTO
    {
        [Required(ErrorMessage = "em_Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "em_Surname")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "em_Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "em_RegExEmail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "em_Pass")]
        [RegularExpression(@"^((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!\s).{8,16})$", ErrorMessage = "em_RegExPass")]
        public string Password { get; set; }
        [Required(ErrorMessage = "em_Repit")]
        [Compare("Password", ErrorMessage = "em_Compare")]
        public string RepitPass { get; set; }
    }
    public class SettingsDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        [RegularExpression(@"^((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!\s).{8,16})$", ErrorMessage = "em_RegExPass")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "em_Compare")]
        public string RepitPass { get; set; }
    }
}
