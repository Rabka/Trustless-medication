using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;

namespace TrustlessClientWeb.Models
{
    public class Repository
    {
        static ConcurrentDictionary<string, NewStatement> _DebatableStatements = new ConcurrentDictionary<string, NewStatement>();

        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://wattosshop.azurewebsites.net/") };

        public Repository()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //_client.DefaultRequestHeaders.Add("Authorization", Repository.Token);
        }

        public void SendNewStatment(string drug1, string drug2, string description)
        {
            
            //TODO send newStatment to server
        }

        public List<NewStatement> GetDebatableStatements()
        {
            //TODO ask server for debateble statements

            return new List<NewStatement>();
        }

        public void ReplyDebatableStatement(NewStatement item, bool reply)
        {
            //TODO reply on a debateble statement
        }

        public int GetS()
        {
            //TODO ask client node for S'er

            var client = new MultiChainLib.MultiChainClient("130.226.133.59", 7172, false, "multichainrpc", "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");

            // get some info back...
            var info = await client.GetInfoAsync();
            Console.WriteLine("Chain: {0}, difficulty: {1}", info.Result.ChainName, info.Result.Difficulty);

            return 0;
        }

        public int GetF()
        {
            //TODO ask client node for F'er

            return 0;
        }
    }
}