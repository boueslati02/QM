namespace Ponant.Medical.Common.Interfaces
{
    public interface IFileHelper
    {
        bool FileExists(string path);

        void MoveFile(string sourceFileName, string destFileName);
    }
}
