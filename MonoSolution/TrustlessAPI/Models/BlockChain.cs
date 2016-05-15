using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MultichainCliLib;
using TrustLessAPI.Models;
using TrustLessModelLib;
using System.Web.Configuration;
using MultichainCliLib.Responses;

namespace TrustLessAPI.Storage
{
    public static class BlockChain
	{ 
		public static Object issueLock = new object();
		public static void ImportPublicKeyToWallet(Person person)
		{
			//Make RPC connection to servernode 
			string chainName = WebConfigurationManager.AppSettings["ChainName"];
			string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
			MultiChainClient client = new MultiChainClient(chainName,nodeIp); 
			client.ImportAddress(person.PublicKey); 
		}

        public static void MakeRecommendation(DataContext db, TrustLessModelLib.Recommendation recommendation)
        {
			//Make RPC connection to servernode 
			string chainName = WebConfigurationManager.AppSettings["ChainName"];
			string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
			MultiChainClient client = new MultiChainClient(chainName,nodeIp);
			Dictionary<string,int> dictionary = new Dictionary<string, int>();
			AddressBalance[] addressBalances = client.GetTotalBalances ().Balances;		
			var lastFasset = addressBalances.OrderBy (x => x.name).LastOrDefault (x => x.name.StartsWith ("F_")); 
			var lastSasset = addressBalances.OrderBy (x => x.name).LastOrDefault (x => x.name.StartsWith ("S_")); 
			if (lastFasset == null || lastSasset == null) {
				GenerateFunds (client);
				MakeRecommendation (db, recommendation);
				return;
			} 
			string lastF = lastFasset.name;
			string lastS = lastSasset.name;


			dictionary.Add(lastF,1);
			dictionary.Add(lastS,1);
			try
			{
				var resp = client.PrepareLockUnspent(dictionary);

				recommendation.Transaction = new ReservedTransaction()
				{
					Tx = resp.txid,
					Vout = resp.vout
				};
			}
			catch {
				//Insurficient funds 
				GenerateFunds(client);
				MakeRecommendation (db, recommendation);
				return;
			} 


			db.SaveChanges ();
        }

		private static void GenerateFunds(MultiChainClient client)
		{
			lock (issueLock) {
				AddressBalance[] addressBalances = client.GetTotalBalances ().Balances;		
				ListAssetsResponseAsset[] assets = client.ListAssets ().Assets; 	
					string rootKey = WebConfigurationManager.AppSettings["MultichainPublicKey"];
					ListAssetsResponseAsset lastSasset = assets.OrderBy (x => x.name).LastOrDefault (x => x.name.StartsWith ("S_"));
					string lastS = lastSasset != null ? lastSasset.name : "S_0";
					ListAssetsResponseAsset lastFasset = assets.OrderBy (x => x.name).LastOrDefault (x => x.name.StartsWith ("F_"));
					string lastF = lastSasset != null ? lastSasset.name : "F_0";
					string newS = "S_" + (Convert.ToInt32 (lastS.Substring (2)) + 1);
					string newF = "F_" + (Convert.ToInt32 (lastS.Substring (2)) + 1);
				var balanceS = addressBalances.FirstOrDefault (x => x.name == lastS);
				var balanceF = addressBalances.FirstOrDefault (x => x.name == lastF);

				if (balanceS == null || balanceS.qty == 0)
					client.Issue (rootKey, newS, 100000, 1.0);
				if (balanceF == null || balanceF.qty == 0)
					client.Issue (rootKey, newF, 100000, 1.0);
			}
		}


		public static void IssueS(TrustLessModelLib.Recommendation recommendation)
        {
			
			//Make RPC connection to servernode 
				string chainName = WebConfigurationManager.AppSettings["ChainName"];
				string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
				MultiChainClient client = new MultiChainClient(chainName,nodeIp);

				GetTxOutResponse getTxOutResponse = client.GetTxOut(recommendation.Transaction.Tx,recommendation.Transaction.Vout);
			if (getTxOutResponse == null)
				return;

			Dictionary<string,int> amount = new Dictionary<string, int>();  
				amount.Add(getTxOutResponse.assets.First(x => x.name.StartsWith("S_")).name,1);
				  
				string multichainPublicKey = WebConfigurationManager.AppSettings["MultichainPublicKey"]; 
			var resp = client.CreateRawTransaction (recommendation.Person.PublicKey, recommendation.Transaction.Tx,recommendation.Transaction.Vout, amount);
				var respAppendRawChange = client.AppendRawChange (resp.Hex, multichainPublicKey);
			var respSignTransaction = client.SignRawTransaction (respAppendRawChange.Hex);
			if (respSignTransaction.complete) {
				var respSendRawTransaction = client.SendRawTransaction (respSignTransaction.hex);
				if (String.IsNullOrEmpty (respSendRawTransaction.TransactionId))
					client.LockUnspent (true,  recommendation.Transaction.Tx,recommendation.Transaction.Vout);
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
			try
			{
				//Make RPC connection to servernode 
				string chainName = WebConfigurationManager.AppSettings["ChainName"];
				string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
				MultiChainClient client = new MultiChainClient(chainName,nodeIp);

				GetTxOutResponse getTxOutResponse = client.GetTxOut(recommendation.Transaction.Tx,recommendation.Transaction.Vout);


				Dictionary<string,int> amount = new Dictionary<string, int>();  
				amount.Add(getTxOutResponse.assets.First(x => x.name.StartsWith("F_")).name,1);

				string multichainPublicKey = WebConfigurationManager.AppSettings["MultichainPublicKey"]; 
				var resp = client.CreateRawTransaction (recommendation.Person.PublicKey, recommendation.Transaction.Tx,recommendation.Transaction.Vout, amount);
				var respAppendRawChange = client.AppendRawChange (resp.Hex, multichainPublicKey);
				var respSignTransaction = client.SignRawTransaction (respAppendRawChange.Hex);
				if (respSignTransaction.complete) {
					var respSendRawTransaction = client.SendRawTransaction (respSignTransaction.hex);
					if (String.IsNullOrEmpty (respSendRawTransaction.TransactionId))
						client.LockUnspent (true,  recommendation.Transaction.Tx,recommendation.Transaction.Vout);
				} 
			}
			catch (Exception ex) {
			}
        }

		public static AddressBalance[] GetPersonBalance(Person person)
		{
			//Make RPC connection to servernode  
			string chainName = WebConfigurationManager.AppSettings["ChainName"];
			string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
			MultiChainClient client = new MultiChainClient(chainName,nodeIp);
			var resp = client.GetAddressbalances (person.PublicKey); 
			return resp.Balances;
		}
      
    }
}