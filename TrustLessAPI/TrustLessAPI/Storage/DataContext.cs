using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;


namespace TrustLessAPI.Models
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
            Database.CommandTimeout = 90000;
        }
 
        public static DataContext Create()
        {
            return new DataContext();
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Statement> Statements { get; set; }
    }
}