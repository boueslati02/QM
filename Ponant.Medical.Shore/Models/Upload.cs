using MsgReader.Outlook;
using Ponant.Medical.Common;
using Ponant.Medical.Common.MailServer;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using Ponant.Medical.Shore.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Ponant.Medical.Shore.Models
{
    #region UploadViewModel
    public class UploadViewModel
    {
        public string Token { get; set; }

        public int IdPassenger { get; set; }

        public string Civility { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int IdLanguage { get; set; }

        public string CruiseName { get; set; }

        public DateTime SaillingDate { get; set; }

        public string AgencyLogo { get; set; }

        public string AgencyName { get; set; }

        public int IdStatus { get; set; }

        [Display(Name = "MedicalSurvey", ResourceType = typeof(ResourcesViews.Index))]
        public HttpPostedFileBase MedicalSurvey { get; set; }

        [Display(Name = "RelatedDocuments", ResourceType = typeof(ResourcesViews.Index))]
        public HttpPostedFileBase RelatedDocuments1 { get; set; }

        public HttpPostedFileBase RelatedDocuments2 { get; set; }

        public HttpPostedFileBase RelatedDocuments3 { get; set; }

        public HttpPostedFileBase RelatedDocuments4 { get; set; }

        public HttpPostedFileBase RelatedDocuments5 { get; set; }
    }
    #endregion

    #region Gestion de l'envoi du QM par le passager
    public class UploadClass : SharedClass
    {
        #region Properties & Constructors

        #region Constants
        private const string _MedicalSurvey = "_MedicalSurveyFile";
        private const string _RelatedDocuments1 = "_RelatedDocuments1";
        private const string _RelatedDocuments2 = "_RelatedDocuments2";
        private const string _RelatedDocuments3 = "_RelatedDocuments3";
        private const string _RelatedDocuments4 = "_RelatedDocuments4";
        private const string _RelatedDocuments5 = "_RelatedDocuments5";
        #endregion

        private PassengerDocumentClass _PassengerDocumentClass;
        private UserClass _userClass;

        public UploadClass(IShoreEntities shoreEntities) : base(shoreEntities)
        {
            _PassengerDocumentClass = new PassengerDocumentClass(shoreEntities);
            ApplicationDbContext db = DependencyResolver.Current.GetService<ApplicationDbContext>();
            _userClass = new UserClass(db);
        }
        #endregion

        #region AddPassenger
        public UploadViewModel GetPassenger(string token)
        {
            UploadViewModel model = new UploadViewModel();
            string DecodeToken = StringHelper.ToBase64Decode(token);
            Passenger passenger = _shoreEntities.Passenger.Where(p => p.Token.Equals(DecodeToken)).FirstOrDefault();
            if (passenger != null)
            {
                BookingCruisePassenger bookingCruisePassenger = passenger.BookingCruisePassenger.FirstOrDefault();
                if (bookingCruisePassenger != null)
                {
                    string cruiseCode = bookingCruisePassenger.Cruise.Code;
                    int bookingNumber = bookingCruisePassenger.Booking.Number;
                    string groupName = bookingCruisePassenger.Booking.GroupName;
                    AgencyAccessRight agencyAccessRight = _shoreEntities.AgencyAccessRight.Where(aar => aar.CruiseCode.Equals(cruiseCode))
                                                                    .Where(aar => aar.BookingNumber.Equals(bookingNumber))
                                                                    .Where(aar => aar.GroupName.Equals(groupName))
                                                                    .FirstOrDefault();
                    model.IdPassenger = passenger.Id;
                    model.Civility = GetLovName(passenger.IdTitle, Constants.LOV_CIVILITY);
                    model.SaillingDate = bookingCruisePassenger.Cruise.SailingDate;
                    model.IdLanguage = bookingCruisePassenger.Booking.IdLanguage;
                    model.FirstName = passenger.FirstName;
                    model.LastName = passenger.LastName;
                    model.CruiseName = GetLovName(bookingCruisePassenger.Cruise.IdShip, Constants.LOV_SHIP);
                    model.AgencyLogo = agencyAccessRight != null ? _userClass.GetLogoName(agencyAccessRight.IdAgency) : null;
                    model.AgencyName = agencyAccessRight != null ? agencyAccessRight.Agency.Name : null;
                    model.IdStatus = passenger.IdStatus;
                }
            }
            return model;
        }
        #endregion

        #region AddDocuments
        public void AddDocuments(UploadViewModel model)
        {
            AddDocument(model.MedicalSurvey, model, _MedicalSurvey);
            AddDocument(model.RelatedDocuments1, model, _RelatedDocuments1);
            AddDocument(model.RelatedDocuments2, model, _RelatedDocuments2);
            AddDocument(model.RelatedDocuments3, model, _RelatedDocuments3);
            AddDocument(model.RelatedDocuments4, model, _RelatedDocuments4);
            AddDocument(model.RelatedDocuments5, model, _RelatedDocuments5);
        }
        #endregion

        #region CheckPassengerLanguage
        public void CheckPassengerLanguage(int IdLanguage)
        {
            if (IdLanguage == Constants.LANGUAGE_FRANCOPHONE)
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
            }
        }
        #endregion

        #region GetFullNamePassenger
        public string GetFullNamePassenger(int idPassenger)
        {
            Passenger passenger = _shoreEntities.Passenger.Find(idPassenger);
            return string.Concat(passenger.FirstName, passenger.LastName);
        }
        #endregion

        #region DeleteInputFiles
        public void DeleteInputFiles(string fullName)
        {
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _MedicalSurvey));
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _RelatedDocuments1));
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _RelatedDocuments1));
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _RelatedDocuments1));
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _RelatedDocuments1));
            FileManager.FileManyDelete(AppSettings.FolderTemp, (fullName + _RelatedDocuments1));
        }
        #endregion

        #region SendMailConfirmation
        public void SendMailConfirmation(int idPassenger, int IdLanguage)
        {
            string mailModel = Path.Combine(AppSettings.FolderMailConfirmationReception, IdLanguage == Constants.LANGUAGE_FRANCOPHONE ? "ConfirmationReceptionFr.msg" : "ConfirmationReception.msg");
            SendMailCommon(idPassenger, IdLanguage, mailModel);
        }
        #endregion

        #region SendMailError
        public void SendMailError(int idPassenger, int IdLanguage)
        {
            string mailModel = Path.Combine(AppSettings.FolderMailErrorSubmission, IdLanguage == Constants.LANGUAGE_FRANCOPHONE ? "ErrorSubmissionFr.msg" : "ErrorSubmission.msg");
            SendMailCommon(idPassenger, IdLanguage, mailModel);
        }
        #endregion

        #region private

        #region GetLovName
        private string GetLovName(int idLov, int lovType)
        {
            return _shoreEntities.Lov.Where(l => l.Id.Equals(idLov))
                                                    .Where(l => l.IdLovType.Equals(lovType))
                                                    .Where(l => l.IsEnabled)
                                                    .FirstOrDefault()
                                                    ?.Name;
        }
        #endregion

        #region CheckDocument
        private void AddDocument(HttpPostedFileBase httpPostedFileBase, UploadViewModel model, string fileInput)
        {
            if (httpPostedFileBase != null)
            {
                string[] allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".tif", ".tiff", ".bmp", ".xps" };
                if (httpPostedFileBase.ContentLength > 8192000 || !allowedExtensions.Contains(Path.GetExtension(httpPostedFileBase.FileName).ToLower()))
                {
                    throw new FormatException();
                }

                string fullName = GetFullNamePassenger(model.IdPassenger);
                string tmpFilename = fullName + fileInput + httpPostedFileBase.FileName;
                FileManager.FileSave(AppSettings.FolderTemp, tmpFilename, httpPostedFileBase, false, true);

                _PassengerDocumentClass.AddDocument(new AddDocumentViewModel
                {
                    IdPassenger = model.IdPassenger,
                    Name = string.Concat(model.FirstName, " ", model.LastName),
                    PassengerDocumentName = httpPostedFileBase.FileName
                }, fileInput);
            }
        }
        #endregion

        #region SendMailCommon
        private void SendMailCommon(int idPassenger, int IdLanguage, string mailModel)
        {
            BookingCruisePassenger bookingCruisePassenger = (from p in _shoreEntities.Passenger
                                                             join bcp in _shoreEntities.BookingCruisePassenger on p.Id equals bcp.IdPassenger
                                                             join boo in _shoreEntities.Booking on bcp.IdBooking equals boo.Id
                                                             where (p.Id == idPassenger)
                                                             select bcp).First();

            if (!string.IsNullOrWhiteSpace(mailModel) && File.Exists(mailModel))
            {
                using (Storage.Message messageAuto = new Storage.Message(mailModel))
                {
                    Mail mail = new Mail()
                    {
                        Body = MailServer.ReplaceTags(messageAuto.BodyHtml, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger),
                        From = AppSettings.AddressNoReply,
                        Subject = MailServer.ReplaceTags(messageAuto.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger)
                    };
                    MailServer.SendMailLinkPassengerToDocument(mail, new Recipient("", bookingCruisePassenger.Passenger.Email), AppSettings.AddressDebug);
                }
            }
        }
        #endregion

        #endregion
    }
    #endregion
}