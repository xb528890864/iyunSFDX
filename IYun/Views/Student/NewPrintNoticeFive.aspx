<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
    <%@ Import Namespace="IYun.Models" %>
    
    <!DOCTYPE html>
    
    <% 
        var list = ViewData["list"] as List<VW_StuInfo>;
        list = list ?? new List<VW_StuInfo>();
        var month =12;
        var day = 30;
        var schoolname = System.Configuration.ConfigurationManager.AppSettings["SchoolTable"];
    %>
    <html>
    <head>
        <meta name="viewport" content="width=device-width" />
        <script src="../../Content/IYun/js/jquery-1.9.1.min.js"></script>
        <script src="../../Content/IYun/jquery.jqprint-0.3.js"></script>
        <script src="../../Content/IYun/jquery-migrate-1.1.0.js"></script>
        <link href="../../Content/IYun/print.css" rel="stylesheet" />
        <link href="../../Content/IYun/css/common.css" rel="stylesheet" />
        <link href="../../Content/IYun/css/css.css" rel="stylesheet" />
        <title>入学通知书</title>
        <script>
            $(document).ready(function () {
                $(".book").jqprint({ debug: true, importCSS: true, printContainer: true, operaSupport: true });
            })
    
        </script>
        <style>
            *{box-sizing:border-box;··
    -moz-box-sizing:border-box; /* Firefox */
    -webkit-box-sizing:border-box; /* Safari */}
            .subpage {
                border: 0px red solid;
                height: 100%;
                background: url(../../Content/IYun/images/img1.jpg);
                background-repeat: no-repeat;
                background-size:100% 100%;
                padding: 0px;
            }
    
        </style>
    </head>
    <body>
    
        <div class="book" style="margin: 0 auto;">
            <style>
                .ov{overflow:hidden}
                .a1{width:100%;padding-top:83pt; /*background:url(../images/img.jpg) no-repeat;*/}
                .ul-a{width:100%; margin-left:0; overflow:hidden; margin-bottom:303pt;}
                .ul-a li{float: left;
        line-height: 28pt;
        font-size: 14px;
        margin-bottom: 3pt;}
                .ul-a .b1{padding-left:27%; width:60%;}
                .ul-a .b2{padding-left:13%; width:40%;}
                .ul-a li input{width:99%; border:0px; padding:0px; background:none;font-size:14px;}
    
    
                .ul-b{width:800px; margin-left:36px;margin-top:40pt;overflow:hidden;}
                .ul-b li{float:left;line-height:30pt;}
                .ul-b li input{border:0px; text-align:left; background:none; font-size:19px;}
                    .ul-b li .b1 {
                        width: 500px;
                        margin-left: 110px;
                        text-align: center;
                    }
                .ul-b li .b2{width:180px; text-align:center;}
                .ul-b li .b3{margin-left: 40px;width:98px; text-align:center; margin-right:28px;}
                .ul-b li .b4{width:86px; text-align:center; margin-right:52px;}
                .ul-b li .b5{width:70px; margin-right:22px; text-align:center;}
                .ul-b li .b6{width:70px; text-align:center;}
    
                .ul-c{margin-top: 145px;margin-left: 0}
                .ul-c li{height:12pt; line-height:12pt; font-size:12px; float:left;width:62pt}
                .ul-c li input{border:0px; text-align:center; background:none;}
                .ul-c li .b1{width:60px; margin-right:18px;}
                .ul-c li .b2{width:60px;}
    
                .a2 {
                    line-height: 18px;
                    padding-left: 0;
                    width: 100%;
                    margin-top:5px;
                }
                .a2 input{ text-align:right; width:50%; background:none; border:0px;font-size:20px;float:left;font-weight:bold;padding:0 40pt}
                .vvv{padding-left:10pt}
                .f_tab td{font-size:10px}
            </style>
            <%
                foreach (var stu in list)
                {
                    %>
                    <div class="page w3cbbs">
                        <div class="subpage">
                            <div class="a1">
                                <ul class="ul-a">
                                    <li class="b1"><input type="text" value="<%= stu.y_name %>"/></li>
                                    <li class="b2"><input type="text" value=""/></li>
                                    <li class="b1"><input type="text" value="<%= schoolname %>"/></li>
                                    <li class="b2"><input type="text" value="<%= stu.majorLibraryName %>"/></li>
                                    <li class="b1"><input type="text" value="<%= stu.schoolName %>"/></li>
                                    <li class="b2"><input type="text" value="<%= stu.subcontact %>"/></li>
                                </ul>
        
                                <div class="a2 ov">
                                    <input type="text" value="<%= stu.y_examNum %>"/>
                                    <input type="text" value="<%= stu.y_name %>"/ style="text-align:left">
    
                                </div>
    
                                <ul class="ul-b">
                                    <li style="width:100%">
                                        <input type="text" class="b1 bolderfont" value="<%= schoolname %>" style="font-weight:bold">
                                        
                                    </li>
                                    <li style="width:100%">
                                        <input type="text" style="margin-left: 160px;width:100%;font-weight:bold" value="<%= stu.majorLibraryName %>">
                                    </li >
                                    <li>
                                        <span style="margin-left:120px">二</span>
                                        <span style="margin-left:70px">十八</span>
                                        <span style="margin-left:90px">二</span>
                                        <span style="margin-left:70px">二十五</span>
                                    </li >
                                     <%if (ConfigurationManager.AppSettings["SchoolName"].ToString() == "JXLG")
                                         {%>
                                    <li style="width:100%">
                                        <input type="text" style="margin-left: 60px;width:100%;font-weight:bold" value="元">
                                    </li>
                                    <%} %> 
                                    
                                </ul>
                        <div class="ov" style="padding-top:0pt">
                                <ul class="ul-c fr" style="margin-top:110pt; margin-right:50pt">
                                    <li><input type="text" class="b1 bolderfont" value="<%= month %>" /></li>
                                    <li><input type="text" class="b2 bolderfont" value="<%= day %>" /></li>
                                </ul>
                                <ul class="fl" style="width: 60%;margin-top: 90pt;padding-left: 9%;">
                                    <%if (ConfigurationManager.AppSettings["SchoolName"].ToString() == "JXLG")
                                        {%>
                                    <li class="fl" style="width:32%">
                                        <img src="../../Content/images/erweim.png" style="width: 100pt;height: 100pt;">
                                    </li>
                                    <%}else if (ConfigurationManager.AppSettings["SchoolName"].ToString() == "JXSFDX")
                                        {%>
                                    <li class="fl" style="width:32%">
                                        <img src="../../Content/images/erweim.jpg" style="width: 100pt;height: 100pt;">
                                    </li>
                                    <%} %>
                                    
                                    <%if (ConfigurationManager.AppSettings["SchoolName"].ToString() == "JXSFDX")
                                        { %>
                                            <li class="fl" style="width:68%;margin-top:10px">
                                                <table border="0" width="80%" class="f_tab">
                                                    <tbody>
                                                        <tr>
                                                            <td align="right" width="25%">所属站点:</td>
                                                            <td class="vvv"><%=stu.schoolName %></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="top">地址:</td>
                                                            <td class="vvv"><%=stu.subaddress %></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">负责人:</td>
                                                            <td class="vvv"><%=stu.subcontact %></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">联系电话:</td>
                                                            <td class="vvv"><%=stu.subtel %></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </li>
                                    <%} %>
                                </ul>
                        </div>
        
                            </div>
                        </div>
                    </div>
            <%
                }
                 %>
        </div>
    </body>
    </html>
    