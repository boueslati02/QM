using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Common;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.Model
{
    public class LovModel
    {
        /// <summary>
        /// Comparaison des Lov shore et BDVérification existance d
        /// </summary>
        /// <param name="lst">Liste de Lov Webservice base Shore</param>
        /// <returns></returns>
        #region Getlov
        public static async Task UpdateLov()
        {
            using (BoardEntities db = new BoardEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Resultat de la liste Lov charger de la shore
                        List<Medical.Data.Shore.Lov> lstLov = await ShoreService.Instance.GetLov();

                        // Test si la liste est différent de null

                        foreach (Medical.Data.Shore.Lov lovShore in lstLov)
                        {
                            Data.Lov lovBoard = db.Lov.FirstOrDefault(l => l.Id.Equals(lovShore.Id));

                            if (lovBoard == null)
                            {
                                // Insert
                                db.Lov.Add(new Data.Lov
                                {
                                    Id = lovShore.Id,
                                    IdLovType = lovShore.IdLovType,
                                    Code = lovShore.Code,
                                    Name = lovShore.Name,
                                    IsEnabled = lovShore.IsEnabled,
                                    Creator = lovShore.Creator,
                                    CreationDate = lovShore.CreationDate,
                                    Editor = lovShore.Editor,
                                    ModificationDate = lovShore.ModificationDate
                                });
                            }

                            else
                            {
                                // Update
                                lovBoard.IdLovType = lovShore.IdLovType;
                                lovBoard.Code = lovShore.Code;
                                lovBoard.Name = lovShore.Name;
                                lovBoard.IsEnabled = lovShore.IsEnabled;
                                lovBoard.Creator = lovShore.Creator;
                                lovBoard.CreationDate = lovShore.CreationDate;
                                lovBoard.Editor = lovShore.Editor;
                                lovBoard.ModificationDate = lovShore.ModificationDate;
                            }
                            db.SaveChanges();

                        }
                            transaction.Commit();
                    }


                    catch (Exception exception)
                    {
                        transaction.Rollback();

                        LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Lov, LogManager.LogAction.Integration, AppSettings.UserName, exception.Message);

                        if (exception.InnerException != null)
                        {
                            LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Lov, LogManager.LogAction.Integration, AppSettings.UserName, exception.InnerException.Message);
                        }
                    }
                }
            }
        }
    }
}

#endregion
