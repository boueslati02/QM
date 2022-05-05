using System.Configuration;
 
    public static class AppSettings {
        public static int DelayToDeleteCruise { get { return int.Parse(ConfigurationManager.AppSettings["DelayToDeleteCruise"]); }}
        public static int DelayToQmAlertDays { get { return int.Parse(ConfigurationManager.AppSettings["DelayToQmAlertDays"]); }}
        public static string CurrentCruisesFolder { get { return ConfigurationManager.AppSettings["CurrentCruisesFolder"]; }}
        public static string CurrentCruises { get { return ConfigurationManager.AppSettings["CurrentCruises"]; }}
        public static string CruisesToDoFolder { get { return ConfigurationManager.AppSettings["CruisesToDoFolder"]; }}
        public static string CruisesToDo { get { return ConfigurationManager.AppSettings["CruisesToDo"]; }}
        public static string DocumentFolder { get { return ConfigurationManager.AppSettings["DocumentFolder"]; }}
        public static string UserName { get { return ConfigurationManager.AppSettings["UserName"]; }}
        public static string IdShip { get { return ConfigurationManager.AppSettings["IdShip"]; }}
        public static string System { get { return ConfigurationManager.AppSettings["System"]; }}
        public static string Board { get { return ConfigurationManager.AppSettings["Board"]; }}
        public static string ShoreUserName { get { return ConfigurationManager.AppSettings["ShoreUserName"]; }}
        public static string ShorePassword { get { return ConfigurationManager.AppSettings["ShorePassword"]; }}
        public static string WebServiceUrl { get { return ConfigurationManager.AppSettings["WebServiceUrl"]; }}
    }