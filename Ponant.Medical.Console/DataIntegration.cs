using Ponant.Medical.Console.Properties;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Ponant.Medical.Console
{
    public class DataIntegration
    {
        private const string filefilter = "*.xml";
        private const string BookingFolder = @"C:\MedicalApplication\MedicalFiles\Booking";
        private const string BookingErrorFolder = @"C:\MedicalApplication\MedicalFiles\BookingError";

        FileSystemWatcher watcher;

        public DataIntegration()
        {
            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();
            watcher.Path = BookingFolder;
            // Watch for changes in LastWrite times of files.
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName; //  
            // Only watch XML files.
            watcher.Filter = filefilter;

            // Add event handlers.
            watcher.Error += Watcher_Error;
            watcher.Created += new FileSystemEventHandler(OnCreated);
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);
            //watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        }

        public void Start()
        {
            System.Console.WriteLine("DataIntegration");

            //ProcessExistingFiles();

            Task t = new Task(() => ProcessExistingFiles());
            t.Start();

            System.Console.WriteLine("Begin watching");
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            //throw new NotImplementedException();
            System.Console.WriteLine("Watcher_Error : " + e.GetException().Message);
            //TODO: logguer erreur
        }

        // Define the event handlers.
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            System.Console.WriteLine("File created: " + e.FullPath + " " + e.ChangeType);
            ProcessIncomingFile2(e.FullPath);
        }
        /*
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File changed: " + e.FullPath + " - " + e.ChangeType);
            Console.WriteLine("File changed: Exist = " + File.Exists(e.FullPath).ToString());
        }
        
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} deleted", e.FullPath);
        }
        */
        private void ProcessExistingFiles()
        {
            System.Console.WriteLine("ProcessExistingFiles");

            IEnumerable<string> files = Directory.EnumerateFiles(BookingFolder, filefilter);

            foreach (string file in files)
            {
                ProcessIncomingFile2(file);
            }
        }

        private void ProcessIncomingFile(string file)
        {
            System.Console.WriteLine("Processing file: " + file);
            try
            {
                DataSet ds = new DataSet();

                Stream s = new MemoryStream(Resources.BookingQM);
                ds.ReadXmlSchema(s);

                foreach (DataTable dt in ds.Tables)
                    dt.BeginLoadData();

                ds.ReadXml(file, XmlReadMode.ReadSchema);

                foreach (DataTable dt in ds.Tables)
                    dt.EndLoadData();

                //AddToDatabase(ds);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ReadXml exception : " + e.Message);

                using (ShoreEntities db = new ShoreEntities())
                {
                    // Log
                    db.Log.Add(new Log()
                    {
                        Action = "Intégration",
                        Date = DateTime.Now,
                        Details = e.Message,
                        Level = "Error",
                        Type = "Flux booking",
                        User = "System",
                        Booking = 0 //"TODO n° sur le fichier"
                    });

                    if(e.InnerException != null)
                    {
                        db.Log.Add(new Log()
                        {
                            Action = "Intégration",
                            Date = DateTime.Now,
                            Details = e.InnerException.Message,
                            Level = "Error",
                            Type = "Flux booking",
                            User = "System",
                            Booking = 0 //"TODO n° sur le fichier"
                        });
                    }

                    db.SaveChanges();
                }

                string dst = Path.Combine(BookingErrorFolder, Path.GetFileName(file));
                File.Move(file, dst);
            }
            File.Delete(file);
        }

        private void ProcessIncomingFile2(string file)
        {
            System.Console.WriteLine("Processing file: " + file);
            try
            {
                ValidationEventHandler validation = new ValidationEventHandler(ValidationEventHandler);

                Stream s = new MemoryStream(Resources.BookingQM);

                XmlTextReader xtr = new XmlTextReader(s);
                XmlSchema myschema = XmlSchema.Read(xtr, validation);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(myschema);
                settings.ValidationType = ValidationType.Schema;

                using (XmlReader reader = XmlReader.Create(file, settings))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);

                    // the following call to Validate succeeds.
                    document.Validate(validation);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ReadXml exception : " + e.Message);
#if LOG_TO_DB
                using (ShoreEntities db = new ShoreEntities())
                {
                    // Log
                    db.Log.Add(new Log()
                    {
                        Action = "Intégration",
                        Date = DateTime.Now,
                        Details = e.Message,
                        Level = "Error",
                        Type = "Flux booking",
                        User = "System",
                        Booking = 0 //"TODO n° sur le fichier"
                    });

                    if (e.InnerException != null)
                    {
                        db.Log.Add(new Log()
                        {
                            Action = "Intégration",
                            Date = DateTime.Now,
                            Details = e.InnerException.Message,
                            Level = "Error",
                            Type = "Flux booking",
                            User = "System",
                            Booking = 0 //"TODO n° sur le fichier"
                        });
                    }

                    db.SaveChanges();
                }
#endif
                try
                {
                    string dst = Path.Combine(BookingErrorFolder, Path.GetFileName(file));
                    File.Move(file, dst);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Move exception : " + ex.Message);
                }
            }
            
            try
            {
                File.Delete(file);
            }
            catch(Exception e)
            {
                System.Console.WriteLine("Delete exception : " + e.Message);
            }
        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    System.Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    System.Console.WriteLine("Warning {0}", e.Message);
                    break;
            }
        }


        private void AddToDatabase(DataSet ds)
        {
            using (ShoreEntities db = new ShoreEntities())
            {
                List<Data.Shore.Booking> l = new List<Data.Shore.Booking>();

                db.Agency.Add(new Agency
                {
                    Id = 1,
                    Booking = l
                });

                db.SaveChanges();
            }
        }
    }
}
