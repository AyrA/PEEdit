using System;
using System.Collections.Generic;
using System.IO;

namespace AyrA.IO
{

    /// <summary>
    /// File/Directroy flag for MaskMatch
    /// </summary>
    [Flags]
    public enum MatchType : int
    {
        File = 1,
        Directory = 2,
        All = File | Directory
    }

    /// <summary>
    /// Provides multi-level file and directory matching
    /// </summary>
    public static class MaskMatch
    {
        private static readonly char[] Masks = new char[] { '*', '?' };

        /// <summary>
        /// Searches the file system form atching directories and files
        /// </summary>
        /// <param name="Mask">Mask; Example: C:\te?t\*\q12???8*\*.xls*</param>
        /// <param name="Condition">Conditions</param>
        /// <returns>Found files and folders</returns>
        public static string[] Match(string Mask, MatchType Condition = MatchType.All)
        {
            List<string> Matches = new List<string>();
            string[] Patterns = getPatterns(Mask);
            for (int i = 0; i < Patterns.Length; i++)
            {
                if (i == 0)
                {
                    if ((Condition & MatchType.Directory) > 0 || i < Patterns.Length - 1)
                    {
                        Matches.AddRange(getDirectories(Patterns[i]));
                    }
                    if ((Condition & MatchType.File) > 0)
                    {
                        Matches.AddRange(getFiles(Patterns[i]));
                    }
                }
                else
                {
                    string[] temp = Matches.ToArray();
                    Matches.Clear();
                    foreach (string d in temp)
                    {
                        if ((Condition & MatchType.Directory) > 0 || i < Patterns.Length - 1)
                        {
                            Matches.AddRange(getDirectories(Path.Combine(d, Patterns[i])));
                        }
                        if ((Condition & MatchType.File) > 0)
                        {
                            Matches.AddRange(getFiles(Path.Combine(d, Patterns[i])));
                        }
                    }
                }
            }

            return Matches.ToArray();
        }

        /// <summary>
        /// Searches the file system for matching directories and files
        /// </summary>
        /// <param name="Mask">Multiple masks; Example: C:\te?t\*\q12???8*\*.xls*</param>
        /// <param name="Condition">Conditions</param>
        /// <param name="Masks">Removes duplicates from final list</param>
        /// <returns>Found files and folders</returns>
        public static string[] Match(string[] Masks, MatchType Condition = MatchType.All, bool RemoveDuplicates = true)
        {
            List<string> L = new List<string>();
            foreach (string Mask in Masks)
            {
                string[] result = Match(Mask, Condition);
                if (RemoveDuplicates)
                {
                    foreach (string entry in result)
                    {
                        if (!L.Contains(entry))
                        {
                            L.Add(entry);
                        }
                    }
                }
                else
                {
                    L.AddRange(result);
                }
            }
            return L.ToArray();
        }

        /// <summary>
        /// Splits a mask in System.IO capable parts
        /// </summary>
        /// <param name="Mask">Complete mask</param>
        /// <returns>splitted mask</returns>
        private static string[] getPatterns(string Mask)
        {
            List<string> Patterns = new List<string>();
            string[] temp = Mask.Split(Path.DirectorySeparatorChar);
            string current = temp[0] + (temp[0].Contains(":") ? Path.DirectorySeparatorChar.ToString() : string.Empty);
            for (int i = 1; i < temp.Length; i++)
            {
                if (temp[i].IndexOfAny(Masks) >= 0)
                {
                    Patterns.Add(Path.Combine(current, temp[i]));
                    current = string.Empty;
                }
                else
                {
                    current = Path.Combine(current, temp[i]);
                }
            }
            return Patterns.ToArray();
        }

        /// <summary>
        /// Searches a directroy for matchign subdirectories
        /// </summary>
        /// <param name="Mask">Mask</param>
        /// <returns>Matching directories</returns>
        private static string[] getDirectories(string Mask)
        {
            string Dir = Mask.Substring(0, Mask.LastIndexOf(Path.DirectorySeparatorChar));
            string Arg = Mask.Substring(Mask.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            try
            {
                return Directory.GetDirectories(Dir, Arg);
            }
            catch
            {
            }
            return new string[0];
        }

        /// <summary>
        /// Searches a directroy for matchign files
        /// </summary>
        /// <param name="Mask">Mask</param>
        /// <returns>Matching files</returns>
        private static string[] getFiles(string Mask)
        {
            string Dir = Mask.Substring(0, Mask.LastIndexOf(Path.DirectorySeparatorChar));
            string Arg = Mask.Substring(Mask.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            try
            {
                return Directory.GetFiles(Dir, Arg);
            }
            catch
            {
            }
            return new string[0];
        }
    }
}