namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Data.Shore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Classe de méthode commune aux autres classes
    /// </summary>
    public class SharedClass
    {
        #region Properties & Constructors

        protected IShoreEntities _shoreEntities;

        public SharedClass(
            IShoreEntities shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }

        #endregion

        #region GetLovList
        /// <summary>
        /// Retourne une liste de paramétres
        /// </summary>
        /// <param name="idLovType">Identifiant du type de paramétre</param>
        /// <returns>Liste des paramétres</returns>
        public List<SelectListItem> GetLovList(int idLovType)
        {
            return (from lov in _shoreEntities.Lov.AsEnumerable()
                    where lov.IdLovType.Equals(idLovType) && lov.IsEnabled
                    orderby lov.Name ascending
                    select new SelectListItem()
                    {
                        Text = lov.Name,
                        Value = lov.Id.ToString()
                    }).ToList();
        }
        #endregion
    }
}