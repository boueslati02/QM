using Ponant.Medical.Board.Data;
using Ponant.Medical.Common;
using System;
using System.Windows;

namespace Ponant.Medical.Board.Helpers
{
    /// <summary>
    /// Permet de tracer les actions sur l'application
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Enregistre en base 
        /// </summary>
        /// <param name="level">Niveau d'erreur</param>
        /// <param name="details">Détails de l'erreur</param>
        public static void Log(string level, string type, string action, string details)
        {
            using (BoardEntities db = new BoardEntities())
            {
                // Log
                db.Log.Add(new Log()
                {
                    Date = DateTime.Now,
                    User = Application.Current.Properties[AppSettings.UserName].ToString(),
                    Level = level,
                    Type = type,
                    Action = action,
                    Details = details,
                });


                db.SaveChanges();
            }
        }

        /// <summary>
        /// Enregistre en base les exceptions
        /// </summary>
        /// <param name="exception">Instance de l'exception</param>
        public static void Log(string type, string action, Exception exception)
        {
            using (BoardEntities db = new BoardEntities())
            {
                // Log l'exception
                db.Log.Add(new Log()
                {
                    Date = DateTime.Now,
                    User = Application.Current.Properties[AppSettings.UserName].ToString(),
                    Level = LogManager.LogLevel.Error.ToString(),
                    Type = type,
                    Action = action,
                    Details = exception.GetBaseException().Message
                });

                db.SaveChanges();
            }
        }
    }
}
