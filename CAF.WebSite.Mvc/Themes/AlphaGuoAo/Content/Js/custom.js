function goTop(){
	$(window).scroll(function(e) {
		if($(window).scrollTop()>100)
			$(".gotop").fadeIn(350).css("display","block");
		else
			$(".gotop").fadeOut(350).css("display","none");
	});
		
	$(".gotop").click(function(e) {
		$('body,html').animate({scrollTop:0},500);
		return false;
	});		
};


$(document).ready(function() { 
	//底部固定区域占位
	if( $(".fixed-bottom").is(":visible") ){
		$("#wrapper").css( "paddingBottom",$(".fixed-bottom").height() );
	}else{
	}
	
	
	//返回顶部
	goTop();
	
	//内容加载后的运动效果
	dataAnimate();	
	
	
	// 多语言
	$('.language ul.sf-menu').superfish({ 
		delay:       500,
		animation:   {opacity:'fast',height:'show'},
		speed:       'fast',
		autoArrows:  true,
		dropShadows: false
	});
	
	// 导航菜单
	$('.main-nav ul.sf-menu').superfish({ 
		delay:       500,
		animation:   {opacity:'fast',height:'show'},
		speed:       'fast',
		autoArrows:  true,
		dropShadows: false 
	});
	$('.main-nav ul.sf-menu > li').last().addClass('last').end().hover(function(){ $(this).addClass('nav-hover'); },function(){ $(this).removeClass('nav-hover'); });
	
	
	//菜单动态切换页面效果
	//$(".main-nav li a, .logo a").click(function(){		
	//	if( $(this).attr("target") != "_blank"){
	//		if( $(this).attr("href") != "javascript:;" && $(this).attr("href") != "#" )
	//		return openwork($(this).attr("href"));
		
	//	}
    //});
	//function openwork(url){
	//	$("#wrapper").css({'-webkit-animation':"bounceIn 0.5s .25s ease both",'-moz-animation':'bounceIn 1s .25s ease both','animation':'bounceIn 0.5s .25s ease both'});
	//	$("body").append("<div class='page-cover'></div>").css("position","relative");		
	//	$(".page-cover").delay(600).animate({"height":$(document).height()},800,null,function(){
    //    	location.href = url;
    //    });
    //    return false
    //};
			
		
	
	//tab		
	$(".tabs-nav").tabs(" > .tabs-panes > div");
	
	//scrollable in tab
	$(".tabs-nav > li > a").click(function(){
		var _tabIndex = $(this).parents(".tabs-nav").find("li").index($(this).parent());
		
		if( $(this).parents(".tabs-default").find(".tab-box").eq(_tabIndex).find(".scrollable-default").length > 0 ){
			$(".scrollable-default").carouFredSel({
				width   	: '100%',
				infinite 	: false,
				circular 	: false,
				auto 	  	: { pauseOnHover: true, timeoutDuration:3500 },
				swipe    	: { onTouch:true, onMouse:true },
				prev 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-prev"); }},
				next 		: { button:function() {	return $(this).parent().next('.carousel-direction').find(".carousel-next"); }},
				pagination 	: { container:function() { return $(this).parent().next('.carousel-pagi-btn'); }}
			});
			$(".scrollable-default").parents(".scrollable").css("overflow","visible");
		}		
		
		if( $(this).parents(".tabs-default").find(".tab-box").eq(_tabIndex).find(".full-scrollable-default").length > 0 ){
			$(".full-scrollable-default").carouFredSel({
				infinite 	: false,
				circular 	: false,
				auto 		: false,
				swipe    	: { onTouch:true, onMouse:true },
				responsive	: true,
				items		: {
					visible		: {
						min			: 2,
						max			: 8
					}
				},
				prev 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-prev"); }},
				next 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-next"); }},
				pagination 	: { container:function() { return $(this).parent().next('.carousel-pagi-btn'); }}							
			});
			
			//重置高度
			/*$(".full-scrollable-default").parents('.caroufredsel_wrapper').css({
				'height': ($(".full-scrollable-default").find('li').outerHeight()) + 'px'
			});*/
		}
		
		
	});	
	
	// accordion
	$(".accordion").tabs(".accordion .accordion-pane", {tabs: '.accordion-handle', effect: 'slide',initialIndex: 0});	
	$.tools.tabs.addEffect("slide", function(tabIndex, done) {
		this.getPanes().slideUp("fast").eq(tabIndex).slideDown("fast");
		done.call();
	});
	
	
	
	//图库切换
	$('.pgwSlideshow-gallery').pgwSlideshow({
		mainClassName : 'pgwSlideshow-gallery pgwSlideshow'
	});
	$('.pgwSlideshow-gallery-simple').pgwSlideshow({
		mainClassName : 'pgwSlideshow-gallery-simple pgwSlideshow',
		displayList : false		
	});	
	$('.pgwSlideshow-gallery-zoom').pgwSlideshow({
		mainClassName : 'pgwSlideshow-gallery-zoom pgwSlideshow',
		displayControls : false		
	});
	
	
	//table	
	//$(".qhd-content table tbody>tr:odd").addClass("odd-row");
	//$(".qhd-content table tbody>tr:even").addClass("even-row");
	$(".qhd-content table tbody>tr").hover(function(){ 
		$(this).addClass("trhover");
	},function(){
		$(this).removeClass("trhover");
	});
	
	//增加响应式表格外层div
	$(".qhd-content table").each(function(){
		if( !$(this).parent().hasClass("table-responsive") ){
			$(this).wrap("<div class='table-responsive'></div>");
		}
	});
	
	
	//左右悬浮
	$(".fixed-left").fixed({halfTop : true});
	$(".fixed-right").fixed({halfTop : true});	
	//
	$(".service-close-btn").click(function(){
		var serviceMax =  $(this).parents(".service-max"),
			serviceMin =  serviceMax.next(".service-min");		
		serviceMin.show();
		serviceMax.hide();
	});		
	$(".service-open-btn").click(function(){
		var serviceMax =  $(this).prev(".service-max"),
			fixedELement = $(this).parents(".fixed");		
		$(this).hide();
		serviceMax.show();
	});
	
	
	//图标-名称-单列链接
	$(".link-fixed-side > ul > li").each(function(){
		$(this).hover(function(){
			$(this).addClass("active").find(".link-summary").show();	
		},function(){
			$(this).removeClass("active").find(".link-summary").hide();
		});
		
		if( $(this).find('a').attr('href').indexOf("#popup") != -1 ){
			$(this).find('a').addClass('popup-show-btn');
		}
		
	});
	//弹窗视频
	$('.fancybox-video-play').fancybox({
		'autoScale'   		: false,
		'transitionIn'		: 'elastic',
		'transitionOut'		: 'elastic',
		'hideOnOverlayClick': false,
		'centerOnScroll'	: true,
		'overlayColor'		: '#000',
		'padding'			: '5'
	});

	
	//弹出窗口区域
	$("a").each(function(){		
		if( typeof($(this).attr('href'))!="undefined" && $(this).attr('href').indexOf("#popup") != -1 ){
			$(this).addClass('popup-show-btn');
		}
	});
	$('.popup-show-btn').click(function(){
		$('.popup').show();
		$('.popup-overlay').height($(document).height());
		$('.popup-content').css({marginLeft:-($('.popup-content').outerWidth()/2), marginTop:-($('.popup-content').outerHeight()/2)});
		$('.popup-close-btn').click(function(){
			$(this).parents('.popup').hide();	
		});
		
		return false;
	
	});	
	
	
	
	
	//设置弹窗视频的宽度
	var touchWindowWidth = $(window).width();
	if( touchWindowWidth < 1000 ){
		$('.article-detail-fancybox').css("width", touchWindowWidth-80);
	}
	
	
	//移动端 JS	
	var isMobile = device.mobile(),
    	isTable  = device.tablet(),
    	isiPhone = device.iphone(),
    	isiPad   = device.ipad();    
	if(isMobile || isTable || isiPhone || isiPad){
		//移除新窗口打开
		$('a').not('[data-mobile-target="_blank"]').removeAttr('target');
    }
	
	
});

$(window).load(function() {
	//scrollable-default
	$(".scrollable-default").carouFredSel({
		width   	: '100%',
		infinite 	: false,
		//circular 	: false,
		auto 	  	: { pauseOnHover: true, timeoutDuration:3500 },
		swipe    	: { onTouch:true, onMouse:true },
		prev 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-prev"); }},
		next 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-next"); }},
		pagination 	: { container:function() { return $(this).parent().next('.carousel-pagi-btn'); }}
	});
	$(".scrollable-default").parents(".scrollable").css("overflow","visible");
	
	//full-scrollable-default
	$(".full-scrollable-default").carouFredSel({
		infinite 	: false,
		circular 	: false,
		auto 		: false,
		swipe    	: { onTouch:true, onMouse:true },
		responsive	: true,
		items		: {
			visible		: {
				min			: 2,
				max			: 8
			}
		},
		prev 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-prev"); }},
		next 		: { button:function() { return $(this).parent().next('.carousel-direction').find(".carousel-next"); }},
		pagination 	: { container:function() { return $(this).parent().next('.carousel-pagi-btn'); }}						
	});	
	//重置高度
	$(".full-scrollable-default").parents('.caroufredsel_wrapper').css({
		'height': ($(".full-scrollable-default").find('li').outerHeight()) + 'px'
	});
	
	
});



$(window).bind("load resize", function() {
	
	var postList = $(".post-list");	
	postList.each(function(){
		var postImg = $(this).find(".post-img");
		var postTextBox = $(this).find(".post-text-box");	
		var postText = $(this).find(".post-text");
		var postTextSummary = $(this).find(".post-text-summary");
		var postMaxHeight = postImg.eq(0).height();
		var summaryMaxHeight = 0;			
			
		postImg.each(function(){
			postMaxHeight = $(this).height() > postMaxHeight ? $(this).height() : postMaxHeight;
		}).find("img").height( postMaxHeight );
		
		postTextBox.each(function(){
			$(this).height( postMaxHeight - parseInt($(this).css("paddingTop")) - parseInt($(this).css("paddingBottom")) );
		});
		
		postTextSummary.each(function(){
			summaryMaxHeight = postMaxHeight - $(this).prev("h2").height() - parseInt($(this).prev("h2").css("marginBottom")) - parseInt($(this).parent(".post-text").css("paddingBottom"))*2 - $(this).next(".post-text-detail").height() - 10;
			if( $(this).height() > summaryMaxHeight ){
				$(this).height( summaryMaxHeight );
			}
		});		
		
		//三列 - 特殊处理第二列
		if( $(this).hasClass("post-list-3col") ){
			$(this).find(".post-list-item-spec").find(".post-img").css("top", postMaxHeight);
			$(this).find(".post-list-item-spec").find(".post-text-box").css("top", -postMaxHeight);
		}	
		
	});
	
});
	



