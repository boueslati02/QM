﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ponant.Medical.Board.Converters
{
    /// <summary>
    /// Convertisseur de gestion de la visibilité
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
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
            if(value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
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
