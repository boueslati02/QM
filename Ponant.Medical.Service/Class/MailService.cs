using MsgReader.Outlook;
using Ponant.Medical.Common;
using Ponant.Medical.Common.MailServer;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ponant.Medical.Service.Class
{
    /// <summary>
    /// Classe d'envoi des mails du service
    /// </summary>
    public class MailService
    {
        #region SendDeadline
        /// <summary>
        /// Envoi du mail de rappel si des QMs restent à traiter au médecin de bord
        /// </summary>
        public async void SendDeadline()
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    // Si les mails n'ont pas encore été envoyé aujourd'hui
                    if (!db.Log.Any(l => DbFunctions.TruncateTime(l.Date) == DateTime.Today && l.Type == LogManager.LogType.Assignment.ToString() && l.Action == LogManager.LogAction.Send.ToString()))
                    {
                        List<vCruiseBoard> cruises = (from cruise in db.vCruiseBoard
                                                    where cruise.NbQMAvailable > 0
                                                    && cruise.Deadline < DateTime.Now
                                                    && cruise.SailingDate > DateTime.Now
                                                    select cruise).ToList();

                        foreach (vCruiseBoard cruise in cruises)
                        {
                            using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailDeadlineExceeded)))
                            {
                                Lov lovShip = db.Lov.Find(cruise.IdShipAssigned);
                                
                                Mail mail = new Mail()
                                {
                                    Body = message.BodyHtml.Replace(AppSettings.TagCruise, cruise.Code).Replace(AppSettings.TagShip, lovShip.Name).Replace(AppSettings.TagQmNotValidated, cruise.NbQMAvailable.ToString()),
                                    From = AppSettings.AddressNoReply,
                                    Recipients = message.Recipients.Select(r => new Recipient(r.DisplayName.Replace(AppSettings.TagShipCode, lovShip.Code), r.DisplayName.Replace(AppSettings.TagShipCode, lovShip.Code))).ToList(),
                                    Subject = message.Subject.Replace(AppSettings.TagCruise, lovShip.Code)
                                };
#if DEV || INTEGRATION || RECETTE
                                foreach (Recipient recipient in mail.Recipients)
                                {
                                    recipient.Address = "x" + recipient.Address;
                                    recipient.Name = "x" + recipient.Name;
                                }
                                mail.Recipients.Add(new Recipient(AppSettings.AddressDebug, AppSettings.AddressDebug));
#endif
                                await MailServer.Send(mail);

                                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Assignment, LogManager.LogAction.Send, AppSettings.UserAccount, "Envoi du mail de rappel pour le navire " + lovShip.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Assignment, LogManager.LogAction.Send, AppSettings.UserAccount, exception);
            }
        }
        #endregion

        #region SendReminder
        /// <summary>
        /// Envoit les mails de rappels aux passagers qui n'ont pas encore renvoyés leur questionnaire
        /// </summary>
        public async void SendReminder()
        {
            try
            {
                Survey survey = new Survey();

                using (ShoreEntities db = new ShoreEntities())
                {
                    // Si les mails n'ont pas encore été envoyé aujourd'hui
                    if (!db.Log.Any(l => DbFunctions.TruncateTime(l.Date) == DateTime.Today && l.Type == LogManager.LogType.Reminder.ToString() && l.Action == LogManager.LogAction.Send.ToString()))
                    {
                        // Envoi des rappels X jours avant la croisière
                        foreach (Reminder reminder in db.Reminder.Where(r => r.Id < 3 && r.Enabled).OrderBy(r => r.Order).ToList())
                        {
                            DateTime date = DateTime.Today.AddDays(reminder.Length);

                            foreach (Cruise cruise in db.Cruise.Where(c => c.SailingDate == date).ToList())
                            {
                                foreach (int bookingNumber in cruise.BookingCruisePassenger.Select(bcp => bcp.Booking.Number).Distinct())
                                {
                                    if (await survey.Sent(bookingNumber, true,false))
                                    {
                                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Reminder, LogManager.LogAction.Send, AppSettings.UserAccount, string.Format("Envoi du rappel {0} jours avant la croisière", reminder.Length), bookingNumber);
                                    }
                                }
                            }
                        }

                        // Envoi des rappels tous les X jours avant la croisière
                        Reminder thirdReminder = db.Reminder.Find(3);
                        Reminder secondReminder = db.Reminder.Find(2);

                        if (secondReminder.Enabled && thirdReminder.Enabled)
                        {
                            foreach (Cruise cruise in db.Cruise.Where(c => c.SailingDate >= DateTime.Today && c.SailingDate <= DbFunctions.AddDays(DateTime.Today, secondReminder.Length) &&
                                                                           DbFunctions.DiffDays(DateTime.Today, c.SailingDate) % thirdReminder.Length == 0).ToList())
                            {
                                foreach (int bookingNumber in cruise.BookingCruisePassenger.Select(bcp => bcp.Booking.Number).Distinct())
                                {
                                    if (await survey.Sent(bookingNumber, true,false))
                                    {
                                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Reminder, LogManager.LogAction.Send, AppSettings.UserAccount, string.Format("Envoi du rappel tous les {0} jours avant la croisière", thirdReminder.Length), bookingNumber);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Reminder, LogManager.LogAction.Send, AppSettings.UserAccount, exception);
            }
        }
        #endregion

        #region SendSummary
        /// <summary>
        /// Envoi du mail récapitulatif des croisières au médecin de bord
        /// </summary>
        /// <param name="delay">Nombre de jours avant la croisière</param>
        public async void SendSummary(int delay, LogManager.LogType logType)
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    // Si les mails n'ont pas encore été envoyé aujourd'hui
                    if (!db.Log.Any(l => DbFunctions.TruncateTime(l.Date) == DateTime.Today && l.Type == logType.ToString() && l.Action == LogManager.LogAction.Send.ToString()))
                    {
                        DateTime date = DateTime.Today.AddDays(delay);

                        foreach (Cruise cruise in db.Cruise.Where(c => c.SailingDate == date).ToList())
                        {
                            using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailSummaryMail)))
                            {
                                Mail mail = new Mail()
                                {
                                    Body = message.BodyHtml.Replace(AppSettings.TagTable, MailServer.BuildTableForService(cruise.BookingCruisePassenger.ToList())).Replace(AppSettings.TagCruise, cruise.Code),
                                    From = AppSettings.AddressNoReply,
                                    Recipients = message.Recipients.Select(r => new Recipient(r.DisplayName.Replace(AppSettings.TagShipCode, cruise.LovShip.Code), r.DisplayName.Replace(AppSettings.TagShipCode, cruise.LovShip.Code))).ToList(),
                                    Subject = message.Subject.Replace(AppSettings.TagCruise, cruise.Code)
                                };
#if DEV || INTEGRATION || RECETTE
                                foreach (Recipient recipient in mail.Recipients)
                                {
                                    recipient.Address = "x" + recipient.Address;
                                    recipient.Name = "x" + recipient.Name;
                                }
                                mail.Recipients.Add(new Recipient(AppSettings.AddressDebug, AppSettings.AddressDebug));
#endif
                                await MailServer.Send(mail);

                                LogManager.InsertLog(LogManager.LogLevel.Info, logType, LogManager.LogAction.Send, AppSettings.UserAccount, "Envoi du mail récapitulatif pour la croisière " + cruise.Code);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(logType, LogManager.LogAction.Send, AppSettings.UserAccount, exception);
            }
        }
        #endregion

        #region SendQmReceived
        /// <summary>
        /// Envoi du mail récapitulatif des QM reçus dernieres 24H
        /// </summary>
        public async void SendQmReceived()
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    //Si les mails n'ont pas encore été envoyé aujourd'hui

                    if (!db.Log.Any(l => DbFunctions.TruncateTime(l.Date) == DateTime.Today && l.Type == LogManager.LogType.QmReceived.ToString() && l.Action == LogManager.LogAction.Send.ToString()))
                    {
                        // Requête ramenant la liste des QM reçus
                        var QmReceivedList =
                                (from bcp in db.BookingCruisePassenger
                                join b in db.Booking on bcp.IdBooking equals b.Id
                                join c in db.Cruise on bcp.IdCruise equals c.Id
                                join p in db.Passenger on bcp.IdPassenger equals p.Id
                                join d in db.Document on p.Id equals d.IdPassenger
                                where d.IdPassenger != 0 && d.ReceiptDate < DateTime.Now && d.ReceiptDate > DbFunctions.AddDays(DateTime.Now, -1)
                                
                                group new { b, p, c, d} by new { b.Number, p.FirstName, p.LastName, c.Code }
                                into grp
                                orderby grp.Key.Code, grp.Key.Number
                                select new QmReceived
                                {
                                    BookingNumber = grp.Key.Number,
                                    FirstName = grp.Key.FirstName,
                                    LastName = grp.Key.LastName,
                                    ReceiptDate = grp.Max(m => m.d.ReceiptDate),
                                    CruiseNumber = grp.Key.Code
                                }).ToList();
                        
                        using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailSendQmReceived)))
                        {
                            List<Recipient> recipients = new List<Recipient>
                            {
                                new Recipient(AppSettings.QmReceivedList, AppSettings.QmReceivedList)
                            };
                            Mail mail = new Mail()
                            {
                                Body = message.BodyHtml.Replace(AppSettings.TagTable, MailServer.BuildTableForQmReceived(QmReceivedList)),
                                From = AppSettings.AddressNoReply,
                                Recipients = recipients,
                                Subject = message.Subject
                            };
#if DEV || INTEGRATION || RECETTE
                            mail.Recipients.Add(new Recipient(AppSettings.AddressDebug, AppSettings.AddressDebug));
#endif
                            await MailServer.Send(mail);
                            
                            LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.QmReceived, LogManager.LogAction.Send, AppSettings.UserAccount, "Envoi du mail des QM reçus des dernières 24H");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.QmReceived, LogManager.LogAction.Send, AppSettings.UserAccount, exception);
            }
        }
        #endregion

        #region cryptage model
        
        #endregion
    }
}
