namespace Ponant.Medical.Common.Tests.Mocks
{
    using Ponant.Medical.Common.Interfaces;

    public class TestArchiveHelper : IArchiveHelper
    {       
        public byte[] UnZip(string path)
        {
            byte[] tab = new byte[] { 1, 2, 3, 4 };
            return tab;
        }

        public string Zip(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}
