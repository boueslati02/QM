using Ponant.Medical.Common.Interfaces;
using System.IO;

namespace Ponant.Medical.Common
{
    public class FileHelper : IFileHelper
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void MoveFile(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }
    }
}
