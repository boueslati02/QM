using System;
using System.Windows;
using System.Windows.Controls;

namespace Ponant.Medical.Board.Extended
{
    /// <summary>
    /// Permet de gérer une ligne personnalisée
    /// </summary>
    /// <example>Visibilité de la ligne affichant la barre de progression</example>
    public class RowDefinitionExtended : RowDefinition
    {
        #region Attributes
        public static DependencyProperty VisibleProperty;
        #endregion

        #region Accessors
        public Boolean Visible
        {
            get { return (Boolean)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructeur
        /// </summary>
        static RowDefinitionExtended()
        {
            VisibleProperty = DependencyProperty.Register("Visible",
                typeof(Boolean),
                typeof(RowDefinitionExtended),
                new PropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged)));

            RowDefinition.HeightProperty.OverrideMetadata(typeof(RowDefinitionExtended),
                new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null,
                    new CoerceValueCallback(CoerceHeight)));

            RowDefinition.MinHeightProperty.OverrideMetadata(typeof(RowDefinitionExtended),
                new FrameworkPropertyMetadata((Double)0, null,
                    new CoerceValueCallback(CoerceMinHeight)));
        }
        #endregion

        #region Private Static Methods
        static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(RowDefinition.HeightProperty);
            obj.CoerceValue(RowDefinition.MinHeightProperty);
        }
        static Object CoerceHeight(DependencyObject obj, Object nValue)
        {
            return (((RowDefinitionExtended)obj).Visible) ? nValue : new GridLength(0);
        }
        static Object CoerceMinHeight(DependencyObject obj, Object nValue)
        {
            return (((RowDefinitionExtended)obj).Visible) ? nValue : (Double)0;
        }
        #endregion

        #region Public Static Methods
        // Get/Set
        public static void SetVisible(DependencyObject obj, Boolean nVisible)
        {
            obj.SetValue(VisibleProperty, nVisible);
        }
        public static Boolean GetVisible(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(VisibleProperty);
        }
        #endregion
    }
}
