using Microsoft.Experimental.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace rm2
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RemoveDirectory(string lpPathName);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var dirs = args.Where(a => !a.StartsWith("-")).ToList();            
            var recurse = args.Where(a => a.StartsWith("-")).Any(a => a.Contains("R"));

            if (dirs.Count == 0)
            {
                PrintUsage();
                Environment.Exit(2);
            }
            else
            {
                try
                {
                    foreach (var d in dirs)
                    {
                        var watch = new Stopwatch();
                        int counter = 0;
                        Console.WriteLine("deleting{0} \"{1}\"", recurse ? " recurse" : string.Empty, LongPathCommon.NormalizeLongPath(d));
                        watch.Start();

                        Delete(d, recurse, ref counter);
                        Console.WriteLine(".");
                        
                        Console.WriteLine("elapsed time: {0}", watch.Elapsed);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("** ERROR: " + ex.Message);
                    Environment.Exit(1);
                }
            }
            Environment.Exit(0);
        }

        private static void Delete(string dir, bool recurse, ref int counter)
        {
            if (!recurse)
            {
                LongPathDirectory.Delete(dir);
            }
            else
            {
                foreach (var subdir in LongPathDirectory.EnumerateDirectories(dir))
                {
                    Delete(subdir, true, ref counter);
                }

                foreach (var file in LongPathDirectory.EnumerateFiles(dir))
                {
                    LongPathFile.Delete(file);
                    if (++counter % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }

                LongPathDirectory.Delete(dir);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: rm2 [options] directory [directory directory ...]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -R Recurse");
        }
    }
}
