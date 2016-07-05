//$.fn.dataTable.TableTools.defaults.aButtons = ["copy", "csv", "xls"];
var Datatable = function () {
    var tableOptions; // main options
    var dataTable; // datatable object
    var table; // actual table jquery object
    var tableContainer; // actual table container object
    var tableWrapper; // actual table wrapper jquery object
    var tableInitialized = false;
    var isSearchSubmit = false;
    var ajaxParams = {}; // set filter mode
    var filterFormId;
    var refreshButton;
    var the;

    var countSelectedRecords = function () {
        var selected = $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        var text = tableOptions.dataTable.oLanguage.metronicGroupActions;
        if (selected > 0) {
            $('.table-group-actions > span', tableWrapper).text(text.replace("_TOTAL_", selected));
        } else {
            $('.table-group-actions > span', tableWrapper).text("");
        }
    };
    //通过获取#*#字段，然后从aData获取字段值，替换#*#返回URL与字段
    var GetLinkUrl = function (html, aData) {
        var reg = /#\w+#/i;//构造一个含有目标参数的正则表达式对象
        var repReg = new RegExp("(#)", "g");
        var fieldKey = html.match(reg)[0];
        fieldKey = fieldKey.replace(/(#)/g, '').replace(/(#)/g, '');
        var repFieldReg = new RegExp("(#" + fieldKey + "#)", "g");
        var url = html.replace(repFieldReg, aData[fieldKey]);
        var parm = { url: url, key: fieldKey };
        return parm;
    }
    //通过获取#*#字段，然后从aData获取字段值
    var GetFieldValue = function (html, aData) {
        var reg = /(#\w+#)|(#\w+\-\w+#)/g;//构造一个含有目标参数的正则表达式对象
        var repReg = new RegExp("(#)", "g");
        var fieldMatchKeys = html.match(reg);
        var fieldKeys = [];
        var values = [];
        if (fieldMatchKeys != null) {
            for (var i = 0; i < fieldMatchKeys.length; i++) {
                var fieldKeyOriginal = fieldMatchKeys[i].replace(/(#)/g, '').replace(/(#)/g, '');
                var fieldKeySplit = fieldKeyOriginal.split('-');
                var fieldKey = fieldKeySplit[0];
                var fieldKey2 = "";
                if (fieldKeySplit.length > 1)
                    fieldKey2 = fieldKeySplit[1];
                var keyData = aData[fieldKey];
                if (keyData != null && keyData != "" && typeof keyData == "string" && keyData.indexOf('/Date(') > -1) {
                    //&& !(new Date(keyData) instanceof Date)
                    var date = eval('new ' + keyData.substr(1, keyData.length - 2));
                    if (fieldKey2 == "Date") {
                        keyData = TimeObjectUtil.formatterDate(date);
                    }
                    else if (fieldKey2 == "DateTime") {
                        keyData = TimeObjectUtil.formatterDateTime(date);
                    }
                    else if (fieldKey2 == "Time") {
                        keyData = TimeObjectUtil.formatterDateTime2(new Date(keyData));
                    }
                }
                var vls = { key: fieldKeyOriginal, value: keyData != null ? keyData : '' };
                if ($.inArray(fieldKeyOriginal, fieldKeys) == -1) {
                    values.push(vls);
                    fieldKeys.push(fieldKeyOriginal);
                }
            }
        }

        return values;
    }
    //Ajax请求
    var tableAjax = function (url, param) {
        $.ajax({
            cache: false,
            type: "POST",
            url: url,
            data: param,
            success: function (data) {
                if (data.Result) {
                    Datatable.reload();
                    ShowMetronic(data.Message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowMetronic(thrownError);
            }
        });
    }

    return {

        //main function to initiate the module
        init: function (options) {
            if (!$().dataTable) {
                return;
            }
            the = this;

            options = $.extend(true, {
                keyId: null,
                tableId: "", // actual table  
                isTree: false,
                treeId: "",
                refreshBtnId: null,
                formId: null,
                // operations: { "edit": false, "edittext": "编辑", "del": false, "deltext": "删除" },
                dataTable: {
                    //"scrollY":        "300px",
                    //"scrollX":        true,
                    //"scrollCollapse": true,
                    //"paging":         false,
                    //"colReorder": {
                    //    "fixedColumnsLeft": 2
                    //},
                    //"sDom":'', // datatable layout
                    //"sDom": "<'row-fluid'<'span6 myBtnBox'><'span6'f>r>t<'bottom'<'bottom'i><'bottom 'p>>",
                    "bingTooltip": false,
                    "pageLength": 10, // default records per page
                    "sAjaxSource": '',
                    "lengthMenu": [[10, 25, 50], ["10", "25", "50"]],
                    //vm.Language = { 'sUrl': '/Content/assets/global/scripts/jquery.dataTables.lang.de-En.txt' };
                    "oLanguage": {
                        "metronicGroupActions": "已选择 _TOTAL_ 条记录:  ",
                        "metronicAjaxRequestGeneralError": "无法完成请求。请检查你的网络连接",
                        "sProcessing": "<img src='/Content/assets/global/img/ajax-loading.gif' />",
                        "sLengthMenu": "每页显示 _MENU_ 条记录",
                        "sZeroRecords": "没有找到匹配的记录.",
                        "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
                        "sInfoEmpty": "没有找到记录",
                        "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
                        "sInfoPostFix": "",
                        "sSearch": "查询:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "首页",
                            "sPrevious": "前一页",
                            "sNext": "后一页",
                            "sLast": "尾页"
                        }
                    },
                    "fnServerParams": function (aoData) {
                        //如果不是筛选按钮提交即重新获取一次条件参数
                        if (!isSearchSubmit)
                            the.getAjaxParam();
                        $.each(ajaxParams, function (key, value) {
                            aoData.push({ name: key, value: value });
                        });
                    },
                    "fnCreatedRow": function (nRow, aData, iDataIndex) {
                        //替换主键ID属性
                        if (options.keyId)
                            $(nRow).attr('data-id', aData[options.keyId]);
                        //树结构
                        if (options.isTree) {
                            $(nRow).attr('data-tt-id', aData[options.keyId]);
                            $(nRow).attr('data-tt-parent-id', aData[options.treeId]);
                        }
                        //替换关键字
                        var rowhtml = $(nRow).html();
                        var values = GetFieldValue(rowhtml, aData);
                        $.each(values, function (i, n) {
                            var repFieldReg = new RegExp("(#" + n.key + "#)", "g");
                            rowhtml = rowhtml.replace(repFieldReg, n.value);
                        });
                        $(nRow).html(rowhtml);
                    },
                    "aoColumnDefs": [{ // define columns sorting options(by default all columns are sortable extept the first checkbox column)
                        'orderable': false,
                        'targets': [0]
                    }],
                    "oTableTools": { 'sSwfPath': '//cdn.datatables.net/tabletools/2.2.1/swf/copy_csv_xls_pdf.swf' },
                    "pagingType": "bootstrap_full_number", // pagination type(bootstrap, bootstrap_full_number or bootstrap_extended)
                    "colVis": { 'buttonText': '隐藏/显示' },
                    "bAutoWidth": false, // disable fixed width and enable fluid table
                    "bProcessing": false, // enable/disable display message box on record load
                    "bServerSide": true, // enable/disable server side ajax loading
                    "bSortMulti": false, // Enable or display DataTables' ability to sort multiple columns at the 
                    "drawCallback": function (oSettings) { // run some code on table redraw
                        if (tableInitialized === false) { // check if table has been initialized
                            tableInitialized = true; // set table initialized
                            table.show(); // display table
                        }
                        Metronic.initUniform($('input[type="checkbox"]', table)); // reinitialize uniform checkboxes on each table reload
                        countSelectedRecords(); // reset selected records indicator

                        // callback for ajax data load
                        if (tableOptions.dataTable.onDataLoad) {
                            tableOptions.dataTable.onDataLoad.call(undefined, the);
                        }
                        //if (tableOptions.dataTable.bingTooltip) {
                        //    the.bingTooltip();
                        //}
                    },
                    "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                        /* Append the grade to the default row class name */
                        var row = $(nRow);
                        $('td', row).each(function () {
                            if ($(this).text() == "true") $(this).html('<span class=""><i class="fa fa-check"></i></span>');
                            else if ($(this).text() == "false") $(this).html('<span class=""><i class="fa fa-times"></i></span>');
                        });
                        $('td input[type="checkbox"]', row).each(function () {
                            var checkedvalue = $(this).attr("checkedvalue");
                            if (checkedvalue != null) {
                                if (checkedvalue.toLocaleLowerCase() == "true") {
                                    $(this).attr("checked", true);
                                } else {
                                    $(this).attr("checked", false);
                                }
                            }

                        });
                        if (the.bindRowCallback) {
                            the.bindRowCallback(nRow, aData, iDisplayIndex);
                        }
                        return nRow;
                    },
                    "fnInitComplete": function (oSettings, json) {
                        $('<a href="#" id="refresh"><i class="fa-refresh"></i></a>').appendTo($('.refBtnBox'));
                        $("#refresh").on("click", function () {
                            the.reload();
                        });
                        the.bindInitComplete();
                    },
                    "fnServerData": function (sSource, aoData, fnCallback) {
                        var ajaxOptions = {
                            'dataType': 'json', 'type': 'POST', 'url': sSource, 'data': aoData, 'success': function (data) {
                                the.bindAjaxCallback(data.aaData);
                                fnCallback(data);
                            }
                        };
                        ajaxOptions['error'] = function (jqXHR, textStatus, errorThrown) {
                            //Metronic.alert({
                            //    type: 'danger',
                            //    icon: 'warning',
                            //    message: tableOptions.dataTable.oLanguage.metronicAjaxRequestGeneralError,
                            //    container: tableWrapper,
                            //    place: 'prepend'
                            //}); 
                            window.alert('error loading data: ' + textStatus + ' - ' + errorThrown);
                            console.log(arguments);

                        };
                        $.ajax(ajaxOptions);
                    }
                    //"ajax": { // define ajax settings
                    //    "url": "", // ajax URL
                    //    "type": "POST", // request type
                    //    "timeout": 20000,
                    //    "data": function (data) { // add request parameters before submit
                    //        $.each(ajaxParams, function (key, value) {
                    //            data[key] = value;
                    //        });
                    //        //Metronic.blockUI({
                    //        //    message: tableOptions.loadingMessage,
                    //        //    target: tableContainer,
                    //        //    overlayColor: 'none',
                    //        //    cenrerY: true,
                    //        //    boxed: true
                    //        //});
                    //    },
                    //    "error": function () { // handle general connection errors
                    //        if (tableOptions.onError) {
                    //            tableOptions.onError.call(undefined, the);
                    //        }

                    //        Metronic.alert({
                    //            type: 'danger',
                    //            icon: 'warning',
                    //            message: tableOptions.dataTable.language.metronicAjaxRequestGeneralError,
                    //            container: tableWrapper,
                    //            place: 'prepend'
                    //        });

                    //        Metronic.unblockUI(tableContainer);
                    //    }
                    //}
                }
            }, options);

            tableOptions = options;
            filterFormId = options.formId;
            refreshButton = options.refreshBtnId;
            table = $("#" + options.tableId);
            tableContainer = table.parents(".table-container");

            // apply the special class that used to restyle the default datatable
            var tmp = $.fn.dataTableExt.oStdClasses;

            $.fn.dataTableExt.oStdClasses.sWrapper = $.fn.dataTableExt.oStdClasses.sWrapper + " dataTables_extended_wrapper";
            $.fn.dataTableExt.oStdClasses.sFilterInput = "form-control input-small input-sm input-inline";
            $.fn.dataTableExt.oStdClasses.sLengthSelect = "form-control input-xsmall input-sm input-inline";

            // initialize a datatable
            dataTable = table.DataTable(options.dataTable);

            // revert back to default
            $.fn.dataTableExt.oStdClasses.sWrapper = tmp.sWrapper;
            $.fn.dataTableExt.oStdClasses.sFilterInput = tmp.sFilterInput;
            $.fn.dataTableExt.oStdClasses.sLengthSelect = tmp.sLengthSelect;
            // get table wrapper
            tableWrapper = table.parents('.dataTables_wrapper');
            // build table group actions panel
            if ($('.table-actions-wrapper', tableContainer).size() === 1) {
                $('.table-group-actions', tableWrapper).html($('.table-actions-wrapper', tableContainer).html()); // place the panel inside the wrapper
                $('.table-actions-wrapper', tableContainer).remove(); // remove the template container
            }
            // 处理组复选框选中或取消选择
            $('.group-checkable', table).change(function () {
                var set = $('tbody > tr > td:nth-child(1) input[type="checkbox"]', table);
                var checked = $(this).is(":checked");
                $(set).each(function () {
                    $(this).attr("checked", checked);
                    if (checked) {
                       // $(this).val('true');
                        $(this).attr("checked", true);
                        $(this).parents('tr').addClass("active");
                    } else {
                       // $(this).val('false');
                        $(this).attr("checked", false);
                        $(this).parents('tr').removeClass("active");
                    }

                });

                $.uniform.update(set);
                countSelectedRecords();

            });

            // 复选框点击更改事件
            table.on('change', 'tbody > tr > td:nth-child(1) input[type="checkbox"]', function () {
                // $(this).parents('tr').toggleClass("active");
                var checked = $(this).is(":checked");
                if (checked) {
                   // $(this).val('true');
                    $(this).parents('tr').addClass("active");
                } else {
                   // $(this).val('false');
                    $(this).parents('tr').removeClass("active");
                }
                countSelectedRecords();
            });
            ////复选框
            //table.on('change', 'tbody tr .checkboxes', function () {

            //    $(this).parents('tr').toggleClass("active");
            //});
            //筛选提交按钮
            // handle filter submit button click
            table.on('click', '.filter-submit', function (e) {
                e.preventDefault();
                the.submitFilter();
            });
            //第一列排序
            //table.on('order.dt search.dt', function () {
            //    dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            //        cell.innerHTML = i + 1;
            //    });
            //}).draw();
            //取消筛选
            table.on('click', '.filter-cancel', function (e) {
                e.preventDefault();
                the.resetFilter();
            });
            //点击删除
            table.on('click', '.delete', function (e) {
                e.preventDefault();
                var delobj = this;
                var url = $(delobj).attr("href");
                bootbox.confirm("你确定删除?", function (result) {
                    if (result) {
                        //var nRow = $(delobj).parents('tr')[0];
                        //// dataTable.fnDeleteRow(nRow);
                        //var aData = dataTable.data()[nRow.rowIndex - 1];
                        //var url = $(delobj).attr("href");
                        //// var parm = GetLinkUrl(html, aData);
                        tableAjax(url)
                    }
                });
                ;

            });
            //点击编辑
            table.on('click', '.edit', function (e) {
                e.preventDefault();

                /* Get the row as a parent of the link that was clicked on */
                var nRow = $(this).parents('tr')[0];
                var aData = dataTable.data()[nRow.rowIndex - 1];
                var html = $(this).attr("href");
                // var parm = GetLinkUrl(html, aData);
                var value = GetFieldValue(html, aData);

            });
            //table.on('column-visibility.dt', function (e, settings, column, state) {
            //    console.log(
            //        'Column ' + column + ' has changed to ' + (state ? 'visible' : 'hidden')
            //    );
            //    // var nodes = dataTable.table(0).GetHiddenNodes(table);
            //    // var columnIndex = dataTable.GetColumnIndex("排序");
            //});
            //table.on('page.dt,search.dt', function () {
            //    e.preventDefault();
            //    the.submitFilter();
            //});
            //table.on('length.dt', function (e, settings, len) {
            //    e.preventDefault();
            //    the.submitFilter();
            //});
            // 初始化上升按钮
            table.on('click', 'tbody a.up', function (e) {
                e.preventDefault();

                var index = dataTable.row($(this).parents('tr')).index();
                if ((index - 1) >= 0) {
                    var data = dataTable.data();
                    dataTable.clear();
                    data.splice((index - 1), 0, data.splice(index, 1)[0]);
                    dataTable.rows.add(data).draw();
                } else {
                    alert("已经到顶了");
                }

            });

            // 初始化下降按钮
            table.on('click', 'tbody a.down', function (e) {
                e.preventDefault();

                var index = dataTable.row($(this).parents('tr')).index();
                var max = dataTable.rows().data().length;
                if ((index + 1) < max) {
                    var data = dataTable.data();
                    dataTable.clear();
                    data.splice((index + 1), 0, data.splice(index, 1)[0]);
                    dataTable.rows.add(data).draw();
                } else {
                    alert("已经到底了");
                }
            });
            //刷新
            if (refreshButton != null) {
                $(refreshButton).on('click', function (e) {
                    e.preventDefault();
                    the.submitFilter();

                });
            }
            return the;
        },

        submitFilter: function () {
            isSearchSubmit = true;
            the.getAjaxParam();
            dataTable.ajax.reload();
            the.clearAjaxParams();
            isSearchSubmit = false;
        },

        resetFilter: function () {
            $('textarea.form-filter, select.form-filter, input.form-filter', table).each(function () {
                $(this).val("");
            });
            $('input.form-filter[type="checkbox"]', table).each(function () {
                $(this).attr("checked", false);
            });
            the.clearAjaxParams();
            the.addAjaxParam("action", tableOptions.filterCancelAction);
            dataTable.ajax.reload();
        },

        reload: function () {
            dataTable.ajax.reload();
        },

        getSelectedRowsCount: function () {
            return $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        },

        getSelectedRows: function () {
            var rows = [];
            $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                // rows.push($(this).val());
            });
            $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).parents("tr").each(function () {
                var sel = $(this);
                rows.push(sel.data("id"));
            });
            return rows;
        },

        getAjaxParam: function () {
            the.setAjaxParam("action", tableOptions.filterApplyAction);

            // get all typeable inputs
            $('textarea.form-filter, select.form-filter, input.form-filter:not([type="radio"],[type="checkbox"])', table).each(function () {
                the.setAjaxParam($(this).attr("name"), $(this).val());
            });

            // get all checkboxes
            $('input.form-filter[type="checkbox"]:checked', table).each(function () {
                the.addAjaxParam($(this).attr("name"), $(this).val());
            });

            // get all radio buttons
            $('input.form-filter[type="radio"]:checked', table).each(function () {
                the.setAjaxParam($(this).attr("name"), $(this).val());
            });
            if (filterFormId != null) {
                var form = $(filterFormId);
                // get all typeable inputs
                $('textarea.form-filter, select.form-filter, input.form-filter:not([type="radio"],[type="checkbox"])', form).each(function () {
                    if ($(this).val() != "") the.setAjaxParam($(this).attr("name"), $(this).val());
                });

                // get all checkboxes
                $('input.form-filter[type="checkbox"]:checked', form).each(function () {
                    if ($(this).val() != "") the.setAjaxParam($(this).attr("name"), $(this).val());
                });

                // get all radio buttons
                $('input.form-filter[type="radio"]:checked', form).each(function () {
                    if ($(this).val() != "") the.setAjaxParam($(this).attr("name"), $(this).val());
                });
            }
            var data = the.bindDataParamBinding();
            ajaxParams = $.extend({}, ajaxParams, data);
        },

        setAjaxParam: function (name, value) {
            ajaxParams[name] = value;
        },

        addAjaxParam: function (name, value) {
            if (!ajaxParams[name]) {
                ajaxParams[name] = [];
            }

            skip = false;
            for (var i = 0; i < (ajaxParams[name]).length; i++) { // check for duplicates
                if (ajaxParams[name][i] === value) {
                    skip = true;
                }
            }

            if (skip === false) {
                ajaxParams[name].push(value);
            }
        },

        clearAjaxParams: function (name, value) {
            ajaxParams = {};
        },

        getDataTable: function () {
            return dataTable;
        },

        getTableWrapper: function () {
            return tableWrapper;
        },

        gettableContainer: function () {
            return tableContainer;
        },

        getTable: function () {
            return table;
        },

        bingTooltip: function () {
            //点击行提示
            // handle filter cancel button click
            $('tbody tr', table).each(function () {
                var sTitle = "提示";
                var nTds = $('td', this);
                //var sBrowser = $(nTds[1]).text();
                //var sGrade = $(nTds[4]).text();

                //if (sGrade == "A")
                //    sTitle = sBrowser + ' will provide a first class (A) level of CSS support.';
                //else if (sGrade == "C")
                //    sTitle = sBrowser + ' will provide a core (C) level of CSS support.';
                //else if (sGrade == "X")
                //    sTitle = sBrowser + ' does not provide CSS support or has a broken implementation. Block CSS.';
                //else
                //    sTitle = sBrowser + ' will provide an undefined level of CSS support.';
                this.setAttribute('title', sTitle);
            });

            /* Apply the tooltips */
            $('tbody tr[title]', table).tooltip({
                placement: 'auto',
                viewport: {
                    selector: 'body',
                    padding: 2
                }
            });
        },

        bindInitComplete: function () {

        },

        bindDataParamBinding: function () {
            return [];
        },

        bindRowCallback: function (nRow, aData, iDisplayIndex) {

        },
        //服务器AJAX回调函数
        bindAjaxCallback: function (aData) {

        },
    };

}();

//column.visible
// Get the column API object
//var column = table.column($(this).attr('data-column'));

// Toggle the visibility
//column.visible(!column.visible());