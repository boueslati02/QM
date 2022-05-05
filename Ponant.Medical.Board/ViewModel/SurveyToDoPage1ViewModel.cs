using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des interactions sur la liste des croisières à traiter
    /// </summary>
    public class SurveyToDoPage1ViewModel : BasePageViewModel
    {
        #region Attributes

        /// <summary>
        /// Nombre de passagers reçus
        /// </summary>
        private int downloadReceived;

        /// <summary>
        /// Nombre de passagers pour une croisière
        /// </summary>
        private int downloadAvailable;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la liste des croisières disponibles
        /// </summary>
        public List<AvailableCruiseItemViewModel> AvailableCruiseItems
        {
            get
            {
                using (BoardEntities db = new BoardEntities())
                {
                    List<AvailableCruiseItemViewModel> result = db.Cruise
                        .Where(c => (c.IsExtract || c.SurveyNumberAvailable > 0) && c.SailingDate >= DateTime.Today)
                        .OrderBy(c => c.SailingDate)
                        .Select(c => new AvailableCruiseItemViewModel
                    {
                        //Parent = this,  //Impossible de créer une valeur constante de type 'Ponant.Medical.Board.ViewModel.SurveyToDoPage1ViewModel'. Seuls les types primitifs ou les types énumération sont pris en charge dans ce contexte.
                        Entity = c,
                    }).ToList();
                    return result;
                }
            }
        }

        /// <summary>
        /// Retourne/Positionne le nombre questionnaires téléchargés
        /// </summary>
        public int DownloadReceived
        {
            get
            {
                return downloadReceived;
            }
            set
            {
                downloadReceived = value;
                OnPropertyChanged("DownloadReceived");
            }
        }

        /// <summary>
        /// Retourne/Positionne le nombre questionnaires disponible sur le serveur.
        /// Affiche la barre de progression si différent de zéro et la masque si zéro.
        /// </summary>
        public int DownloadAvailable
        {
            get
            {
                return downloadAvailable;
            }
            set
            {
                downloadAvailable = value;
                OnPropertyChanged("DownloadAvailable");
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mainvm">Viewmodel de la vue principale</param>
        public SurveyToDoPage1ViewModel(IMainViewModel mainvm)
        {
            MainViewModel = (MainViewModel)mainvm;
        }
        #endregion
    }
}