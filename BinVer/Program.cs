using System;

namespace BinVer
{
    class Program
    {
        static void Main(string[] args)
        {
            var P = new PE(@"C:\Tools\Apps\gifsicle-1.88-win64\gifsicle.exe");
            Console.Write("Is64={0}", P.MachineType.HasFlag(PEMachineType.IMAGE_FILE_MACHINE_AMD64));
            foreach(var Section in P.Sections)
            {
                Console.WriteLine(Section.Name);
            }
            Console.ReadKey(true);
        }
    }
}
