using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Classe des méthodes communes des utilisateurs
    /// </summary>
    public class UserHelper
    {
        #region CalculateHash
        /// <summary>
        /// Crypte le mot de passe
        /// </summary>
        /// <param name="password">Mot de passe</param>
        /// <returns>Mot de passe crypté</returns>
        public static string CalculateHash(string password)
        {
            return new PasswordHasher().HashPassword(password);
        }
        #endregion

        #region CheckPassword
        /// <summary>
        /// Vérifie le mot de passe en clair avec le mot de passe haché
        /// </summary>
        /// <param name="passwordHash">Mot de passe haché</param>
        /// <param name="password">Mot de passe en clair</param>
        /// <returns></returns>
        public static bool CheckPassword(string passwordHash, string password)
        {
            return (new PasswordHasher().VerifyHashedPassword(passwordHash, password) != PasswordVerificationResult.Failed);
        }
        #endregion

        #region CreateRandomPassword
        /// <summary>
        /// Génère un mot de passe selon les critères de complexité définis en paramètres
        /// </summary>
        /// <param name="isRequireDigit">Le mot de passe doit contenir des chiffres</param>
        /// <param name="isRequireLowercase">Le mot de passe doit contenir des minuscules</param>
        /// <param name="isRequireNonLetterOrDigit">Le mot de passe doit contenir des caractères spéciaux</param>
        /// <param name="isRequireUppercase">Le mot de passe doit contenir des majuscules</param>
        /// <param name="requireLength">Longueur minimale du mot de passe</param>
        /// <returns>Nouveau mot de passe</returns>
        public static string CreateRandomPassword(bool isRequireDigit, bool isRequireLowercase, bool isRequireNonLetterOrDigit, bool isRequireUppercase, int requireLength)
        {
            string allowedChars = string.Empty;

            if (isRequireDigit)
            {
                allowedChars += "0123456789";
            }

            if (isRequireLowercase)
            {
                allowedChars += "abcdefghijkmnopqrstuvwxyz";
            }

            if (isRequireNonLetterOrDigit)
            {
                allowedChars += "!@$?_-";
            }

            if (isRequireUppercase)
            {
                allowedChars += "ABCDEFGHJKLMNOPQRSTUVWXYZ9";
            }

            char[] chars = new char[requireLength];
            RandomNumberGenerator random = RandomNumberGenerator.Create();

            for (int i = 0; i < requireLength; i++)
            {
                byte[] randomNumber = new byte[sizeof(int)];
                random.GetNonZeroBytes(randomNumber);
                int value = (BitConverter.ToInt32(randomNumber, 0));

                int min = 0;
                int max = allowedChars.Length - 1;
                int index = ((value - min) % (max - min + 1) + (max - min + 1)) % (max - min + 1) + min;

                chars[i] = allowedChars[index];
            }

            return new string(chars);
        }
        #endregion

        #region GetUserId
        public static string GetUserId(string username)
        {
            using (Data.Auth.AuthContext db = new Data.Auth.AuthContext())
            {
                return db.AspNetUsers.First(u => u.UserName == username)?.Id;
            }
        }
        #endregion

#if DEV
        #region ReinitPassword
        /// <summary>
        /// Réinitialisation des mots de passe Bord
        /// </summary>
        public static void ReinitPassword()
        {
            Data.Auth.AspNetUsers user = null;

            using (Data.Auth.AuthContext db = new Data.Auth.AuthContext())
            {
                user = db.AspNetUsers.Find("32ced21e-bb46-478c-b8cb-a5449fa39977");
                user.PasswordHashInit = user.PasswordHash = CalculateHash("Boreal17");
                db.SaveChanges();

                user = db.AspNetUsers.Find("5e27ee81-331c-460f-b479-de57d4ae6c20");
                user.PasswordHashInit = user.PasswordHash = CalculateHash("Austral17");
                db.SaveChanges();

                user = db.AspNetUsers.Find("bd208bf2-84fa-4652-9df3-3ff1b06dc960");
                user.PasswordHashInit = user.PasswordHash = CalculateHash("Soleal17");
                db.SaveChanges();

                user = db.AspNetUsers.Find("e9c8df5f-4886-463d-94c4-0ecd712de0b6");
                user.PasswordHashInit = user.PasswordHash = CalculateHash("Ponant17");
                db.SaveChanges();

                user = db.AspNetUsers.Find("f3720841-a291-4430-a6d3-6e37bf89c70d");
                user.PasswordHashInit = user.PasswordHash = CalculateHash("Lyrial17");
                db.SaveChanges();
            }
        }
        #endregion
#endif
    }
}