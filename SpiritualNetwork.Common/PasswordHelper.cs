using DevOne.Security.Cryptography.BCrypt;

namespace SpiritualNetwork.Common
{
    public static class PasswordHelper 
    {
        public static string EncryptPassword(string password)
        {
            string salt = BCryptHelper.GenerateSalt();
            string hashedPassword = BCryptHelper.HashPassword(password, salt);
            return hashedPassword + "|" + salt;
        }

        public static bool VerifyPassword(string password, string encryptedpassword)
        {
            string[] strarr = encryptedpassword.Split('|');
            string pass = BCryptHelper.HashPassword(password, strarr[1]);

            if (encryptedpassword == pass + "|" + strarr[1])
            {
                return true;
            }
            return false;
        }
    }
}