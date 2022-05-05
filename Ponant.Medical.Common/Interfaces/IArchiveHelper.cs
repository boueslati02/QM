namespace Ponant.Medical.Common.Interfaces
{
    public interface IArchiveHelper
    {
        string Zip(string path);
        byte[] UnZip(string path);
    }
}
