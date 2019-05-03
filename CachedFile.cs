namespace CachedFileSystemWatcher
{
    internal sealed class CachedFile
    {
        public string Path { get; set; }
        public int RetryCount { get; set; }
    }
}
