//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace demo
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            // The code provided will print ‘Hello World’ to the console.
//            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
//            Console.WriteLine("Hello World!");
//            Console.ReadKey();

//            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
//        }
//    }
//}

using System;
using Glean;

namespace demo
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");

      Glean.Glean.Initialize(
        "fxrpc",
        "0.1.1",
        true,
        new Glean.Configuration(
        Glean.Configuration.DEFAULT_TELEMETRY_ENDPOINT, null, null, Glean.Configuration.DEFAULT_MAX_EVENTS, true,
        "fxrpc-test-tag"),
        "data"
      );
    }
  }
}

