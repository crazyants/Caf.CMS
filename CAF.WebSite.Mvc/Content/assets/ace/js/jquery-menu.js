!function ($) {

    // Le menuleft-menu sign
    /* for older jquery version
    $('#menuleft ul.nav li.parent > a > span.sign').click(function () {
        $(this).find('i:first').toggleClass("icon-minus");
    }); */

    $(document).on("click", "#menuleft ul.nav li.parent > a > span.sign", function () {
        $(this).find('i:first').toggleClass("fa-minus");
    });
    $(document).on("click", "#menuleft ul > li > a > span.lbl", function () {
        $("#menuleft ul > li").removeClass("active");
        $(this).parents('li.parent').toggleClass("active");
    });
    // Open Le current menu
    $("#menuleft ul.nav li.parent.active > a > span.sign").find('i:first').addClass("icon-minus");
    $("#menuleft ul.nav li.current").parents('ul.children').addClass("in");

}(window.jQuery);