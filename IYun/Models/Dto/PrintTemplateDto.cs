using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models.Dto
{
    [Serializable]
    public class PrintTemplateDto
    {
        public int id { get; set; }
        public string y_name { get; set; }
        public string y_dataview { get; set; }
        public List<Div> divs { get; set; }
    }
    [Serializable]
    public class Div
    {
        public int id { get; set; }
        public string type { get; set; }
        public string range { get; set; }
        public string fontsize { get; set; }
        public string bold { get; set; }
    }
}