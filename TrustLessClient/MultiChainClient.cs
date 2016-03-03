using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace TrustLessClient
{
    class MultiChainClient
    {
        static void Main(string[] args)
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    await new MultiChainClient().DoMagic();
                });
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("**********************");
                Console.WriteLine(ex);
            }
            finally
            {
                if (Debugger.IsAttached)
                    Console.ReadLine();
            }
        }

        internal async Task DoMagic()
        {
            // connect to the client... til server
            var client = new MultiChainLib.MultiChainClient("130.226.133.59", 7172, false, "multichainrpc", "BvrGYKXpxyFGxxzsqnwe3qs8hSbFvRM6fB6X3bjyyEaK", "trustChain");
            
            // get some info back...
            var info = await client.GetInfoAsync();
            Console.WriteLine("Chain: {0}, difficulty: {1}", info.Result.ChainName, info.Result.Difficulty);
        }

    }
}
