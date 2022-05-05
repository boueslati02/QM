using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion des croisières
    /// </summary>
    public static class CruiseModel
    {
        #region Public methods

        #region CheckExtractCruise
        /// <summary>
        /// Vérifie si la croisière est extraite
        /// </summary>
        /// <returns></returns>
        public static async Task CheckExtractCruise()
        {
            try
            {
                using (BoardEntities db = new Data.BoardEntities())
                {
                    Data.Cruise cruise = db.Cruise.SingleOrDefault(c => c.SurveyNumberDownloaded > 0 && !c.Passenger.Any(p => p.IdStatus.Equals(Constants.BOARD_STATUS_QM_DOWNLOAD_BEFORE_CRUISE)));

                    if (cruise != null)
                    {
                        // Si la croisière n'est plus extraite sur la terre, on libère la croisière à bord
                        bool? isExtract = await ShoreService.Instance.IsExtractCruise(cruise.Id);

                        if (isExtract.HasValue && !isExtract.Value)
                        {
                            await FreeCruise(cruise.Id);
                            MessageBox.Show("The cruise was released on land. You can no longer process this cruise on board.", "Information", MessageBoxButton.OK);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "CheckExtractCruise", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion

        #region DownloadCruises
        /// <summary>
        /// Télécharge les croisières disponibles et les stockent dans la BD locale
        /// </summary>
        public static async Task DownloadCruises()
        {
            try
            {
                List<vCruiseBoard> cruises = await ShoreService.Instance.GetCruises();

                if (cruises != null)
                {
                    using (BoardEntities db = new BoardEntities())
                    {
                        foreach (vCruiseBoard vCruise in cruises)
                        {
                            Data.Cruise cruise = db.Cruise.Find(vCruise.Id);
                            bool isAdded = false;

                            if (cruise == null)
                            {
                                cruise = new Data.Cruise
                                {
                                    Id = vCruise.Id,
                                    Code = vCruise.Code,
                                    SurveyNumberDownloaded = 0,
                                    SurveyNumberDoneBoard = 0,
                                    SurveyNumberSent = 0,
                                    Creator = Application.Current.Properties[AppSettings.UserName].ToString(),
                                    CreationDate = DateTime.Now,
                                    IdShip = vCruise.IdShip
                                };

                                isAdded = true;
                            }

                            cruise.SailingDate = vCruise.SailingDate;
                            cruise.SailingLengthDays = vCruise.SailingLengthDays;
                            cruise.PassengersNumber = vCruise.NbPassenger;
                            cruise.SurveyNumberAvailable = vCruise.NbQMAvailable;
                            cruise.SurveyNumberDoneShore = vCruise.NbQMDone;
                            cruise.SurveyNumberNotAvailable = vCruise.NbQMNotAvailable;
                            cruise.SurveyNumberReceive = vCruise.NbQMReceive;
                            cruise.SurveyNumberValidate = vCruise.NbQMValidate;
                            cruise.SurveyNumberWaiting = vCruise.NbQMWaiting;
                            cruise.SurveyNumberRefused = vCruise.NbQMRefused;
                            cruise.Deadline = vCruise.Deadline;
                            cruise.IdShipAssigned = vCruise.IdShipAssigned;
                            cruise.Editor = Application.Current.Properties[AppSettings.UserName].ToString();
                            cruise.ModificationDate = DateTime.Now;
                            cruise.IdShip = vCruise.IdShip;

                            if (isAdded)
                            {
                                db.Cruise.Add(cruise);
                            }

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "DownloadCruises", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion

        #region DeleteCruises
        /// <summary>
        /// Nettoie les croisières terminées et les questionnaires
        /// </summary>
        /// <returns></returns>
        public static void DeleteCruises()
        {
            try
            {
                using (BoardEntities db = new Data.BoardEntities())
                {
                    List<Data.Cruise> cruises = db.Cruise
                        .Where(c => DbFunctions.AddDays(c.SailingDate, c.SailingLengthDays + AppSettings.DelayToDeleteCruise) <= DateTime.Today
                        && !c.IsExtract)
                        .ToList();

                    if (cruises.Any())
                    {
                        foreach (Data.Cruise cruise in cruises)
                        {
                            foreach (Data.Passenger passenger in cruise.Passenger.ToList())
                            {
                                if (passenger.Cruise.Count == 1)
                                {
                                    // Nettoyage des documents
                                    List<Data.Document> documents = db.Document.Where(d => d.IdPassenger.Equals(passenger.Id)).ToList();
                                    CleanCruiseDocuments(documents);

                                    // MAJ en base
                                    db.Document.RemoveRange(documents);
                                    db.Information.RemoveRange(db.Information.Where(i => i.IdPassenger.Equals(passenger.Id)));
                                    db.Passenger.Remove(passenger);
                                }
                            }
                            db.Cruise.Remove(cruise);
                        }

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "DeleteCruise", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion

        #region FreeCruise
        /// <summary>
        /// Libération de la croisière
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns></returns>
        public static async Task FreeCruise(
            int idCruise,
            bool isImminentCruise = false)
        {
            try
            {
                // Libération sur la terre
                await ShoreService.Instance.FreeCruise(idCruise);

                // MAJ sur la base bord
                using (BoardEntities db = new Data.BoardEntities())
                {
                    Data.Cruise cruise = db.Cruise.Find(idCruise);

                    if (!isImminentCruise)
                    {
                        foreach (Data.Passenger passenger in cruise.Passenger.ToList())
                        {
                            // Nettoyage des documents
                            List<Data.Document> documents = db.Document.Where(d => d.IdPassenger.Equals(passenger.Id)).ToList();
                            CleanCruiseDocuments(documents);

                            // MAJ en base de données
                            db.Document.RemoveRange(documents);
                            db.Information.RemoveRange(db.Information.Where(i => i.IdPassenger.Equals(passenger.Id)));
                            db.Passenger.Remove(passenger);
                        }
                    }

                    cruise.IsExtract = false;
                    cruise.SurveyNumberSent = 0;
                    if (!isImminentCruise)
                    {
                        cruise.SurveyNumberDownloaded = 0;
                    }
                    cruise.SurveyNumberDoneBoard = 0;
                    cruise.Editor = App.Current.Properties[AppSettings.UserName].ToString();
                    cruise.ModificationDate = DateTime.Now;

                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "FreeCruise", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion

        #region FreeOldCruises
        /// <summary>
        /// Libération des anciennes croisères extraites à bord et à terre
        /// </summary>
        /// <returns></returns>
        public static async Task FreeOldCruises()
        {
            try
            {
                using (BoardEntities db = new BoardEntities())
                {
                    List<Data.Cruise> cruiseList = db.Cruise
                        .Where(c => c.IsExtract && c.SailingDate < DateTime.Today)
                        .ToList();

                    if (cruiseList.Any())
                    {
                        foreach (Data.Cruise cruise in cruiseList)
                        {
                            await FreeCruise(cruise.Id);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "FreeOldCruises", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion

        #endregion

        #region CleanCruiseDocuments
        /// <summary>
        /// Nettoie les documents médicaux de la croisière
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns>Liste des documents à supprimer</returns>
        private static void CleanCruiseDocuments(List<Data.Document> documents)
        {
            try
            {
                if (documents != null)
                {
                    foreach (Data.Document document in documents)
                    {
                        string path = Path.Combine(App.Current.Properties[AppSettings.CruisesToDoFolder].ToString(), document.FileName);

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("CruiseModel", "CleanCruiseDocuments", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }
        }
        #endregion
    }
}