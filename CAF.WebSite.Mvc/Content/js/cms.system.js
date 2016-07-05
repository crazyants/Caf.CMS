/* cms.system.js
-------------------------------------------------------------- */
;

(function ($) {

    var formatRe = /\{(\d+)\}/g;

    String.prototype.format = function () {
        var s = this, args = arguments;
        return s.replace(formatRe, function (m, i) {
            return args[i];
        });
    };
    Date.prototype.format = function (format) {
        var o = {
            "M+": this.getMonth() + 1, //month
            "d+": this.getDate(),    //day
            "h+": this.getHours(),   //hour
            "m+": this.getMinutes(), //minute
            "s+": this.getSeconds(), //second
            "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
            "S": this.getMilliseconds() //millisecond
        }
        if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o) if (new RegExp("(" + k + ")").test(format))
            format = format.replace(RegExp.$1,
            RegExp.$1.length == 1 ? o[k] :
            ("00" + o[k]).substr(("" + o[k]).length));
        return format;
    }


    // define noop funcs for window.console in order
    // to prevent scripting errors
    var c = window.console = window.console || {};
    function noop() { };
    var funcs = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd',
					'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'],
		flen = funcs.length,
		noop = function () { };
    while (flen) {
        if (!c[funcs[--flen]]) {
            c[funcs[flen]] = noop;
        }
    }

    // define default secure-casts
    jQuery.extend(window, {

        toBool: function (val) {
            var defVal = typeof arguments[1] === "boolean" ? arguments[1] : false;
            var t = typeof val;
            if (t === "boolean") {
                return val;
            }
            else if (t === "string") {
                switch (val.toLowerCase()) {
                    case "1": case "true": case "yes": case "on": case "checked":
                        return true;
                    case "0": case "false": case "no": case "off":
                        return false;
                    default:
                        return defVal;
                }
            }
            else if (t === "number") {
                return Boolean(val);
            }
            else if (t === "null" || t === "undefined") {
                return defVal;
            }
            return defVal;
        },

        toStr: function (val) {
            var defVal = typeof arguments[1] === "string" ? arguments[1] : "";
            if (!val || val === "[NULL]") {
                return defVal;
            }
            return String(val) || defVal;
        },

        toInt: function (val) {
            var defVal = typeof arguments[1] === "number" ? arguments[1] : 0;
            var x = parseInt(val);
            if (isNaN(x)) {
                return defVal;
            }
            return x;
        },

        toFloat: function (val) {
            var defVal = typeof arguments[1] === "number" ? arguments[1] : 0;
            var x = parseFloat(val);
            if (isNaN(x)) {
                return defVal;
            }
            return x;
        },

    });


})(jQuery);
