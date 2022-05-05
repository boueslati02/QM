using hMailServer;
using MsgReader.Outlook;
using Ponant.Medical.Data.Shore;
using QRCodeDecoderLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Classe de gestion des mails hMailServer
    /// </summary>
    /// <see cref="https://www.hmailserver.com/documentation/latest/?page=com_objects"/>
    public class MailServer
    {
        #region Constants
        /// <summary>
        /// Nom d'utilisateur du serveur de messagerie
        /// </summary>
        private const string MAIL_USERNAME = "Administrator";

        /// <summary>
        /// Nom du dossier de réception
        /// </summary>
        private const string MAIL_INBOX = "INBOX";

        /// <summary>
        /// Nom du dossier poubelle
        /// </summary>
        private const string MAIL_TRASH = "TRASH";

        /// <summary>
        /// Extensions de fichiers autorisés
        /// </summary>
        private const string _pdfExtension = ".pdf";
        private const string _jpgExtension = ".jpg";
        private const string _jpegExtension = ".jpeg";
        private const string _pngExtension = ".png";
        private const string _gifExtension = ".gif";
        private const string _tiffExtension = ".tiff";
        private const string _tifExtension = ".tif";
        private const string _xpsExtension = ".xps";
        private const string _bmpExtension = ".bmp";
        private const string _docExtension = ".doc";
        private const string _docxExtension = ".docx";
        private const string _zipExtension = ".zip";
        private const string _msgExtension = ".msg";
        private const string _emlExtension = ".eml";
        #endregion

        #region Properties
        /// <summary>
        /// Objet Application
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// Compte hMailServer
        /// </summary>
        private readonly Account account;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public MailServer()
        {
        }

        /// <summary>
        /// Initialise le serveur de messagerie
        /// </summary>
        /// <param name="email">Adresse électronique</param>
        public MailServer(string email)
        {
            this.application = GetApplication();
            this.account = GetAccount(email);
        }
        #endregion

        #region Public methods

        #region Delete
        /// <summary>
        /// Supprime le mail du dossier
        /// </summary>
        /// <param name="idMessage">Identifiant du message</param>
        public void Delete(long idMessage)
        {
            if (this.application != null && this.account != null)
            {
                IMAPFolder folder = this.account.IMAPFolders.ItemByName[MAIL_INBOX];

                if (folder != null)
                {
                    // A tester si un déplacement vers le dossier Trash est demandé
                    //IMAPFolder trash = this.account.IMAPFolders.ItemByName[MAIL_TRASH];
                    //if(trash != null)
                    //{
                    //    Message message = trash.Messages.Add();
                    //    message = folder.Messages.ItemByDBID[idMessage];
                    //    message.Save();
                    //}

                    folder.Messages.DeleteByDBID(idMessage);
                }
            }
        }
        #endregion

        #region Receive
        /// <summary>
        /// Récupère les messages et les pièces jointes du compte
        /// </summary>
        /// <returns>Liste des messages et des pièces jointe</returns>
        public List<MailMessage> Receive(string folderSandBox, string folderPassenger)
        {
            List<MailMessage> messages = null;

            if (this.application != null && this.account != null)
            {
                messages = new List<MailMessage>();
                IMAPFolder folder = this.account.IMAPFolders.ItemByName[MAIL_INBOX];

                if (folder != null)
                {
                    #region Parcour des Messages dans la boite mail
                    for (int i = 0; i < folder.Messages.Count; i++)
                    {
                        Message message = folder.Messages[i];
                        try
                        {
                            // Si le message comporte une piècece jointe
                            if (message != null && message.Attachments.Count > 0)
                            {
                                #region Initialise l'objet de retour
                                MailMessage mailMessage = new MailMessage
                                {
                                    Attachments = new List<MailAttachment>(),
                                    Email = message.From,
                                    IdMessage = message.ID,
                                    Message = message.Body,
                                    ReceivedDate = DateTimeOffset.Parse(message.Date.Split('(')[0]).UtcDateTime.ToLocalTime() //Traitement correctif pour les email envoyé depuis d'autre fuseau horaire
                                };
                                #endregion

                                #region Extrait l'ID du passager et vérification de son existence
                                mailMessage.IdPassenger = ExtractIdPassenger(message);                                  
                                #endregion

                                #region Récupére les pièces jointes
                                for (int j = 0; j < message.Attachments.Count; j++)
                                {
                                    try
                                    {
                                        string extension = Path.GetExtension(message.Attachments[j].Filename).ToLower();

                                        if (extension.Equals(_pdfExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_jpgExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_jpegExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_pngExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_gifExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_tiffExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_tifExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_xpsExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_bmpExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_docExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_docxExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_zipExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_msgExtension, StringComparison.CurrentCultureIgnoreCase) ||
                                            extension.Equals(_emlExtension, StringComparison.CurrentCultureIgnoreCase)
                                            )
                                        {
                                            DateTime dateMailMessage = mailMessage.ReceivedDate != null ? mailMessage.ReceivedDate : DateTime.Now;
                                            string attachementFilename = StringHelper.RemoveDiacritics(StringHelper.CleanFileName(message.Attachments[j].Filename));
                                            string filename = string.Format("{0}_{1}", dateMailMessage.ToString("yyyy-MM-dd-HH-mm-ss"), attachementFilename);

                                            string directoryPath = (mailMessage.IdPassenger == 0) 
                                                ? Path.Combine(folderSandBox) 
                                                : Path.Combine(folderPassenger, dateMailMessage.ToString("yyyy-MM"), mailMessage.IdPassenger.ToString());
                                            string filePath = GetDocumentFilePath(directoryPath, filename);

                                            message.Attachments[j].SaveAs(filePath);

                                            #region Traitement des extensions pièces jointes si l'extension n'est pas un doc, docx, zip, msg, eml
                                            if (!extension.Equals(_docExtension) && 
                                                !extension.Equals(_docxExtension) && 
                                                !extension.Equals(_zipExtension) && 
                                                !extension.Equals(_msgExtension) && 
                                                !extension.Equals(_emlExtension))
                                            {
                                                Picture picture = new Picture(filePath);
                                                List<string> files = picture.ConvertToJpeg();

                                                if (files != null)
                                                {
                                                    foreach (string file in files)
                                                    {
                                                        string newFilePath = file;
                                                        try
                                                        {
                                                            int? idPassenger = GetQRCodeValue(file);
                                                            picture.Treat(file);

                                                            if (idPassenger.HasValue && File.Exists(file)) // Traitement si une valeur du QR Code a été récupérée
                                                            {
                                                                using (ShoreEntities db = new ShoreEntities())
                                                                {
                                                                    if (!db.Passenger.Any(p => p.Id.Equals(idPassenger.Value)))
                                                                    {
                                                                        idPassenger = null;
                                                                    }
                                                                }

                                                                if (idPassenger.HasValue) // Traitement si l'identifiant récuperer via QR Code est bien un passager existant
                                                                {
                                                                    string newDirectoryPath = Path.Combine(folderPassenger, dateMailMessage.ToString("yyyy-MM"), idPassenger.ToString());
                                                                    string newFilename = Path.GetFileName(file);                                                                    
                                                                    newFilePath = Path.Combine(newDirectoryPath, newFilename);
                                                                    if (!file.Equals(newFilePath))
                                                                    {
                                                                        newFilePath = GetDocumentFilePath(newDirectoryPath, newFilename);
                                                                        File.Move(file, newFilePath);
                                                                    }
                                                                    if (Directory.GetFiles(directoryPath).Length == 0)
                                                                    {
                                                                        Directory.Delete(directoryPath);
                                                                    }
                                                                }
                                                            }

                                                            // Archive des fichiers
                                                            Archive archive = new Archive();
                                                            filePath = archive.Zip(newFilePath);

                                                            // Attachement des fichiers
                                                            mailMessage.Attachments.Add(new MailAttachment
                                                            {
                                                                Name = Path.GetFileName(newFilePath).Substring(Path.GetFileName(newFilePath).IndexOf("_") + 1),
                                                                FileName = Path.GetFileName(filePath),
                                                                IdPassenger = idPassenger
                                                            });
                                                        }
                                                        catch (Exception exception)
                                                        {
                                                            LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, "System", " Message Attachement From : " + message.From + " Date : " + message.Date + " File : " + newFilePath
                                                                + " (" + string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region Traitement si le fichier est un ".ZIP"
                                            if (extension.Equals(_zipExtension, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                bool errorZipFile = false;
                                                using (ZipArchive archive = ZipFile.OpenRead(filePath))
                                                {
                                                    foreach (ZipArchiveEntry entry in archive.Entries)
                                                    {
                                                        if (entry.FullName.EndsWith(_zipExtension))
                                                        {
                                                            errorZipFile = true;
                                                        }
                                                    }
                                                }

                                                if (errorZipFile)
                                                {
                                                    File.Delete(filePath);
                                                    throw new InvalidOperationException("The file attachment must not contain the other Zip");
                                                }
                                                else
                                                {
                                                    mailMessage.Attachments.Add(new MailAttachment
                                                    {
                                                        Name = Path.GetFileName(filename).Substring(Path.GetFileName(filePath).IndexOf("_") + 1),
                                                        FileName = Path.GetFileName(filePath),
                                                        IdPassenger = null
                                                    });
                                                }
                                            }
                                            #endregion

                                            #region Traitement si c'est un fichier .msg ou .eml
                                            if (extension.Equals(_msgExtension, StringComparison.CurrentCultureIgnoreCase) || 
                                                extension.Equals(_emlExtension, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                if (extension.Equals(_emlExtension, StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    MsgReader.Mime.Message emlFile = MsgReader.Mime.Message.Load(new FileInfo(filePath));
                                                    bool errorEmlFile = false;

                                                    foreach (MsgReader.Mime.MessagePart attachment in emlFile.Attachments)
                                                    {
                                                        if (attachementFilename.Contains(_emlExtension) 
                                                            || attachementFilename.Contains(_msgExtension))
                                                        {
                                                            errorEmlFile = true;
                                                        }
                                                    }

                                                    if (errorEmlFile) // Condition si il y a une Erreur Piece jointe EML
                                                    {
                                                        using (FileStream fileStream = File.OpenRead(filePath))
                                                        {
                                                            MemoryStream memStream = new MemoryStream();
                                                            memStream.SetLength(fileStream.Length);
                                                            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                                                        }
                                                        File.Delete(filePath);
                                                        throw new InvalidOperationException("The file attachment must not contain the other EML File");
                                                    }
                                                    else
                                                    {
                                                        mailMessage.Attachments.Add(new MailAttachment
                                                        {
                                                            Name = Path.GetFileName(filename).Substring(Path.GetFileName(filePath).IndexOf("_") + 1),
                                                            FileName = Path.GetFileName(filePath),
                                                            IdPassenger = null
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    bool errorMsgFile = false;

                                                    using (Storage.Message msgFile = new Storage.Message(filePath))
                                                    {
                                                        foreach (Storage.Message attachment in msgFile.Attachments)
                                                        {
                                                            if (attachementFilename.Contains(_emlExtension) 
                                                                || attachementFilename.Contains(_msgExtension))
                                                            {
                                                                errorMsgFile = true;
                                                            }
                                                        }
                                                    }

                                                    if (errorMsgFile)  // Condition si il y a une Erreur PIece Jointe MSG
                                                    {
                                                        File.Delete(filePath);
                                                        throw new InvalidOperationException("The file attachment must not contain the other MSG File");
                                                    }
                                                    else
                                                    {
                                                        mailMessage.Attachments.Add(new MailAttachment
                                                        {
                                                            Name = Path.GetFileName(filename).Substring(Path.GetFileName(filePath).IndexOf("_") + 1),
                                                            FileName = Path.GetFileName(filePath),
                                                            IdPassenger = null
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region Traitement si le fichier est un ".doc" ".docx"
                                            if (extension.Equals(_docExtension, StringComparison.CurrentCultureIgnoreCase) || 
                                                extension.Equals(_docxExtension, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                mailMessage.Attachments.Add(new MailAttachment
                                                {
                                                    Name = Path.GetFileName(filename).Substring(Path.GetFileName(filePath).IndexOf("_") + 1),
                                                    FileName = Path.GetFileName(filePath),
                                                    IdPassenger = null
                                                });
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("The file attachment is not allowed");
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, "System", " Message Attachement From : " + message.From + " Date : " + message.Date + " Attachement : " + message.Attachments[j].Filename
                                                      + " (" + string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
                                    }
                                }
                                #endregion

                                if (mailMessage.Attachments.Count > 0)
                                {
                                    messages.Add(mailMessage);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, "System", " Message From : " + message.From + " Date : " + message.Date
                                + " (" + string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
                        }
                    }
                    #endregion
                }
            }

            return messages;
        }
        #endregion

        #endregion

        #region Static methods

        #region BuildTableForService
        /// <summary>
        /// Construction du tableau des passagers de la croisière pour l'envoi d'email via le service
        /// </summary>
        /// <param name="bookingCruisesPassengers">Instance des passagers de la croisière</param>
        /// <returns>Le tableau au format HTML</returns>
        public static string BuildTableForService(List<BookingCruisePassenger> bookingCruisesPassengers)
        {
            StringBuilder html = new StringBuilder();
            int? age = null;

            html.Append("<style> "
                  + "table { "
                      + " border-collapse: collapse; "
                  + " } "
                  + " table, th, td {"
                      + " border: 1px solid #ddd; "
                      + " padding: 5px; "
                      + " font-family: \"Segoe UI Light\", \"Segoe UI\" !important; "
                      + " font-size:14.5 !important; "
                  + " } "
              + " </style>");

            html.Append("<table cellpadding='5px' cellspacing='5px'");
            html.Append("<tr>");
            html.Append("<th>Passenger</th>");
            html.Append("<th>Age</th>");
            html.Append("<th>Status</th>");
            html.Append("</tr>");

            foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisesPassengers.OrderBy(bcp => bcp.Passenger.LastName).ThenBy(bcp => bcp.Passenger.FirstName))
            {
                if (bookingCruisePassenger.Passenger.BirthDate.HasValue)
                {
                    age = DateTime.Today.Year - bookingCruisePassenger.Passenger.BirthDate.Value.Year;
                    if (DateTime.Today < bookingCruisePassenger.Passenger.BirthDate.Value.AddYears(age.Value)) age--;
                }

                html.Append("<tr>");
                html.AppendFormat("<td>{0} {1}</td>", !string.IsNullOrWhiteSpace(bookingCruisePassenger.Passenger.UsualName) 
                    ? bookingCruisePassenger.Passenger.UsualName 
                    : bookingCruisePassenger.Passenger.LastName, bookingCruisePassenger.Passenger.FirstName);
                html.AppendFormat("<td>{0}</td>", age);
                html.AppendFormat("<td>{0}, {1}</td>", bookingCruisePassenger.Passenger.LovStatus.Name, !bookingCruisePassenger.Passenger.IdAdvice.Equals(0) 
                    ? bookingCruisePassenger.Passenger.LovAdvice.Name 
                    : "No advice");
                html.Append("</tr>");
            }

            html.Append("</table>");

            return html.ToString();
        }
        #endregion

        #region BuildTableForRelaunch
        /// <summary>
        /// Construction du tableau des passagers de la croisière pour les relances 
        /// </summary>
        /// <param name="bookingPassengers">Instance des groupes de la croisière</param>
        /// <returns>Le tableau au format HTML</returns>
        public static string BuildTableForRelaunch(List<BookingCruisePassenger> bookingCruisesPassengers)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<style> "
                    + "table { "
                        + " border-collapse: collapse; "
                    + " } "
                    + " table, th, td {"
                        + " border: 1px solid #ddd; "
                        + " padding: 5px; "
                        + " font-family: \"Segoe UI Light\", \"Segoe UI\" !important; "
                        + " font-size:14.5 !important; "
                    + " } "
                + " </style>");

            html.Append("<table cellpadding='5px' cellspacing='5px'");
            html.Append("<tr>");
            html.Append("<th>Passenger</th>");
            html.Append("<th>Status</th>");
            html.Append("<th>Status date</th>");
            html.Append("</tr>");

            foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisesPassengers)
            {
                html.Append("<tr>");
                html.AppendFormat("<td>{0} {1}</td>", !string.IsNullOrWhiteSpace(bookingCruisePassenger.Passenger.UsualName) 
                    ? bookingCruisePassenger.Passenger.UsualName 
                    : bookingCruisePassenger.Passenger.LastName, bookingCruisePassenger.Passenger.FirstName);
                html.AppendFormat("<td>{0}, {1}</td>", bookingCruisePassenger.Passenger.LovStatus.Name, !bookingCruisePassenger.Passenger.IdAdvice.Equals(0) 
                    ? bookingCruisePassenger.Passenger.LovAdvice.Name 
                    : "No advice");
                html.AppendFormat("<td>{0}</td>", bookingCruisePassenger.Passenger.TreatmentDate.HasValue 
                    ? bookingCruisePassenger.Passenger.TreatmentDate.Value.ToShortDateString() 
                    : "");
                html.Append("</tr>");
            }

            html.Append("</table>");

            return html.ToString();
        }
        #endregion

        #region BuildTableForQmReceived
        /// <summary>
        /// Construction du tableau des passagers
        /// pour les QM reçus dans les dernieres 24h
        /// </summary>
        /// <param name="infos">Instance des QMs reçus</param>
        /// <returns>Le tableau au format HTML</returns>
        public static string BuildTableForQmReceived(List<QmReceived> infos)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<style> "
                    + "table { "
                        + " border-collapse: collapse; "
                    + " } "
                    + " table, th, td {"
                        + " border: 1px solid #ddd; "
                        + " padding: 5px; "
                        + " font-family: \"Segoe UI Light\", \"Segoe UI\" !important; "
                        + " font-size:14.5 !important; "
                    + " } "
                + " </style>");

            html.Append("<table cellpadding='5px' cellspacing='5px'");
            html.Append("<tr>");
            html.Append("<th>Cruise</th>");
            html.Append("<th>Booking Number</th>");
            html.Append("<th>Passenger</th>");
            html.Append("<th>Reception date</th>");
            html.Append("</tr>");
            

            foreach (QmReceived line in infos)
            {
                html.Append("<tr>");
                html.AppendFormat("<td>{0}</td>", line.CruiseNumber);
                html.AppendFormat("<td>{0}</td>", line.BookingNumber);
                html.AppendFormat("<td>{0} {1}</td>", !string.IsNullOrWhiteSpace(line.UsualName) 
                    ? line.UsualName 
                    : line.LastName, line.FirstName);
                html.AppendFormat("<td>{0}</td>", line.ReceiptDate);
                html.Append("</tr>");
            }

            html.Append("</table>");

            return html.ToString();
        }
        #endregion

        #region IsValidEmail
        /// <summary>
        /// Permet de tester si l'adresse mail est valide
        /// </summary>
        /// <param name="email">Adresse électronique</param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        #endregion

        #region ReplaceTags
        /// <summary>
        /// Remplace les tags des modèles par les valeurs de la base de données
        /// </summary>
        /// <param name="str">Chaîne de texte à modifier</param>
        /// <param name="passenger">Instance du passager</param>
        /// <returns>La chaîne de texte complétée</returns>
        public static string ReplaceTags(string str, Booking booking, BookingCruisePassenger bookingCruisePassenger, Passenger passenger)
        {
            if (booking != null)
            {
                str = str.Replace(CommonSettings.TagBooking, booking.Number.ToString());
            }

            if (bookingCruisePassenger != null)
            {
                str = str.Replace(CommonSettings.TagShip, bookingCruisePassenger.Cruise.LovShip.Name);
                str = str.Replace(CommonSettings.TagCruise, bookingCruisePassenger.Cruise.Code);
                str = str.Replace(CommonSettings.TagCabin, bookingCruisePassenger.CabinNumber);
                str = str.Replace(CommonSettings.TagSailingDate, bookingCruisePassenger.Cruise.SailingDate.ToLongDateString());
                str = str.Replace(CommonSettings.TagDestination, bookingCruisePassenger.Cruise.LovDestination.Name);
                str = str.Replace(CommonSettings.TagGroup, bookingCruisePassenger.Booking.GroupName);
            }

            if (passenger != null)
            {
                str = str.Replace(CommonSettings.TagTitle, ((passenger.IdTitle != 0) 
                    ? passenger.LovTitle.Name 
                    : ""));
                str = str.Replace(CommonSettings.TagLastName, passenger.LastName);
                str = str.Replace(CommonSettings.TagUsualName, !string.IsNullOrWhiteSpace(passenger.UsualName) 
                    ? passenger.UsualName 
                    : passenger.LastName);
                str = str.Replace(CommonSettings.TagFirstName, passenger.FirstName);
                str = str.Replace(CommonSettings.TagBirthDate, ((passenger.BirthDate.HasValue) 
                    ? passenger.BirthDate.Value.ToShortDateString() 
                    : ""));
                str = str.Replace(CommonSettings.TagAdvice, passenger.LovAdvice.Name);
                str = str.Replace(CommonSettings.TagIdPassenger, passenger.Id.ToString());
                str = str.Replace(CommonSettings.TagComments, passenger.Review);
                str = str.Replace(CommonSettings.TagUrl, String.Concat(CommonSettings.UploadPassengerUrl,  StringHelper.ToBase64Encode(passenger.Token)));
            }

            return str;
        }
        #endregion

        #region Send
        /// <summary>
        /// Envoi du mail
        /// </summary>
        /// <param name="mail">Paramètres du mail</param>
        public async static Task Send(Mail mail)
        {
            Message message = new Message
            {
                From = mail.From,
                FromAddress = mail.FromAddress,
                Subject = mail.Subject,
                HTMLBody = mail.Body
            };

            foreach (Recipient recipient in mail.Recipients)
            {
                message.AddRecipient(recipient.Name, recipient.Address);
            }

            if (mail.Attachments != null)
            {
                foreach (string filename in mail.Attachments)
                {
                    message.Attachments.Add(filename);
                }
            }
            message.Save();
        }
        #endregion

        #region SendMailToRecipient
        /// <summary>
        /// Envoi d'un mail à un unique destinataire de manière asynchrone
        /// </summary>
        /// <param name="mail">Email a envoyé</param>
        /// <param name="destEmail">Email du destinataire</param>
        /// <param name="adressDebug">Email de debug</param>
        public static async Task SendMailToRecipient(Mail mail, Recipient recipient, string adressDebug)
        {
            mail.Recipients = CreateRecipientList(recipient, adressDebug);
            await MailServer.Send(mail);
        }
        #endregion

        #region SendMailLinkPassengerToDocument
        /// <summary>
        /// Envoi d'un mail à un unique destinataire de manière synchrone
        /// </summary>
        /// <param name="mail">Email a envoyé</param>
        /// <param name="destEmail">Email du destinataire</param>
        /// <param name="adressDebug">Email de debug</param>
        public static void SendMailLinkPassengerToDocument(Mail mail, Recipient recipient, string adressDebug)
        {
            mail.Recipients = CreateRecipientList(recipient, adressDebug);
            MailServer.Send(mail);
        }
        #endregion

        #endregion

        #region Private

        #region GetApplication
        /// <summary>
        /// Retoune l'objet application instancié et authentifié
        /// </summary>
        /// <returns>Objet Application</returns>
        private static Application GetApplication()
        {
            Application currentApplication = new Application();
            currentApplication.Authenticate(MAIL_USERNAME, CommonSettings.MailPassword);

            return currentApplication;
        }
        #endregion

        #region GetAccount
        /// <summary>
        /// Retourne l'objet account instancié sur l'adresse mail
        /// </summary>
        /// <param name="email">Adresse électronique du compte</param>
        /// <returns>Objet Account</returns>
        private static Account GetAccount(string email)
        {
            Domain domain = GetApplication().Domains.ItemByName[email.Split('@')[1]];
            return domain.Accounts.ItemByAddress[email];
        }
        #endregion

        #region CreateRecipientList
        /// <summary>
        /// Creer la liste des destinataire d'un email
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="adressDebug"></param>
        /// <returns></returns>
        private static List<Recipient> CreateRecipientList(Recipient recipient, string adressDebug)
        {
            List<Recipient> recipients = new List<Recipient>
            {
                recipient
            };

#if DEV || INTEGRATION || RECETTE
            if (!string.IsNullOrWhiteSpace(adressDebug))
            {
                recipients.Add(new Recipient(adressDebug, adressDebug));
            }
#endif
            return recipients;
        }
        #endregion

        #region ExtractIdPassenger

        /// <summary>
        /// Extrait l'identifiant du passager depuis un message
        /// </summary>
        /// <param name="message">Message d'origine</param>
        /// <returns>Identifiant du passager</returns>
        private static int ExtractIdPassenger(Message message)
        {
            int idPassenger = 0;

            using (ShoreEntities db = new ShoreEntities())
            {
                List<int> idPassengersCorresp = db.Passenger
                    .Where(p => p.Email == message.FromAddress)
                    .Select(p => p.Id)
                    .ToList();

                if (idPassengersCorresp.Count == 1)
                {
                    idPassenger = idPassengersCorresp[0];
                }

                if (idPassenger == 0 && (message.Subject.Contains("#") || message.Body.Contains("#")))
                {
                    idPassenger = ExtractIdPassenger(message.Subject);
                    idPassenger = idPassenger != 0
                        ? idPassenger
                        : ExtractIdPassenger(message.Body);

                    if (!db.Passenger.Any(p => p.Id.Equals(idPassenger)))
                    {
                        idPassenger = 0;
                    }
                }
            }

            return idPassenger;
        }

        /// <summary>
        /// Extrait l'ID du passager de la chaîne de caractères
        /// </summary>
        /// <param name="contentString">Chaîne de caractère à analyser</param>
        /// <returns></returns>
        private static int ExtractIdPassenger(string contentString)
        {
            int idPassenger = 0;

            int index = contentString.IndexOf("#");
            if (index > -1)
            {
                string subString = contentString.Substring(index);
                string passenger = subString.Split(' ')?[0];
                if (!string.IsNullOrWhiteSpace(passenger))
                {
                    int.TryParse(passenger.Replace("#", ""), out idPassenger);
                }
            }

            return idPassenger;
        }
        #endregion

        #region GetQRCodeValue
        /// <summary>
        /// Récupere la valeur stockée dans le QR code de l'image
        /// </summary>
        /// <param name="imagePath">Chemin de l'image</param>
        /// <returns>Valeur du QR Code</returns>
        private static int? GetQRCodeValue(string imagePath)
        {
            int? result = null;

            try
            {
                QRDecoder qrDecoder = new QRDecoder();
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    byte[][] dataByteArray = qrDecoder.ImageDecoder(bitmap);
                    if (dataByteArray != null)
                    {
                        Decoder decoder = Encoding.UTF8.GetDecoder();
                        foreach (byte[] dataArray in dataByteArray)
                        {
                            int CharCount = decoder.GetCharCount(dataArray, 0, dataArray.Length);
                            char[] CharArray = new char[CharCount];
                            decoder.GetChars(dataArray, 0, dataArray.Length, CharArray, 0);
                            string stringResult = new string(CharArray);

                            if (int.TryParse(stringResult, out int intResult))
                            {
                                result = intResult;
                            }
                        }
                    }
                }

                if (result.HasValue)
                {
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Associate, "System", string.Concat("Read QR Code value : ", result.ToString(), " From File : ", imagePath));
                }
            }
            catch (Exception exception)
            {
                result = null;
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, "System", "Read QR Code From File : " + imagePath
                    + " (" + string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
            }

            return result;
        }
        #endregion

        #region GetDocumentFilePath
        /// <summary>
        /// Format le chemin d'accès au fichier et crée ou nettoie les elements du systeme de fichier si necessaire
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string GetDocumentFilePath(string directoryPath, string filename)
        {
            string filePath = Path.Combine(directoryPath, filename);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return filePath;
        }
        #endregion

        #endregion
    }
}