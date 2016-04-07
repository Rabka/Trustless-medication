using System;
using System.Data.Entity;

namespace TrustlessAPI
{
	public class MyDbInitializer : CreateDatabaseIfNotExists<DbContext>
	{
		protected override void Seed(DbContext context)
		{
			// create 3 students to seed the database
	  		base.Seed(context);
		}
	}
}

