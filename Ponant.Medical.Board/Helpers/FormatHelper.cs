using Ponant.Medical.Board.Data;
using System.Linq;

namespace Ponant.Medical.Board.Helpers
{
    public static class FormatHelper
    {
        #region PassengerFormatComments
        /// <summary>
        /// Format les commentaires du passager à afficher
        /// </summary>
        /// <param name="passenger">Passager courrant</param>
        /// <returns>Commentaire formaté</returns>
        public static string PassengerFormatComments(this Passenger passenger)
        {
            string formatedComments = passenger.ReviewBoard;

            if (!string.IsNullOrWhiteSpace(passenger.Review))
            {
                formatedComments += string.IsNullOrWhiteSpace(formatedComments)
                    ? passenger.Review
                    : string.Concat("\n - ", passenger.Review);
            }

            if (passenger.Information.Count > 0)
            {
                formatedComments += string.IsNullOrWhiteSpace(formatedComments)
                    ? "- "
                    : "\n - ";

                formatedComments += string.Join("\n - ", (from info in passenger.Information orderby info.Lov.Name ascending select info.Lov.Name)
               .Distinct()
               .ToList());
            }

            return formatedComments;
        }
        #endregion
    }
}
