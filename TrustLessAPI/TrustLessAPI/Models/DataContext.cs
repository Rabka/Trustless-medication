using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace TrustLessAPI.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.CommandTimeout = 90000;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

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