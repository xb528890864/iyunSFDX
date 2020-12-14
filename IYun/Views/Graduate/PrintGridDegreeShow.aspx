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
    <title>学士学位证书</title>
    <script>
        $(document).ready(function () {
            $(".degree").jqprint({ debug: true, importCSS: true, printContainer: true, operaSupport: true });
        })

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
                font: 12pt "Tahoma";
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
                height: 100%;
                background: url(../../Content/IYun/images/xwimg.jpg);
                background-repeat: no-repeat;
                background-size: 100% 100%;
                padding: 0px;
            }

            .a1 {
                width: 511px;
                height: 605px;
                padding-top: 250px; 
                overflow:hidden;
            }
            .ul-b {
                width: 800px;
                margin-left: 36px;
                overflow: hidden;
            }             
        }

        .subpage {
            border: 0px red solid;
            height: 100%;
            background: url(../../Content/IYun/images/xwimg.jpg);
            background-repeat: no-repeat;
			background-size:100% 100%;
            padding: 0px;
        }

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
        <div class="page " style="height:210mm; overflow:hidden">
            <div class="subpage">
                <div class="a1">
                    <div style="width:600px; font-size:20px; font-weight:500 ; margin-top:200px">
                        <div style="margin-left:160px; width:80px; float:left"><%= stu.y_name %></div>
                        <div style="margin-left:32px;width:30px;float:left"><%= stu.y_sex == 0 ? "男" : "女" %></div>
                        <div  style="margin-left:22px;width:60px;float:left"><%= stu.y_birthday.Year %></div>
                        <div style="margin-left:42px;width:20px;float:left"><%= stu.y_birthday.Month  %></div>
                        <div  style="margin-left:45px;width:80px;float:left"><%= stu.y_birthday.Day  %></div>
                    </div>
                 
                    <div style="clear:both"></div>
                    <div class="ul-b">
                     

                            <div  style="font-size:20px; font-weight:800 ;margin-left:200px; margin-top:27px" >（自学考试）电子商务</div>
                       
                    </div>
                    <div style="clear:both"></div>
                    <div class="ul-d" style="padding-top:23px" >
                        <div  style="font-size:20px; font-weight:800" >管理学










</div>
                      
                    </div>    
                    <div style="clear:both"></div>
                    <div class="ul-e" style="margin-top:57px; width:1000px">
                         <span class="b2 bolderfont"  style="float:left; margin-left:185px; width:200px;"> <%= stu.y_degreeNum %></span>
                         <span style="float:right; width:250px; font-size:18px">
                             <span style="margin-left:17px">二〇一八</span>
                              <span style="margin-left:30px">十二</span>
                              <span style="margin-left:30px">二十四</span>
                         </span>
                    </div>
 
                </div>
            </div>
        </div>
      <% } %>
    </div>
</body>
</html>
