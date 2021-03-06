//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ponant.Medical.Data.Shore
{
    using System;
    using System.Collections.Generic;
    
    public partial class Language
    {
        public int Id { get; set; }
        public int IdSurvey { get; set; }
        public int IdLanguage { get; set; }
        public string IndividualSurveyMail { get; set; }
        public string GroupSurveyMail { get; set; }
        public bool IsDefault { get; set; }
        public string Creator { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Editor { get; set; }
        public System.DateTime ModificationDate { get; set; }
        public string IndividualSurveyFileName { get; set; }
        public string GroupSurveyFileName { get; set; }
        public string IndividualAutomaticResponse { get; set; }
        public string EmailFormat { get; set; }
        public string EmailFormatGroup { get; set; }
        public string TextFormatWebSite { get; set; }
        public string TextFormatWebSiteGroup { get; set; }
        public string GroupAutomaticResponse { get; set; }
        public string IndividualAdditionalMail { get; set; }
        public string GroupAdditionalMail { get; set; }
    
        public virtual Lov LovLanguage { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
