using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// MD5的帮组类,包含产生字符串的MD5和文件流的MD5
/// </summary>
public class MD5Helper
{
    private MD5 md5 = MD5.Create();
    /// <summary>
    /// 生成字符串的MD5
    /// </summary>
    /// <param name="str">要生成MD5的字符串</param>
    /// <returns>MD5码</returns>
    public string MD5String(string str)
    {
        //将字符串转换成字节数组
        byte[] bt = Encoding.GetEncoding("gb2312").GetBytes(str);
        //获得MD5的字节数组
        bt = md5.ComputeHash(bt);
        return GetMD5ByBytes(bt);
    }
    /// <summary>
    /// 生成流的MD5
    /// </summary>
    /// <param name="path">要生成MD5的流的地址</param>
    /// <returns>MD5码</returns>
    public string MD5Stream(string path)
    {
        byte[] bt = md5.ComputeHash(new FileStream(path, FileMode.OpenOrCreate));
        return GetMD5ByBytes(bt);
    }
    /// <summary>
    /// 根据字节数组返回MD5值
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    private static string GetMD5ByBytes(byte[] bt)
    {
        List<string> md5String = new List<string>();
        //将数组中每个字节转换成16进制的string类型
        for (int i = 0; i < bt.Length; i++)
        {
            md5String.Add(bt[i].ToString("x2"));
        }
        return string.Concat(md5String);
    }
}
