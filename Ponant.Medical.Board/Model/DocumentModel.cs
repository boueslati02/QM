using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Board.ViewModel;
using Ponant.Medical.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion des documents
    /// </summary>
    public class DocumentModel
    {
        #region Open
        /// <summary>
        /// Ouverture du document
        /// </summary>
        /// <param name="documentItem">Instance du document à ouvrir</param>
        public static void Open(DocumentItemViewModel documentItem)
        {
            string folder = documentItem.HasStatus ? App.Current.Properties["CruisesToDoFolder"].ToString() : App.Current.Properties["CurrentCruisesFolder"].ToString();
            string path = Path.Combine(folder, documentItem.FileName);

            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path));

                if (!documentItem.Id.Equals(0))
                {
                    using (BoardEntities db = new BoardEntities())
                    {
                        Document document = db.Document.Find(documentItem.Id);
                        document.IdStatus = Constants.DOCUMENT_STATUS_SEEN;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                MessageBox.Show("This file doesn't exists.", "File not found", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Unlink
        /// <summary>
        /// Détache le document du passager
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <returns></returns>
        public static async Task Unlink(int id)
        {
            try
            {
                using (BoardEntities db = new BoardEntities())
                {
                    Document document = db.Document.Find(id);

                    if (document != null)
                    {
                        document.IsToDetach = true;
                        document.ModificationDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }

                await Unlink();
            }
            catch (Exception exception)
            {
                Logger.Log("DocumentModel", "Unlink", exception);
#if DEV || INTEGRATION
                throw new Exception("DocumentModel.Unlink", exception);
#endif
            }
        }

        /// <summary>
        /// Détache les documents marqués
        /// </summary>
        /// <returns></returns>
        public static async Task Unlink()
        {
            try
            {
                using (BoardEntities db = new BoardEntities())
                {
                    foreach (Data.Document document in db.Document.Where(d => d.IsToDetach).ToList())
                    {
                        bool result = await ShoreService.Instance.UnlinkDocument(document.Id);

                        if (result)
                        {
                            string path = Path.Combine(App.Current.Properties[AppSettings.CruisesToDoFolder].ToString(), document.FileName);

                            // Suppression en base
                            db.Document.Remove(document);
                            db.SaveChanges();

                            // Suppression physique
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("DocumentModel", "Unlink", exception);
#if DEV || INTEGRATION
                throw new Exception("DocumentModel.Unlink", exception);
#endif
            }
        }
        #endregion
    }
}