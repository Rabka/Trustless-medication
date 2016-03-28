using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MultiChainLib;
using TrustLessAPI.Models;

namespace TrustLessAPI.Storage
{
    public static class BlockChain
    {
        public static async void CreateNewUser(Person person)
        {
            //Make RPC connection to servernode
            string ipToNode = System.Configuration.ConfigurationManager.AppSettings["RpcServerIp"];
            MultiChainClient client = new MultiChainClient(ipToNode, 7172, false, "multichainrpc",
                "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");

            var personAddress = await client.GetNewAddressAsync();
            person.ReservedServerWalletKey = personAddress.Result;
        }


        public static async void MakeRecommendation(Recommendation recommendation)
        {
            //Make RPC connection to servernode
            string ipToNode = System.Configuration.ConfigurationManager.AppSettings["RpcServerIp"];
            MultiChainClient client = new MultiChainClient(ipToNode, 2892, false, "multichainrpc",
                "GrR32CfpW7Kzk3dRz5cx6Vbaz6FQtp5br2cHxe7QeGdr", "infoChain"); 
            Dictionary<string,int> dictionary = new Dictionary<string, int>();
            dictionary.Add("F",1);
            dictionary.Add("S",1);
            var resp = await client.PrepareLockUnspent(dictionary);
            recommendation.Transaction = new ReservedTransaction()
            {
                Tx = resp.Result.result.txid,
                Vout = resp.Result.result.vout
            };
        }


        public static async void IssueS(Person toPerson, Statement s)
        {
            //Make RPC connection to servernode
            string ipToNode = System.Configuration.ConfigurationManager.AppSettings["RpcServerIp"];
            MultiChainClient client = new MultiChainClient(ipToNode, 7172, false, "multichainrpc",
                "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");
            var permissionsKey = (await client.ListPermissions(BlockchainPermissions.Issue)).Result.First().Address;

           
            await client.SendWithMetadataFromAsync(toPerson.ReservedServerWalletKey, toPerson.PublicKey,"S", 1, BitConverter.GetBytes(s.Id));
            await client.SendWithMetadataFromAsync(toPerson.ReservedServerWalletKey, permissionsKey,"F", 1, BitConverter.GetBytes(s.Id));


        }

        //private static bool ContainsTransactionWithStatement(MultiChainClient client,Statement s)
        //{
        //    foreach (var VARIABLE in client.list)
        //    {
                
        //    }
        //}

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static async void IssueF(Person toPerson,Statement s)
        {

            //Make RPC connection to servernode
            string ipToNode = System.Configuration.ConfigurationManager.AppSettings["RpcServerIp"];
            MultiChainClient client = new MultiChainClient(ipToNode, 7172, false, "multichainrpc",
                "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");
            var permissionsKey = (await client.ListPermissions(BlockchainPermissions.Issue)).Result.First().Address;


            client.SendWithMetadataFromAsync(toPerson.ReservedServerWalletKey, toPerson.PublicKey, "F", 1, BitConverter.GetBytes(s.Id));
            client.SendWithMetadataFromAsync(toPerson.ReservedServerWalletKey, permissionsKey, "S", 1, BitConverter.GetBytes(s.Id));
            
        }

      
    }
}