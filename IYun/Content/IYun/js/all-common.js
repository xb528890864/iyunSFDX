

//下拉列表框
$(document).ready(function(){

	jQuery.navlevel2 = function(level1,dytime) {	
	  $(level1).mouseenter(function(){
		  varthis = $(this);
		  delytime=setTimeout(function(){
			varthis.find('.lev2').slideDown();
		},dytime);
	  });
	  $(level1).mouseleave(function(){
		 clearTimeout(delytime);
		 $(this).find('.lev2').slideUp();
	  }); 
	};
  $.navlevel2(".navlev",100);
});

/////////////////////*
$(document).ready(function(){
   $(".ds-left li").mouseover(function(){
	    $(this).addClass("hover")
	
   $(".ds-left li").mouseout(function(){
    	$(this).removeClass("hover")  
	}); 
 	}); 		  
});

/***********LEFT下拉**************/
$(document).ready(function(){
	$('.drop-li span').click(function(){
        $(".drop-li2").not($(this).parents(".drop-li").find(".drop-li2")).css("display","none");
        $('.drop-li2 font').removeClass("le_on1")
		$(this).parents(".drop-li").find(".drop-li2").slideToggle();
		$(this).toggleClass("le-on1");
	});
});
/***********LEFT下拉**************/
$(document).ready(function(){
	$('.drop-li2 font').click(function(){
        $(".drop-li3").not($(this).parents(".drop-li2").find(".drop-li3")).css("display","none");
		$('.drop-li2 font').removeClass("le_on2")
		$(this).parents(".drop-li2").find(".drop-li3").slideToggle();
		$(this).toggleClass("le_on2");
	});
});

//滚动条

$(document).ready(function(){
	  
	$("#wenzhang").niceScroll({  
	cursorcolor:"",  
	cursoropacitymax:2,  
	touchbehavior:false,  
	cursorwidth:"6px",  
	cursorborder:"0",  
	cursorborderradius:"0px"  
	});

});	

$(document).ready(function(){
	  
	$("#wenzhang2").niceScroll({  
	cursorcolor:"",  
	cursoropacitymax:2,  
	touchbehavior:false,  
	cursorwidth:"6px",  
	cursorborder:"0",  
	cursorborderradius:"0px"  
	});

});				
    //全局可使用的轮播图
function mwhtab(onclass,offclass,tabclass,obj) 
		/*
			参数说明：
			onclass 字符串类型 代表选中时选项卡对应的Class类
			offclass 字符串类型 代表选项卡未选中时对应的Class类
			tabclass 字符串类型 代表选项卡内容对应的Class类
			obj 对象类型 代表用户点击对象的对象类型 前端使用this调用即可
			
			其他说明：
			请把选项卡对应的内容DIV放置在标签以内例如：
			<div class="开启的选项卡类">
				<div class="选项卡对应的内容类">
					这里放置内容
				</div>
			</div>
		*/
		{
			var $target = $(obj);//将JS DOM对象转换为JQ对象
			
			if (obj.className == offclass)//如果点击的是已经选中的选项卡那么不需要任何操作，所以取反
			{
				//大体思路：先把所有的选项卡作为未选中然后根据对象来确定选中的一个
				$("."+onclass).attr("class",offclass);//把所有选项卡设置为未选中Class
				$target.attr("class",onclass);//点击的选项卡设置为选中Class
				
				//大体思路:先把所有的选项卡内容消失然后根据对象来确定展示的一个
				$("."+tabclass).attr("style","display:none");//把所有选项卡对应的内容设置为隐藏
				$target.find("div").attr("style","display:block");//把点击的选项卡对应内容设置为块状

			}	
		};


//时间
	   var week; 
        if(new Date().getDay()==0)          week="星期天"
        if(new Date().getDay()==1)          week="星期一"
        if(new Date().getDay()==2)          week="星期二" 
        if(new Date().getDay()==3)          week="星期三"
        if(new Date().getDay()==4)          week="星期四"
        if(new Date().getDay()==5)          week="星期五"
        if(new Date().getDay()==6)          week="星期六"
    document.getElementById("mdate").innerHTML=(((new Date().getFullYear())+"年"+(new Date().getMonth()+1))+"月"+new Date().getDate()+"日 ");
	document.getElementById("mdate2").innerHTML=(week);
////////////滚动
(function(){
	$("#marquee2").kxbdMarquee({direction:"left"});
})();