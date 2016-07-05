;
(function ($) {
    $(function () {
        $(".caf-colorbox").colorpicker();
        $(".caf-colorbox .colorval").on("keyup change", function (e) {
        	var el = $(this);
        	var picker = el.parent().data("colorpicker");
        	var val = el.val();
        	picker.setValue(val || el.attr('placeholder'));
        })
        //$('.colorpicker-default').colorpicker({
        //    format: 'hex'
        //});
    });
}(jQuery));
