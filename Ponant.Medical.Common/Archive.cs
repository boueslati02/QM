using Ponant.Medical.Common.Interfaces;
using System.IO;
using System.IO.Compression;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Classe de gestion des archives
    /// </summary>
    public class Archive : IArchiveHelper
    {
        #region Constants
        /// <summary>
        /// Extension des fichiers ZIP
        /// </summary>
        private const string EXTENSION_ZIP = ".zip";
        #endregion

        #region Public methods

        #region Zip
        /// <summary>
        /// Compresse un fichier
        /// </summary>
        /// <param name="path">Chemin d'accès au fichier à compresser</param>
        /// <returns>Chemin d'accès à l'archive</returns>
        public string Zip(string path)
        {
            string archiveFileName = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + EXTENSION_ZIP);

            if (File.Exists(archiveFileName))
            {
                File.Delete(archiveFileName);
            }

            using (ZipArchive archive = ZipFile.Open(archiveFileName, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(path, Path.GetFileName(path), CompressionLevel.Optimal);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return archiveFileName;
        }
        #endregion

        #region UnZip
        /// <summary>
        /// Décompresse une archive
        /// </summary>
        /// <param name="path">Chemin d'accès de l'archive à décompresser</param>
        /// <returns>Flux de fichier</returns>
        public byte[] UnZip(string path)
        {
            byte[] bytes = null;

            using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    Stream stream = entry.Open();
                    bytes = ReadStream(stream);
                }
            }

            return bytes;
        }
        #endregion

        #endregion

        #region Private methods
        /// <summary>
        /// Retourne un tableau de données binaires
        /// </summary>
        /// <param name="stream">Flux à convertir</param>
        /// <returns>Tableau de données binaires</returns>
        private byte[] ReadStream(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        #endregion
    }
}
