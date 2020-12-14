/**********首页轮播*********/
 $(function(){
	$('.flexslider').flexslider({
		animation: "fade",
		slideshowSpeed: 4000,
		directionNav: true,
		pauseOnAction: false
		
	});
});

//公告滚动
function AutoScroll(obj){
	$(obj).find("ul:first").animate({
		marginTop:"-37px"
	},500,function(){
		$(this).css({marginTop:"0px"}).find("li:first").appendTo(this);
	});
}
$(document).ready(function(){
	setInterval('AutoScroll("#scrolldiv")',4000);
});
