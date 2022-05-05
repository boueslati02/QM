namespace Ponant.Medical.Common.Tests.Mocks
{
    using Ponant.Medical.Common.Interfaces;

    public class TestFileHelper : IFileHelper
    {
        public bool FileExists(string path)
        {
            return true;
        }

        public void MoveFile(string sourceFileName, string destFileName)
        { }
    }
}
