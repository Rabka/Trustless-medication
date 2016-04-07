using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TrustlessAPI;
using TrustLessAPI.Storage;
using TrustLessModelLib;
 

namespace TrustLessAPI.Models
{
	[DbConfigurationType(typeof(MyConfiguration))] 
    public class DataContext : DbContext
    {
        public DataContext()
			: base(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlTest"].ConnectionString)
        {			
			Database.SetInitializer<DataContext>(new MyDbInitializer());
            Database.CommandTimeout = 90000;
        }
 
        public static DataContext Create()
        {
            return new DataContext();
        }

        public DbSet<Person> Persons { get; set; }
		public DbSet<Recommendation> Recommendations { get; set; }
		public DbSet<ReservedTransaction> ReservedTransactions { get; set; }
        public DbSet<Statement> Statements { get; set; }
    }
}