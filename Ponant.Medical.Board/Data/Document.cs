//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ponant.Medical.Board.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        public int Id { get; set; }
        public int IdPassenger { get; set; }
        public int IdStatus { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Creator { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Editor { get; set; }
        public System.DateTime ModificationDate { get; set; }
        public bool IsToDetach { get; set; }
    
        public virtual Lov Lov { get; set; }
        public virtual Passenger Passenger { get; set; }
    }
}
