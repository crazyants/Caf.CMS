

(function ($, window, document, undefined) {

    window.displayAjaxLoading = function (display) {
        if ($.throbber === undefined)
            return;

        if (display) {
            $.throbber.show({ speed: 50, white: true });
        }
        else {
            $.throbber.hide();
        }
    }

    window.getPageWidth = function () {
        return parseFloat($("#content").css("width"));
    }

    var _commonPluginFactories = [
        // select2
        function (ctx) {
            if (!Modernizr.touch) {
                if ($.fn.select2 === undefined || $.fn.selectWrapper === undefined)
                    return;
                ctx.find("select:not(.noskin), input:hidden[data-select]").selectWrapper();
            }
        },
        // tooltips
        function (ctx) {
            if ($.fn.tooltip === undefined)
                return;
            if (!Modernizr.touch) {
                ctx.tooltip({ selector: "a[rel=tooltip], .tooltip-toggle" });
            }
        },
        // column equalizer
        function (ctx) {
            if ($.fn.equalizeColumns === undefined)
                return;
            ctx.find(".equalized-column").equalizeColumns({ /*deep: true,*/ responsive: true });
        }
    ];

    var _commonPluginFactories = [
      // select2
      function (ctx) {
          ctx.find(".form-group select:not(.noskin), .form-group input:hidden[data-select]").selectWrapper();
      },
       // tooltips
      function (ctx) {
          if ($.fn.tooltip === undefined)
              return;
          if (!Modernizr.touch) {
              ctx.tooltip({ selector: "a[rel=tooltip], .tooltip-toggle" });

          }
      },
      // column equalizer
      function (ctx) {
          if ($.fn.equalizeColumns === undefined)
              return;
          ctx.find(".equalized-column").equalizeColumns({ /*deep: true,*/ responsive: true });
      }
    ];
    /* 
    Helpful in AJAX scenarios, where jQuery plugins has to be applied 
    to newly created html.
    */
    window.applyCommonPlugins = function (/* jQuery */ context) {
        $.each(_commonPluginFactories, function (i, val) {
            val.call(this, $(context));
        });
    }

    //显示操作信息
    window.ShowMetronic = function (msg) {
        Metronic.alert({
            container: '', // alerts parent container(by default placed after the page breadcrumbs) #Id
            place: 'append', // append or prepent in container  prepend,append
            type: 'success',  // alert's type danger,warning,info,success
            message: msg,  // alert's message
            close: true, // make alert closable
            reset: true, // close all previouse alerts first
            focus: true, // auto scroll to the alert after shown
            closeInSeconds: 3, // auto close after defined seconds
            icon: 'check' // put icon before the message warning,user,check
        });
    }

    window.displayNotification = function (message) {
        if (window._ === undefined)
            return;

        if (_.isArray(message)) {
            $.each(message, function (i, val) {
                toastr['info'](msg, val);
            });
        }
        else {
            toastr['info'](msg, message);
        }
    }

    $(document).ready(function () {

        var html = $("html");

        applyCommonPlugins($("body"));

        // 保存继续编辑
        $(".btn-group button[value=save-continue],.actions button[value=save-continue]").click(function () {
            var btn = $(this);
            btn.closest("form").append('<input type="hidden" name="save-continue" value="true" />');
        });
        // 保存并新建
        $(".btn-group button[value=save-create],.actions button[value=save-create]").click(function () {
            var btn = $(this);
            btn.closest("form").append('<input type="hidden" name="save-create" value="true" />');
        });

        $(document).ajaxStart(function () {
            Pace.start();
        }).ajaxStop(function () {
            window.setTimeout(function () {
                Pace.stop();
            }, 300);
        });

        // check overridden store settings
        $('input.multi-site-override-option').each(function (index, elem) {
            Admin.checkOverriddenSiteValue(elem);
        });

        // publish entity commit messages
        $('.entity-commit-trigger').on('click', function (e) {
            var el = $(this);
            if (el.data('commit-type')) {
                EventBroker.publish("entity-commit", {
                    type: el.data('commit-type'),
                    action: el.data('commit-action'),
                    id: el.data('commit-id')
                });
            }
        });

        //隐藏/显示 提示信息
        $(".hint").tooltip({
            placement: 'left',
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

        function getFunction(code, argNames) {
            var fn = window, parts = (code || "").split(".");
            while (fn && parts.length) {
                fn = fn[parts.shift()];
            }
            if (typeof (fn) === "function") {
                return fn;
            }
            argNames.push(code);
            return Function.constructor.apply(null, argNames);
        }
        // tab strip smart auto selection
        $('.tabs-autoselect > ul.nav a[data-toggle=tab]').on('shown', function (e) {
            var tab = $(e.target),
				strip = tab.closest('.tabbable'),
				href = strip.data("tabselector-href"),
				hash = tab.attr("href");

            if (hash)
                hash = hash.replace(/#/, "");

            if (href) {
                $.ajax({
                    type: "POST",
                    url: href,
                    async: true,
                    data: { navId: strip.attr('id'), tabId: hash, path: location.pathname + location.search },
                    global: false
                });
            }
        });

        // AJAX tabs
        $('.nav a[data-ajax-url]').on('click', function (e) {
            var newTab = $(e.target),
				tabbable = newTab.closest('.tabbable'),
				pane = tabbable.find(newTab.attr("href")),
				url = newTab.data('ajax-url'),
                tabName1 = newTab.data('tab-name');
            if (newTab.data("loaded") || !url)
                return;

            $.ajax({
                cache: false,
                type: "GET",
                async: false,
                global: false,
                url: url,
                beforeSend: function (xhr) {
                    getFunction(tabbable.data("ajax-onbegin"), ["tab", "pane", "xhr"]).apply(this, [newTab, pane, xhr]);
                },
                success: function (data, status, xhr) {
                    pane.html(data);
                    getFunction(tabbable.data("ajax-onsuccess"), ["tab", "pane", "data", "status", "xhr"]).apply(this, [newTab, pane, data, status, xhr]);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    pane.html('<div class="text-error">Error while loading resource: ' + thrownError + '</div>');
                    getFunction(tabbable.data("ajax-onfailure"), ["tab", "pane", "xhr", "ajaxOptions", "thrownError"]).apply(this, [newTab, pane, xhr, ajaxOptions, thrownError]);
                },
                complete: function (xhr, status) {
                    newTab.data("loaded", true);
                    var tabName = newTab.data('tab-name') || newTab.attr("href").replace(/#/, "");
                    tabbable.append('<input type="hidden" class="loaded-tab-name" name="LoadedTabs" value="' + tabName + '" />');

                    getFunction(tabbable.data("ajax-oncomplete"), ["tab", "pane", "xhr", "status"]).apply(this, [newTab, pane, xhr, status]);
                }
            });
        });

    });


})(jQuery, this, document);