using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace TrustlessClientWeb.Models
{
    public class Repository
    {
        static ConcurrentDictionary<string, NewStatement> _NewStatments = new ConcurrentDictionary<string, NewStatement>();
        static ConcurrentDictionary<string, NewStatement> _DebatableStatements = new ConcurrentDictionary<string, NewStatement>();

        public Repository()
        {
            
        }

        public NewStatement MakeNewStatment()
        {
            NewStatement item = new NewStatement();
            item.Key = Guid.NewGuid().ToString();
            _NewStatments[item.Key] = item;
            return item;
        }

        public void SendNewStatment(NewStatement item)
        {
            _NewStatments.TryGetValue(item.Key, out item);
            _NewStatments.TryRemove(item.Key, out item);
            
            //TODO send newStatment to server
        }

        public List<NewStatement> GetDebatableStatements(string key)
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

            return 0;
        }

        public int GetF()
        {
            //TODO ask client node for F'er

            return 0;
        }
    }
}