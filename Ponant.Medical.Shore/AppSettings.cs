﻿using System.Configuration; 
 
    public static class AppSettings {
        public static bool ClientValidationEnabled { get { return bool.Parse(ConfigurationManager.AppSettings["ClientValidationEnabled"]); }}
        public static bool UnobtrusiveJavaScriptEnabled { get { return bool.Parse(ConfigurationManager.AppSettings["UnobtrusiveJavaScriptEnabled"]); }}
        public static bool MVCGridShowErrorDetail { get { return bool.Parse(ConfigurationManager.AppSettings["MVCGridShowErrorDetail"]); }}
        public static bool AllowOnlyAlphanumericUserNames { get { return bool.Parse(ConfigurationManager.AppSettings["AllowOnlyAlphanumericUserNames"]); }}
        public static bool RequireUniqueEmail { get { return bool.Parse(ConfigurationManager.AppSettings["RequireUniqueEmail"]); }}
        public static int RequiredLength { get { return int.Parse(ConfigurationManager.AppSettings["RequiredLength"]); }}
        public static bool RequireNonLetterOrDigit { get { return bool.Parse(ConfigurationManager.AppSettings["RequireNonLetterOrDigit"]); }}
        public static bool RequireDigit { get { return bool.Parse(ConfigurationManager.AppSettings["RequireDigit"]); }}
        public static bool RequireLowercase { get { return bool.Parse(ConfigurationManager.AppSettings["RequireLowercase"]); }}
        public static bool RequireUppercase { get { return bool.Parse(ConfigurationManager.AppSettings["RequireUppercase"]); }}
        public static bool UserLockoutEnabledByDefault { get { return bool.Parse(ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"]); }}
        public static int DefaultAccountLockoutTimeSpan { get { return int.Parse(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"]); }}
        public static int MaxFailedAccessAttemptsBeforeLockout { get { return int.Parse(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"]); }}
        public static int ValidateInterval { get { return int.Parse(ConfigurationManager.AppSettings["ValidateInterval"]); }}
        public static int ExpireTimeSpan { get { return int.Parse(ConfigurationManager.AppSettings["ExpireTimeSpan"]); }}
        public static int TwoFactorInterval { get { return int.Parse(ConfigurationManager.AppSettings["TwoFactorInterval"]); }}
        public static int TokenLifespan { get { return int.Parse(ConfigurationManager.AppSettings["TokenLifespan"]); }}
        public static string FolderMail { get { return ConfigurationManager.AppSettings["FolderMail"]; }}
        public static string FolderMailIndividual { get { return ConfigurationManager.AppSettings["FolderMailIndividual"]; }}
        public static string FolderMailGroup { get { return ConfigurationManager.AppSettings["FolderMailGroup"]; }}
        public static string FolderSurveyIndividual { get { return ConfigurationManager.AppSettings["FolderSurveyIndividual"]; }}
        public static string FolderSurveyGroup { get { return ConfigurationManager.AppSettings["FolderSurveyGroup"]; }}
        public static string FolderTemp { get { return ConfigurationManager.AppSettings["FolderTemp"]; }}
        public static string FolderPassenger { get { return ConfigurationManager.AppSettings["FolderPassenger"]; }}
        public static string FolderAvailable { get { return ConfigurationManager.AppSettings["FolderAvailable"]; }}
        public static string FolderMailIndividualAutomaticResponse { get { return ConfigurationManager.AppSettings["FolderMailIndividualAutomaticResponse"]; }}
        public static string FolderMailGroupAutomaticResponse { get { return ConfigurationManager.AppSettings["FolderMailGroupAutomaticResponse"]; }}
        public static string FolderLogos { get { return ConfigurationManager.AppSettings["FolderLogos"]; }}
        public static string FolderVirtualLogo { get { return ConfigurationManager.AppSettings["FolderVirtualLogo"]; }}
        public static string FolderMailConfirmationReception { get { return ConfigurationManager.AppSettings["FolderMailConfirmationReception"]; }}
        public static string FolderMailErrorSubmission { get { return ConfigurationManager.AppSettings["FolderMailErrorSubmission"]; }}
        public static string AddressDebug { get { return ConfigurationManager.AppSettings["AddressDebug"]; }}
        public static string AddressNoReply { get { return ConfigurationManager.AppSettings["AddressNoReply"]; }}
        public static string AddressFrom { get { return ConfigurationManager.AppSettings["AddressFrom"]; }}
        public static string AddressGroupTo { get { return ConfigurationManager.AppSettings["AddressGroupTo"]; }}
        public static string AddressBookingTo { get { return ConfigurationManager.AppSettings["AddressBookingTo"]; }}
        public static string MailAccountPassword { get { return ConfigurationManager.AppSettings["MailAccountPassword"]; }}
        public static string MailAccountUserName { get { return ConfigurationManager.AppSettings["MailAccountUserName"]; }}
        public static string MailNewUserName { get { return ConfigurationManager.AppSettings["MailNewUserName"]; }}
        public static string MailNotifyAdvice { get { return ConfigurationManager.AppSettings["MailNotifyAdvice"]; }}
        public static string MailResetPassword { get { return ConfigurationManager.AppSettings["MailResetPassword"]; }}
        public static string MailSendCode { get { return ConfigurationManager.AppSettings["MailSendCode"]; }}
        public static string MailSummaryMail { get { return ConfigurationManager.AppSettings["MailSummaryMail"]; }}
        public static string MailUnfavorableAdvice { get { return ConfigurationManager.AppSettings["MailUnfavorableAdvice"]; }}
        public static string MailAssignmentQm { get { return ConfigurationManager.AppSettings["MailAssignmentQm"]; }}
        public static string MailDeadlineExceeded { get { return ConfigurationManager.AppSettings["MailDeadlineExceeded"]; }}
        public static string GroupFrom { get { return ConfigurationManager.AppSettings["GroupFrom"]; }}
        public static string ReportServerUrl { get { return ConfigurationManager.AppSettings["ReportServerUrl"]; }}
        public static string ReportPathQMStatistics { get { return ConfigurationManager.AppSettings["ReportPathQMStatistics"]; }}
        public static string ReportPathQMTreatment { get { return ConfigurationManager.AppSettings["ReportPathQMTreatment"]; }}
        public static string ReportPathPassengersGroupExport { get { return ConfigurationManager.AppSettings["ReportPathPassengersGroupExport"]; }}
        public static string TagTitle { get { return ConfigurationManager.AppSettings["TagTitle"]; }}
        public static string TagUserName { get { return ConfigurationManager.AppSettings["TagUserName"]; }}
        public static string TagPassword { get { return ConfigurationManager.AppSettings["TagPassword"]; }}
        public static string TagShip { get { return ConfigurationManager.AppSettings["TagShip"]; }}
        public static string TagShipCode { get { return ConfigurationManager.AppSettings["TagShipCode"]; }}
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
        public static string TagTable { get { return ConfigurationManager.AppSettings["TagTable"]; }}
        public static string TagTableNoDocument { get { return ConfigurationManager.AppSettings["TagTableNoDocument"]; }}
        public static string TagTableDocument { get { return ConfigurationManager.AppSettings["TagTableDocument"]; }}
        public static string TagComments { get { return ConfigurationManager.AppSettings["TagComments"]; }}
        public static string TagDestination { get { return ConfigurationManager.AppSettings["TagDestination"]; }}
        public static string TagDeadline { get { return ConfigurationManager.AppSettings["TagDeadline"]; }}
        public static string TagQmNotValidated { get { return ConfigurationManager.AppSettings["TagQmNotValidated"]; }}
        public static int IdAgency { get { return int.Parse(ConfigurationManager.AppSettings["IdAgency"]); }}
        public static string AgencyName { get { return ConfigurationManager.AppSettings["AgencyName"]; }}
        public static string SsrsWsUrl { get { return ConfigurationManager.AppSettings["SsrsWsUrl"]; }}
        public static string SsrsUserName { get { return ConfigurationManager.AppSettings["SsrsUserName"]; }}
        public static string SsrsPassword { get { return ConfigurationManager.AppSettings["SsrsPassword"]; }}
        public static string SsrsDomain { get { return ConfigurationManager.AppSettings["SsrsDomain"]; }}
        public static string TagUrl { get { return ConfigurationManager.AppSettings["TagUrl"]; }}
        public static string UploadPassengerUrl { get { return ConfigurationManager.AppSettings["UploadPassengerUrl"]; }}
    }