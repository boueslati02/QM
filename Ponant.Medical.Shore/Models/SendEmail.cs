using Newtonsoft.Json;
using Ponant.Medical.Common;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using Ponant.Medical.Data.Shore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ponant.Medical.Shore.Models
{
    public class SendEmail : SharedClass
    {
        private readonly IShoreEntities _shoreEntities;
        public SendEmail(IShoreEntities shoreEntities) : base(shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }
        public Survey GetSurveyById(int Id)
        {
            return _shoreEntities.Survey.Where(s => s.Id == Id).FirstOrDefault();
        }
        public string EncryptString(string plainText)
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public string DecryptString(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public void SendHtmlMail(UserToken userToken)
        {
            try
            {
                string SerializeuserToken = JsonConvert.SerializeObject(userToken);
                string TokenGenerated = EncryptString(SerializeuserToken);
                MailMessage mail = new MailMessage();
                string body = string.Empty;
                Lov lov = _shoreEntities.Lov.Where(l => l.Name.Contains(userToken.Language)).FirstOrDefault();
                string BouttonText = "Confirm Your Medical Question";
                string SujectText = "Confirm Medical Question";
                if (userToken.Language.ToUpper().Contains("FRANC"))
                {
                    BouttonText = "Confirmer Votre Questionnnaire médical";
                    SujectText = "Confirmation Questionnaire médicale";
                }else if (userToken.Language.ToUpper().Contains("ALLEMA"))
                {
                    BouttonText = "Ihren medizinischen Fragebogen bestätigen";
                    SujectText = "Bestätigung Medizinischer Fragebogen";
                }
                string content = _shoreEntities.Language.Where(l => l.IdLanguage == lov.Id && l.IdSurvey == 11).FirstOrDefault().EmailFormat;
                if (String.IsNullOrEmpty(content))
                {
                    body = _shoreEntities.Language.Where(l => l.IsDefault == true).FirstOrDefault().EmailFormat;
                }
                else
                {
                    body = content;
                }
                body = body.Replace("{FirstName}", userToken.FirstName);
                body = body.Replace("{LastName}", userToken.LastName);
                body = body.Replace("{CruiseNumber}", userToken.CruiseNumber);
                var request = HttpContext.Current.Request;
                string baseUrl = request.Url.Scheme + "://" + request.Url.Authority;
                body = body + "<br><br><br><a href='" + baseUrl + "/Email/ValidQM?token=" + TokenGenerated + "' title='Titre' style='padding: 10px 15px;border-radius: 100px;border-style: solid;border-width: 1px;color: #fff;text-align: center;font-weight: 800;border-color: #26afc2;background-color: #26afc2;'>"+ BouttonText + "</a> ";
                mail.To.Add(userToken.Email);
                mail.From = new MailAddress("boueslati-ext@ponant.com");
                mail.Subject = SujectText;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.outlook.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("boueslati-ext@ponant.com", "6azerty*");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public bool ValidQM(int IdPassager)
        {
            
            try
            {
                Passenger passenger = _shoreEntities.Passenger.Find(IdPassager);
                passenger.IdAdvice = Constants.ADVICE_FAVORABLE_OPINION;
                _shoreEntities.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
            
        }
        public void SendConfirmMail(UserToken userToken)
        {
            try
            {
                MailMessage mail = new MailMessage();
                string body = string.Empty;
                Lov lov = _shoreEntities.Lov.Where(l => l.Name.Contains(userToken.Language)).FirstOrDefault();
                string SujectText = "Confirm Medical Question";
                body = "{FirstName} {LastName} <br> Thank you for confirming your cruise QM {CruiseNumber} <br> Sincerely.";
                if (userToken.Language.ToUpper().Contains("FRANC"))
                {
                    SujectText = "Confirmation questionnaire médicale";
                    body = "{FirstName} {LastName} <br> Merci d’avoir confirmé votre croisière QM {CruiseNumber} <br> Cordialement.";
                }
                else if (userToken.Language.ToUpper().Contains("ALLEMA"))
                {
                    SujectText = "Medizinische Frage bestätigen";
                    body = "{FirstName} {LastName} <br> Vielen Dank für die Bestätigung Ihrer Kreuzfahrt QM {CruiseNumber} <br> Mit freundlichen Grüßen.";
                }
                body = body.Replace("{FirstName}", userToken.FirstName);
                body = body.Replace("{LastName}", userToken.LastName);
                body = body.Replace("{CruiseNumber}", userToken.CruiseNumber);
                mail.To.Add(userToken.Email);
                mail.From = new MailAddress("boueslati-ext@ponant.com");
                mail.Subject = SujectText;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.outlook.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("boueslati-ext@ponant.com", "6azerty*");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}