using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using IYun.Models;
using NPOI.SS.Formula.Functions;

namespace IYun.Object
{
    public class HelperParameterM
    {
        public string className { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int? index { get; set; }
        public string style { get; set; }
        public string whereSql { get; set; }
    }
}