using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    public class Combortree
    {
        /// <summary>
        /// value值
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 展示文本
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public List<Combortree> children { get; set; }
    }
}