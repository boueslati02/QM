namespace Ponant.Medical.Shore.Helpers
{
    using Ponant.Medical.Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Classe de gestion des fichiers
    /// </summary>
    public static class FileManager
    {
        #region FileGet
        /// <summary>
        /// Récupération du chemin d'un fichier physique
        /// </summary>
        /// <param name="path">Chemin du fichier</param>
        /// <param name="filename">Nom du fichier</param>
        /// <returns>Le chemin du fichier si celui-ci existe, null sinon</returns>
        public static string FileGetPath(string path, string filename, bool withReplace = false)
        {
            string currentPath = null;
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
            {
                if(withReplace)
                {
                    filename = ReplaceCharFilename(filename);
                }

                currentPath = Path.Combine(path, filename);
                if (!File.Exists(currentPath))
                {
                    currentPath = null;
                }
            }
            return currentPath;
        }
        #endregion

        #region FileSave
        /// <summary>
        /// Enregistrement d'un fichier physique sans compression
        /// </summary>
        /// <param name="path">Chemin du fichier</param>
        /// <param name="filename">Nom du fichier</param>
        /// <param name="file">Fichier à enregistré</param>
        /// <param name="forceReplace">Indique si le fichier peut remplacer un fichier existant</param>
        public static void FileSave(string path, string filename, HttpPostedFileBase file, bool forceReplace = false,bool isPassengerUpload = false)
        {
            try
            {
                if ((file != null) && (file.ContentLength > 0))
                {
                    if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
                    {
                        string currentPath = Path.Combine(path, ReplaceCharFilename(filename));
                        if (!File.Exists(currentPath) || (File.Exists(currentPath) && forceReplace) || isPassengerUpload)
                        {
                            Directory.CreateDirectory(path);
                            file.SaveAs(currentPath);

                            LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.File, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add File Path : " + path + " Name : " + filename);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.File, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add File Path : " + path + " Name : " + filename + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region FileMove
        /// <summary>
        /// Déplace un fichier physique
        /// </summary>
        /// <param name="sourcePath">Chemin du fichier source</param>
        /// <param name="sourceFilename">Nom du fichier source</param>
        /// <param name="destinationPath">Chemin du fichier destination</param>
        /// <param name="destinationFilename">Nom du fichier destination</param>
        /// <returns>Nouveau nom du fichier</returns>
        public static string FileMove(string sourcePath, string sourceFilename, string destinationPath, string destinationFilename, bool withReplace = false, bool withZipFile = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath) && !string.IsNullOrEmpty(sourceFilename) && !string.IsNullOrEmpty(destinationFilename))
                {
                    string currentSourcePath = Path.Combine(sourcePath, withReplace ? ReplaceCharFilename(sourceFilename) : sourceFilename);
                    string currentDestinantionPath = Path.Combine(destinationPath, destinationFilename);
                    if(File.Exists(currentSourcePath))
                    {
                        if(File.Exists(currentDestinantionPath))
                        {
                            File.Delete(currentDestinantionPath);
                        }
                        Directory.CreateDirectory(destinationPath);
                        File.Move(currentSourcePath, currentDestinantionPath);

                        if (withZipFile && File.Exists(currentDestinantionPath))
                        {
                            Archive archive = new Archive();
                            destinationPath = archive.Zip(currentDestinantionPath);
                            string[] splitPath = destinationPath.Split('\\');
                            destinationFilename = splitPath[splitPath.Length - 1];
                        }

                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.File, LogManager.LogAction.Move, HttpContext.Current.User.Identity.Name, "Move File Path : " + destinationPath + " Name : " + destinationFilename);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.File, LogManager.LogAction.Move, HttpContext.Current.User.Identity.Name, "Move File Path : " + sourcePath + " Name : " + sourceFilename + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
            return destinationFilename;
        }
        #endregion

        #region FileDelete
        /// <summary>
        /// Suppression d'un fichier physique
        /// </summary>
        /// <param name="path">Chemin du fichier</param>
        /// <param name="filename">Nom du fichier</param>
        public static void FileDelete(string path, string filename, bool withReplace = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
                {
                    string currentPath = Path.Combine(path, withReplace ? ReplaceCharFilename(filename) : filename);
                    if (File.Exists(currentPath))
                    {
                        File.Delete(currentPath);
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.File, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete File Path : " + path + " Name : " + filename);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.File, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete File Path : " + path + " Name : " + filename + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region FileManyDelete
        /// <summary>
        /// Suppression de plusieurs fichiers physiques
        /// </summary>
        /// <param name="directoryPath">Chemin des fichiers</param>
        /// <param name="patternContent">Modéle contenu dans le nom des fichiers a supprimer</param>
        public static void FileManyDelete(string directoryPath, string patternContent)
        {
            try
            {
                if (!string.IsNullOrEmpty(directoryPath)
                    && !string.IsNullOrEmpty(patternContent)
                    && Directory.Exists(directoryPath))
                {
                    List<string> filenames = (from filename in Directory.GetFiles(directoryPath) where filename.Contains(patternContent) select filename).ToList();
                    foreach (string filename in filenames)
                    {
                        FileDelete(directoryPath, filename.Replace('-', '_'));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.File, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete File Path : " + directoryPath + " With pattern : " + patternContent + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region DirectoryDelete
        /// <summary>
        /// Suppression d'un dossier physique
        /// </summary>
        /// <param name="path">Chemin du fichier</param>
        /// <param name="recursive">Indicateur de suppression recursive</param>
        public static void DirectoryDelete(string currentPath, bool recursive = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    int nbFiles = int.MaxValue;
                    if (!recursive)
                    {
                        nbFiles = Directory.GetFiles(currentPath).Count();
                    }

                    if (nbFiles == 0 || recursive)
                    {
                        Directory.Delete(currentPath, recursive);
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Directory, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Directory Path : " + currentPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Directory, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Directory Path : " + currentPath + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region ReplaceCharFilename
        /// <summary>
        /// Remplace les caractère perdu lors de l'upload de fichier
        /// </summary>
        /// <param name="filenme">Nom du fichier à traité</param>
        /// <returns>Nom du fichier traité</returns>
        public static string ReplaceCharFilename(string filenme)
        {
            return Regex.Replace(filenme, "[^0-9a-zA-Z éèà_ .]+", "_");
        }
        #endregion
    }
}