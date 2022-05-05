using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using PdfiumViewer;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace Ponant.Medical.Common
{
    /// <summary>
    /// Classe de gestion des traitements d'image
    /// </summary>
    public class Picture
    {
        #region Constants

        /// <summary>
        /// Extension JPG
        /// </summary>
        private const string EXTENSION_JPG = ".jpg";

        /// <summary>
        /// Extension PDF
        /// </summary>
        private const string EXTENSION_PDF = ".pdf";

        /// <summary>
        /// Extension TIF
        /// </summary>
        private const string EXTENSION_TIF = ".tif";

        /// <summary>
        /// Extension TIFF
        /// </summary>
        private const string EXTENSION_TIFF = ".tiff";

        /// <summary>
        /// Extension XPS
        /// </summary>
        private const string EXTENSION_XPS = ".xps";
        #endregion

        #region Properties & Constructors
        /// <summary>
        /// Chemin d'accès du fichier original
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Liste des fichiers JPG
        /// </summary>
        private List<string> _files;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="path">Chemin d'accès du fichier original</param>
        public Picture(string path)
        {
            _path = path;
        }

        #endregion

        #region Public methods

        #region ConvertToJpeg
        /// <summary>
        /// Convertit le fichier au format JPG
        /// </summary>
        /// <returns>La liste des fchiers JPG</returns>
        public List<string> ConvertToJpeg()
        {
            switch (Path.GetExtension(_path).ToLower())
            {
                case EXTENSION_PDF:
                    ConvertPdfToJpeg();
                    break;
                case EXTENSION_TIF:
                case EXTENSION_TIFF:
                    ConvertTifToJpeg();
                    break;
                case EXTENSION_XPS:
                    ConvertXpsToJpeg();
                    break;
                default:
                    _files = new List<string>
                    {
                        _path
                    };
                    break;
            }

            return _files;
        }
        #endregion

        #region Treat

        /// <summary>
        /// Retraite la qualité des images du document et les redimensionnes
        /// </summary>
        /// <returns>La liste des fchiers JPG optimisés</returns>
        public List<string> Treat()
        {
            if (_files != null)
            {
                foreach (string filename in _files)
                {
                    Treat(filename);
                }
            }

            return _files;
        }

        /// <summary>
        /// Retraite la qualité de l'image en paramètre et la redimensionne
        /// </summary>
        /// <param name="filename">Nom de l'image a retraité</param>
        public void Treat(string filename)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(_path), filename);
            byte[] bytes = File.ReadAllBytes(filePath);
            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 30 };

            Size size = new Size(1654, 2339);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    // Load, resize, set the format and quality and save an image.
                    imageFactory.Load(stream)
                                .Resize(size)
                                .Format(format)
                                .Save(filePath);
                }
            }
        }

        #endregion

        #endregion

        #region Private methods

        #region ConvertPdfToJpeg
        /// <summary>
        /// Convertit le fichier PDF au format JPG
        /// </summary>
        private void ConvertPdfToJpeg()
        {
            using (PdfDocument document = PdfDocument.Load(_path))
            {
                _files = new List<string>();

                for (int i = 0; i < document.PageCount; i++)
                {
                    Image image = document.Render(i, 2480, 3508, 300, 300, false);
                    string filename = Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + "_" + i + EXTENSION_JPG);

                    _files.Add(filename);
                    image.Save(filename, ImageFormat.Jpeg);
                }
            }

            File.Delete(_path);
        }
        #endregion

        #region ConvertXpsToJpeg
        /// <summary>
        /// Convertit le fichier XPS au format JPG
        /// </summary>
        private void ConvertXpsToJpeg()
        {
            Thread STAThread = new Thread(() =>
            {
                using (XpsDocument xps = new XpsDocument(_path, FileAccess.Read))
                {
                    FixedDocumentSequence sequence = xps.GetFixedDocumentSequence();
                    _files = new List<string>();

                    for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; pageCount++)
                    {
                        using (DocumentPage page = sequence.DocumentPaginator.GetPage(pageCount))
                        {
                            RenderTargetBitmap toBitmap = new RenderTargetBitmap(2480, 3508, 300, 300, PixelFormats.Default);
                            toBitmap.Render(page.Visual);

                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(toBitmap));

                            string filename = Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + "_" + pageCount + EXTENSION_JPG);
                            _files.Add(filename);

                            using (FileStream fStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                            {
                                encoder.Save(fStream);
                            }
                        }
                    }
                }

                File.Delete(_path);
            });

            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }
        #endregion

        #region ConvertTifToJpeg
        /// <summary>
        /// Convertit le fichier TIF au format JPG
        /// </summary>
        /// <returns></returns>
        private string[] ConvertTifToJpeg()
        {
            string[] jpegPaths = null;

            using (Image imageFile = Image.FromFile(_path))
            {
                FrameDimension frameDimensions = new FrameDimension(imageFile.FrameDimensionsList[0]);
                _files = new List<string>();

                // Gets the number of pages from the tiff image (if multipage) 
                int frameNum = imageFile.GetFrameCount(frameDimensions);
                jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    // Selects one frame at a time and save as jpeg. 
                    imageFile.SelectActiveFrame(frameDimensions, frame);

                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        jpegPaths[frame] = string.Format("{0}\\{1}{2}" + EXTENSION_JPG,
                            Path.GetDirectoryName(_path),
                            Path.GetFileNameWithoutExtension(_path),
                            frame);
                        _files.Add(jpegPaths[frame]);
                        bmp.Save(jpegPaths[frame], ImageFormat.Jpeg);
                    }
                }
            }

            File.Delete(_path);
            return jpegPaths;
        }
        #endregion

        #endregion
    }
}
