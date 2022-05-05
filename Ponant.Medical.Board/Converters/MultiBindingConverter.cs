using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ponant.Medical.Board.Converters
{
    /// <summary>
    /// Convertisseur permettant de lier plusieurs champs
    /// </summary>
    [MarkupExtensionReturnType(typeof(MultiBindingConverter))]
    public class MultiBindingConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public static MultiBindingConverter converter = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == converter)
            {
                converter = new MultiBindingConverter();
            }
            return converter;
        }

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
            return values.ToList();
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