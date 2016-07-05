(function ($, window, document, undefined) {

    window.setLocation = function (url) {
        window.location.href = url;
    }

    window.OpenWindow = function (query, w, h, scroll) {
        var l = (screen.width - w) / 2;
        var t = (screen.height - h) / 2;

        // TODO: (MC) temp only. Global viewport is larger now.
        // But add this value to the callers later.
        h += 100;

        winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
        if (scroll) winprops += ',scrollbars=1';
        var f = window.open(query, "_blank", winprops);
    }
  
    // on document ready
    $(function () {



    });

})(jQuery, this, document);