namespace SpiritualNetwork.API
{
    public static class GlobalVariables
    {
        public static string SiteName { get; set; }
		public static string SiteUrl { get; set; }
		public static int LoginUserId { get; set; } = 0;
        public static string LoginUserName { get; set; } = "";
        public static string LoginUserEmail { get; set; } = "";
        public static string Token { get; set; } = "";
        public static string NotificationAPIUrl { get; set;}
        public static string ElasticPostNodeUrl { get; set; }
        public static string BookLibrary { get; set; }
        public static string OpenAPIKey { get; set; }
        public static string OpenAIapiURL { get; set; }
        public static string SMTPUsername { get; set; }
		public static string SMTPHost { get; set; }
		public static string SMTPPassword { get; set; }
		public static string SMTPPort { get; set; }
		public static string SSLEnable { get; set; }
        public static string TwilioaccountSid { get; set; }
		public static string TwilioauthToken { get; set; }
	}
    
}
