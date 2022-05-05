using Ponant.Medical.Data.Shore;
using System;
using System.Web;
using System.Web.Http.Filters;

namespace Ponant.Medical.WebServices.Filters
{
    /// <summary>
    /// Filtre permettant de tracer les erreurs dans la base de données
    /// </summary>
    public class TraceFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Trace des erreurs lors de la levée d'une exception
        /// </summary>
        /// <param name="actionExecutedContext">Contexte qui a déclenché l'erreur</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            using (ShoreEntities db = new ShoreEntities())
            {
                db.Log.Add(new Log()
                {
                    Action = actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                    Date = DateTime.Now,
                    Details = actionExecutedContext.Exception.Message,
                    Level = "Error",
                    Type = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
                    User = HttpContext.Current.User.Identity.Name
                });

                if (actionExecutedContext.Exception.InnerException != null)
                {
                    db.Log.Add(new Log()
                    {
                        Action = actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                        Date = DateTime.Now,
                        Details = actionExecutedContext.Exception.InnerException.Message,
                        Level = "Error",
                        Type = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
                        User = HttpContext.Current.User.Identity.Name
                    });
                }
                
                db.SaveChanges();
            }

            base.OnException(actionExecutedContext);
        }
    }
}