using Ponant.Medical.Common.Interfaces;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Xml.Schema;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Classe de gestion des logs
    /// </summary>
    public class LogManager : ILogManager
    {
        #region Properties & Constructors

        private readonly IShoreEntities _shoreEntities;

        /// <summary>
        /// Log Manager
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        public LogManager(IShoreEntities shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }

        #endregion

        #region Enums
        /// <summary>
        /// Enumération des niveaux de log
        /// </summary>
        public enum LogLevel
        {
            Info = 1,
            Warning = 2,
            Error = 3
        }

        /// <summary>
        /// Enumération des types de log
        /// </summary>
        public enum LogType
        {
            Common = 0,
            Reminder = 1,
            Criteria = 2,
            Survey = 3,
            User = 4,
            Language = 5,
            File = 6,
            FluxBooking = 7,
            Cruise = 8,
            Passenger = 9,
            Directory = 10,
            Connection = 11,
            SummaryMail1 = 12,
            SummaryMail2 = 13,
            Document = 14,
            Log = 15,
            Lov = 16,
            QmReceived = 17,
            AgencyAccessRight = 18,
            Assignment = 19
        }

        /// <summary>
        /// Enumération des actions de log
        /// </summary>
        public enum LogAction
        {
            Common = 0,
            Get = 1,
            Add = 2,
            Edit = 3,
            Delete = 4,
            Send = 5,
            Reset = 6,
            Integration = 7,
            Associate = 8,
            Move = 9,
            Unlock = 10,
            Separate = 11,
            Lock = 12,
            Advice = 13,
            Unlink = 14
        }
        #endregion

        #region InsertLog

        /// <summary>
        /// Insert a Log
        /// </summary>
        /// <param name="log"></param>
        public void InsertLog(Log log)
        {
            _shoreEntities.Log.Add(log);
            _shoreEntities.SaveChanges();
        }

        /// <summary>
        /// Insert logs
        /// </summary>
        /// <param name="logs"></param>
        public void InsertLogs(IEnumerable<Log> logs)
        {
            _shoreEntities.Log.AddRange(logs);
            _shoreEntities.SaveChanges();
        }

        /// <summary>
        /// Ajout d'un nouveau Log
        /// </summary>
        /// <param name="level">Niveau du log</param>
        /// <param name="type">Type du log</param>
        /// <param name="action">Action du log</param>
        /// <param name="details">Détails du log</param>
        /// <param name="booking">Numéro de booking</param>
        public static void InsertLog(LogLevel level, LogType type, LogAction action, string user, string details, int? booking = null)
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    Log log = new Log
                    {
                        User = user,
                        Date = DateTime.Now,
                        Level = level.ToString(),
                        Type = type.ToString(),
                        Action = action.ToString(),
                        Details = details,
                        Booking = booking
                    };

                    db.Log.Add(log);
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                if (!type.Equals(LogType.Log))
                {
                    InsertLog(LogType.Log, LogAction.Add, user, exception);
                }
            }
        }

        /// <summary>
        /// Ajout d'un nouveau Log
        /// </summary>
        /// <param name="level">Niveau du log</param>
        /// <param name="type">Type du log</param>
        /// <param name="action">Action du log</param>
        /// <param name="details">Détails du log</param>
        /// <param name="booking">Numéro de booking</param>
        public static void InsertLog(XmlSeverityType level, LogType type, LogAction action, string user, string details, int? booking = null)
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    Log log = new Log
                    {
                        User = user,
                        Date = DateTime.Now,
                        Level = level.ToString(),
                        Type = type.ToString(),
                        Action = action.ToString(),
                        Details = details,
                        Booking = booking
                    };

                    db.Log.Add(log);
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                if (!type.Equals(LogType.Log))
                {
                    InsertLog(LogType.Log, LogAction.Add, user, exception);
                }
            }
        }

        /// <summary>
        /// Enregistre en base les exceptions
        /// </summary>
        /// <param name="exception">Instance de l'exception</param>
        public static void InsertLog(LogType type, LogAction action, string user, Exception exception)
        {
            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    // Log
                    db.Log.Add(new Log()
                    {
                        Date = DateTime.Now,
                        User = user,
                        Level = LogLevel.Error.ToString(),
                        Type = type.ToString(),
                        Action = action.ToString(),
                        Details = exception.Message,
                    });

                    // Enregistrement de l'inner exception
                    if (exception.InnerException != null)
                    {
                        db.Log.Add(new Log()
                        {
                            Date = DateTime.Now,
                            User = user,
                            Level = LogLevel.Error.ToString(),
                            Type = type.ToString(),
                            Action = action.ToString(),
                            Details = exception.Message,
                        });
                    }

                    // Enregistrement des erreurs entity
                    if (exception is DbEntityValidationException)
                    {
                        if (exception is DbEntityValidationException entityException)
                        {
                            foreach (DbEntityValidationResult eve in entityException.EntityValidationErrors)
                            {
                                foreach (DbValidationError ve in eve.ValidationErrors)
                                {
                                    db.Log.Add(new Log()
                                    {
                                        Date = DateTime.Now,
                                        User = user,
                                        Level = LogLevel.Error.ToString(),
                                        Type = type.ToString(),
                                        Action = action.ToString(),
                                        Details = ve.ErrorMessage,
                                    });
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception localException)
            {
                if (!type.Equals(LogType.Log))
                {
                    InsertLog(LogType.Log, LogAction.Add, user, localException);
                }
            }
        }

        #endregion
    }
}