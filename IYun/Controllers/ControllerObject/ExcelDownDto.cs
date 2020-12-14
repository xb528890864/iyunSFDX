namespace IYun.Controllers.ControllerObject
{
    public class ExcelDownDto
    {
        public bool IsOk { get; set; }

        public string Message { get; set; }  //如果操作成功，此属性保存文件地址，否则保存报错信息
    }
}