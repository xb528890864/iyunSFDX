using System.Web.Mvc;
using System;

namespace IYun.Web.Views
{
    public abstract class EduPageBase<TModel> : WebViewPage<TModel>
    {
        protected EduPageBase()
        {
        }

        /// <summary>
        /// 把分数变成String类型，并且判断如果是整数返回整数
        /// </summary>
        /// <param name="score">分数</param>
        /// <returns>分数</returns>
        protected string S(decimal score)
        {
            return Convert.ToInt32(Math.Floor(score)).ToString();

            //decimal eps = Convert.ToDecimal(1e-10); // 精度范围
            //if (score - (int) score >= eps)
            //{
            //    return score.ToString();
            //}
            //else
            //{
            //    return Convert.ToInt32(score).ToString();
            //}
        }

        protected string C(decimal score,string courseName)
        {
            if (courseName.Contains("毕业设计") || courseName.Contains("毕业论文"))
            {
                if (score >= 90)
                {
                    return "优";
                }
                else if (score >= 80)
                {
                    return "良好";
                }
                else if (score >= 70)
                {
                    return "中等";
                }
                else if (score >= 60)
                {
                    return "及格";
                }
                else
                {
                    return "不及格";
                }
            }
            else
            {
                return Convert.ToInt32(Math.Floor(score)).ToString();
            }

        }
    }
}