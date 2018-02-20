using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLibTests.Interfaces
{
    public interface IFileManager
    {
        string ReadContentsOf(string fileName);
        void WriteToFile(string fileName, string textToWrite);
        void WriteToFile(string fileName, byte[] bytes);
        string ConvertToFileSystemPath(string relativeWebPath);
        bool FileExists(string fileNameWithPath);
        void DeleteFile(string fileNameWithPath);
    }
}
