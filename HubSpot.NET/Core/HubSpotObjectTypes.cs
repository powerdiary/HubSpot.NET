namespace HubSpot.NET.Core
{
    /// <summary>
    /// Source: https://developers.hubspot.com/docs/guides/api/crm/using-object-apis
    /// </summary>
    public static class HubSpotObjectTypes
    {
        public const string CONTACT = "0-1";
        public const string COMPANY = "0-2";
        public const string DEAL = "0-3";
        public const string TICKET = "0-5";
        public const string QUOTE = "0-14";
        public const string PRODUCT = "0-7";
        public const string LINE_ITEM = "0-8";
        public const string CALL = "0-48";
        public const string EMAIL = "0-49";
        public const string MEETING = "0-47";
        public const string TASK = "0-27";
        public const string NOTE = "0-46";

        // Rare or domain-specific types
        public const string APPOINTMENT = "0-421";
        public const string COMMUNICATION = "0-18";
        public const string COURSE = "0-410";
        public const string LEAD = "0-136";
        public const string LISTING = "0-420";
        public const string MARKETING_EVENT = "0-54";
        public const string ORDER = "0-123";
        public const string POSTAL_MAIL = "0-116";
        public const string SERVICE = "0-162";
        public const string SUBSCRIPTION = "0-69";
        public const string USER = "0-115";

        /// <summary>
        /// Use this for custom objects — replace XXX with your object-specific ID.
        /// </summary>
        public const string CUSTOM_OBJECT_PREFIX = "2-";
    }
} 