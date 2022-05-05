using System;
using System.Globalization;
using System.Windows.Data;

namespace Ponant.Medical.Board.Converters
{
    /// <summary>
    /// Convertisseur de gestion des types de document
    /// </summary>
    public class DocumentTypeGroupNameConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                string documenttypename = (string)value;
                return documenttypename.Substring(documenttypename.IndexOf('_') + 1);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
