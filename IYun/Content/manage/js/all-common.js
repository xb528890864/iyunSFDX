

//下拉列表框
$(document).ready(function () {

    jQuery.navlevel2 = function (level1, dytime) {
        $(level1).mouseenter(function () {
            varthis = $(this);
            delytime = setTimeout(function () {
                varthis.find('.lev2').slideDown();
            }, dytime);
        });
        $(level1).mouseleave(function () {
            clearTimeout(delytime);
            $(this).find('.lev2').slideUp();
        });
    };
    $.navlevel2(".navlev", 200);
});

/////////////////////*
$(document).ready(function () {
    $(".navlev").mouseover(function () {
        $(this).addClass("hover")

        $(".navlev").mouseout(function () {
            $(this).removeClass("hover")
        });
    });
});

/********placeholder//////**///
$(function () {
    if (!placeholderSupport()) {   // 判断浏览器是否支持 placeholder
        $('[placeholder]').focus(function () {
            var input = $(this);
            if (input.val() == input.attr('placeholder')) {
                input.val('');
                input.removeClass('placeholder');
            }
        }).blur(function () {
            var input = $(this);
            if (input.val() == '' || input.val() == input.attr('placeholder')) {
                input.addClass('placeholder');
                input.val(input.attr('placeholder'));
            }
        }).blur();
    };
})
function placeholderSupport() {
    return 'placeholder' in document.createElement('input');
}

/********提问弹出*************/
$(document).ready(function () {
    $(".show-c").css('display', 'none');
    $(".nava2").click(function () {
        $(this).parents(".nava1").find(".show-bg").stop().fadeTo(500, 1);
        $(this).parents(".nava1").find(".show-c").css("display", "block");
    });
    $(".close").click(function () {
        $(this).parents(".nava1").find(".show-bg").css('display', 'none');
        $(this).parents(".nava1").find(".show-c").css("display", "none");
    });
});

/********提问弹出*************/
$(document).ready(function () {
    $(".show-c").css('display', 'none');
    $(".show-btn").click(function () {
        $(this).parents(".show-par").find(".show-bg").stop().fadeTo(500, 1);
        $(this).parents(".show-par").find(".show-c").css("display", "block");
    });
    $(".close").click(function () {
        $(this).parents(".show-par").find(".show-bg").css('display', 'none');
        $(this).parents(".show-par").find(".show-c").css("display", "none");
    });
});

/////悬浮顶部///////
$(document).ready(function () {
    var headHeight = 0;

    var nav = $("#main-tl");
    $(window).scroll(function () {

        if ($(this).scrollTop() > headHeight) {
            nav.addClass("navfix");
        }
        else {
            nav.removeClass("navfix");
        }
    })
})

// 点击选择框
function AllSel(object)//全选和反选
{
    if (object.checked == true) {

        $(object).parents(".tb-chk").find("input[type=checkbox]").attr("checked", true);
        //$("."+classname).attr("checked",true);
    }
    else {

        $(object).parents(".tb-chk").find("input[type=checkbox]").attr("checked", false);
        //$("."+classname).attr("checked",false);
    }
}


//展开功能
//$(document).ready(function () {
//    $(".gr_edit").click(function () {
//        $(this).parents(".xx-js").find(".td-drop").css("height", "auto");

//        if ($(this).html() == "展开") {
//            $(this).html("收起");
//            $(this).parents(".xx-js").css("background", "#fdfff7");

//        }
//        else {
//            $(this).html("展开");
//            $(this).parents(".xx-js").find(".td-drop").css("height", "30px");
//            $(this).parents(".xx-js").css("background", "#fff");

//        }

//    });
//});

function StudentInfoGetEdit(obj) {
    $(obj).parents(".xx-js").find(".td-drop").css("height", "auto");

    if ($(obj).html() === "展开") {
        $(obj).html("收起");
        $(obj).parents(".xx-js").css("background", "#fdfff7");

    }
    else {
        $(obj).html("展开");
        $(obj).parents(".xx-js").find(".td-drop").css("height", "30px");
        $(obj).parents(".xx-js").css("background", "#fff");

    }
}