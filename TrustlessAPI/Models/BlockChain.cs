using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MultichainCliLib;
using MultichainCliLib.Responses;
using TrustLessAPI.Models;
using TrustLessModelLib;

namespace TrustLessAPI.Storage
{
    public static class BlockChain
	{ 
		public static void ImportPublicKeyToWallet(Person person)
		{
			//Make RPC connection to servernode 
			string chainName = System.Configuration.ConfigurationManager.AppSettings["ChainName"];
			MultiChainClient client = new MultiChainClient(chainName); 
			client.ImportAddress(person.PublicKey); 
		}

        public static void MakeRecommendation(DataContext db, Recommendation recommendation)
        {
			//Make RPC connection to servernode 
			string chainName = System.Configuration.ConfigurationManager.AppSettings["ChainName"];
			MultiChainClient client = new MultiChainClient(chainName);
            Dictionary<string,int> dictionary = new Dictionary<string, int>();
            dictionary.Add("F",1);
            dictionary.Add("S",1);
			var resp = client.PrepareLockUnspent(dictionary);

            recommendation.Transaction = new ReservedTransaction()
            {
				Tx = resp.txid,
                Vout = resp.vout
            };

			db.Recommendations.Add (recommendation);
			db.SaveChanges ();
        }


		public static void IssueS(Recommendation recommendation)
        {
			//Make RPC connection to servernode 
			Dictionary<string,object> txVouts = new Dictionary<string, object>();
			txVouts.Add("txid",recommendation.Transaction.Tx);
			txVouts.Add("vout",recommendation.Transaction.Vout);

			Dictionary<string,int> amount = new Dictionary<string, int>(); 
			amount.Add("S",1);

			string chainName = System.Configuration.ConfigurationManager.AppSettings["ChainName"];
			MultiChainClient client = new MultiChainClient(chainName);
			var resp = client.CreateRawTransaction (recommendation.Person.PublicKey, txVouts, amount);
			var respAddresses = client.GetAddresses ();
			var rootPublicKey = respAddresses.Addresses.First ();
			var respAppendRawChange = client.AppendRawChange (resp.Hex, rootPublicKey);
			var respSignTransaction = client.SignRawTransaction (respAppendRawChange.Hex);
			if (respSignTransaction.complete) {
				var respSendRawTransaction = client.SendRawTransaction (respSignTransaction.hex);
				if (!String.IsNullOrEmpty (respSendRawTransaction.TransactionId))
					client.LockUnspent (true, txVouts);
			} 

        } 

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

		public static void IssueF(Recommendation recommendation)
        {
			//Make RPC connection to servernode 
			Dictionary<string,object> txVouts = new Dictionary<string, object>();
			txVouts.Add("txid",recommendation.Transaction.Tx);
			txVouts.Add("vout",recommendation.Transaction.Vout);

			Dictionary<string,int> amount = new Dictionary<string, int>(); 
			amount.Add("F",1);

			string chainName = System.Configuration.ConfigurationManager.AppSettings["ChainName"];
			MultiChainClient client = new MultiChainClient(chainName);
			var resp = client.CreateRawTransaction (recommendation.Person.PublicKey, txVouts, amount);
			var respAddresses = client.GetAddresses ();
			var rootPublicKey = respAddresses.Addresses.First ();
			var respAppendRawChange = client.AppendRawChange (resp.Hex, rootPublicKey);
			var respSignTransaction = client.SignRawTransaction (respAppendRawChange.Hex);
			if (respSignTransaction.complete) {
				var respSendRawTransaction = client.SendRawTransaction (respSignTransaction.hex);
				if (!String.IsNullOrEmpty (respSendRawTransaction.TransactionId))
					client.LockUnspent (true, txVouts);
			} 
            
        }

		public static AddressBalance[] GetPersonBalance(Person person)
		{
			//Make RPC connection to servernode  
			string chainName = System.Configuration.ConfigurationManager.AppSettings["ChainName"];
			MultiChainClient client = new MultiChainClient(chainName);
			var resp = client.GetAddressbalances (person.PublicKey); 
			return resp.Balances;
		}
      
    }
}