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
    /// <summary>
    /// Entity framework database context class
    /// </summary>
	[DbConfigurationType(typeof(MyConfiguration))] 
    public class DataContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataContext()
			: base(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlTest"].ConnectionString)
        {			
			Database.SetInitializer<DataContext>(new MyDbInitializer());
            Database.CommandTimeout = 90000;
        }
        
        /// <summary>
        /// Creates a new data context.
        /// </summary>
        /// <returns></returns>
        public static DataContext Create()
        {
            return new DataContext();
        }

        public DbSet<Person> Persons { get; set; }
		public DbSet<Recommendation> Recommendations { get; set; }
		public DbSet<ReservedTransaction> ReservedTransactions { get; set; }
		public DbSet<Statement> Statements { get; set; }
		public DbSet<LoginSession> LoginSessions { get; set; }
    }
}