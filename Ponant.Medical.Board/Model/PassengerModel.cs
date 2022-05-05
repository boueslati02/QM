using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion des passagers
    /// </summary>
    public class PassengerModel
    {
        #region DownloadPassengers
        /// <summary>
        /// Récupère la liste des identifiants des passagers de la croisière
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idStatus">Identifiant du statut du QM</param>
        /// <param name="forCurrentCruises">Indique que la récupération des passagers se fait pour les croisières à venir</param>
        /// <returns>Liste des identifiants des passagers de la croisière</returns>
        public static async Task<List<PassengerCruise>> DownloadPassengers(
            int? idCruise, 
            int? idStatus, 
            bool forCurrentCruises,
            bool updateShorePassengerStatus)
        {
            List<PassengerCruise> list = null;

            try
            {
                if (idCruise.HasValue) // Récupére la liste des passager d'une cruise
                {
                    list = await ShoreService.Instance.GetPassengersByCruise(idCruise.Value, idStatus, forCurrentCruises);
                }
                else // Récupére la liste des passagers des croisières imminentes
                {
                    using (BoardEntities db = new Data.BoardEntities())
                    {
                        // Récupère les passagers pour les croisières à venir
                        if (Application.Current.Properties[AppSettings.IdShip] == null)
                        {
                            throw new ArgumentException("No boat is assigned to the current user. Please contact the administrator");
                        }

                        IEnumerable<Data.Cruise> cruises = GetImminentCruise(db);                    
                        foreach (Data.Cruise cruise in cruises)
                        {
                            List<PassengerCruise> passengers = await DownloadPassengers(cruise.Id, null, true, updateShorePassengerStatus);
                            if (passengers != null)
                            {
                                foreach (PassengerCruise passengerCruise in passengers)
                                {
                                    await DownloadPassenger(passengerCruise.IdPassenger, passengerCruise.IdCruise, Constants.BOARD_STATUS_QM_DOWNLOAD_BEFORE_CRUISE, updateShorePassengerStatus);
                                }
                            }
                        }

                        try // Récupère la liste des identifiants des passagers restants à télécharger
                        {
                            Data.Cruise cruiseRemaining = db.Cruise.SingleOrDefault(c => c.IsExtract);
                            if (cruiseRemaining != null)
                            {
                                list = await DownloadPassengers(cruiseRemaining.Id, Constants.SHORE_STATUS_QM_RECEIVED, false, updateShorePassengerStatus);
                            }
                        }
                        catch (SocketException)
                        {
                            string message = "\n\nWhat do you want to do ?" +
                                "\nPress 'Yes' button to retry download" +
                                "\nPress 'No' button to stop download and retain downloaded data" +
                                "\nPress 'Cancel' button to abandon download. Downloaded data will be deleted";
                            MessageBoxResult mbr = MessageBox.Show(message, "Download Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                            switch (mbr)
                            {
                                case MessageBoxResult.Yes:      // Retry
                                    return await DownloadPassengers(idCruise, Constants.SHORE_STATUS_QM_RECEIVED, false, updateShorePassengerStatus);
                                case MessageBoxResult.No:       // Stop
                                    return new List<PassengerCruise>();
                                case MessageBoxResult.Cancel:   // Abandon
                                    return null;
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }
            catch(Exception exception)
            {
                Logger.Log("PassengerModel", "DownloadPassengers", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerModel.DownloadPassengers", exception);
#endif             
            }
            return list;
        }
        #endregion

        #region DownloadPassenger
        /// <summary>
        /// Télécharge un passager et ses documents. 
        /// </summary>
        /// <remarks>Les passagers sont stockés dans la BD locale et les documents sont stockés sur le disque.</remarks>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idStatus">Identifiant du statut du QM</param>
        public static async Task DownloadPassenger(
            int idPassenger, 
            int idCruise, 
            int idStatus,
            bool updateShorePassengerStatus)
        {
            try
            {
                using (BoardEntities db = new BoardEntities())
                {
                    vPassengerBoard vPassenger = await ShoreService.Instance.GetPassenger(idPassenger, idCruise);
                    Data.Passenger passenger = db.Passenger.Find(idPassenger);
                    Data.Cruise cruise = db.Cruise.Find(vPassenger.IdCruise);
                    bool isAdded = false;

                    if (passenger == null)
                    {
                        passenger = new Data.Passenger
                        {
                            Id = vPassenger.Id,
                            Creator = Application.Current.Properties[AppSettings.UserName].ToString(),
                            CreationDate = DateTime.Now
                        };
                        isAdded = true;
                    }

                    db.Document.RemoveRange(passenger.Document);
                    db.Information.RemoveRange(passenger.Information);

                    passenger.Cruise.Clear();
                    passenger.Cruise.Add(cruise);
                    passenger.IdStatus = idStatus;
                    passenger.LastName = vPassenger.LastName;
                    passenger.UsualName = vPassenger.UsualName;
                    passenger.FirstName = vPassenger.FirstName;
                    passenger.Email = vPassenger.Email;
                    passenger.IdAdvice = vPassenger.IdAdvice;
                    passenger.Review = vPassenger.Review;
                    passenger.QmReceiptDate = vPassenger.ReceiptDate;
                    passenger.IsDownloaded = vPassenger.IsDownloaded;
                    passenger.Editor = Application.Current.Properties[AppSettings.UserName].ToString();
                    passenger.ModificationDate = DateTime.Now;

                    if (vPassenger.Informations != null)
                    {
                        foreach (int idInformation in vPassenger.Informations)
                        {
                            passenger.Information.Add(new Data.Information
                            {
                                IdInformation = idInformation,
                                IdPassenger = vPassenger.Id,
                                Creator = Application.Current.Properties[AppSettings.UserName].ToString(),
                                CreationDate = DateTime.Now,
                                Editor = Application.Current.Properties[AppSettings.UserName].ToString(),
                                ModificationDate = DateTime.Now
                            });
                        }
                    }

                    if (vPassenger.Documents != null)
                    {
                        string path = Path.Combine(idStatus.Equals(Constants.BOARD_STATUS_QM_TO_DO) 
                            ? App.Current.Properties[AppSettings.CruisesToDoFolder].ToString() 
                            : App.Current.Properties[AppSettings.CurrentCruisesFolder].ToString(), vPassenger.Id + ".zip");

                        // télécharge le fichier compressé des documents du passager
                        using (FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            fs.Write(vPassenger.Documents, 0, vPassenger.Documents.Length);
                        }

                        // décompresse l'archive des documents du passager
                        using (ZipArchive archive = ZipFile.OpenRead(path))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                passenger.Document.Add(new Data.Document
                                {
                                    Id = vPassenger.DocumentsInfos.Single(di => di.Value.Equals(entry.Name)).Key,
                                    FileName = entry.Name,
                                    Name = (entry.Name.IndexOf("_") > 0)
                                        ? entry.Name.Substring(entry.Name.IndexOf("_") + 1) 
                                        : entry.Name,
                                    IdStatus = Constants.DOCUMENT_STATUS_NOT_SEEN,
                                    CreationDate = DateTime.Now,
                                    Creator = Application.Current.Properties[AppSettings.UserName].ToString(),
                                    ModificationDate = DateTime.Now,
                                    Editor = Application.Current.Properties[AppSettings.UserName].ToString()
                                });

                                entry.ExtractToFile(Path.Combine(Path.GetDirectoryName(path), entry.Name), true);
                            }
                        }

                        File.Delete(path); // supprime l'archive des documents
                    }

                    if (isAdded)
                    {
                        // Met à jour les compteurs de la croisière
                        cruise.SurveyNumberDownloaded = cruise.SurveyNumberDownloaded + 1;
                        db.Passenger.Add(passenger);
                    }

                    db.SaveChanges();

                    // Extraction du passager
                    if (idStatus.Equals(Constants.BOARD_STATUS_QM_TO_DO))
                    {
                        await ShoreService.Instance.ExtractPassenger(idPassenger, idCruise);
                    }

                    if (updateShorePassengerStatus)
                    {
                        await ShoreService.Instance.ChangeStatusPassenger(idPassenger);
                    } 
                }
            }
            catch (Exception exception)
            {
                 Logger.Log("PassengerModel", "DownloadPassenger", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerModel.DownloadPassenger", exception);
#endif
            }
        }
        #endregion

        #region EditAdvice
        /// <summary>
        /// Envoi l'avis du médecin à terre
        /// </summary>
        /// <param name="advice">Avis du médecin</param>
        /// <returns></returns>
        public static async Task EditAdvice(AdviceBoard advice)
        {
            try
            {
                bool result = await ShoreService.Instance.EditAdvice(advice);

                if (result)
                {
                    using (BoardEntities db = new BoardEntities())
                    {
                        // Si l'avis est acquitté, on met à jour en base
                        Data.Passenger passenger = db.Passenger.Find(advice.IdPassenger);

                        if (passenger != null)
                        {
                            foreach (Data.Cruise cruise in passenger.Cruise)
                            {
                                cruise.SurveyNumberSent = cruise.SurveyNumberSent + 1;
                            }

                            passenger.IdStatus = Constants.BOARD_STATUS_QM_ACQUITTED;

                            // Suppression des documents
                            foreach (Data.Document document in passenger.Document.ToList())
                            {
                                string path = Path.Combine(App.Current.Properties[AppSettings.CruisesToDoFolder].ToString(), document.FileName);

                                if (File.Exists(path))
                                {
                                    File.Delete(path);
                                }

                                db.Document.Remove(document);
                            }

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerModel", "EditAdvice", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerModel.EditAdvice", exception);
#endif
            }
        }
        #endregion

        #region SendAdvices
        /// <summary>
        /// Envoi les avis non envoyés vers la terre
        /// </summary>
        /// <returns></returns>
        public static async Task SendAdvices()
        {
            try
            {
                using (BoardEntities db = new BoardEntities())
                {
                    List<Data.Passenger> passengers = db.Passenger.Where(p => p.IdStatus.Equals(Constants.BOARD_STATUS_QM_DONE)).ToList();

                    if (passengers != null)
                    {
                        foreach (Data.Passenger passenger in passengers)
                        {
                            foreach (Data.Cruise cruise in passenger.Cruise)
                            {
                                await EditAdvice(new AdviceBoard
                                {
                                    Comments = passenger.Review,
                                    IdAdvice = passenger.IdAdvice,
                                    IdPassenger = passenger.Id,
                                    IdCruise = cruise.Id,
                                    Informations = passenger.Information.Select(i => i.IdInformation).ToList()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerModel", "SendAdvice", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerModel.SendAdvice", exception);
#endif
            }
        }
        #endregion

        #region GetImminentCruise
        /// <summary>
        /// Récupere la liste des croisière imminentes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Data.Cruise> GetImminentCruise(BoardEntities db)
        {
            string usersIdShip = Application.Current.Properties[AppSettings.IdShip].ToString();

            IEnumerable<Data.Cruise> cruisesList = db.Cruise
                .Where(c => DbFunctions.TruncateTime(c.SailingDate) >= DbFunctions.AddDays(DateTime.Today, -c.SailingLengthDays)
                    && c.IdShip.ToString().Equals(usersIdShip))
                .OrderBy(c => c.SailingDate)
                .Take(5);
            return cruisesList;
        }
        #endregion
    }
}