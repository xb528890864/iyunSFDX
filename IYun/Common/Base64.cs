using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;



/// <summary>
///Base64 的摘要说明
/// </summary>
public class Base64
{
    //private static string sKeyForEncryptTitle;


	public Base64()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    #region 解密Base64
    /// <summary>
    /// 解密Base64
    /// </summary>
    /// <param name="asContent"></param>
    /// <returns></returns>
    public static string Base64TextUTF8Decode(string asContent)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(asContent);
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region 加密Base64
    /// <summary>
    /// 加密Base64
    /// </summary>
    /// <param name="asText"></param>
    /// <returns></returns>
    public static string Base64UTF8Encode(string asText)
    {
        try
        {
            string s = asText;
            UTF8Encoding encoding = new UTF8Encoding();
            return Convert.ToBase64String(encoding.GetBytes(s));
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region  解密UTF8
    /// <summary>
    /// 解密UTF8
    /// </summary>
    /// <param name="asEnCodeText"></param>
    /// <returns></returns>
    public static string DecodeUTF8Text(string asEnCodeText)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(asEnCodeText);
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region static 加密UTF8
    /// <summary>
    /// 加密UTF8
    /// </summary>
    /// <param name="asDecodeText"></param>
    /// <returns></returns>
    public string EnCodeUTF8Text(string asDecodeText)
    {
        try
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return Convert.ToBase64String(encoding.GetBytes(asDecodeText));
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region EncryptHelper加密

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string Encrypt(string Text)
    {
        if (Text == null || Text == "")
        {
           return "";
        }
        //return Encrypt(Text, "shionc");
        return Base64UTF8Encode(Text);
    }
    /// <summary> 
    /// 加密数据 
    /// </summary> 
    /// <param name="Text"></param> 
    /// <param name="sKey"></param> 
    /// <returns></returns> 
    public static string Encrypt(string Text, string sKey)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray;
        inputByteArray = Encoding.Default.GetBytes(Text);
        des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        StringBuilder ret = new StringBuilder();
        foreach (byte b in ms.ToArray())
        {
            ret.AppendFormat("{0:X2}", b);
        }
        return ret.ToString();
    }

    #endregion

    #region EncryptHelper解密


    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string Decrypt(string Text)
    {
        if (Text == null || Text == "")
        {
            return "";
        }
        return Base64TextUTF8Decode(Text);
       // return Decrypt(Text, "shionc");
    }
    /// <summary> 
    /// 解密数据 
    /// </summary> 
    /// <param name="Text"></param> 
    /// <param name="sKey"></param> 
    /// <returns></returns> 
    public static string Decrypt(string Text, string sKey)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        int len;
        len = Text.Length / 2;
        byte[] inputByteArray = new byte[len];
        int x, i;
        for (x = 0; x < len; x++)
        {
            i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
            inputByteArray[x] = (byte)i;
        }
        des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }

    #endregion

    #region DeCodeID 解密
    public static string DecodeID(string Encode)
    {
        if (Encode == "")
        {
            return "";
        }
        int length = Encode.Length;
        if (length < 5)
        {
            return "";
        }
        int num2 = Convert.ToInt32(Encode.Substring(2, 1));
        int num3 = Convert.ToInt32(Encode.Substring(4, 1));
        int startIndex = ((num2 * 2) + (num3 * 2)) + 7;
        if (length < (startIndex + 20))
        {
            return "";
        }
        num2 = (Convert.ToInt32(Encode.Substring(startIndex, 1)) * 10) + Convert.ToInt32(Encode.Substring(startIndex + 1, 1));
        startIndex = (startIndex + 2) - 1;
        string str = "";
        for (int i = 0; i < num2; i++)
        {
            startIndex = (startIndex + 3) + 1;
            str = str + Encode.Substring(startIndex, 1);
        }
        return str;
    }
    #endregion

    #region EncodeID 加密
    public static string EncodeID(string ID)
    {
        int num6;
        int num = (DateTime.Now.Hour * DateTime.Now.Minute) * DateTime.Now.Second;
        if (num < 100)
        {
            num = (num + 0x70) * 0x25;
        }
        string str = "";
        for (num6 = 1; num6 < 100; num6++)
        {
            int num2 = num * (num6 % 50);
            str = str + num2.ToString().Trim();
            if (str.Length > 120)
            {
                break;
            }
        }
        int num3 = Convert.ToInt32(str.Substring(2, 1));
        int num4 = Convert.ToInt32(str.Substring(4, 1));
        int length = ((num3 * 2) + (num4 * 2)) + 7;
        num3 = ID.Length;
        string str2 = str.Substring(0, length);
        if (num3 < 10)
        {
            str2 = str2 + "0";
        }
        str2 = str2 + num3.ToString().Trim();
        for (num6 = 0; num6 < num3; num6++)
        {
            str2 = str2 + str.Substring(1 + (num6 * 3), 3) + ID.Substring(num6, 1);
        }
        num3 = str2.Length;
        try
        {
            str2 = str2 + str.Substring(20, 110 - num3);
        }
        catch
        {
        }
        return str2;
    }
    #endregion

    #region DecodeURL 解密
    public static string DecodeURL(string NameValueStr)
    {
        uint num;
        if ((NameValueStr == null) || (NameValueStr == ""))
        {
            return "";
        }
        string str = NameValueStr.Trim().Replace("X", "1U1").Replace("Y", "2U2").Replace("Z", "3U3").Replace("V", "4U5").Replace("W", "7U6").Replace("O", "8U7").Replace("P", "0U8").Replace("Q", "0U9").Replace("R", "1U0").Replace("S", "2U4").Replace("x", "U1").Replace("y", "U2").Replace("z", "U3").Replace("u", "U4").Replace("v", "U5").Replace("w", "U6").Replace("o", "U7").Replace("p", "U8").Replace("q", "U9").Replace("r", "U0");
        string str2 = "";
        string str3 = "";
        for (int i = 1; i < str.Length; i++)
        {
            if (str[i] == 'U')
            {
                if (str2 != "")
                {
                    num = Convert.ToUInt32(str2);
                    str3 = str3 + Convert.ToChar(num).ToString().Trim();
                    str2 = "";
                }
            }
            else
            {
                char ch2 = str[i];
                str2 = str2 + ch2.ToString().Trim();
            }
        }
        if (str2 != "")
        {
            num = Convert.ToUInt32(str2);
            str3 = str3 + Convert.ToChar(num).ToString().Trim();
        }
        return str3;
    }
    #endregion

    #region EncodeURL 加密
    public static string EncodeURL(string NameValueStr)
    {
        if ((NameValueStr == null) || (NameValueStr == ""))
        {
            return "";
        }
        string str = "";
        NameValueStr = NameValueStr.Trim();
        for (int i = 0; i < NameValueStr.Length; i++)
        {
            str = str + "U" + Convert.ToUInt16(NameValueStr[i]).ToString().Trim();
        }
        return str.Replace("1U1", "X").Replace("2U2", "Y").Replace("3U3", "Z").Replace("4U5", "V").Replace("7U6", "W").Replace("8U7", "O").Replace("0U8", "P").Replace("0U9", "Q").Replace("1U0", "R").Replace("2U4", "S").Replace("U1", "x").Replace("U2", "y").Replace("U3", "z").Replace("U4", "u").Replace("U5", "v").Replace("U6", "w").Replace("U7", "o").Replace("U8", "p").Replace("U9", "q").Replace("U0", "r");
    }
    #endregion

    /// <summary>
    /// 加密sessionId，先base64，再md5
    /// </summary>
    /// <returns></returns>
    public static string Base64MD5SessionId(string sessionid)
    {
        return PageValidate.StrMd5(Base64UTF8Encode(sessionid));
    }
}