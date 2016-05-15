using System;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using MySql.Data.Entity;
using System.Data;
using System.Configuration;

namespace TrustlessAPI
{
    /// <summary>
    /// Entity framework configuration class to configure/setup MySQL adapter.
    /// </summary>
	public class MyConfiguration : DbConfiguration 
	{ 
		public MyConfiguration() 
		{  
			try { 
				var dataSet = (DataSet)ConfigurationManager.GetSection("system.data"); 
				dataSet.Tables[0].Rows.Add( 
				                           "MySQL Data Provider", 
				                           ".Net Framework Data Provider for MySQL", 
				                           "MySql.Data.MySqlClient", 
				                           typeof(MySqlClientFactory).AssemblyQualifiedName 
				                           ); 
			} 
			catch (ConstraintException) 
			{ 
				// MySQL provider is already installed, just ignore the exception 
			} 

			SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices()); 
			SetDefaultConnectionFactory(new MySqlConnectionFactory()); 
		} 
	} 
}

