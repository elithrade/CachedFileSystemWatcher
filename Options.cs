
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace CachedFileSystemWatcher
{
    public sealed class Options
    {
        [Option('r', "RootFolder", Required = true, HelpText = "Root folder to monitor for new files")]
        public string RootFolder { get; set; }

        [Option('e', "FileExtension", Required = true, HelpText = "Root folder to monitor for new files")]
        public string FileExtension { get; set; }

        [Usage(ApplicationAlias = "CachedFileSystemWatcher.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example( "Monitors the specified root folders for new jpg files", new Options {RootFolder = @"/home/user", FileExtension = @"*.jpg"});
            }
        }
    }
}
