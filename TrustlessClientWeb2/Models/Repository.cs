using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using TrustLessModelLib;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace TrustlessClientWeb2.Models
{
    public class Repository
    {
        static ConcurrentDictionary<string, Statement> _DebatableStatements = new ConcurrentDictionary<string, Statement>();

        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://wattosshop.azurewebsites.net/") };

        public Repository()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //_client.DefaultRequestHeaders.Add("Authorization", Repository.Token);
        }

        /// <summary>
        /// Send a new statment to the server, with the given peremeters.
        /// </summary>
        /// <param name="drug1">The first drug</param>
        /// <param name="drug2">The secund drug</param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task SendNewStatment(string medicinOne, string medicinTwo, string description)
        {
            Person p = new Person();
            Statement S = new Statement() { Person = p, MedicinOne = medicinOne, MedicinTwo = medicinTwo, Description = description };

            StringContent content = new StringContent(JsonConvert.SerializeObject(S));
            HttpResponseMessage response = await _client.PostAsync("CreateStatement", content);

            //TODO give response
        }

        public async Task<List<Statement>> GetDebatableStatements()
        {
            //TODO ask server for debateble statements

            HttpResponseMessage response = await _client.GetAsync("");

            if(response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                List<Statement> statementList = (List<Statement>) formatter.Deserialize(stream);

                return statementList;
            }

            return new List<Statement>();
        }

        public void ReplyDebatableStatement(Statement item, bool reply)
        {
            //TODO reply on a debateble statement
        }

        public int GetS()
        {
            //TODO ask client node for S'er

            //var client = new MultiChainLib.MultiChainClient("130.226.133.59", 7172, false, "multichainrpc", "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");

            // get some info back...
            //var info = await client.GetInfoAsync();
            //Console.WriteLine("Chain: {0}, difficulty: {1}", info.Result.ChainName, info.Result.Difficulty);

            return 0;
        }

        public int GetF()
        {
            //TODO ask client node for F'er

            return 0;
        }
    }
}