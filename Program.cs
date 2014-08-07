using Microsoft.Experimental.IO;
using System;
using System.Collections.Generic;
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
            var searchpatterns = args.Where(a => !a.StartsWith("-")).ToList();            
            var recurse = args.Where(a => a.StartsWith("-")).Any(a => a.Contains("R"));

            if (searchpatterns.Count == 0)
            {
                PrintUsage();
            }
            else
            {
                try
                {
                    foreach (var s in searchpatterns)
                    {
                        Delete(s, recurse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("** ERROR: " + ex.Message);
                }
            }
        }

        private static void Delete(string dir, bool recurse)
        {
            if (!recurse)
            {
                LongPathDirectory.Delete(dir);
            }
            else
            {
                foreach (var subdir in LongPathDirectory.EnumerateDirectories(dir))
                {
                    Delete(subdir, true);
                }

                foreach (var file in LongPathDirectory.EnumerateFiles(dir))
                {
                    LongPathFile.Delete(file);
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
