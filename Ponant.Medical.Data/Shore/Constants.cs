

namespace Ponant.Medical.Data
{
	/// <summary>
    /// Classe de gestion des constantes de base de données
    /// </summary>
    public static partial class Constants
    {
		public const int NOT_APPLICABLE_NOT_APPLICABLE = 0;
		public const int LOV_NOT_APPLICABLE = 0;
		public const int LOV_CRUISE_TYPE = 1;
		public const int CRUISE_TYPE_EXPEDITION = 1;
		public const int LOV_DESTINATION = 2;
		public const int CRUISE_TYPE_NOT_EXPEDITION = 2;
		public const int CRUISE_TYPE_SEA_TRIP = 3;
		public const int LOV_SHIP = 3;
		public const int DESTINATION_ADRIATIC = 4;
		public const int LOV_SHORE_STATUS = 4;
		public const int DESTINATION_AFRICA = 5;
		public const int LOV_BOARD_STATUS = 5;
		public const int DESTINATION_ALASKA = 6;
		public const int LOV_ERROR_TYPE = 6;
		public const int LOV_ACTIVITY = 7;
		public const int DESTINATION_LATIN_AMERICA = 7;
		public const int LOV_AGENCY = 8;
		public const int DESTINATION_NORTH_AMERICA = 8;
		public const int LOV_ADVICE = 9;
		public const int DESTINATION_ANTARCTICA = 9;
		public const int DESTINATION_ARCTIC = 10;
		public const int LOV_LANGUAGE = 10;
		public const int DESTINATION_ASIA = 11;
		public const int LOV_OFFICE = 11;
		public const int DESTINATION_CARIBBEAN = 12;
		public const int LOV_UNFAVORABLE_ADVICE = 12;
		public const int DESTINATION_NORTH_AND_BALTIC_EUROPE = 13;
		public const int LOV_RESTRICTION_ADVICE = 13;
		public const int LOV_RESTRICTION_PERSON = 14;
		public const int DESTINATION_SOUTHERN_EUROPE_AND_MEDITERRANEAN = 14;
		public const int LOV_ADDITIONAL_DOCUMENTS = 15;
		public const int DESTINATION_FLUVIAL = 15;
		public const int LOV_DOCUMENT_STATUS = 16;
		public const int DESTINATION_GREEK_ISLANDS = 16;
		public const int LOV_CIVILITY = 17;
		public const int DESTINATION_INDIAN_OCEAN = 17;
		public const int DESTINATION_RED_SEA_AND_MIDDLE_EAST = 18;
		public const int DESTINATION_OCEANIA = 19;
		public const int DESTINATION_RUSSIA = 20;
		public const int DESTINATION_TURKEY_AND_BLACK_SEA = 21;
		public const int DESTINATION_OCEAN_VOYAGE = 22;
		public const int SHIP_LAUSTRAL = 23;
		public const int SHIP_LE_BOREAL = 24;
		public const int SHIP_LE_LYRIAL = 25;
		public const int SHIP_LE_PONANT = 26;
		public const int SHIP_LE_SOLEAL = 27;
		public const int SHORE_STATUS_QM_SENT = 28;
		public const int SHORE_STATUS_QM_NOT_SENT = 29;
		public const int SHORE_STATUS_QM_RECEIVED = 30;
		public const int SHORE_STATUS_QM_INCOMPLETE = 31;
		public const int SHORE_STATUS_QM_NEW_DOCUMENTS = 32;
		public const int SHORE_STATUS_QM_CLOSED = 33;
        public const int SHORE_STATUS_QM_IN_PROGRESS = 34;
        public const int SHORE_STATUS_QM_DOWNLOAD_BEFORE_CRUISE = 35;
        public const int BOARD_STATUS_QM_TO_DO = 36;
		public const int BOARD_STATUS_QM_DONE = 37;
		public const int BOARD_STATUS_QM_ACQUITTED = 38;
		public const int BOARD_STATUS_QM_DOWNLOAD_BEFORE_CRUISE = 39;
		public const int ERROR_TYPE_THE_XML_FILE_DOES_NOT_FOLLOW_THE_XSD_SCHEMA = 40;
		public const int ERROR_TYPE_FORMAT_OF_DATA_IS_INVALID = 41;
		public const int ERROR_TYPE_THE_EMAIL_ADDRESS_DOES_NOT_EXIST = 42;
		public const int ERROR_TYPE_THE_SIZE_OF_THE_MAIL_EXCEEDS_THE_ALLOWED_LIMIT = 43;
		public const int ADVICE_FAVORABLE_OPINION = 44;
		public const int ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS = 45;
		public const int ADVICE_UNFAVORABLE_OPINION = 46;
		public const int ADVICE_WAITING_FOR_CLARIFICATION = 47;
		public const int LANGUAGE_FRANCOPHONE = 48;
		public const int LANGUAGE_ANGLOPHONE = 49;
        public const int OFFICE_MAIN_OFFICE = 50;
        public const int OFFICE_USA_OFFICE = 51;
        public const int UNFAVORABLE_ADVICE_CHRONIC_RESPIRATORY_INSUFFICIENCY_UNDER_OXYGEN = 52;
		public const int UNFAVORABLE_ADVICE_RESPIRATORY_INSUFFICIENCY_WITH_RESPIRATOR = 53;
		public const int UNFAVORABLE_ADVICE_UNSTEADY_HEART_FAILURE = 54;
		public const int UNFAVORABLE_ADVICE_UNSTABLE_ISCHEMIC_HEART_DISEASE = 55;
		public const int UNFAVORABLE_ADVICE_UNCONTROLLED_HTA = 56;
		public const int UNFAVORABLE_ADVICE_SEVERE_RENAL_IMPAIRMENT_CLEARANCE__ML__MIN = 57;
		public const int UNFAVORABLE_ADVICE_DISORDERS_OF_HIGHER_FUNCTIONS_DEMENTIA_ = 58;
		public const int UNFAVORABLE_ADVICE_CURRENT_OR_RECENT_CHEMOTHERAPY = 59;
		public const int UNFAVORABLE_ADVICE_IMMUNO_SEVERE_DEPRESSION = 60;
		public const int UNFAVORABLE_ADVICE_HEPATIC_INSUFFICIENCY_TP__AND__OR_EDEMAASCITIC_SYNDROME = 61;
		public const int UNFAVORABLE_ADVICE_DEPRESSIVE_SYNDROME_WITH_SUICIDAL_RISK = 62;
		public const int UNFAVORABLE_ADVICE_PREGNANCY = 63;
		public const int UNFAVORABLE_ADVICE_MYASTHENIA_GRAVIS = 64;
		public const int UNFAVORABLE_ADVICE_OTHER_SPECIFY = 65;
		public const int UNFAVORABLE_ADVICE_INFECTION_IN_PROGRESS_TUBERCULOSIS_ENDOCARDITIS_INFECTION_ON_PROSTHESIS = 66;
		public const int RESTRICTION_ADVICE_NECESSITY_OF_AN_ACCOMPANYING_PERSON_FOR_MOBILITY = 67;
		public const int RESTRICTION_ADVICE_NECESSITY_OF_A_CARER = 68;
		public const int RESTRICTION_ADVICE_NO_LANDING_ZODIAC = 69;
		public const int RESTRICTION_PERSON_SOMEONE_WITH_REDUCED_MOBILITY = 70;
		public const int RESTRICTION_PERSON_NEUROPATHY_WITH_WALKING_DISORDER = 71;
		public const int RESTRICTION_PERSON_SEQUELAE_OF_STROKE = 72;
		public const int RESTRICTION_PERSON_MORBID_OBESITY = 73;
		public const int RESTRICTION_PERSON_COMPLEX_DRUG_THERAPY = 74;
		public const int RESTRICTION_PERSON_OTHER_SPECIFY = 75;
		public const int ADDITIONAL_DOCUMENTS_SPECIALIST_CONSULTATION_REPORT_SPECIFY_SPECIALTY = 76;
		public const int ADDITIONAL_DOCUMENTS_BIOLOGY_REPORT = 77;
		public const int ADDITIONAL_DOCUMENTS_ELECTROCARDIOGRAM = 78;
		public const int ADDITIONAL_DOCUMENTS_IMAGING_SPECIFY = 79;
		public const int ADDITIONAL_DOCUMENTS_SPECIFIC_CERTIFICATE_SPECIFY = 80;
		public const int ADDITIONAL_DOCUMENTS_OTHER = 81;
		public const int DOCUMENT_STATUS_SEEN = 82;
		public const int DOCUMENT_STATUS_NOT_SEEN = 83;
		public const int CIVILITY_MR = 84;
		public const int CIVILITY_MRS = 85;
	}
}

