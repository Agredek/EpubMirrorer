using System.IO;
using System.IO.Compression;

namespace VersOne.Epub.Environment.Implementation
{
    internal class FileSystem : IFileSystem
    {
#if NETFRAMEWORK
        private const int MAX_PATH = 260;
#endif

        public bool FileExists(string path)
        {
            return File.Exists(GetCompatibleFilePath(path));
        }

        public IZipFile OpenZipFile(string path)
        {
            return new ZipFile(System.IO.Compression.ZipFile.Open(path, ZipArchiveMode.Update));
        }

        public IZipFile OpenZipFile(Stream stream)
        {
            return new ZipFile(new ZipArchive(stream, ZipArchiveMode.Update));
        }

        private string GetCompatibleFilePath(string path)
        {
#if NETFRAMEWORK
            if (path.Length >= MAX_PATH && !path.StartsWith(@"\\?\"))
            {
                return @"\\?\" + path;
            }
#endif
            return path;
        }
    }
}
