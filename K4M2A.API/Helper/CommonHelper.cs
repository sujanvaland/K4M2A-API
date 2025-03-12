using System.Text;
using System.Text.RegularExpressions;

namespace SpiritualNetwork.API.Helper
{
    public static class CommonHelper
    {
        public static string EncodeBase64(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeBase64(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            byte[] bytes = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Regex.Replace(input, "<.*?>", string.Empty); // Removes HTML tags
        }
    }
}
