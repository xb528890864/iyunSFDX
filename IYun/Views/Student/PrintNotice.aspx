<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="IYun.Models" %>

<!DOCTYPE html>

<% 
    var list = ViewData["list"] as List<VW_StuInfo>;
    list = list ?? new List<VW_StuInfo>();
    var month = DateTime.Now.Month;
    var day = DateTime.Now.Day;
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
                page-break-after: always;
            }

            .w3cbbs {
                page-break-after: always;
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
                width: 21cm;
                height: 29.7cm;
                margin: 0 auto;
                border: 1px #D3D3D3 solid;
                border-radius: 0px;
                background: white;
                box-shadow: 0 0 0px rgba(0, 0, 0, 0.1);
            }

            .subpage {
                border: 0px red solid;
                height: 100%;
                background: url(../../Content/IYun/images/img.jpg);
                background-repeat: no-repeat;
                background-size: 100% 100%;
                padding: 0px;
            }

            .a1 {
                width: 511px;
                height: 605px;
                padding-top: 194px; /*background:url(../images/img.jpg) no-repeat;*/
            }

            .ul-a {
                width: 800px;
                margin-left: 36px;
                overflow: hidden;
                margin-bottom: 290px;
            }

                .ul-a li {
                    float: left;
                    height: 31px;
                    line-height: 31px;
                    font-size: 14px;
                }

                .ul-a .b1 {
                    padding-left: 150px;
                    width: 252px;
                }

                .ul-a .b2 {
                    padding-left: 218px;
                    width: 460px;
                }

                .ul-a li input {
                    width: 99%;
                    border: 0px;
                    padding: 0px;
                    background: none;
                }


            .ul-b {
                width: 800px;
                margin-left: 36px;
                overflow: hidden;
            }

                .ul-b li {
                    float: left;
                    height: 54px;
                    line-height: 54px;
                }

                    .ul-b li input {
                        border: 0px;
                        text-align: left;
                        background: none;
                        font-size: 18px;
                    }

                    .ul-b li .b1 {
                        width: 233px;
                        margin-right: 68px;
                        text-align: right;
                    }

                    .ul-b li .b2 {
                        width: 180px;
                        text-align: center;
                    }

                    .ul-b li .b3 {
                        margin-left: 178px;
                        width: 98px;
                        text-align: center;
                        margin-right: 28px;
                    }

                    .ul-b li .b4 {
                        width: 86px;
                        text-align: center;
                        margin-right: 52px;
                    }

                    .ul-b li .b5 {
                        width: 70px;
                        margin-right: 22px;
                        text-align: center;
                    }

                    .ul-b li .b6 {
                        width: 70px;
                        text-align: center;
                    }

            .ul-c {
                margin-top: 145px;
                margin-left: 535px;
                width: 200px;
            }

                .ul-c li {
                    height: 12px;
                    line-height: 12px;
                    font-size: 12px;
                    float: left;
                }

                    .ul-c li input {
                        border: 0px;
                        text-align: center;
                        background: none;
                    }

                    .ul-c li .b1 {
                        width: 60px;
                        margin-right: 18px;
                    }

                    .ul-c li .b2 {
                        width: 60px;
                    }

            .a2 {
                line-height: 18px;
                padding-left: 36px;
                width: 336px;
                margin-bottom: 65px;
            }

                .a2 input {
                    text-align: right;
                    width: 100%;
                    background: none;
                    border: 0px;
                    font-size: 18px;
                }
        }

        .subpage {
            border: 0px red solid;
            height: 100%;
            background: url(../../Content/IYun/images/img.jpg);
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

    <div class="book" style="margin: 0 auto;">
        <%
            foreach (var stu in list)
            {
                %>
                <div class="page w3cbbs">
                    <div class="subpage">
                        <div class="a1">
                            <ul class="ul-a">
                                <li class="b1"><input type="text" value="<%= stu.y_name %>"/></li>
                                <li class="b2"><input type="text" value="<%= stu.y_sex==0?"男":"女" %>"/></li>
                                <li class="b1"><input type="text" value="<%= schoolname %>"/></li>
                                <li class="b2"><input type="text" value="<%= stu.majorLibraryName %>"/></li>
                                <li class="b1"><input type="text" value=""/></li>
                                <li class="b2"><input type="text" value="<%= stu.schoolName %>"/></li>
                            </ul>
    
                            <div class="a2"><input type="text" value="<%= stu.y_name %>"/></div>

                            <ul class="ul-b">
                                <li>
                                    <input type="text" class="b1 bolderfont" value="<%= schoolname %>">
                                    <input type="text" class="b2 bolderfont" value="<%= stu.majorLibraryName %>">
                                </li>
                                <li>
                                    <input type="text" class="b3 bolderfont" value="3" />
                                    <input type="text" class="b4 bolderfont" value="1" />
                                    <input type="text" class="b5 bolderfont" value="3" />
                                    <input type="text" class="b6 bolderfont" value="3" />
                                </li>
                            </ul>
    
                            <ul class="ul-c">
                                <li><input type="text" class="b1 bolderfont" value="<%= month %>" /></li>
                                <li><input type="text" class="b2 bolderfont" value="<%= day %>" /></li>
                            </ul>
    
    
    
                        </div>
                    </div>
                </div>
        <%
            }
             %>
    </div>
</body>
</html>
