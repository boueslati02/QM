namespace Ponant.Medical.Shore.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection;
    using System.Web;

    #region MaxFileSizeAttribute
    /// <summary>
    /// Attribut de validation pour la taille d'un fichier
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize * 1024; // Passage en Ko
        }

        public override bool IsValid(object value)
        {
            if (!(value is HttpPostedFileBase file))
            {
                return true;
            }
            return file.ContentLength <= _maxFileSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage((_maxFileSize / 1024).ToString());
        }
    }
    #endregion

    #region NotEqualAttribute
    /// <summary>
    /// Attribut de validation pour la comparaison différente entre des propriétés
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotEqualAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The new password must be different from the old one.";
        public string BasePropertyName { get; private set; }
        public NotEqualAttribute(string basePropertyName)
            : base(DefaultErrorMessage)
        {
            BasePropertyName = basePropertyName;
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, BasePropertyName);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(BasePropertyName);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format(
                        CultureInfo.CurrentCulture, "{0} is invalid property", BasePropertyName
                    )
                );
            }

            object otherValue = property.GetValue(validationContext.ObjectInstance, null);
            if (object.Equals(value, otherValue))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
    #endregion
}