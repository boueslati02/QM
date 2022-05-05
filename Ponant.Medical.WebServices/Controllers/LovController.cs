using Ponant.Medical.Common;
using Ponant.Medical.Data.Shore;
using Ponant.Medical.Data.Shore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Ponant.Medical.WebServices.Controllers
{
    [Authorize]
    public class LovController : ApiController
    {
        #region Properties & Constructors

        private readonly IShoreEntities _shoreEntities;

        /// <summary>
        /// Lov Controller
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        public LovController(IShoreEntities shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }

        #endregion

        #region GetLov
        /// <summary>
        /// Retourne Une list de Lov de la base Shore 
        /// </summary>
        /// <returns>List des lov de la base Shore</returns>
        [HttpGet]
        public IHttpActionResult GetLov()
        {
            List<LovBoard> lovs;

            try
            {
                lovs = _shoreEntities.Lov.Select(l => new LovBoard
                {
                    Id = l.Id,
                    IdLovType = l.IdLovType,
                    Code = l.Code,
                    Name = l.Name,
                    IsEnabled = l.IsEnabled,
                    Creator = l.Creator,
                    CreationDate = l.CreationDate,
                    Editor = l.Editor,
                    ModificationDate = l.ModificationDate
                }).ToList();
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Lov, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }
            return Json(lovs);
        }
        #endregion
    }
}