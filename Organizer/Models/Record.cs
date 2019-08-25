using System;
using System.ComponentModel.DataAnnotations;

namespace Organizer.Models
{
    public class Record
    {
        [Required]
        public int Id { get; set; }
        public DateTime CreateTime{ get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
    }
}
