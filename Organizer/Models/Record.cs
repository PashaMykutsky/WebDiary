using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Models
{
    public class Record
    {
        [Required]
        public int Id { get; set; }
        public DateTime CreateTime{ get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public User User { get; set; }
    }
}
