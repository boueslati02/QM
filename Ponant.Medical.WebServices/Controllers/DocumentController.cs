using Ponant.Medical.Common;
using Ponant.Medical.Common.Interfaces;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Ponant.Medical.WebServices.Controllers
{
    /// <summary>
    /// Gestion des ressources de documents
    /// </summary>
    [Authorize]
    public class DocumentController : ApiController
    {
        #region Properties & Constructors

        private readonly IShoreEntities _shoreEntities;

        private readonly IFileHelper _fileHelper;

        /// <summary>
        /// Document Controller
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        /// <param name="fileHelper">File Helper</param>
        public DocumentController(IShoreEntities shoreEntities, IFileHelper fileHelper)
        {
            _shoreEntities = shoreEntities;
            _fileHelper = fileHelper;
        }

        #endregion

        #region UnlinkDocument
        /// <summary>
        /// Détachement d'un document d'un passager
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <returns>Ok si le document a été correctement détaché</returns>
        [HttpPut]
        public IHttpActionResult UnlinkDocument(int id)
        {
            Document document = null;

            try
            {
                try
                {
                    document = _shoreEntities.Document.Single(d => d.Id.Equals(id));
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }

                // Déplacement du document dans le bac à sable
                Passenger passenger = document.Passenger;
                document.IdPassenger = Constants.NOT_APPLICABLE_NOT_APPLICABLE;
                document.ModificationDate = DateTime.Now;
                document.Editor = HttpContext.Current.User.Identity.Name;
                _shoreEntities.SaveChanges();

                // Déplacement physique du document dans le bac à sable
                string path = Path.Combine(AppSettings.FolderPassenger, document.ReceiptDate.ToString("yyyy-MM"), passenger.Id.ToString(), document.FileName);
                if (_fileHelper.FileExists(path))
                {
                    _fileHelper.MoveFile(path, Path.Combine(AppSettings.FolderSandBox, document.FileName));
                }

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Document, LogManager.LogAction.Unlink, HttpContext.Current.User.Identity.Name, string.Format("Détachement du document {0} du passager {1} {2}", document.Name, passenger.LastName, passenger.FirstName));
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Document, LogManager.LogAction.Unlink, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Ok();
        }
        #endregion
    }
}