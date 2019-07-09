using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Net;

namespace Cinevo.Azure.Functions
{
    class Program
    {


        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...");
                Program p = new Program();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

    }
}
