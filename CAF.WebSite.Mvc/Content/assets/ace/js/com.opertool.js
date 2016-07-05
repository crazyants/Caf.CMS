$.browser = {
    mozilla: false,
    webkit: false,
    opera: false,
    msie: false,
    ie8: false,
    ie7: false,
    safari: false,
    ios_safari: false,
    android: false,
    isMoveBrowser: false,
    scType: ""
};
var opt_Agent = navigator.userAgent;
$.browser.mozilla = "MozAppearance" in document.documentElement.style;
$.browser.webkit = !!opt_Agent.match(/AppleWebKit/i);
$.browser.opera = /opera/.test(navigator.userAgent.toLowerCase());
$.browser.msie = window.navigator.msPointerEnabled || document.all && document.querySelector;
$.browser.ie7 = document.all && !document.querySelector;
$.browser.ie8 = document.all && !document.addEventListener && $.browser.ie7 != true;
$.browser.safari = !!opt_Agent.match(/Safari/i) && !opt_Agent.match(/Chrome/i);
$.browser.ios_safari = !!opt_Agent.match(/OS ([4-9])(_\d)+ like Mac OS X/i) && !opt_Agent.match(/CriOS/i) || opt_Agent.indexOf("iPhone") > -1;
$.browser.android = $.browser.safari && !!opt_Agent.match(/Android/i) || opt_Agent.indexOf("Android") > -1 || opt_Agent.indexOf("Linux") > -1;
$.browser.isMoveBrowser = $.browser.android || $.browser.ios_safari;
$.browser.isapp = !!opt_Agent.match(/iosapp/i);
$.browser.useapp = false;
$.browser.set_scType = function () {
    var e = parseInt($(window).width());
    if (e >= 1200) {
        $.browser.scType = "lg"
    } else if (e >= 992) {
        $.browser.scType = "md"
    } else if (e >= 768) {
        $.browser.scType = "sm"
    } else {
        $.browser.scType = "xs"
    }
    $.browser.useapp = $.browser.isMoveBrowser || $.browser.isapp
};
$.browser.set_scType();

/*Json解析*/
function StringToJson(str) {
    if (!str.indexOf) {
        str.indexOf = function (e) {
            var t = this.length >>> 0;
            var i = Number(arguments[1]) || 0;
            i = i < 0 ? Math.ceil(i) : Math.floor(i);
            if (i < 0) i += t;
            for (; i < t; i++) {
                if (i in this && this[i] === e) return i
            }
            return -1
        }
    }
    if (str.indexOf("[") + str.indexOf("{") > -2) {
        return eval("(" + str + ")")
    }
    return []
}
function jsonToString(e) {
    var t = this;
    switch (typeof e) {
        case "string":
            return '"' + e.replace(/(["\\])/g, "\\$1") + '"';
        case "array":
            return "[" + e.map(t.jsonToString).join(",") + "]";
        case "object":
            if (e instanceof Array) {
                var i = [];
                var a = e.length;
                for (var n = 0; n < a; n++) {
                    i.push(t.jsonToString(e[n]))
                }
                return "[" + i.join(",") + "]"
            } else if (e == null) {
                return "null"
            } else {
                var s = [];
                for (var r in e) s.push(t.jsonToString(r) + ":" + t.jsonToString(e[r] == "undefined" ? "" : e[r]));
                return "{" + s.join(",") + "}"
            }
        case "number":
            return e;
        case "boolean":
            if (e) {
                return true
            } else {
                return false
            };
        case "function":
            return '"' + getFnName(e) + '"';
        case false:
            return e
    }
}


$.fn.resize = function () {
    $(".easyui_dg", this).each(function () {
        var e = $(this);
        e.datagrid("resize")
    });
    $(".bs_dialog.open", this).each(function () {
        $(this).bs_dialog("resize")
    })
};
$.fn.destroy = function () {
    this.each(function () {
        $(this).remove(); {
            this.outerHTML = ""
        }
    })
};
$.fn.iFrameClear = function () {
    var e = this[0];
    var t = e.contentWindow;
    if (e) {
        e.src = "about:blank";
        try {
            t.document.clear();
            this.remove()
        } catch (i) { }
    }
};
/*信息提示*/
$.fn.showPageLoading = function (e) {
    this.find("div.div-page-mask-msg").remove();
    this.find("div.div-page-mask").remove();
    msg = "";
    var t = this;
    if (e) {
        t = document
    }
    var i = $(t).width();
    var a = $(t).height();
    var n = 0;
    var s = 0;
    if ($(this).css("position") != "absolute") {
        n = $(this).offset().left;
        s = $(this).offset().top
    }
    $('<div class="div-page-mask"></div>').css({
        display: "block",
        left: n,
        top: s,
        width: i,
        height: a
    }).appendTo(this);
    $('<div class="div-page-mask-msg"></div>').html(msg).appendTo(this).css({
        display: "block",
        left: (i - $("div.div-page-mask-msg", this).outerWidth()) / 2 + n,
        top: (a - $("div.div-page-mask-msg", this).outerHeight()) / 2 + s
    })
};
$.fn.hidePageLoading = function () {
    this.find("div.div-page-mask-msg").remove();
    this.find("div.div-page-mask").remove()
};
$.fn.showLoading = function (e) {
    if (window.frames.length != parent.frames.length) {
        if ($(parent.document).find("div.widget-box-overlay").length > 0) {
            return
        }
    }
    $(this).find(">div.widget-box-overlay").remove();
    var t = '<div class="widget-box-overlay"><button type="button" class="close"  >×</button><span class="loading-icon"></span></div>';
    $(this).append(t);
    $(this).find("div.widget-box-overlay .close").on("click", function (e) {
        var t = $(this);
        t.closest("div.widget-box-overlay").remove()
    })
};
$.fn.hideLoading = function () {
    $(this).find(">div>i.fa-spin").html("");
    var e = this;
    $(e).find(">div.widget-box-overlay").remove()
};

/*设置UI显示状态*/
$.fn.setuibystate = function (e) {
    if ($(this).find(":hidden[name='formStatus']").length <= 0) {
        $(this).append('<input  name="formStatus" type="hidden" />')
    }
    $(this).find(":hidden[name='formStatus']").val(e);
    $(this).find(":text:not(.reftext-text):not(.combo-text),:password,textarea").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).attr("readonly", "readonly")
        } else {
            $(this).removeAttr("readonly")
        }
    });
    $(this).find(":checkbox,:radio,select").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).attr("disabled", "disabled")
        } else {
            $(this).removeAttr("disabled")
        }
    });
    $(this).find("input[comboName].combobox-f,select[comboName].combobox-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).combobox("disable")
        } else {
            $(this).combobox("enable")
        }
    });
    $(this).find("input[comboName].combotree-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
           // $(this).combotree("disable")
        } else {
           // $(this).combotree("enable")
        }
    });
    $(this).find("input[comboName].datebox-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).datebox("disable")
        } else {
            $(this).datebox("enable")
        }
    });
    $(this).find("input[comboName].datetimebox-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).datetimebox("disable")
        } else {
            $(this).datetimebox("enable")
        }
    });
    $(this).find("input[reftextName].reftext-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).reftext("disable")
        } else {
            $(this).reftext("enable")
        }
    });
    $(this).find("input[mxcalcName].mxcalc-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).mxcalc("disable")
        } else {
            $(this).mxcalc("enable")
        }
    });
    $(this).find("input.mxdate-f").each(function () {
        var t = e == "read" ? false : true;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("readedit") != -1) {
                    t = true
                };
                break;
            case "edit":
                if (i.indexOf("editdis") != -1) {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("adddis") != -1) {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).mxdate("disable")
        } else {
            $(this).mxdate("enable")
        }
    });
    $(this).find("[settag]").each(function () {
        var t = $(this).attr("settag");
        var i = false;
        switch (e) {
            case "read":
                if (t.indexOf("readhide") != -1) {
                    i = true
                };
                break;
            case "edit":
                if (t.indexOf("edithide") != -1) {
                    i = true
                };
                break;
            case "add":
                if (t.indexOf("addhide") != -1) {
                    i = true
                };
                break;
            default:
                break
        }
        if (i) {
            $(this).hide()
        } else {
            $(this).show()
        }
    })
};
$.fn.settbuibystate = function (e) {
    $(this).find("[settag].btn").each(function () {
        var t = false;
        var i = "";
        if ($(this).attr("settag")) {
            i = $(this).attr("settag")
        }
        switch (e) {
            case "read":
                if (i.indexOf("read") != -1) {
                    t = true
                } else {
                    t = false
                };
                break;
            case "edit":
                if (i.indexOf("edit") != -1 || i.indexOf("modify") != -1) {
                    t = true
                } else {
                    t = false
                };
                break;
            case "add":
                if (i.indexOf("add") != -1 || i.indexOf("modify") != -1) {
                    t = true
                } else {
                    t = false
                };
                break;
            default:
                break
        }
        if (!t) {
            $(this).addClass("disabled")
        } else {
            $(this).removeClass("disabled")
        }
    });
    if ($.smallscreen.dh) {
        $.smallscreen.dh.find("li[settag]").each(function () {
            var t = false;
            var i = "";
            if ($(this).attr("settag")) {
                i = $(this).attr("settag")
            }
            switch (e) {
                case "read":
                    if (i.indexOf("read") != -1) {
                        t = true
                    } else {
                        t = false
                    };
                    break;
                case "edit":
                    if (i.indexOf("edit") != -1 || i.indexOf("modify") != -1) {
                        t = true
                    } else {
                        t = false
                    };
                    break;
                case "add":
                    if (i.indexOf("add") != -1 || i.indexOf("modify") != -1) {
                        t = true
                    } else {
                        t = false
                    };
                    break;
                default:
                    break
            }
            if (!t) {
                $(this).addClass("hidden")
            } else {
                $(this).removeClass("hidden")
            }
        })
    }
};
$.fn.setuibyauth = function (e) {
    if (!e.IsSearchAuth) {
        $(this).find("*").css({
            display: "none"
        });
        $(this).append("<b>您无权限查看此页面!</b>");
        return
    }
    $(this).find("[authtag]").addClass("hidden auth-hidden");
    $(this).find("[authtag='']").removeClass("hidden");
    if (e.IsAddAuth) {
        $(this).find('[authtag*="IsAddAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsModifyAuth) {
        $(this).find('[authtag*="IsModifyAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsDeleteAuth) {
        $(this).find('[authtag*="IsDeleteAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsPrintAuth) {
        $(this).find('[authtag*="IsPrintAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsExportAuth) {
        $(this).find('[authtag*="IsExportAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsVerifyAuth) {
        $(this).find('[authtag*="IsVerifyAuth"]:hidden').removeClass("hidden")
    }
    if (e.IsVoidAuth) {
        $(this).find('[authtag*="IsVoidAuth"]:hidden').removeClass("hidden")
    }
    var t = this;
    if (e.OtherAuth) {
        $.each(e.OtherAuth, function (e, i) {
            $(t).find('[authtag*="' + i + '"]:hidden').removeClass("hidden")
        })
    }
    $(this).find("[authtag].hidden.auth-hidden").remove()
}

