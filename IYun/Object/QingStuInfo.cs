using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public class QingStuInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string phoneNum { get; set; }
        public string identityCard { get; set; }
        public string genderName { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string photoPath { get; set; }
        public int centerId { get; set; }
        public int majorId { get; set; }
        public int semesterId { get; set; }
        public string studentNumber { get; set; }
        public int studyStatusId { get; set; }
        public string studyStatusName { get; set; }
    }
}