//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// Ce code source a été automatiquement généré par xsd, Version=4.6.1590.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class Booking {
    
    private BookingBookingContext bookingContextField;
    
    private BookingAgencyAddress agencyAddressField;
    
    private BookingParticipantData[] participantListField;
    
    private BookingCruiseBookings cruiseBookingsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BookingBookingContext BookingContext {
        get {
            return this.bookingContextField;
        }
        set {
            this.bookingContextField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BookingAgencyAddress AgencyAddress {
        get {
            return this.agencyAddressField;
        }
        set {
            this.agencyAddressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("ParticipantData", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public BookingParticipantData[] ParticipantList {
        get {
            return this.participantListField;
        }
        set {
            this.participantListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BookingCruiseBookings CruiseBookings {
        get {
            return this.cruiseBookingsField;
        }
        set {
            this.cruiseBookingsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingBookingContext {
    
    private string languageCodeField;
    
    private int bookingNoField;
    
    private string bookingStatusCodeField;
    
    private string officeNameField;
    
    private bool isGroupField;
    
    private string groupIdField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string LanguageCode {
        get {
            return this.languageCodeField;
        }
        set {
            this.languageCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int BookingNo {
        get {
            return this.bookingNoField;
        }
        set {
            this.bookingNoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string BookingStatusCode {
        get {
            return this.bookingStatusCodeField;
        }
        set {
            this.bookingStatusCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string OfficeName {
        get {
            return this.officeNameField;
        }
        set {
            this.officeNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool IsGroup {
        get {
            return this.isGroupField;
        }
        set {
            this.isGroupField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string GroupId {
        get {
            return this.groupIdField;
        }
        set {
            this.groupIdField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingAgencyAddress {
    
    private int agencyIdField;
    
    private string agencyNameField;
    
    private string emailField;
    
    private string lastNameField;
    
    private string firstNameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int AgencyId {
        get {
            return this.agencyIdField;
        }
        set {
            this.agencyIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AgencyName {
        get {
            return this.agencyNameField;
        }
        set {
            this.agencyNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string LastName {
        get {
            return this.lastNameField;
        }
        set {
            this.lastNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FirstName {
        get {
            return this.firstNameField;
        }
        set {
            this.firstNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingParticipantData {
    
    private string passengerNoField;
    
    private string lastNameField;
    
    private string usualNameField;
    
    private string firstNameField;
    
    private string civilityField;
    
    private string emailField;
    
    private string dateOfBirthField;
    
    private string telephoneNoField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PassengerNo {
        get {
            return this.passengerNoField;
        }
        set {
            this.passengerNoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string LastName {
        get {
            return this.lastNameField;
        }
        set {
            this.lastNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string UsualName {
        get {
            return this.usualNameField;
        }
        set {
            this.usualNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FirstName {
        get {
            return this.firstNameField;
        }
        set {
            this.firstNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Civility {
        get {
            return this.civilityField;
        }
        set {
            this.civilityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string DateOfBirth {
        get {
            return this.dateOfBirthField;
        }
        set {
            this.dateOfBirthField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string TelephoneNo {
        get {
            return this.telephoneNoField;
        }
        set {
            this.telephoneNoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingCruiseBookings {
    
    private BookingCruiseBookingsCruiseSailing[] cruiseSailingField;
    
    private BookingCruiseBookingsActivityBooking[] activityBookingsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CruiseSailing", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BookingCruiseBookingsCruiseSailing[] CruiseSailing {
        get {
            return this.cruiseSailingField;
        }
        set {
            this.cruiseSailingField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("ActivityBooking", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public BookingCruiseBookingsActivityBooking[] ActivityBookings {
        get {
            return this.activityBookingsField;
        }
        set {
            this.activityBookingsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingCruiseBookingsCruiseSailing {
    
    private string cruiseIDField;
    
    private string cruiseTypeField;
    
    private string shipCodeField;
    
    private string shipNameField;
    
    private string destinationCodeField;
    
    private string destinationNameField;
    
    private System.DateTime sailingDateField;
    
    private int sailingLengthDaysField;
    
    private string cabinNoField;
    
    private int[] participantIDsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CruiseID {
        get {
            return this.cruiseIDField;
        }
        set {
            this.cruiseIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CruiseType {
        get {
            return this.cruiseTypeField;
        }
        set {
            this.cruiseTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ShipCode {
        get {
            return this.shipCodeField;
        }
        set {
            this.shipCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ShipName {
        get {
            return this.shipNameField;
        }
        set {
            this.shipNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string DestinationCode {
        get {
            return this.destinationCodeField;
        }
        set {
            this.destinationCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string DestinationName {
        get {
            return this.destinationNameField;
        }
        set {
            this.destinationNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime SailingDate {
        get {
            return this.sailingDateField;
        }
        set {
            this.sailingDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int SailingLengthDays {
        get {
            return this.sailingLengthDaysField;
        }
        set {
            this.sailingLengthDaysField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CabinNo {
        get {
            return this.cabinNoField;
        }
        set {
            this.cabinNoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("PassengerNo", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public int[] ParticipantIDs {
        get {
            return this.participantIDsField;
        }
        set {
            this.participantIDsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class BookingCruiseBookingsActivityBooking {
    
    private string activityCodeField;
    
    private string activityDescriptionField;
    
    private int[] participantIDsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ActivityCode {
        get {
            return this.activityCodeField;
        }
        set {
            this.activityCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ActivityDescription {
        get {
            return this.activityDescriptionField;
        }
        set {
            this.activityDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("PassengerNo", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public int[] ParticipantIDs {
        get {
            return this.participantIDsField;
        }
        set {
            this.participantIDsField = value;
        }
    }
}
