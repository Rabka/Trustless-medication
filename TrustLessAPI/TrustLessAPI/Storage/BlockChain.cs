using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustLessAPI.Models;

namespace TrustLessAPI.Storage
{
    public static class BlockChain
    {
        static readonly object LockChain = new object();
        public static void IssueS(Person toPerson,Statement s)
        {
            lock (LockChain)
            {
                //Make RPC connection to servernode
                //Check for existing transaction with statement.
                //If exist, do nothing.
                //If not exist, issue s.
            }
        }
        public static void IssueF(Person toPerson,Statement s)
        {
            lock (LockChain)
            {
                //Make RPC connection to servernode
                //Check for existing transaction with statement.
                //If exist, do nothing.
                //If not exist, issue f.
            }
        }
    }
}