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
        public List<Statement> _Statements = new List<Statement>();
        public Dictionary<int, List<Recommendation>> _StatementsRecommendations = new Dictionary<int, List<Recommendation>>();
        public Person _thisPerson;

        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://10.26.50.194:8080/") };

        public Repository()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //_client.DefaultRequestHeaders.Add("Authorization", Repository.Token);

            _thisPerson = new Person { Username = "Patr0805"};
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
            Statement S = new Statement() { Person = _thisPerson, MedicinOne = medicinOne, MedicinTwo = medicinTwo, Description = description };

            StringContent content = new StringContent(JsonConvert.SerializeObject(S));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( "application/json");
            HttpResponseMessage response = await _client.PostAsync("Statement/CreateNewStatement", content);

            if (!response.IsSuccessStatusCode)
            {
                System.Web.HttpContext.Current.Response.Write(
                    "<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Make new statement failed: " + response.StatusCode + "\")</SCRIPT>");
            }
        }

        public async Task GetRecommendation(string medicinOne, string medicinTwo)
        {
            HttpResponseMessage response = await _client.GetAsync("Statement/SearchStatement?medicinOne=" + medicinOne + "&medicinTwo=" + medicinTwo);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                List<Statement> statementList = JsonConvert.DeserializeObject<List<Statement>>(stream);
                _Statements = statementList;

                if (statementList.Count == 0)
                {
                    System.Web.HttpContext.Current.Response.Write(
                        "<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"No recommendation was found.\")</SCRIPT>");
                }
                else
                {
                    foreach (Statement s in _Statements)
                    {
                        HttpResponseMessage response2 = await _client.GetAsync("Statement/GetRecommendations?statement=" + s.Id);

                        if (response.IsSuccessStatusCode)
                        {
                            var stream2 = await response2.Content.ReadAsStringAsync();
                            List<Recommendation> recommendationList = JsonConvert.DeserializeObject<List<Recommendation>>(stream2);
                            _StatementsRecommendations.Add(s.Id, recommendationList);
                        }
                        else
                        {
                            System.Web.HttpContext.Current.Response.Write(
                                "<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Search recommendation failed: " + response2.StatusCode + "\")</SCRIPT>");
                        }

                    }
                }
            }
            else
            {
                System.Web.HttpContext.Current.Response.Write(
                    "<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Search statement failed: " + response.StatusCode + "\")</SCRIPT>");
            }

        }

        /// <summary>
        /// Gets the list of statments this user can judge
        /// </summary>
        /// <returns>A list of debatable statments</returns>
        public async Task<IEnumerable<Statement>> GetDebatableStatements()
        {
            //TODO ask server for debateble statements

            HttpResponseMessage response = await _client.GetAsync("Statement/GetStatements?username=" + _thisPerson.Username);

            if(response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                List<Statement> statementList = JsonConvert.DeserializeObject<List<Statement>>(stream);

                return statementList;
            }
            else
            {
                System.Web.HttpContext.Current.Response.Write(
                    "<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Make new statement failed: " + response.StatusCode + "\")</SCRIPT>");

                List<Statement> failList = new List<Statement>();
                Statement failStatement = new Statement() { Person = new Person(), MedicinOne = "No", MedicinTwo = "successefull", Description = "response" };
                failList.Add(failStatement);

                return failList;
            }
        }

        /// <summary>
        /// Reply on a debatable statement
        /// </summary>
        /// <param name="item">the debatable statement</param>
        /// <param name="thisPerson">this client</param>
        /// <param name="reply">if the statement is true</param>
        /// <returns></returns>
        public async Task ReplyDebatableStatement(int itemId, bool reply, string description)
        {
            StringContent content = new StringContent(description);
            HttpResponseMessage response = await _client.PostAsync(
                    "Statement/Recommend?trust=" + reply.ToString() + 
                    "&statement=" + itemId + 
                    "&username=" + _thisPerson.Username
                    , content);

            if (response.IsSuccessStatusCode)
            {
                Statement s;
                _DebatableStatements.TryRemove(itemId, out s);
            }
            else
            {

                System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Recommendation failed: " + response.StatusCode + "\")</SCRIPT>");
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