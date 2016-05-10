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
using System.Web.Configuration;
using MultichainCliLib;

namespace TrustlessClientWeb.Models
{
    public class Repository
    {
        public ConcurrentDictionary<int, Statement> _DebatableStatements = new ConcurrentDictionary<int, Statement>();
        public List<Statement> _Statements = new List<Statement>();
        public Dictionary<int, List<Recommendation>> _StatementsRecommendations = new Dictionary<int, List<Recommendation>>();
        public Person _thisPerson;
		public LoginSession _loginToken;

		private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri(WebConfigurationManager.AppSettings["ServerUrl"]) };

        public Repository()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			_loginToken = (LoginSession)HttpContext.Current.Session ["token"];
			_thisPerson = (Person)HttpContext.Current.Session ["person"];
            //_client.DefaultRequestHeaders.Add("Authorization", Repository.Token);
        }

		public void LoginAttempt(string username, string password)
		{
			string publicKey = GetPublicKey ();
			if (publicKey != "") {
				Person person = new Person () { PublicKey = publicKey, Username = username, Password = password };

				StringContent content = new StringContent (JsonConvert.SerializeObject (person));
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue ("application/json");
				HttpResponseMessage response = _client.PostAsync ("Person/Login", content).Result; 

				if (!response.IsSuccessStatusCode) {
					System.Web.HttpContext.Current.Response.Write ("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Login failed: " + response.StatusCode + ", make sure your login is correct and that your multichain node is the node associated with the account.\")</SCRIPT>");
				} else {
					var stream = response.Content.ReadAsStringAsync ().Result;
					LoginSession session = JsonConvert.DeserializeObject<LoginSession> (stream);
					_loginToken = session;
					person.LoginSession = _loginToken;
					person.LoginSessionToken = _loginToken.Token;
					_thisPerson = person; 
					HttpContext.Current.Session ["token"] = _loginToken;
					HttpContext.Current.Session ["person"] = _thisPerson;
				}
			} else {
				System.Web.HttpContext.Current.Response.Write ("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Could not contact multichain node, make sure its running.\")</SCRIPT>");
			}
		}

		private string GetPublicKey()
		{
			//Make RPC connection to servernode  
			try
			{
				string chainName = WebConfigurationManager.AppSettings["ChainName"];
				string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
				MultiChainClient client = new MultiChainClient(chainName,nodeIp);
				var resp = client.GetAddresses ();
				return resp.Addresses.First ();
			}
			catch {
				return "";
			} 
		}

        /// <summary>
        /// Send a new statment to the server, with the given peremeters.
        /// </summary>
        /// <param name="drug1">The first drug</param>
        /// <param name="drug2">The secund drug</param>
        /// <param name="description"></param>
        /// <returns></returns>
        public void SendNewStatment(string medicinOne, string medicinTwo, string description)
        {
            Statement S = new Statement() { Person = _thisPerson, MedicinOne = medicinOne, MedicinTwo = medicinTwo, Description = description };

            StringContent content = new StringContent(JsonConvert.SerializeObject(S));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( "application/json");
			HttpResponseMessage response = _client.PostAsync("Statement/CreateNewStatement", content).Result;

            if (!response.IsSuccessStatusCode)
			{
                //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Make new statement failed: " + response.StatusCode + "\")</SCRIPT>");
            }
        }

        /// <summary>
        /// When given 2 medicin, put at rekomendation in _Statements if ther is one
        /// </summary>
        /// <param name="medicinOne"></param>
        /// <param name="medicinTwo"></param>
        /// <returns></returns>
        public void GetRecommendation(string medicinOne, string medicinTwo)
        {
            HttpResponseMessage response = _client.GetAsync("Statement/SearchStatement?medicinOne=" + medicinOne + "&medicinTwo=" + medicinTwo).Result;

            if (response.IsSuccessStatusCode)
            {
				var stream =  response.Content.ReadAsStringAsync().Result;
                List<Statement> statementList = JsonConvert.DeserializeObject<List<Statement>>(stream);
                _Statements = statementList;

                if (statementList.Count == 0)
                {
                    //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"No recommendation was found.\")</SCRIPT>");
                }
                else
                {
					_StatementsRecommendations.Clear ();
                    foreach (Statement s in _Statements)
                    {
						HttpResponseMessage response2 =  _client.GetAsync("Statement/GetRecommendations?statement=" + s.Id).Result;

                        if (response.IsSuccessStatusCode)
                        {
							var stream2 =  response2.Content.ReadAsStringAsync().Result;
                            List<Recommendation> recommendationList = JsonConvert.DeserializeObject<List<Recommendation>>(stream2);
                            _StatementsRecommendations.Add(s.Id, recommendationList);
                        }
                        else
                        {
                            //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Search recommendation failed: " + response2.StatusCode + "\")</SCRIPT>");
                        }

                    }
                }
            }
            else
            {
                //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Search statement failed: " + response.StatusCode + "\")</SCRIPT>");
            }

        }

        /// <summary>
        /// Gets the list of statments this user can judge
        /// </summary>
        /// <returns>A list of debatable statments</returns>
        public IEnumerable<Statement> GetDebatableStatements()
        {
			if (_thisPerson == null) {

				List<Statement> failList = new List<Statement>();

				return failList;
			}
			//List<Statement> failList = new List<Statement>();
			//Statement failStatement = new Statement() { Person = new Person(), MedicinOne = "No", MedicinTwo = "successefull", Description = "response" };
			//failList.Add(failStatement);

			//return failList;

            //TODO ask server for debateble statements
			HttpResponseMessage response = _client.GetAsync("Statement/GetStatements?token=" + _loginToken.Token).Result;

            if(response.IsSuccessStatusCode)
            {
				var stream = response.Content.ReadAsStringAsync().Result;
				List<Statement> statementList = JsonConvert.DeserializeObject<List<Statement>>(stream, new JsonSerializerSettings{
					MissingMemberHandling = MissingMemberHandling.Ignore
				});

                return statementList;
            }
            else
            {
				HttpContext.Current.Session ["token"] = null;
				HttpContext.Current.Session ["person"]= null;
                System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Make new statement failed: " + response.StatusCode + "\")</SCRIPT>");

                List<Statement> failList = new List<Statement>();

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
        public void ReplyDebatableStatement(int itemId, bool reply, string description)
        {
            StringContent content = new StringContent(description);
            HttpResponseMessage response = _client.PostAsync(
                    "Statement/Recommend?trust=" + reply.ToString() + 
                    "&statement=" + itemId + 
				"&token=" + _loginToken.Token
				, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Statement s;
                _DebatableStatements.TryRemove(itemId, out s);
            }
            else
            {

                //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"Recommendation failed: " + response.StatusCode + "\")</SCRIPT>");

            }
        }

        public int GetS()
        {
            //TODO ask client node for S'er

			//Make RPC connection to servernode  
			string chainName = WebConfigurationManager.AppSettings["ChainName"];
			string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
			MultiChainClient client = new MultiChainClient(chainName,nodeIp);
			var resp = client.GetTotalBalances ();
			return resp.Balances.Where (x => x.name.StartsWith("S_")).Sum(y => (int)y.qty);
        }

        public int GetF()
        {
            //TODO ask client node for F'er

			//Make RPC connection to servernode  
			string chainName = WebConfigurationManager.AppSettings["ChainName"];
			string nodeIp = WebConfigurationManager.AppSettings["NodeIp"];
			MultiChainClient client = new MultiChainClient(chainName,nodeIp);
			var resp = client.GetTotalBalances ();
			return resp.Balances.Where (x => x.name.StartsWith("F_")).Sum(y => (int)y.qty);
        }
    }
}