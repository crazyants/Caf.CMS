
(function ($, window, document, undefined) {
    $(function () {

        //var _order = 0;

        //$('.text-order').on('focus', function () {
        //    _order = parseInt($(this).val());
        //});

        //$('.text-name,.text-order').on('blur', function () {
        //    var id = $(this).parent('td').find('.hidden_id').val();

        //    var value = $.trim($(this).val());
        //    if (!value) {
        //        $(this).val($(this).attr('oriValue'));
        //    }
        //    else {
        //        if ($(this).hasClass('text-order')) {
        //            if (isNaN($(this).val()) || parseInt($(this).val()) <= 0) {
        //                //$.dialog({
        //                //    title: '更新分类信息',
        //                //    lock: true,
        //                //    width: '400px',
        //                //    padding: '20px',
        //                //    content: ['<div class="dialog-form">您输入的序号不合法,此项只能是大于零的整数.</div>'].join(''),
        //                //    button: [
        //                //    {
        //                //        name: '关闭',
        //                //    }]
        //                //});
        //                bootbox.alert("您输入的序号不合法,此项只能是大于零的整数.");
        //                $(this).val(_order);
        //            } else {
        //                if (parseInt($(this).val()) === _order) return;
        //                updateOrderOrName("/Admin/Article/ArticlePictureUpdate", { Id: id, DisplayOrder: parseInt(value) }, $(this), 1);
        //            }
        //        }
        //        else {
        //            updateOrderOrName("UpdateName", { Id: id, Name: value }, $(this));
        //        }
        //    }
        //});

    });

    window.updateOrderOrName = function (actionName, param, obj, type) {

        $.post(actionName, param, function (data) {
            if (data.Result) {
                obj.attr('oriValue', type == 1 ? param.DisplayOrder : param.Name);
                ShowMetronic("更新成功！");
            }
        });
    }
    //图片删除
    window.deletePicture = function (id, obj) {
        deleteColumn("/Admin/Article/ArticlePictureDelete", id, obj);
    }
    //删除请求
    window.deleteColumn = function (actionName, id, obj) {

        $.post(actionName, { Id: id }, function (data) {
            if (data.Result) {
                $(obj).parent().parent().remove();
                ShowMetronic(data.Message);
            }
        });
    }
    window.OnGridRowEditPictureUpdate = function (id, name, obj) {
        OnGridRowEditDisplayOrderUpdate(id, 'Picture', obj);
    }
    //更新图片排序
    window.OnGridRowEditDisplayOrderUpdate = function (id, name, obj) {
        var link = $(obj).parent().parent().find(".attr-opt");
        $(link).editable({
            url: '/Admin/Common/UpdateDisplayOrder/' + id,
            type: 'text',
            pk: 1,
            name: name,
            title: '请输入排序',
            validate: function (value) {
                if ($.trim(value) == '') return '必填项!';
            },
            success: function (response, newValue) {
                if (response.Result) {
                    {
                        getDataTable.reload();
                        ShowMetronic(response.Message);
                    }
                }
            }
        });
        setTimeout(function () {
            $(link).editable('show');
        }, 200);

    }


})(jQuery, this, document);