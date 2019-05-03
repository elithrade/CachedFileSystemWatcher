using System.IO;

namespace CachedFileSystemWatcher
{
    public static class FileInfoExtensions
    {
        public static bool IsLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }

    }
}
