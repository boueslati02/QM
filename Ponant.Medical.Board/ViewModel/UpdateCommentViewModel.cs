using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using System;
using System.Windows;
using System.Linq;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des interactions de mise à jour du commentaire du passager
    /// </summary>
    public class UpdateCommentViewModel : BaseViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande de sauvegarde des modifications avec fermeture de la fenêtre
        /// </summary>
        private readonly DelegateCommand saveCommand;

        /// <summary>
        /// Vue associé à ce viewmodel
        /// </summary>
        private readonly PassengerItemViewModel passengerItemView;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande de sauvegarde des modifications avec fermeture de la fenêtre
        /// </summary>
        public DelegateCommand SaveCommand
        {
            get
            {
                return saveCommand;
            }
        }

        /// <summary>
        /// Retourne/Positionne le titre de la fenêtre
        /// </summary>
        public string ViewTitle { get; set; }

        /// <summary>
        /// Retourne/Positionne le commentaire Board sur un passager
        /// </summary>
        public string CommentBoard { get; set; }

        /// <summary>
        /// Retourne/Positionne le commentaire Terre sur un passager
        /// </summary>
        public string CommentsShore { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="passenger">Viewmodel du passager à mettre à jour</param>
        public UpdateCommentViewModel(PassengerItemViewModel passenger)
        {
            saveCommand = new DelegateCommand(Save);
            passengerItemView = passenger;

            if (passenger != null)
            {
                ViewTitle = String.Format("Edit comment for {0} {1}", passenger.LastName, passenger.FirstName);
                CommentBoard = passenger.CommentBoard;
                CommentsShore = passenger.CommentShore;
            }
        }
        #endregion

        #region Private 

        #region Save
        /// <summary>
        /// Sauvegarde la mise à jour de l'avis
        /// </summary>
        /// <param name="parameter">NU</param>
        private async void Save(object parameter)
        {
            if (parameter is Window modalView)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CommentBoard))
                    {
                        CommentBoard = null;
                    }

                    // Enregistrement en base
                    int nbLignesMaj = 0;
                    Passenger passenger = null;
                    using (BoardEntities db = new BoardEntities())
                    {
                        passenger = db.Passenger.Include("Information").Include("Information.Lov")
                            .SingleOrDefault(p => p.Id == passengerItemView.Id);
                        passenger.ReviewBoard = CommentBoard;
                        passenger.Editor = Application.Current.Properties[AppSettings.UserName].ToString();
                        passenger.ModificationDate = DateTime.Now;

                        nbLignesMaj = db.SaveChanges();
                    }

                    Logger.Log("Info", "CommentBoard", "Edit", string.Format("Edit board comments for {0} {1}", passengerItemView.LastName, passengerItemView.FirstName));

                    // Maj de la vue
                    if (nbLignesMaj > 0)
                    {
                        passengerItemView.Comments = passenger.PassengerFormatComments();
                        passengerItemView.CommentBoard = CommentBoard;
                        passengerItemView.CommentShore = CommentsShore;
                        passengerItemView.OnPropertyChanged("Comments");
                    }

                    // Fermeture
                    modalView.DialogResult = true;
                    modalView.Close();
                }
                catch (Exception exception)
                {
                    Logger.Log("EditCommentViewModel", "Save", exception);
#if DEV || INTEGRATION
                    throw;
#endif
                }
            }
        }
        #endregion

        #endregion
    }
}
