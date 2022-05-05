using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Data;
using System.Collections.Generic;
using System.Linq;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions avec les filtres de recherche
    /// </summary>
    public class CruiseItemViewModel : BaseViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande de recherche
        /// </summary>
        private readonly DelegateCommand searchCommand;

        /// <summary>
        /// Liste des items passagers
        /// </summary>
        private List<PassengerItemViewModel> passengersList;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande de recherche
        /// </summary>
        public DelegateCommand SearchCommand
        {
            get
            {
                return searchCommand;
            }
        }

        /// <summary>
        /// Retourne/Positionne l'entité de la croisière
        /// </summary>
        public Cruise Entity { get; set; }

        /// <summary>
        /// Retourne l'Id de la croisière
        /// </summary>
        public int CruiseId { get { return Entity.Id; } }

        /// <summary>
        /// Retourne le code de la croisière
        /// </summary>
        public string CruiseCode { get { return Entity.Code; } }

        /// <summary>
        /// Retourne/Positionne le nom du passager à rechercher
        /// </summary>
        public string SearchPassenger { get; set; }

        /// <summary>
        /// Retourne/Positionne la liste des différents avis
        /// </summary>
        public List<Lov> SearchAdviceItems { get; private set; }

        /// <summary>
        /// Retourne/Positionne l'Id de l'entité Lov sélectionnée
        /// </summary>
        public int SelectedLovId { get; set; }

        /// <summary>
        /// Retourne/Positionne la liste des items passagers
        /// </summary>
        public List<PassengerItemViewModel> PassengerItems
        {
            get
            {
                if(passengersList == null)
                {
                    Search(null);
                }
                return passengersList;
            }
            private set
            {
                passengersList = value;
                OnPropertyChanged("PassengerItems");
            }
        }

        public int PassengersNumber
        {
            get { return Entity.PassengersNumber; }
        }

        public int SurveyNumberReceive
        {
            get { return Entity.SurveyNumberReceive; }
        }

        public int SurveyNumberValidate
        {
            get { return Entity.SurveyNumberValidate; }
        }

        public int SurveyNumberWaiting
        {
            get { return Entity.SurveyNumberWaiting; }
        }

        public int SurveyNumberRefused
        {
            get { return Entity.SurveyNumberRefused; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public CruiseItemViewModel()
        {
            searchCommand = new DelegateCommand(Search);
            CreateSearchAdviceItems();
        }
        #endregion

        #region Private

        /// <summary>
        /// Crée la liste des différents avis
        /// </summary>
        private void CreateSearchAdviceItems()
        {
            using (BoardEntities db = new BoardEntities())
            {
                SearchAdviceItems = new List<Lov>()
                {
                    new Lov
                    {
                        Name = "All"
                    }
                };

                SearchAdviceItems.AddRange(db.Lov
                    .Where(p => p.IdLovType.Equals(Constants.LOV_ADVICE))
                    .ToList());
            }
        }

        /// <summary>
        /// Affichage de la liste des passagers + Execute la commande de recherche passager
        /// </summary>
        /// <param name="parameter">NU</param>
        private void Search(object parameter)
        {
            using (BoardEntities db = new BoardEntities())
            {
                this.PassengerItems = db.Passenger.Include("LovAdvice")
                    .Where(p =>
                    (
                        p.Cruise.Any(c => c.Id.Equals(CruiseId)) &&
                        p.IdAdvice.Equals(SelectedLovId == 0 ? p.IdAdvice : SelectedLovId) &&
                        (p.LastName.Contains(string.IsNullOrEmpty(SearchPassenger) ? p.LastName : SearchPassenger) ||
                        p.UsualName.Contains(string.IsNullOrEmpty(SearchPassenger) ? p.UsualName : SearchPassenger) ||
                        p.FirstName.Contains(string.IsNullOrEmpty(SearchPassenger) ? p.FirstName : SearchPassenger))
                    )).AsEnumerable()
                    .Select(p => new PassengerItemViewModel
                    {
                        Id = p.Id,
                        LastName = p.LastName,
                        UsualName = p.UsualName,
                        Email = p.Email,
                        FirstName = p.FirstName,
                        Advice = (p.IdAdvice == 0)
                            ? string.Empty
                            : p.LovAdvice.Name,
                        CommentBoard = p.ReviewBoard,
                        CommentShore = p.Review,
                        Comments = p.PassengerFormatComments(),
                        HasDocument = (p.Document.Count > 0)
                    }).ToList();
            }
        }

       
        #endregion
    }
}
