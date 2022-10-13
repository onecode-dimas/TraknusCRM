using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit
{
    public class Generator
    {
        #region Constants
        private const string _classname = "Generator";
        #endregion

        #region Privates
        private static string byteArrayToString(byte[] hash)
        {
            String result = "";
            for (int i = 0; i < hash.Length; i++)
            {
                result += hash[i].ToString("X2");
            }
            return (result);
        }
        #endregion

        #region Publics
        public string Encrypt(string text, string key)
        {
            try
            {
                UTF8Encoding e = new UTF8Encoding();
                byte[] keyByte = e.GetBytes(key);
                byte[] passByte = e.GetBytes(text);
                HMACSHA256 hash = new HMACSHA256(keyByte);
                String hashedPwd = byteArrayToString(hash.ComputeHash(passByte));

                return hashedPwd;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodBase.GetCurrentMethod().Name + " : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
