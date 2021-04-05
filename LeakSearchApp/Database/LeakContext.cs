using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LeakSearchApp.Database
{
    public class LeakContext : DbContext
    {
        public LeakContext(DbContextOptions<LeakContext> options)
            : base(options)
        {
            
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}