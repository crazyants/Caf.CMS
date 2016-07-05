/* 
 * jQuery Fixed Plugins 1.5.1
 * Author:
 * Url:
 * Data
 *
 *  Update Log:
 * 
 *  Status       Date            Name      Version           BUG-Description
 *  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
 *  Created      2012-08-15    	 Ru	       1.0               None
 
 *  Modified     2012-09-02      Ru        1.4.1             修复了webkit内核浏览器右边浮动有一定距离的bug(负外边距),增加了悬浮靠边的定位、是否显示关闭按钮、是否垂直居中定位
 
 *  Modified     2013-01-02    	 Ru	       1.5.1             增加了垂直方向的位置;把核心函数(关闭、展开、定位、最小化)重构,修复了webkit内核浏览器右边浮动最小化时没有显示出来
 *  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

;(function($){
    $.fn.fixed = function(options){
        var defaults = {
			top    			: null,
			halfTop     	: false,
			durationTime 	: 500
        }
        var options = $.extend(defaults, options);		

        this.each(function(){			
           
			var thisBox = $(this),
				contentHeight = thisBox.height(),
				boxTop = null,
				defaultTop = thisBox.offset().top,
				halfTop = ($(window).height() - contentHeight)/2
				;
				
			if(options.top == null){
				boxTop = defaultTop;
			}else {
				boxTop = options.top;
			}
			if( options.halfTop ) {	boxTop = halfTop; }
						
			thisBox.css("top", boxTop);
						
						
			//核心scroll事件			
			$(window).bind("scroll",function(){
				var offsetTop = boxTop + $(window).scrollTop() + "px";
	            thisBox.animate({
					top: offsetTop
	            },{
	            	duration: options.durationTime,	
	                queue: false
	            });
			});
			
				
        });	//end this.each

    };
})(jQuery);