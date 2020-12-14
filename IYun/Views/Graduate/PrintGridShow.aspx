<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="IYun.Models" %>

<!DOCTYPE html>

<% 
    var list = ViewData["list"] as List<VW_StuInfo>;
    list = list ?? new List<VW_StuInfo>();

%>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <script src="../../Content/IYun/js/jquery-1.9.1.min.js"></script>
    <script src="../../Content/IYun/jquery.jqprint-0.3.js"></script>
    <script src="../../Content/IYun/jquery-migrate-1.1.0.js"></script>
    <link href="../../Content/IYun/css/PrintGrid.css" rel="stylesheet" />
    <link href="../../Content/IYun/css/common.css" rel="stylesheet" />
    <link href="../../Content/IYun/css/Printcss.css" rel="stylesheet" />
    <title>毕业证书</title>
    <script>

        $(document).ready(function () {
            $(".degree").jqprint({ debug: false, importCSS: true, printContainer: true, operaSupport: true });

        })

        function convertToChinaNumyear(num) {
            var arr1 = new Array('〇', '一', '二', '三', '四', '五', '六', '七', '八', '九');

            //  var arr2 = new Array('', '十', '百', '千', '万', '十', '百', '千', '亿', '十', '百', '千', '万', '十', '百', '千', '亿');//可继续追加更高位转换值

            if (!num || isNaN(num)) {
                return "〇";
            }
            var english = num.toString().split("")
            var result = "";
            for (var i = 0; i < english.length; i++) {
                var des_i = english.length - 1 - i;//倒序排列设值
                result = result;//arr2[i] + result;
                var arr1_index = english[des_i];
                result = arr1[arr1_index] + result;
            }
            ////将【零千、零百】换成【零】 【十零】换成【十】
            //result = result.replace(/零(千|百|十)/g, '零').replace(/十零/g, '十');
            ////合并中间多个零为一个零
            //result = result.replace(/零+/g, '零');
            ////将【零亿】换成【亿】【零万】换成【万】
            //result = result.replace(/零亿/g, '亿').replace(/零万/g, '万');
            ////将【亿万】换成【亿】
            //result = result.replace(/亿万/g, '亿');
            ////移除末尾的零
            //result = result.replace(/零+$/, '')
            ////将【零一十】换成【零十】
            ////result = result.replace(/零一十/g, '零十');//貌似正规读法是零一十
            ////将【一十】换成【十】
            //result = result.replace(/^一十/g, '十');
            return result;
        }
        function convertToChinaNum(num) {
            var arr1 = new Array('', '一', '二', '三', '四', '五', '六', '七', '八', '九');
           
              var arr2 = new Array('', '十', '百', '千', '万', '十', '百', '千', '亿', '十', '百', '千', '万', '十', '百', '千', '亿');//可继续追加更高位转换值
            ////if (num==10) {
            ////    return "十";
            ////}
           
            ////if (num==20) {
            ////    return "二十";
            ////}
            ////if (num==30) {
            ////    return "三十";
            ////}
            if (!num || isNaN(num)) {
                return "〇";
            }
            var english = num.toString().split("")
            var result = "";
            for (var i = 0; i < english.length; i++) {
                var des_i = english.length - 1 - i;//倒序排列设值
                result = arr2[i] + result;
                var arr1_index = english[des_i];
                result = arr1[arr1_index] + result;
            }
            //将【零千、零百】换成【零】 【十零】换成【十】
            result = result.replace(/零(千|百|十)/g, '零').replace(/十零/g, '十');
            //合并中间多个零为一个零
            result = result.replace(/零+/g, '零');
            //将【零亿】换成【亿】【零万】换成【万】
            result = result.replace(/零亿/g, '亿').replace(/零万/g, '万');
            //将【亿万】换成【亿】
            result = result.replace(/亿万/g, '亿');
            //移除末尾的零
            result = result.replace(/零+$/, '')
            //将【零一十】换成【零十】
            //result = result.replace(/零一十/g, '零十');//貌似正规读法是零一十
            //将【一十】换成【十】
            result = result.replace(/^一十/g, '十');
            return result;
        }

    </script>
    <style>
        @page {
            size: A4;
            margin: 0;
        }

      @media print {
            .page {
                margin: 0;
                border: initial;
                border-radius: initial;
                width: initial;
                min-height: initial;
                box-shadow: initial;
                background: initial;
              height:210mm; overflow:hidden
            }



            body {
                margin: 0;
                padding: 0;
                background-color: #FAFAFA;
                font: 12pt "楷体";
                font-family:楷体;
            }

            * {
                box-sizing: border-box;
                -moz-box-sizing: border-box;
            }

            .page {
                width: 297cm;
                height: 210cm;
                margin: 0 auto;
                border: 1px #D3D3D3 solid;
                border-radius: 0px;
                background: white;
                box-shadow: 0 0 0px rgba(0, 0, 0, 0.1);
                overflow:hidden
            }

            .subpage {
                border: 0px red solid;
                height: 500px;
                /*background: url(../../Content/IYun/images/xwimg.jpg);*/
                background-repeat: no-repeat;
                background-size: 100% 100%;
                padding: 0px;
            }

            .a1 {
                width: 511px;
                height: 500px;
                /*padding-top: 250px;*/ 
                overflow:hidden;
            }
            .ul-b {
                width: 800px;
                margin-left: 36px;
                overflow: hidden;
            }             
        }

        /*.subpage {
            border: 0px red solid;
            height: 100%;
            background: url(../../Content/IYun/images/xwimg.jpg);
            background-repeat: no-repeat;
			background-size:100% 100%;
            padding: 0px;
        }*/

        .bolderfont {
            font-size: 14px !important;
            font-weight: bolder !important;
        }

        input {
            font-weight: 700;
        }
    </style>
</head>
<body>

    <div class="degree" style="margin: 0 auto;">
          <%
    foreach (var stu in list)
    {
        %>
        <div class="page " style="height:200mm; overflow:hidden;background-color:aquamarine">
            <div style="height:150px"></div><%--height:150px--%>
            <div class="subpage" style="">
                <div class="a1">

                    <div style="width:1200px; font-size:20px; font-weight:bold ;font-family:华文隶书;">
                        <div style=" width:400px; float:left">&nbsp;</div>
                        <div style="width:112px; float:left;font-size:25px;margin-top:16px;"><%= stu.y_name %></div>
                        <div style="margin-left:58px;width:30px;float:left;margin-top:16px;"><%= stu.y_sex == 0 ? "男" : "女" %></div>
                        <div style="margin-left:28px;float:left;margin-top:16px;" id="Year<%= stu.id %>"> </div>
                        <div style="margin-left:40px;width:43px;float:left;margin-top:16px;" id="Month<%= stu.id %>"></div>
                        <div style="margin-left:39px;width:62px;float:left;margin-top:16px;font-size:17px;"  id="Day<%= stu.id %>"></div>
                        <%--<div  style="margin-left:22px;width:60px;float:left"><%= stu.y_birthday.Year %></div>
                        <div style="margin-left:42px;width:20px;float:left"><%= stu.y_birthday.Month  %></div>
                        <div  style="margin-left:45px;width:80px;float:left"><%= stu.y_birthday.Day  %></div>--%>
                        <div  style="margin-left:100px;width:90px;float:left;margin-top:16px;" id="InYear<%= stu.id %>"><%--<%= stu.y_parentName2 %>--%></div> 
                        <div style="margin-left:40px;width:45px;float:left"><%--<%= stu.y_parentCard1 %>--%></div>
                        <div  style="margin-left:27px;width:80px;float:left"><%--<%= stu.y_parentCard2 %>--%></div>
                        <div  style="margin-left:90px;width:90px;float:left"><%--<%= stu.y_parentName1 %>--%></div>
                    </div>
                                                                             <script>    
                                                                                 $("#Year<%= stu.id %>").html(convertToChinaNumyear(<%= stu.y_birthday.Year %>));

                                                                                 $("#Month<%= stu.id %>").html(convertToChinaNum(<%= stu.y_birthday.Month %>));
                                                                                 $("#Day<%= stu.id %>").html(convertToChinaNum(<%= stu.y_birthday.Day %>));
                                                                                 $("#InYear<%= stu.id %>").html(convertToChinaNumyear(<%= stu.y_inYear %>));
                                                                             </script>
                    <div style="clear:both"></div>
                    <div class="ul-b" style="font-size:20px;width:1200px;font-weight:bold ;font-family:华文隶书;">
                     
                        <div style=" width:300px; float:left">&nbsp;</div>
                        <div  style="float:left;width:30px;margin-top:9px">二</div>
                        <div  style="margin-left:60px;float:left;width:90px;margin-top:9px">二〇二一</div>
                        <div  style="margin-left:35px;float:left;width:100px;margin-top:9px">一</div>
                        <div  style="/*font-weight:800 ;*/float:left;font-size:25px; margin-top:5px;margin-left:56px" ><%= stu.majorLibraryName %></div>
                       
                    </div>

                    <div style="clear:both"></div>
                    <div class="ul-b" style="font-size:20px;width:1200px;font-weight:bold ;font-family:华文隶书" >
                        <div style=" width:380px; float:left">&nbsp;</div>
                        <div  style="width:100px; /*font-weight:800;*/float:left;margin-top:5px;margin-left:-15px; font-size:25px;"><%= stu.stuTypeName %></div>
                        <%
                            if (stu.eduTypeName=="高起本")
                            {
                                stu.eduTypeName = "本";
                            }                       
                            if (stu.eduTypeName=="专升本")
                            {
                                stu.eduTypeName = "专科起点本";
                            }             
                            if (stu.eduTypeName=="高起专")
                            {
                                stu.eduTypeName = "专";
                            }
                            %>
                        <div  style="margin-left:150px; /*font-weight:800;*/ float:left;margin-top:5px;font-size:25px;"><%= stu.eduTypeName %></div>
                      
                    </div>    
                    <div style="clear:both"></div>
                    <div class="ul-b" style="width:1200px;font-family:方正舒体;">
                        <div style=" width:420px; float:left">&nbsp;</div>
                        <div  style="width:200px;font-size:30px; font-weight:800;float:left;margin-top:54px;font-family:方正新舒体简体; ">江西师范大学</div>
                        <%--<div  style="font-size:20px; /*font-weight:800;*/ float:left;margin-top:27px"><%= stu.eduTypeName %></div>--%>
                      
                    </div> 
                    
                    <div style="clear:both"></div>
                    <div class="ul-b" style="width:1200px;font-weight:bold ;font-family:楷体;">
                        <div style=" width:365px; float:left">&nbsp;</div>
                        <div  style="width:250px;font-size:20px;float:left;margin-top:20px;margin-left:10px;font-family:方正新舒体简体; ">（83）教成字002号</div>
                        <%--<div  style="font-size:20px; /*font-weight:800;*/ float:left;margin-top:27px"><%= stu.eduTypeName %></div>--%>
                      
                    </div> 
                    <div style="clear:both"></div>
                    <div style="margin-top:0px; width:1200px;font-size:20px;font-weight:bold ;font-family:华文隶书;">
                        <div style=" width:400px; float:left">&nbsp;</div>
                         <span  style="float:left; width:200px;font-family:楷体;margin-left:10px;margin-top:0px"> <%= stu.y_graduateNumber %></span>
                         <span style="float:left; margin-left:180px;width:300px; ">
                             <span style="margin-left:-20px;margin-top:10px">二〇二一</span>
                              <span style="margin-left:40px;margin-top:10px">一</span>
                              <span style="margin-left:65px;margin-top:10px">十</span>
                         </span>
                    </div>
 
                </div>
            </div>
        </div>
        <div style="page-break-after:always;"></div>
      <% } %>
    </div>
</body>
</html>
