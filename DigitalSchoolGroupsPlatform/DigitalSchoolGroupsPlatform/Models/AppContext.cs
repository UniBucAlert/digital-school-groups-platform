using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DigitalSchoolGroupsPlatform.Models
{
    // Tehnica "code first"
    // Avem nevoie de EntityFramework!
    public class AppContext : DbContext
    {
        // cauta o cale pe local (connection string) in care este stocata BD
        public AppContext() : base("DBConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext,
                DigitalSchoolGroupsPlatform.Migrations.Configuration>("DBConnectionString"));
        }
        // standard ca obiectul sa fie pluralizat
        public DbSet<Group> Groups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}