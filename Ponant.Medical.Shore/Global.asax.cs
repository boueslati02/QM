using Ponant.Medical.Common;
using Ponant.Medical.Shore.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ponant.Medical.Shore
{
    public class MvcApplication : HttpApplication
    {
        #region Application_Start
        protected void Application_Start(Object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcGridConfig.RegisterGrids();
            CreateApplicationTree();
        }
        #endregion

        #region Application_Error
        protected void Application_Error(Object sender, EventArgs e)
        {
            LogManager.InsertLog(LogManager.LogType.Common, LogManager.LogAction.Common, HttpContext.Current.User.Identity.Name, Server.GetLastError());
        }
        #endregion

        #region Private

        /// <summary>
        /// Creation de l'arborescence de l'application lors de son lancement
        /// </summary>
        private void CreateApplicationTree()
        {
            try
            {
                // Création des répertoires
                Directory.CreateDirectory(AppSettings.FolderMail);
                Directory.CreateDirectory(AppSettings.FolderMailIndividual);
                Directory.CreateDirectory(AppSettings.FolderMailGroup);

                Directory.CreateDirectory(AppSettings.FolderSurveyIndividual);
                Directory.CreateDirectory(AppSettings.FolderSurveyGroup);

                Directory.CreateDirectory(AppSettings.FolderTemp);
                Directory.CreateDirectory(AppSettings.FolderPassenger);
                Directory.CreateDirectory(AppSettings.FolderAvailable);

                // Copie des fichiers
                Dictionary<string, byte[]> resources = new Dictionary<string, byte[]>
                {
                    { "AccountPassword.msg", Resources.AccountPassword },
                    { "AccountUserName.msg", Resources.AccountUserName },
                    { "NewUserName.msg", Resources.NewUserName },
                    { "NotifyAdvice.msg", Resources.NotifyAdvice },
                    { "ResetPassword.msg",  Resources.ResetPassword },
                    { "SendCode.msg", Resources.SendCode },
                    { "SummaryMail.msg",  Resources.SummaryMail },
                    { "UnfavorableAdvice.msg", Resources.UnfavorableAdvice }
                };

                foreach (KeyValuePair<string, byte[]> file in resources)
                {
                    string path = Path.Combine(AppSettings.FolderMail, file.Key);

                    if (!File.Exists(path))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            fs.Write(file.Value, 0, file.Value.Length);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Directory, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Create Directories Application (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
            }
        }

        #endregion
    }
}
