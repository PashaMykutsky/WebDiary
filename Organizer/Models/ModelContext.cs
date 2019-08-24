using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Models
{
    public class ModelContext : DbContext
    {
        public ModelContext(DbContextOptions<ModelContext> options) : base(options) {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}
