﻿namespace SpiritualNetwork.API
{
    public static class GlobalVariables
    {
        public static int LoginUserId { get; set; } = 0;
        public static string LoginUserName { get; set; } = "";
        public static string LoginUserEmail { get; set; } = "";
        public static string Token { get; set; } = "";
        public static string NotificationAPIUrl { get; set;}
        public static string ElasticPostNodeUrl { get; set; }
        public static string BookLibrary { get; set; }
        public static string OpenAPIKey { get; set; }
        public static string OpenAIapiURL { get; set; }

    }
    
}
