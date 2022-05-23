using Ponant.Medical.Common;
using Ponant.Medical.Service.Class;
using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace Ponant.Medical.Service
{
    public partial class Service : ServiceBase
    {
        #region Constants
        /// <summary>
        /// Filtre de fichier
        /// </summary>
        private const string FILE_FILTER = "*.xml";
        #endregion

        #region Properties
        /// <summary>
        /// Composant permettant de scruter les actions sur le système de fichier
        /// </summary>
        private FileSystemWatcher watcher;

        /// <summary>
        /// Composant permettant de déclencher les actions journalières
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// Instance d'intégration des données
        /// </summary>
        private DataIntegration data;

        /// <summary>
        /// Instance d'envoi des mails du service
        /// </summary>
        private MailService mailService;

        /// <summary>
        /// Instance de nettoyage des documents médicaux
        /// </summary>
        private Survey survey;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialise les composants
        /// </summary>
        public Service()
        {
            InitializeComponent();
            data = new DataIntegration();
            survey = new Survey();
            mailService = new MailService();



            // Wire up the UnhandledExcepetion event of the current AppDomain.  
            // This will fire any time an undandled exception is thrown
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
        }

        /// <summary>
        /// Gestion des erreurs non gérées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Cache the unhandled exception and begin a shutdown of the service
            LogManager.InsertLog(LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, e.ExceptionObject as Exception);
        }
        #endregion

        #region Events service
        /// <summary>
        /// Paramétrage et initialisation des composants
        /// </summary>
        /// <param name="args">Argument de démarrage du service</param>
        protected override void OnStart(string[] args)
        {
            // Création des répertoires
            Directory.CreateDirectory(AppSettings.FolderPonantBooking);
            Directory.CreateDirectory(AppSettings.FolderPonantBookingError);
            Directory.CreateDirectory(AppSettings.FolderShoreBooking);

            // Lancement du watcher
            watcher = new FileSystemWatcher
            {
                InternalBufferSize = 1000000,
                Path = AppSettings.FolderPonantBooking,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Filter = FILE_FILTER,
                EnableRaisingEvents = true
            };
            watcher.Error += Watcher_Error;
            watcher.Created += Watcher_Created;

            // Schedule to run once a day at 9:00 a.m.
            timer = new Timer
            {
                Interval = DateTime.Today.AddDays(1).AddHours(AppSettings.TimerHour).Subtract(DateTime.Now).TotalSeconds * 1000,
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;

            // Reprise des fichiers non traités dans le répertoire
            foreach (string filename in Directory.EnumerateFiles(AppSettings.FolderPonantBooking))
            {
                Watcher_Created(null, new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(filename), Path.GetFileName(filename)));
            }


        }
        #endregion

        #region Events components
        /// <summary>
        /// Erreur du composant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            LogManager.InsertLog(LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, e.GetException());
        }

        /// <summary>
        /// Lorsqu'un fichier est déposé
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private async void Watcher_Created(object source, FileSystemEventArgs e)
        {
            // Boucle pour attendre la fin de la copie du fichier
            for (int i = 1; i <= AppSettings.NumberOfRetries; i++)
            {
                try
                {
                    // Do stuff with file
                    if (File.Exists(e.FullPath))
                    {
                        FileInfo file = new FileInfo(e.FullPath);

                        // Lève une exception si le fichier n'a pas fini d'être copié
                        using (file.Open(FileMode.Open, FileAccess.Read, FileShare.None))

                        // Traitement habituel d'intégration des fichiers
                        data.FileName = e.FullPath;

                        bool isProcessIncomingFile = data.ProcessIncomingFile();
                        if (isProcessIncomingFile && data.IsIntegrated)
                        {
                            int? bookingNumber = data.GetBookingNumber();
                            if (bookingNumber.HasValue)
                            {
                                await survey.Sent(bookingNumber.Value, false, data.IsLanguageUpdated);
                            }
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }

                    break; // When done we can break loop
                }
                catch
                {
                    // Last one, (re)throw exception and exit
                    if (i == AppSettings.NumberOfRetries)
                    {
                        throw;
                    }

                    System.Threading.Thread.Sleep(AppSettings.DelayOnRetry);
                }
            }
        }

        /// <summary>
        /// Lorsque l'intervalle du timer est déclenché
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            survey.Associate();
            survey.Clean();

            mailService.SendDeadline();
            mailService.SendSummary(AppSettings.FirstSummaryMail, LogManager.LogType.SummaryMail1);
            mailService.SendSummary(AppSettings.SecondSummaryMail, LogManager.LogType.SummaryMail2);
            mailService.SendReminder();
            mailService.SendQmReceived();

            // If tick for the first time, reset next run to every 24 hours
            if (timer.Interval != 24 * 60 * 60 * 1000)
            {
                timer.Interval = 24 * 60 * 60 * 1000;
            }
        }
        #endregion
    }
}