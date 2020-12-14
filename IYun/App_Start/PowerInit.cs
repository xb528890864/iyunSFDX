using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun
{
    public class PowerInit
    {

        public static List<IGrouping<int, VW_Power>> InitPower()
        {
            using (var ad = new IYunEntities())
            {
                var list = ad.VW_Power.Where(u => u.y_vaild == 1).GroupBy(u => u.y_roleID).ToList();

                HttpRuntime.Cache.Insert("PowerList", list);

                return list;
            }
        }
    }
}