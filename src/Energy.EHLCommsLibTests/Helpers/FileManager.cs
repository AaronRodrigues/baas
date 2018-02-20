using System.IO;
using Energy.EHLCommsLibTests.Interfaces;

namespace Energy.EHLCommsLibTests.Helpers
{
    public class FileManager : IFileManager
    {
        public string ReadContentsOf(string fileName)
        {
            var readContentsOf = File.ReadAllText(fileName);
            return readContentsOf;
        }

        public void WriteToFile(string fileName, string textToWrite)
        {
            File.WriteAllText(fileName, textToWrite);
        }

        public void WriteToFile(string fileName, byte[] bytes)
        {
            File.WriteAllBytes(fileName, bytes);
        }

        public string ConvertToFileSystemPath(string relativeWebPath)
        {
            return relativeWebPath.Replace('/', '_').Trim('_').Replace(":", string.Empty);
        }

        public bool FileExists(string fileNameWithPath)
        {
            return File.Exists(fileNameWithPath);
        }

        public void DeleteFile(string fileNameWithPath)
        {
            File.Delete(fileNameWithPath);
        }
    }
}
