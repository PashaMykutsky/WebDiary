using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime LastActive { get; set; }
        public bool IsBanned { get; set; }
        public bool IsAdmin { get; set; }
    }
}
