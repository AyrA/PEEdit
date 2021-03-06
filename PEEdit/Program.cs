﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PEEdit
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            if (args == null || args.Length == 0)
            {
                args = new string[]
                {
                    "/F",
                    @"C:\Apache24\bin\httpd.exe"
                };
            }
#endif
            if (args == null || args.Length == 0 || args.Any(m => m == "/?"))
            {
                ShowHelp();
            }
            else
            {
                var Format = args.Any(m => m.ToUpper() == "/F");
                var UseArray = args.Any(m => m.ToUpper() == "/A");
                args = args
                    .Where(m => m.ToUpper() != "/F" && m.ToUpper() != "/A")
                    .SelectMany(m => ParseFileArguments(m))
                    .ToArray();

                if (args.Length == 1 && !UseArray)
                {
                    try
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(new PE(args[0]), Format ? Formatting.Indented : Formatting.None));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Unable to parse {args[0]} as PE file. Error: {ex.Message}");
                        Console.WriteLine("null");
                    }
                }
                else
                {
                    var Info = new Dictionary<string, PE>();
                    for (var i = 0; i < args.Length; i++)
                    {
                        try
                        {
                            Info.Add(args[i], new PE(args[i]));
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Unable to parse {args[i]} as PE file. Error: {ex.Message}");
                            Info.Add(args[i], null);
                        }
                    }
                    Console.WriteLine(JsonConvert.SerializeObject(Info, Format ? Formatting.Indented : Formatting.None));
                }
            }
#if DEBUG
            Console.ReadKey(true);
#endif
        }

        public static string[] ParseFileArguments(string Arg)
        {
            List<string> FileNames = new List<string>();
            if (!string.IsNullOrEmpty(Arg))
            {
                if (Arg.Any(m => m == '?' || m == '*'))
                {
                    FileNames.AddRange(AyrA.IO.MaskMatch.Match(Path.Combine(Environment.CurrentDirectory, Arg), AyrA.IO.MatchType.File));
                }
                else
                {
                    FileNames.Add(Path.GetFullPath(Arg));
                }

            }
            return FileNames.ToArray();
        }

        private static void ShowHelp()
        {
            Console.Error.WriteLine(@"{0} [/A] [/F] <File> [...]
Reads PE header and dumps it as JSON to the console

/A     - Output JSON object with a single entry even when only one file was specified
/F     - Format output, otherwise it will be a single line.
File   - File to analyze. You can supply multiple files at once and use wildcards
         For non-existing files or problems during the analyze, it will add 'null'
         as entry for that file. The output will be a JSON dictionary with full
         file names as key and the PE info as data.",
         Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName));
        }
    }
}
