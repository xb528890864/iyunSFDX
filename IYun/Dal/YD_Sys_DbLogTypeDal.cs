
using IYun.Models;
using Newtonsoft.Json;

namespace IYun.Dal
{
    public class YD_Sys_DbLogTypeDal:BaseDal<YD_Sys_DbLogType>
    {
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public string DbLogTypeList(IYunEntities yunEntities)
        {
            return JsonConvert.SerializeObject(List(u => true, yunEntities));
        }
    }
}