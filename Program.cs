using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace CachedFileSystemWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.Parsed)
            {
                var options = ((Parsed<Options>) result).Value;
                var root = options.RootFolder;

                using (var cachedFileSystemWatcher = new CachedFileSystemWatcher(
                    root,
                    options.FileExtension,
                    x => ProcessFiles(x)))
                {
                    cachedFileSystemWatcher.Start();
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
            }
        }

        private static void ProcessFiles(List<string> files)
        {
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                Console.WriteLine($"{file} has been created at {fileInfo.CreationTimeUtc}");
            }
        }
    }
}
