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
            Console.WriteLine("test 1");
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
            // connect to the client...
            var client = new MultiChainLib.MultiChainClient("127.0.0.1", 4756, false, "multichainrpc", "8XZtakGxSY8GceRNJragzPHR3hUW2Jf5zQk4zUyjUgky", "trustChain");

            // get some info back...
            var info = await client.GetInfoAsync();
            Console.WriteLine("Chain: {0}, difficulty: {1}", info.Result.ChainName, info.Result.Difficulty);
        }

    }
}
