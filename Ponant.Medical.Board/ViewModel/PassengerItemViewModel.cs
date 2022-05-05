using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.View;
using System;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions pour chaque passager des croisière récentes et imminentes du bateau (ligne du datagrid)
    /// </summary>
    public class PassengerItemViewModel : BaseViewModel, IPassengerDocumentView
    {
        #region Attributes

        /// <summary>
        /// Commande de visualisation des documents associés au passager
        /// </summary>
        private readonly DelegateCommand documentsCommand;

        /// <summary>
        /// Commande d'édition du commentaire associé au passager
        /// </summary>
        private readonly DelegateCommand editCommentCommand;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande de visualisation des documents associés au passager
        /// </summary>
        public DelegateCommand ShowDocumentsListCommand
        {
            get
            {
                return documentsCommand;
            }
        }

        /// <summary>
        /// Retourne la commande d'edition du commentaire associés au passager
        /// </summary>
        public DelegateCommand EditCommentCommand
        {
            get
            {
                return editCommentCommand;
            }
        }

        /// <summary>
        /// Retourne/Positionne l'Id du passager
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de la croisière
        /// </summary>
        public int IdCruise { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom du passager
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom usuel du passager
        /// </summary>
        public string UsualName { get; set; }

        /// <summary>
        /// Retourne/Positionne le prénom du passager
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Retourne/Positionne l'adresse electronique du passager
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Retourne/Positionne l'avis sur le passager
        /// </summary>
        public string Advice { get; set; }

        /// <summary>
        /// Retourne/Positionne le commentaire Board sur le passager
        /// </summary>
        public string CommentBoard { get; set; }

        /// <summary>
        /// Retourne/Positionne le commentaire Terre sur le passager
        /// </summary>
        public string CommentShore { get; set; }

        /// <summary>
        /// Retourne/Positionne les commentaires sur le passager
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Retourne si le passager a au moins 1 document
        /// </summary>
        public bool HasDocument { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public PassengerItemViewModel()
        {
            documentsCommand = new DelegateCommand(ShowDocumentsList);
            editCommentCommand = new DelegateCommand(EditComment);
        }
        #endregion

        #region Private

        /// <summary>
        /// Execute la commande de visualisation des documents associés au passager
        /// </summary>
        /// <param name="parameter">Viewmodel du passager sélectionné</param>
        private static void ShowDocumentsList(object parameter)
        {
            try
            {
                PassengerItemViewModel passenger = parameter as PassengerItemViewModel;
                DocumentsView view = new DocumentsView(new DocumentsViewModel(passenger, false))
                {
                    Owner = App.Current.MainWindow
                };
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerItemViewModel", "ShowDocumentsList", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerItemViewModel.ShowDocumentsList", exception);
#endif
            }
        }

        /// <summary>
        /// Execute la commande d'édition du commentaire associé au passager
        /// </summary>
        /// <param name="parameter">Viewmodel du passager sélectionné</param>
        private static void EditComment(object parameter)
        {
            try
            {
                PassengerItemViewModel passenger = parameter as PassengerItemViewModel;
                UpdateCommentView view = new UpdateCommentView(new UpdateCommentViewModel(passenger))
                {
                    Owner = App.Current.MainWindow
                };
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerItemViewModel", "EditComment", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerItemViewModel.EditComment", exception);
#endif
            }
        }

        #endregion
    }
}
