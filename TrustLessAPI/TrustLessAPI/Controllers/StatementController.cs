using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using TrustLessAPI.Models;
using TrustLessAPI.Storage;

namespace TrustLessAPI.Controllers
{
    [Authorize]
    public class StatementController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            using (DataContext context = new DataContext())
            {
                return Request.CreateResponse<IQueryable<Statement>>(HttpStatusCode.OK, context.Statements.ToList().AsQueryable());
            }
        }

        [AllowAnonymous]
        [Route("Search")]
        [HttpPost]
        public HttpResponseMessage SearchStatement(string medicinOne, string medicinTwo)
        {
            using (DataContext context = new DataContext())
            {
                var match =
                    context.Statements.FirstOrDefault(x => x.MedicinOne == medicinOne && x.MedicinTwo == medicinTwo);
                if (match == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                 
                return Request.CreateResponse<Statement>(HttpStatusCode.OK, match);
            }
        }

        [Route("CreateStatement")]
        [HttpPost]
        public HttpResponseMessage CreateNewStatement([FromBody] Statement statement )
        {
            using (DataContext context = new DataContext())
            {
                var match =
                    context.Statements.FirstOrDefault(x => x.MedicinOne == statement.MedicinOne && x.MedicinTwo == statement.MedicinTwo);

                //Does a statement already exists with the same two medicins.
                if (match != null) // && IsStatementValid(context,match)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                //Security: Don't allow user to specify the person who created the statement. Set it to the current user logged in.
                statement.Person =
                    context.Persons.FirstOrDefault(x => x.Username == RequestContext.Principal.Identity.GetUserName());

                //Rule: refuse a user with zero trust from creating statements.
                if (CalculateBayesianModelTrust(statement.Person) == 0)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                if (statement.Person == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                context.Statements.Add(statement);
                context.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            
        }


        [HttpPost]
        public HttpResponseMessage Recommend([FromBody] Statement statement, Person person,bool trusted)
        {
            if (statement == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            using (DataContext context = new DataContext())
            {
                var match =
                    context.Statements.FirstOrDefault(x => x.MedicinOne == statement.MedicinOne && x.MedicinTwo == statement.MedicinTwo);
                if ((match != null && IsStatementComplete(context, match)) || match == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                //Security: Don't allow user to specify the person who created the statement. Set it to the current user logged in.
                statement.Person =
                    context.Persons.FirstOrDefault(x => x.Username == RequestContext.Principal.Identity.GetUserName());
    
                if (statement.Person == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                if (statement.Person.Username == person.Username || context.Recommendations.Any(x => x.PersonId == person.Id))
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    
                context.Recommendations.Add(new Recommendation{IsRecommended = trusted, PersonId = person.Id, StatementId = statement.Id});
                context.SaveChanges();

                VerifyStatementClosure(context,statement);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        private void VerifyStatementClosure(DataContext context, Statement statement)
        {
            if (IsStatementComplete(context, statement))
            {
                if (IsStatementValid(context, statement))
                {
                    //Issue S
                    BlockChain.IssueS(statement.Person,statement);
                }
                else
                {
                    //Issue F
                    BlockChain.IssueF(statement.Person, statement);
                } 
            }
        }

        private bool IsStatementValid(DataContext context,Statement statement)
        {
            return context.Recommendations.Count(x => x.IsRecommended && x.StatementId == statement.Id) > (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVotings"]) / 2);
        }

        private bool IsStatementComplete(DataContext context, Statement statement)
        {
            return context.Recommendations.Count(x => x.StatementId == statement.Id) == Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVotings"]);
        }

        private double CalculateBayesianModelTrust(Person person)
        {
            double initialTrust = 0.2f;
            double trustProblabilityOfGoodPerson = 0.8f;
            double nonTrustProblabilityOfBadPerson = 1.5f;

            double[] sfFloats = GetSVFromBlockChain(person);

            return (initialTrust*Math.Pow(trustProblabilityOfGoodPerson, sfFloats[0])*
                    Math.Pow(1 - trustProblabilityOfGoodPerson, sfFloats[1]))/
                   (initialTrust*Math.Pow(trustProblabilityOfGoodPerson, sfFloats[0])*
                    Math.Pow(1 - trustProblabilityOfGoodPerson, sfFloats[1]) +
                    (1 - initialTrust)*Math.Pow(nonTrustProblabilityOfBadPerson, sfFloats[0])*
                    Math.Pow(1 - nonTrustProblabilityOfBadPerson, sfFloats[1]));
        }

        private double[] GetSVFromBlockChain(Person person)
        {
            return new double[] {0, 0};
        }

    }
}
