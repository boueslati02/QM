using iTextSharp.text;
using iTextSharp.text.pdf;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using QRCodeEncoderLibrary;
using System;
using System.IO;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Classe de traitement des fichier pdf
    /// </summary>
    public static class Pdf
    {
        #region MergePdf
        /// <summary>
        /// Remplissage des champs de fusion du PDF
        /// </summary>
        /// <param name="sourcePath">Nom du fichier à fusionné</param>
        /// <param name="destinationPath">Chemin du fichier fusionné</param>
        /// <param name="folderTemp">Chemin du dossier temporaire</param>
        /// <param name="bookingCruisePassenger">Données du passager à fusionné</param>
        public static void MergePdf(string sourcePath, string destinationPath, string folderTemp, BookingCruisePassenger bookingCruisePassenger)
        {
            using (PdfReader pdfReader = new PdfReader(sourcePath))
            {
                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(destinationPath, FileMode.Create), '\0', true))
                {
                    SetCustomField(pdfStamper, bookingCruisePassenger);

                    string qrCodePath = EncodeQRCode(bookingCruisePassenger.IdPassenger, folderTemp);
                    if (!string.IsNullOrWhiteSpace(qrCodePath))
                    {
                        MergeQRCodeToPdf(pdfStamper, qrCodePath);
                        if (File.Exists(qrCodePath))
                        {
                            File.Delete(qrCodePath);
                        }
                    }
                }
            }
        }
        #endregion

        #region Private

        /// <summary>
        /// Alimentation des champs de fusion du PDF
        /// </summary>
        /// <param name="pdfStamper">Pdf Stamper</param>
        /// <param name="bookingCruisePassenger">Booking Cruise Passenger</param>
        private static void SetCustomField(PdfStamper pdfStamper, BookingCruisePassenger bookingCruisePassenger)
        {
            string dateFormat = "M/d/yyyy";
            if(bookingCruisePassenger.Booking.IdLanguage == Constants.LANGUAGE_FRANCOPHONE)
            {
                dateFormat = "dd/MM/yyyy";
            }
           
            AcroFields form = pdfStamper.AcroFields;
            form.SetField(CommonSettings.TagAdvice, bookingCruisePassenger.Passenger.LovAdvice.Name);
            form.SetField(CommonSettings.TagBirthDate, bookingCruisePassenger.Passenger.BirthDate.HasValue ? bookingCruisePassenger.Passenger.BirthDate.Value.ToString(dateFormat) : null);
            form.SetField(CommonSettings.TagBooking, bookingCruisePassenger.Booking.Number.ToString());
            form.SetField(CommonSettings.TagCabin, bookingCruisePassenger.CabinNumber);
            form.SetField(CommonSettings.TagCruise, bookingCruisePassenger.Cruise.Code);
            form.SetField(CommonSettings.TagFirstName, bookingCruisePassenger.Passenger.FirstName);
            form.SetField(CommonSettings.TagGroup, bookingCruisePassenger.Booking.GroupName);
            form.SetField(CommonSettings.TagLastName, bookingCruisePassenger.Passenger.LastName);
            form.SetField(CommonSettings.TagSailingDate, bookingCruisePassenger.Cruise.SailingDate.ToString(dateFormat));
            form.SetField(CommonSettings.TagShip, bookingCruisePassenger.Cruise.LovShip.Name);
            form.SetField(CommonSettings.TagUsualName, !string.IsNullOrWhiteSpace(bookingCruisePassenger.Passenger.UsualName)
                        ? bookingCruisePassenger.Passenger.UsualName
                        : bookingCruisePassenger.Passenger.LastName);
        }

        /// <summary>
        /// Création de l'image du QR Code
        /// </summary>
        /// <param name="idPassenger">Id Passenger</param>
        /// <param name="folderTemp">Folder Temp</param>
        /// <returns>QR Code path</returns>
        private static string EncodeQRCode(int idPassenger, string folderTemp)
        {
            try
            {
                string qrCodePath = Path.Combine(folderTemp, string.Concat(idPassenger.ToString(), "_QRCode.png"));
                QREncoder qrEncoder = new QREncoder
                {
                    ModuleSize = 5
                };
                qrEncoder.Encode(idPassenger.ToString());
                qrEncoder.SaveQRCodeToPngFile(qrCodePath);
                return qrCodePath;
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Intégration de l'image du QR Code dans chaque page du QM
        /// </summary>
        /// <param name="pdfStamper">PdfStamper</param>
        /// <param name="qrCodePath">QR Code path</param>
        private static void MergeQRCodeToPdf(PdfStamper pdfStamper, string qrCodePath)
        {
            int xPosition = 475;
            int yPosition = 682;
            int imageWidth = 110;

            int numberOfPage = pdfStamper.Reader.NumberOfPages;
            for (int pageIndex = 1; pageIndex <= numberOfPage; pageIndex++)
            {
                PdfContentByte pdfContentByte = pdfStamper.GetOverContent(pageIndex);
                Image image = Image.GetInstance(qrCodePath);
                image.ScaleAbsolute(imageWidth, imageWidth);
                image.SetAbsolutePosition(xPosition, yPosition);
                pdfContentByte.AddImage(image);
            }
        }

        #endregion
    }
}
