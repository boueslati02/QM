using System;
using System.Globalization;
using System.Windows.Data;

namespace Ponant.Medical.Board.Converters
{
    /// <summary>
    /// Convertisseur de gestion de la barre de progression
    /// </summary>
    public class ProgressReportConverter : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int received = (int)values[0];
                int available = (int)values[1];
                if (available > 0)
                {
                    float value = 100 * (float)received / available;
                    return Math.Round(value);
                }
                return 0;
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
