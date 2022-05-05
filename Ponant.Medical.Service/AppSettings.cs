﻿using System.Configuration;
  
    public static class AppSettings {
        public static int FirstSummaryMail { get { return int.Parse(ConfigurationManager.AppSettings["FirstSummaryMail"]); }}
        public static int SecondSummaryMail { get { return int.Parse(ConfigurationManager.AppSettings["SecondSummaryMail"]); }}
        public static int SandBoxCleaningInterval { get { return int.Parse(ConfigurationManager.AppSettings["SandBoxCleaningInterval"]); }}
        public static int TimerHour { get { return int.Parse(ConfigurationManager.AppSettings["TimerHour"]); }}
        public static int NumberOfRetries { get { return int.Parse(ConfigurationManager.AppSettings["NumberOfRetries"]); }}
        public static int DelayOnRetry { get { return int.Parse(ConfigurationManager.AppSettings["DelayOnRetry"]); }}
        public static string FolderPonantBooking { get { return ConfigurationManager.AppSettings["FolderPonantBooking"]; }}
        public static string FolderPonantBookingError { get { return ConfigurationManager.AppSettings["FolderPonantBookingError"]; }}
        public static string FolderShoreBooking { get { return ConfigurationManager.AppSettings["FolderShoreBooking"]; }}
        public static string FolderMail { get { return ConfigurationManager.AppSettings["FolderMail"]; }}
        public static string FolderMailIndividual { get { return ConfigurationManager.AppSettings["FolderMailIndividual"]; }}
        public static string FolderMailIndividualRelaunch { get { return ConfigurationManager.AppSettings["FolderMailIndividualRelaunch"]; }}
        public static string FolderMailGroup { get { return ConfigurationManager.AppSettings["FolderMailGroup"]; }}
        public static string FolderMailGroupRelaunch { get { return ConfigurationManager.AppSettings["FolderMailGroupRelaunch"]; }}
        public static string FolderSurveyIndividual { get { return ConfigurationManager.AppSettings["FolderSurveyIndividual"]; }}
        public static string FolderSurveyGroup { get { return ConfigurationManager.AppSettings["FolderSurveyGroup"]; }}
        public static string FolderSandBox { get { return ConfigurationManager.AppSettings["FolderSandBox"]; }}
        public static string FolderPassenger { get { return ConfigurationManager.AppSettings["FolderPassenger"]; }}
        public static string FolderTemp { get { return ConfigurationManager.AppSettings["FolderTemp"]; }}
        public static string FolderMailIndividualAutomaticResponse { get { return ConfigurationManager.AppSettings["FolderMailIndividualAutomaticResponse"]; }}
        public static string FolderMailGroupAutomaticResponse { get { return ConfigurationManager.AppSettings["FolderMailGroupAutomaticResponse"]; }}
        public static string AddressDebug { get { return ConfigurationManager.AppSettings["AddressDebug"]; }}
        public static string AddressFrom { get { return ConfigurationManager.AppSettings["AddressFrom"]; }}
        public static string QmReceivedList { get { return ConfigurationManager.AppSettings["QmReceivedList"]; }}
        public static string AddressNoReply { get { return ConfigurationManager.AppSettings["AddressNoReply"]; }}
        public static string GroupFrom { get { return ConfigurationManager.AppSettings["GroupFrom"]; }}
        public static string MailSummaryMail { get { return ConfigurationManager.AppSettings["MailSummaryMail"]; }}
        public static string MailSendQmReceived { get { return ConfigurationManager.AppSettings["MailSendQmReceived"]; }}
        public static string MailAssignmentQm { get { return ConfigurationManager.AppSettings["MailAssignmentQm"]; }}
        public static string MailDeadlineExceeded { get { return ConfigurationManager.AppSettings["MailDeadlineExceeded"]; }}
        public static string MailPassword { get { return ConfigurationManager.AppSettings["MailPassword"]; }}
        public static string UserAccount { get { return ConfigurationManager.AppSettings["UserAccount"]; }}
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
        public static string TagComments { get { return ConfigurationManager.AppSettings["TagComments"]; }}
        public static string TagDestination { get { return ConfigurationManager.AppSettings["TagDestination"]; }}
        public static string TagDeadline { get { return ConfigurationManager.AppSettings["TagDeadline"]; }}
        public static string TagQmNotValidated { get { return ConfigurationManager.AppSettings["TagQmNotValidated"]; }}
    }