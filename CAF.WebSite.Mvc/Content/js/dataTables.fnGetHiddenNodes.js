$.fn.dataTable.Api.register('GetColumnIndex()', function (sCol) {
    var cols = this.context[0].aoColumns;
    for (var x = 0, xLen = cols.length ; x < xLen ; x++) {
        if (cols[x].sTitle.toLowerCase() == sCol.toLowerCase()) {
            return x;
        }
    }
    return -1;

});
$.fn.dataTable.Api.register('table().GetHiddenNodes()', function (table) {

    var nodes;
    var display = jQuery('tbody tr', table);

    if (jQuery.fn.dataTable.versionCheck) {
        // DataTables 1.10
        var api = new jQuery.fn.dataTable.Api(this);
        nodes = api.rows().nodes().toArray();
    }
    else {
        // 1.9-
        nodes = this.oApi._fnGetTrNodes(this);
    }

    /* Remove nodes which are being displayed */
    for (var i = 0 ; i < display.length ; i++) {
        var iIndex = jQuery.inArray(display[i], nodes);

        if (iIndex != -1) {
            nodes.splice(iIndex, 1);
        }
    }

    return nodes;

});

