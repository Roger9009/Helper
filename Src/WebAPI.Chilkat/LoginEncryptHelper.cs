using System;

namespace WebAPI.Login
{
    public static class LoginEncryptHelper
    {

        /// <summary>
        /// 登录密码加密
        /// </summary>
        /// <param name="password">登录密码</param>
        /// <param name="encryptType">加密方式</param>
        /// <returns></returns>
        public static string GetPasswordMD5Value(string password, string encryptType="")
        {
            Chilkat.Crypt2 crypt = new Chilkat.Crypt2();

            if (encryptType == "m")
            {
                crypt.HashAlgorithm = "md5";
                crypt.EncodingMode = "base64";
                crypt.Charset = "Unicode";
            }


            return crypt.HashStringENC(str: password);
        }



    }
}
