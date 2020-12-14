using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun
{
    public class PowerDto
    {
        public int y_roleID { get; set; }

        public int y_moduleID { get; set; }

        public int y_menu { get; set; }

        public int y_insert { get; set; }

        public int y_delete { get; set; }

        public int y_update { get; set; }

        public int y_select { get; set; }

        public string y_moudleName { get; set; }

        public string y_url { get; set; }

        public int y_sort { get; set; }

        public List<PowerDto> children { get; set; }
    }
}