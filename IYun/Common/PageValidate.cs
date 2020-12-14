using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text.RegularExpressions;
using System.Text;

using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
///PageValidate 的摘要说明
/// </summary>
public class PageValidate
{
    private static Regex RegPhone = new Regex("^(13[0-9]|15[0-9]|18[0-9])\\d{8}$");
    private static Regex RegNumber = new Regex("^[0-9]+$");
    private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
    private static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
    private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$
    private static Regex RegEmail = new Regex("([/s/S]*)+@[\\w-]+\\.[\\w-]");//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样     "^[\\w-]+@[\\w-]+\\.[\\w-]"
    private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
    private static Regex RegId = new Regex("^(\\d{15}$|^\\d{18}$|^\\d{17}(\\d|X|x))$");
    private static Regex RegYinWen = new Regex("^[a-zA-Z]+$");
    private static Regex RegYinWenD = new Regex("^[a-zA-Z\\.\\-]"); //只能输入英文和点
    private static Regex RegYinWenNum = new Regex("^[a-zA-Z0-9\\.]+$");
    private static Regex RegEmailName = new Regex("^[\\w-]+$");

    public PageValidate()
    {
    }

    /// <summary>
    /// 手机验证
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsPhone(string inputData)
    {
        //Match m = RegPhone.Match(inputData);
        //return m.Success;
        return RegPhone.IsMatch(inputData);
    }

    /// <summary>
    /// 英文验证
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsYinWen(string inputData)
    {
        return RegYinWen.IsMatch(inputData);
    }

    /// <summary>
    /// 英文和点验证
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsYinWenD(string inputData)
    {
        return RegYinWenD.IsMatch(inputData);
    }
    /// <summary>
    /// 英文数字验证
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsYinWenNum(string inputData)
    {
        return RegYinWenNum.IsMatch(inputData);
    }
    /// <summary>
    /// 检查Request查询字符串的键值，是否是数字，最大长度限制
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="inputKey">Request的键值</param>
    /// <param name="maxLen">最大长度</param>
    /// <returns>返回Request查询字符串</returns>
    public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen)
    {
        string retVal = string.Empty;
        if (!string.IsNullOrEmpty(inputKey))
        {
            retVal = req.QueryString[inputKey];
            if (null == retVal)
                retVal = req.Form[inputKey];
            if (null != retVal)
            {
                retVal = SqlText(retVal, maxLen);
                if (!IsNumber(retVal))
                    retVal = string.Empty;
            }
        }
        if (retVal == null)
            retVal = string.Empty;
        return retVal;
    }
    /// <summary>
    /// 是否数字字符串
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsNumber(string inputData)
    {
        Match m = RegNumber.Match(inputData);
        return m.Success;
    }

    /// <summary>
    /// 身份证号码是否正确
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsIdNumber(string inputData)
    {
        Match m = RegId.Match(inputData);
        return m.Success;
    }

    /// <summary>
    /// 是否数字字符串 可带正负号
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsNumberSign(string inputData)
    {
        Match m = RegNumberSign.Match(inputData);
        return m.Success;
    }
    /// <summary>
    /// 是否是浮点数
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsDecimal(string inputData)
    {
        Match m = RegDecimal.Match(inputData);
        return m.Success;
    }
    /// <summary>
    /// 是否是浮点数 可带正负号
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsDecimalSign(string inputData)
    {
        Match m = RegDecimalSign.Match(inputData);
        return m.Success;
    }

    /// <summary>
    /// 是否是Email用户名
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsEmailName(string inputData)
    {
        Match m = RegEmailName.Match(inputData);
        return m.Success;
    }


    #region 中文检测

    /// <summary>
    /// 检测是否有中文字符
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static bool IsHasCHZN(string inputData)
    {
        Match m = RegCHZN.Match(inputData);
        return m.Success;
    }

    #endregion

    #region 邮件地址
    /// <summary>
    /// 是否正确的邮件地址
    /// </summary>
    /// <param name="inputData">输入字符串</param>
    /// <returns></returns>
    public static bool IsEmail(string inputData)
    {
        Match m = RegEmail.Match(inputData);
        return m.Success;
    }

    #endregion

    #region 日期格式判断
    /// <summary>
    /// 日期格式字符串判断
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsDateTime(string str)
    {
        try
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime.Parse(str);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region 其他

    /// <summary>
    /// 检查字符串最大长度，返回指定长度的串
    /// </summary>
    /// <param name="sqlInput">输入字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns></returns>			
    public static string SqlText(string sqlInput, int maxLength)
    {
        if (!string.IsNullOrEmpty(sqlInput))
        {
            sqlInput = sqlInput.Trim();
            if (sqlInput.Length > maxLength)//按最大长度截取字符串
                sqlInput = sqlInput.Substring(0, maxLength);
        }
        return sqlInput;
    }
    /// <summary>
    /// 字符串MD5加密，32位16进制
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static string StrMd5(string str)
    {
        return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
    }
    /// <summary>
    /// 字符串编码
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public static string HtmlEncode(string inputData)
    {
        return HttpUtility.HtmlEncode(inputData);
    }
    /// <summary>
    /// 设置Label显示Encode的字符串
    /// </summary>
    /// <param name="lbl"></param>
    /// <param name="txtInput"></param>
    public static void SetLabel(Label lbl, string txtInput)
    {
        lbl.Text = HtmlEncode(txtInput);
    }
    public static void SetLabel(Label lbl, object inputObj)
    {
        SetLabel(lbl, inputObj.ToString());
    }
    //字符串清理
    public static string InputText(string inputString, int maxLength)
    {
        StringBuilder retVal = new StringBuilder();

        // 检查是否为空
        if ((inputString != null) && (inputString != String.Empty))
        {
            inputString = inputString.Trim();

            //检查长度
            if (inputString.Length > maxLength)
                inputString = inputString.Substring(0, maxLength);

            //替换危险字符
            for (int i = 0; i < inputString.Length; i++)
            {
                switch (inputString[i])
                {
                    case '"':
                        retVal.Append("&quot;");
                        break;
                    case '<':
                        retVal.Append("&lt;");
                        break;
                    case '>':
                        retVal.Append("&gt;");
                        break;
                    default:
                        retVal.Append(inputString[i]);
                        break;
                }
            }
            retVal.Replace("'", " ");// 替换单引号
        }
        return retVal.ToString();

    }
    /// <summary>
    /// 转换成 HTML code
    /// </summary>
    /// <param name="str">string</param>
    /// <returns>string</returns>
    public static string Encode(string str)
    {
        str = str.Replace("&", "&amp;");
        str = str.Replace("'", "''");
        str = str.Replace("\"", "&quot;");
        str = str.Replace(" ", "&nbsp;");
        str = str.Replace("<", "&lt;");
        str = str.Replace(">", "&gt;");
        str = str.Replace("\n", "<br>");
        return str;
    }
    /// <summary>
    ///解析html成 普通文本
    /// </summary>
    /// <param name="str">string</param>
    /// <returns>string</returns>
    public static string Decode(string str)
    {
        str = str.Replace("<br>", "\n");
        str = str.Replace("&gt;", ">");
        str = str.Replace("&lt;", "<");
        str = str.Replace("&nbsp;", " ");
        str = str.Replace("&quot;", "\"");
        return str;
    }

    public static string SqlTextClear(string sqlText)
    {
        if (sqlText == null)
        {
            return null;
        }
        if (sqlText == "")
        {
            return "";
        }
        sqlText = sqlText.Replace(",", "");//去除,
        sqlText = sqlText.Replace("<", "");//去除<
        sqlText = sqlText.Replace(">", "");//去除>
        sqlText = sqlText.Replace("--", "");//去除--
        sqlText = sqlText.Replace("'", "");//去除'
        sqlText = sqlText.Replace("\"", "");//去除"
        sqlText = sqlText.Replace("=", "");//去除=
        sqlText = sqlText.Replace("%", "");//去除%
        sqlText = sqlText.Replace(" ", "");//去除空格
        return sqlText;
    }
    #endregion

    #region 是否由特定字符组成
    public static bool isContainSameChar(string strInput)
    {
        string charInput = string.Empty;
        if (!string.IsNullOrEmpty(strInput))
        {
            charInput = strInput.Substring(0, 1);
        }
        return isContainSameChar(strInput, charInput, strInput.Length);
    }

    public static bool isContainSameChar(string strInput, string charInput, int lenInput)
    {
        if (string.IsNullOrEmpty(charInput))
        {
            return false;
        }
        else
        {
            Regex RegNumber = new Regex(string.Format("^([{0}])+$", charInput));
            //Regex RegNumber = new Regex(string.Format("^([{0}]{{1}})+$", charInput,lenInput));
            Match m = RegNumber.Match(strInput);
            return m.Success;
        }
    }
    #endregion

    #region 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查
    /// <summary>
    /// 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查
    /// </summary>
    public static bool isContainSpecChar(string strInput)
    {
        string[] list = new string[] { "123456", "654321" };
        bool result = new bool();
        for (int i = 0; i < list.Length; i++)
        {
            if (strInput == list[i])
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion



        /// <summary>
    /// MD5 16位加密 加密后密码为大写
    /// </summary>
    /// <param name="ConvertString"></param>
    /// <returns></returns>
    public static string GetMd5StrT(string ConvertString)
    {
        var md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
        t2 = t2.Replace("-", "");
        return t2;
    }
    /// <summary>
    /// MD5 16位加密 加密后密码为小写
    /// </summary>
    /// <param name="ConvertString"></param>
    /// <returns></returns>
    public static string GetMd5StrL(string ConvertString)
    {
        if (!string.IsNullOrEmpty(ConvertString))
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");

            t2 = t2.ToLower();

            return t2;
        }
        else
        {
            return "";
        }
    }
   

}