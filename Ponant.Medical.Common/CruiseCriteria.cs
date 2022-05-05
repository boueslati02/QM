using Ponant.Medical.Data.Shore;
using System.Collections.Generic;
using System.Linq;

namespace Ponant.Medical.Common
{
    public class CruiseCriteria
    {
        #region Properties & Constructors

        /// <summary>
        /// Shore Entities
        /// </summary>
        private readonly IShoreEntities _shoreEntities;

        /// <summary>
        /// Document Controller
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        public CruiseCriteria(IShoreEntities shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }

        #endregion

        #region GetCriteria
        /// <summary>
        /// Obtient le critère à appliquer pour envoyer le questionnaire
        /// </summary>
        /// <param name="bookingCruisePassengers">Liste des booking cruise passenger</param>
        /// <returns>Vrai si le fichier est intégrable, faux sinon</returns>
        public CruiseCriterion GetCriteria(IEnumerable<BookingCruisePassenger> bookingCruisePassengers)
        {
            IEnumerable<CruiseCriterion> criterionFinalList = new List<CruiseCriterion>();

            IQueryable<CruiseCriterion> criterionBase = _shoreEntities.CruiseCriterion
                .Include("Survey")
                .Include("Survey.Language")
                .Include("CruiseCriterionDestination")
                .Include("CruiseCriterionShip");

            foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisePassengers)
            {
                IQueryable<CruiseCriterion> criterionCruise = criterionBase
                    .Where(c => (c.LovCruiseType.Name.Equals(bookingCruisePassenger.Cruise.LovTypeCruise.Name) || c.IdCruiseType == 0)
                        && (c.CruiseCriterionDestination.Any(d => d.LovDestination.Code.Equals(bookingCruisePassenger.Cruise.LovDestination.Code)) || !c.CruiseCriterionDestination.Any())
                        && (c.CruiseCriterionShip.Any(s => s.LovShip.Code.Equals(bookingCruisePassenger.Cruise.LovShip.Code)) || !c.CruiseCriterionShip.Any())
                        && (bookingCruisePassenger.Cruise.SailingLengthDays > c.Length || !c.Length.HasValue)
                        && (c.Cruise.Contains(bookingCruisePassenger.Cruise.Code) || string.IsNullOrEmpty(c.Cruise)));

                IEnumerable<BookingActivity> bookingActivities = bookingCruisePassenger.Booking.BookingActivity
                    .Where(a => a.IdPassenger == bookingCruisePassenger.IdPassenger);

                if (bookingActivities != null && bookingActivities.Any())
                {
                    IQueryable<CruiseCriterion> criterionActivity;
                    foreach (BookingActivity bookingActivity in bookingActivities)
                    {
                        criterionActivity = criterionCruise
                            .Where(c => c.Activity.Contains(bookingActivity.LovActivity.Code) || string.IsNullOrEmpty(c.Activity));

                        criterionFinalList = criterionFinalList.Union(criterionActivity);
                    }
                }
                else
                {
                    criterionFinalList = criterionFinalList
                        .Union(criterionCruise
                            .Where(c => string.IsNullOrEmpty(c.Activity)));
                }
            }

            CruiseCriterion criterionValue = criterionFinalList.OrderBy(c => c.Order).FirstOrDefault();
            return criterionValue;
        }
        #endregion
    }
}
