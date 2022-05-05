using System.Configuration;
 
    public static class AppSettings {
        public static string FolderMail { get { return ConfigurationManager.AppSettings["FolderMail"]; }}
        public static string FolderPassenger { get { return ConfigurationManager.AppSettings["FolderPassenger"]; }}
        public static string FolderSandBox { get { return ConfigurationManager.AppSettings["FolderSandBox"]; }}
        public static string AddressFrom { get { return ConfigurationManager.AppSettings["AddressFrom"]; }}
        public static string AddressNoReply { get { return ConfigurationManager.AppSettings["AddressNoReply"]; }}
        public static string AddressGroup { get { return ConfigurationManager.AppSettings["AddressGroup"]; }}
        public static string AddressDebug { get { return ConfigurationManager.AppSettings["AddressDebug"]; }}
        public static string MailNotifyAdvice { get { return ConfigurationManager.AppSettings["MailNotifyAdvice"]; }}
        public static string RoleBoard { get { return ConfigurationManager.AppSettings["RoleBoard"]; }}
        public static string TagTitle { get { return ConfigurationManager.AppSettings["TagTitle"]; }}
        public static string TagUserName { get { return ConfigurationManager.AppSettings["TagUserName"]; }}
        public static string TagPassword { get { return ConfigurationManager.AppSettings["TagPassword"]; }}
        public static string TagShip { get { return ConfigurationManager.AppSettings["TagShip"]; }}
        public static string TagCruise { get { return ConfigurationManager.AppSettings["TagCruise"]; }}
        public static string TagCabin { get { return ConfigurationManager.AppSettings["TagCabin"]; }}
        public static string TagBooking { get { return ConfigurationManager.AppSettings["TagBooking"]; }}
        public static string TagLastName { get { return ConfigurationManager.AppSettings["TagLastName"]; }}
        public static string TagFirstName { get { return ConfigurationManager.AppSettings["TagFirstName"]; }}
        public static string TagBirthDate { get { return ConfigurationManager.AppSettings["TagBirthDate"]; }}
        public static string TagSailingDate { get { return ConfigurationManager.AppSettings["TagSailingDate"]; }}
        public static string TagGroup { get { return ConfigurationManager.AppSettings["TagGroup"]; }}
        public static string TagAdvice { get { return ConfigurationManager.AppSettings["TagAdvice"]; }}
        public static string TagIdPassenger { get { return ConfigurationManager.AppSettings["TagIdPassenger"]; }}
        public static string TagComments { get { return ConfigurationManager.AppSettings["TagComments"]; }}
        public static string TagDestination { get { return ConfigurationManager.AppSettings["TagDestination"]; }}
        public static bool ClientValidationEnabled { get { return bool.Parse(ConfigurationManager.AppSettings["ClientValidationEnabled"]); }}
        public static bool UnobtrusiveJavaScriptEnabled { get { return bool.Parse(ConfigurationManager.AppSettings["UnobtrusiveJavaScriptEnabled"]); }}
    }