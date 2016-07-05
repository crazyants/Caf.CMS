function initLayout() {
    
    $("#mainIframe").height($(window).height() - 70);
}

$(document).ready(function () {
    applyCommonPlugins($("body"));

    // 保存继续编辑
    $(".btn-group button[value=save-continue],.actions button[value=save-continue]").click(function () {
        var btn = $(this);
        btn.closest("form").append('<input type="hidden" name="save-continue" value="true" />');
    });

    $(document).ajaxStart(function () {
        Pace.start();
    }).ajaxStop(function () {
        window.setTimeout(function () {
            Pace.stop();
        }, 300);
    });
    //隐藏/显示 提示信息
    $(".hint").tooltip({
        placement:'left',
        viewport: {
            selector: 'body',
            padding: 2
        }
    });

    $('textarea.form-control').maxlength({
        limitReachedClass: "label label-danger",
        alwaysShow: true
    });

    //提示信息配置
    toastr.options = {
        "closeButton": true,
        "debug": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});


