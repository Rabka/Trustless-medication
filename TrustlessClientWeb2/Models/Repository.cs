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
        public ConcurrentDictionary<int, Statement> _DebatableStatements = new ConcurrentDictionary<int, Statement>();
        public List<Statement> _Recommendations = new List<Statement>();

        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://10.26.50.194:8080/") };

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
            

        }

        /// <summary>
        /// Gets the list of statments this user can judge
        /// </summary>
        /// <returns>A list of debatable statments</returns>
        public async Task<IEnumerable<Statement>> GetDebatableStatements()
        {
            //TODO ask server for debateble statements

            HttpResponseMessage response = await _client.GetAsync("Statement/GetStatements");

            if(response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                List<Statement> statementList = JsonConvert.DeserializeObject<List<Statement>>(stream);

                return statementList;
            }

            List<Statement> failList = new List<Statement>();
            Statement failStatement = new Statement() { Person = new Person(), MedicinOne = "No", MedicinTwo = "successefull", Description = "response" };
            failList.Add(failStatement);

            return failList;
        }

        /// <summary>
        /// Reply on a debatable statement
        /// </summary>
        /// <param name="item">the debatable statement</param>
        /// <param name="thisPerson">this client</param>
        /// <param name="reply">if the statement is true</param>
        /// <returns></returns>
        public async Task ReplyDebatableStatement(Statement item, Person thisPerson, bool reply)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(item) + "," + JsonConvert.SerializeObject(thisPerson));
            HttpResponseMessage response = await _client.PostAsync("Recommend?trusted=" + reply.ToString(), content);

            System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Hello this is an Alert\")</SCRIPT>");

            if (response.IsSuccessStatusCode)
            {
                Statement s;
                _DebatableStatements.TryRemove(item.Id, out s);
            }
            else
            {
                
                System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Hello this is an Alert\")</SCRIPT>");
                //ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert( test );", true);
            }
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