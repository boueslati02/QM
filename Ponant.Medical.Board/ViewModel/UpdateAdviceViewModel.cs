using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.Model;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des interactions de mise à jour de l'avis du médecin
    /// </summary>
    public class UpdateAdviceViewModel : BaseViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande de sauvegarde des modifications avec fermeture de la fenêtre
        /// </summary>
        private readonly DelegateCommand saveCommand;

        /// <summary>
        /// Vue associé à ce viewmodel
        /// </summary>
        private readonly IPassengerDocumentView passengerDocumentView;

        /// <summary>
        /// Instance de la page mère
        /// </summary>
        private readonly SurveyToDoPage2ViewModel surveyToDoPage2;

        /// <summary>
        /// Viewmodel de l'avis sélectionné
        /// </summary>
        private AdviceItemViewModel selectedAdviceItem;

        /// <summary>
        /// Message d'erreur 
        /// </summary>
        private string status;

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
        /// Retourne/Positionne la liste des différents avis
        /// </summary>
        public ObservableCollection<AdviceItemViewModel> AdviceItems { get; private set; }

        /// <summary>
        /// Retourne/Positionne le viewmodel l'avis sélectionné
        /// </summary>
        public AdviceItemViewModel SelectedAdviceItem
        {
            get
            {
                return selectedAdviceItem;
            }
            set
            {
                selectedAdviceItem = value;
                OnPropertyChanged("IsDocumentsTypesVisible");
            }
        }

        /// <summary>
        /// Retourne/Positionne les commentaires sur un passager
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Retourne l'état d'affichage des types de documents (si l'avis sélectionné comporte des types de documents)
        /// </summary>
        public bool IsDocumentsTypesVisible
        {
            get
            {
                if(SelectedAdviceItem != null 
                    && SelectedAdviceItem.DocumentTypeItemsView != null 
                    && !SelectedAdviceItem.DocumentTypeItemsView.IsEmpty)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Retourne/Positionne le statut d'erreur de la mise à jour
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="passenger">Viewmodel du passager à mettre à jour</param>
        public UpdateAdviceViewModel(IPassengerDocumentView passenger, SurveyToDoPage2ViewModel page)
        {
            saveCommand = new DelegateCommand(Save, CanSave);
            passengerDocumentView = passenger;
            surveyToDoPage2 = page;

            if (passenger != null)
            {
                ViewTitle = String.Format("Update advice for {0} {1}", passenger.LastName, passenger.FirstName);
            }

            CreateAdviceItems();
        }
        #endregion

        #region Private

        #region CreateAdviceItems
        /// <summary>
        /// Création des items Advice avec leurs type de documents associés
        /// </summary>
        private void CreateAdviceItems()
        {
            AdviceItems = new ObservableCollection<AdviceItemViewModel>();
            AdviceItemViewModel adviceItem;
            List<DocumentTypeItemViewModel> documentTypeItems;

            using (BoardEntities db = new BoardEntities())
            {
                List<Lov> adviceList = db.Lov.Where(p => p.IdLovType.Equals(Constants.LOV_ADVICE)).ToList();
                foreach (Lov l in adviceList)
                {
                    adviceItem = new AdviceItemViewModel()
                    {
                        Id = l.Id,
                        Name = l.Name
                    };

                    switch (l.Id)
                    {
                        case Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS:
                            documentTypeItems = db.Lov.Where(p =>
                                    p.IdLovType.Equals(Constants.LOV_RESTRICTION_ADVICE) ||
                                    p.IdLovType.Equals(Constants.LOV_RESTRICTION_PERSON)
                                ).Select(c => new DocumentTypeItemViewModel
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Type = c.LovType.Name
                                }).ToList();
                            break;

                        case Constants.ADVICE_UNFAVORABLE_OPINION:
                            documentTypeItems = db.Lov.Where(p =>
                                    p.IdLovType.Equals(Constants.LOV_UNFAVORABLE_ADVICE)
                                ).Select(c => new DocumentTypeItemViewModel
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                }).ToList();
                            break;

                        case Constants.ADVICE_WAITING_FOR_CLARIFICATION:
                            documentTypeItems = db.Lov.Where(p =>
                                    p.IdLovType.Equals(Constants.LOV_ADDITIONAL_DOCUMENTS)
                                ).Select(c => new DocumentTypeItemViewModel
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                }).ToList();
                            break;

                        default:
                            documentTypeItems = null;
                            break;
                    }
                    if (documentTypeItems != null && documentTypeItems.Count > 0)
                    {
                        adviceItem.DocumentTypeItemsView = CollectionViewSource.GetDefaultView(documentTypeItems);
                        adviceItem.DocumentTypeItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                    }
                    AdviceItems.Add(adviceItem);
                }
            }
        }
        #endregion

        #region Save
        /// <summary>
        /// Sauvegarde la mise à jour de l'avis
        /// </summary>
        /// <param name="parameter">NU</param>
        private async void Save(object parameter)
        {
            Window view = parameter as Window;
            IEnumerable<DocumentTypeItemViewModel> checkedDocumentsTypes = null;
            
            if (view != null)
            {
                Status = null;

                try
                {
                    // Vérification des champs obligatoires
                    if (SelectedAdviceItem.Id != Constants.ADVICE_FAVORABLE_OPINION)
                    {
                        checkedDocumentsTypes = ((IList<DocumentTypeItemViewModel>)SelectedAdviceItem.DocumentTypeItemsView.SourceCollection).Where(d => d.IsChecked);

                        if (checkedDocumentsTypes.Count() == 0)
                        {
                            Status = "Type of documents required";
                            return;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Comments))
                    {
                        Status = "Comments required";
                        return;
                    }

                    // Enregistrement en base
                    using (BoardEntities db = new BoardEntities())
                    {
                        Passenger passenger = db.Passenger.Find(passengerDocumentView.Id);
                        passenger.IdAdvice = SelectedAdviceItem.Id;
                        passenger.IdStatus = Constants.BOARD_STATUS_QM_DONE;
                        passenger.Review = Comments;
                        passenger.Editor = Application.Current.Properties[AppSettings.UserName].ToString();
                        passenger.ModificationDate = DateTime.Now;

                        Cruise cruise = db.Cruise.Find(passengerDocumentView.IdCruise);
                        cruise.SurveyNumberDoneBoard = cruise.SurveyNumberDoneBoard + 1;
                        cruise.SurveyNumberAvailable = cruise.SurveyNumberAvailable - 1;

                        if (checkedDocumentsTypes != null)
                        {
                            foreach (DocumentTypeItemViewModel document in checkedDocumentsTypes)
                            {
                                passenger.Information.Add(new Information
                                {
                                    IdPassenger = passenger.Id,
                                    IdInformation = document.Id,
                                    Creator = Application.Current.Properties[AppSettings.UserName].ToString(),
                                    CreationDate = DateTime.Now,
                                    Editor = Application.Current.Properties[AppSettings.UserName].ToString(),
                                    ModificationDate = DateTime.Now
                                });
                            }
                        }

                        db.SaveChanges();
                    }

                    // Log
                    Logger.Log("Info", "Advice", "Insert", string.Format("{0} for {1} {2}",
                        SelectedAdviceItem.Name, passengerDocumentView.LastName, passengerDocumentView.FirstName));

                    // Fermeture
                    view.DialogResult = true;
                    view.Close();

                    // Enregistrement sur terre
                    bool? isExtract = await ShoreService.Instance.IsExtractCruise(passengerDocumentView.IdCruise);

                    if (isExtract.HasValue && isExtract.Value)
                    {
                        await PassengerModel.EditAdvice(new Medical.Data.Shore.AdviceBoard
                        {
                            Comments = Comments,
                            IdAdvice = SelectedAdviceItem.Id,
                            IdPassenger = passengerDocumentView.Id,
                            IdCruise = passengerDocumentView.IdCruise,
                            Informations = checkedDocumentsTypes == null 
                                ? null 
                                : checkedDocumentsTypes.Select(d => d.Id).ToList()
                        });

                        // MAJ des indicateurs
                        surveyToDoPage2.RefreshIndicators();
                        surveyToDoPage2.OnPropertyChanged("SurveyBoardCount");
                        surveyToDoPage2.OnPropertyChanged("SurveyProcessedCount");
                        surveyToDoPage2.OnPropertyChanged("SurveySentCount");
                    }

                    using (BoardEntities db = new BoardEntities())
                    {
                        // Si tous les passagers de la croisière ont été traités, on libère la croisière
                        if (!isExtract.Value || !db.Cruise.Any(c => c.Id.Equals(passengerDocumentView.IdCruise) && c.Passenger.Any(p => p.IdStatus.Equals(Constants.BOARD_STATUS_QM_TO_DO))))
                        {
                            await CruiseModel.FreeCruise(passengerDocumentView.IdCruise);
                            surveyToDoPage2.MainViewModel.ActivePage = surveyToDoPage2.ReturnPage;

                            SurveyToDoPage1ViewModel surveyToDoPage1 = surveyToDoPage2.ReturnPage as SurveyToDoPage1ViewModel;
                            surveyToDoPage1.OnPropertyChanged("AvailableCruiseItems");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Status = exception.Message;
                    Logger.Log("UpdateAdviceViewModel", "Save", exception);
#if DEV || INTEGRATION
                    throw new Exception("UpdateAdviceViewModel.Save", exception);
#endif
                }
            }
        }
        #endregion

        #region CanSave
        /// <summary>
        /// Autorise l'action sur le bouton de sauvegrde
        /// </summary>
        /// <param name="parameter">NU</param>
        private bool CanSave(object parameter)
        {
            return SelectedAdviceItem != null;
        }
        #endregion

        #endregion
    }
}