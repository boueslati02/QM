using System.Configuration;
 
    public static class CommonSettings {
        public static string MailPassword { get { return ConfigurationManager.AppSettings["MailPassword"]; }}
        public static string TagTitle { get { return ConfigurationManager.AppSettings["TagTitle"]; }}
        public static string TagUserName { get { return ConfigurationManager.AppSettings["TagUserName"]; }}
        public static string TagPassword { get { return ConfigurationManager.AppSettings["TagPassword"]; }}
        public static string TagShip { get { return ConfigurationManager.AppSettings["TagShip"]; }}
        public static string TagCruise { get { return ConfigurationManager.AppSettings["TagCruise"]; }}
        public static string TagCabin { get { return ConfigurationManager.AppSettings["TagCabin"]; }}
        public static string TagBooking { get { return ConfigurationManager.AppSettings["TagBooking"]; }}
        public static string TagLastName { get { return ConfigurationManager.AppSettings["TagLastName"]; }}
        public static string TagUsualName { get { return ConfigurationManager.AppSettings["TagUsualName"]; }}
        public static string TagFirstName { get { return ConfigurationManager.AppSettings["TagFirstName"]; }}
        public static string TagBirthDate { get { return ConfigurationManager.AppSettings["TagBirthDate"]; }}
        public static string TagSailingDate { get { return ConfigurationManager.AppSettings["TagSailingDate"]; }}
        public static string TagGroup { get { return ConfigurationManager.AppSettings["TagGroup"]; }}
        public static string TagAdvice { get { return ConfigurationManager.AppSettings["TagAdvice"]; }}
        public static string TagIdPassenger { get { return ConfigurationManager.AppSettings["TagIdPassenger"]; }}
        public static string TagComments { get { return ConfigurationManager.AppSettings["TagComments"]; }}
        public static string TagDestination { get { return ConfigurationManager.AppSettings["TagDestination"]; }}
        public static string TagUrl { get { return ConfigurationManager.AppSettings["TagUrl"]; }}
        public static string UploadPassengerUrl { get { return ConfigurationManager.AppSettings["UploadPassengerUrl"]; }}
    }