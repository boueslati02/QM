using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Viewmodel associé à la vue des documents passager
    /// </summary>
    public class DocumentsViewModel : BaseViewModel
    {
        #region Attributes

        private readonly IPassengerDocumentView passengerDocumentView;

        private readonly bool statusColumnVisible;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne/Positionne le titre de la fenêtre
        /// </summary>
        public string ViewTitle { get; set;}
    
        /// <summary>
        /// Retourne/Positionne l'état de visibilité de la colonne Status
        /// </summary>
        public bool IsStatusColumnVisible { get { return statusColumnVisible; } }

        /// <summary>
        /// Retourne la liste des documents associés au passager
        /// </summary>
        public List<DocumentItemViewModel> DocumentItems
        {
            get
            {
                using (BoardEntities db = new BoardEntities())
                {
                    if (IsStatusColumnVisible)
                    {
                        return db.Document.Include("LovStatus")
                            .Where(d => d.IdPassenger.Equals(passengerDocumentView.Id))
                            .Select(d => new DocumentItemViewModel
                            {
                                Id = d.Id,
                                Name = d.Name,
                                FileName = d.FileName,
                                Date = d.CreationDate,
                                Status = d.Lov.Name
                            }).ToList();
                    }
                    else
                    {
                        return db.Document
                            .Where(d => d.IdPassenger.Equals(passengerDocumentView.Id))
                            .Select(d => new DocumentItemViewModel
                            {
                                Name = d.Name,
                                FileName = d.FileName,
                                Date = d.CreationDate,
                            }).ToList();
                    }
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="passenger">Viewmodel du passager selectionné</param>
        /// <param name="statuscolumnvisible">affichage de la colonne Status si true</param>
        public DocumentsViewModel(IPassengerDocumentView passenger, bool statuscolumnvisible)
        {
            passengerDocumentView = passenger;
            statusColumnVisible = statuscolumnvisible;

            if (passenger != null)
            {
                ViewTitle = String.Format("List of documents by {0} {1}", passenger.LastName, passenger.FirstName);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ferme la fenetre de la liste des documents passager
        /// </summary>
        /// <param name="parameter">objet window de la fenetre à fermer</param>
        //private void Close(object parameter)
        //{
        //    Window view = parameter as Window;
        //    if (view != null)
        //    {
        //        view.DialogResult = true;
        //        view.Close();
        //    }
        //}
        #endregion
    }
}
