using Newtonsoft.Json;
using System;
using System.Linq;

namespace BinVer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args.Any(m => m == "/?"))
            {

            }
            else
            {
                var P = new PE(@"C:\Apache24\bin\httpd.exe");
                Console.WriteLine(JsonConvert.SerializeObject(P, Formatting.Indented));
            }
#if DEBUG
            Console.ReadKey(true);
#endif
        }
    }
}
