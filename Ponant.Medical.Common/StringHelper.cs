using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Helper pour les chaine de caractéres
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// Remplace les caractère accentué de la chaine en paramètre
        /// </summary>
        /// <param name="text">Chaine de caractére à traitée</param>
        /// <returns>Chaine traité</returns>
        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Remplace les caratéres spéciaux de la chaine passée en paramètre
        /// </summary>
        /// <param name="name">Chaine de caractère à traité</param>
        /// <returns>Chaine traité</returns>
        public static string CleanFileName(string name)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"\! | \@ | \# | \$ | \% | \^ | \& | \* | \( | \) | \+ | \= | \< | \> | \, | \/ | \\ | \? | \| | \{ | \} | \[ | \] | \: | \; | \' | \"" | \` | \~", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
            return regex.Replace(name, "_");
        }

        /// <summary>
        /// Remplace les caratéres invalide du nom de fichier passée en paramètre
        /// </summary>
        /// <param name="filename">Nom de fichier à traité</param>
        /// <returns>Nom de fichier traité</returns>
        public static string CleanInvalidChar(string filename)
        {
            string newFilename = filename;
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                newFilename = newFilename.Replace(invalidChar.ToString(), string.Empty);
            }
            return newFilename;
        }

        public static string ToBase64Encode(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }

        public static string ToBase64Decode(string base64EncodedText)
        {
            if (String.IsNullOrEmpty(base64EncodedText))
            {
                return base64EncodedText;
            }

            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedText);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsBase64String(string base64EncodedText)
        {
            base64EncodedText = base64EncodedText.Trim();
            return (base64EncodedText.Length % 4 == 0) && Regex.IsMatch(base64EncodedText, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

    }
}
