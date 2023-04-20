/* inicio kpi lpns Dispatched */
function KpiLpnsDispatched(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (idLpnCode == '') {
        idLpnCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        userName: userName,
        idLpnCode: idLpnCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiLpnsDispatched",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiLpnsDispatched(data, idButton, userName, idLpnCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiLpnsDispatched";
    var idCanvas = "chartKpiLpnsDispatched";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiLpnsDispatched").hide();
        $("#btnExcelKpiLpnsDispatched").addClass("hidden");
        return;
    }

    $("#divPagKpiLpnsDispatched").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiLpnsDispatched").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    $("#divChartKpiLpnsDispatched .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "LPNs Despachados",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ratio"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiLpnsDispatched');
    totalKpiLpnsDispatched(data);
    configKpiSummaryLpnsDispatched(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiLpnsDispatched(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (qty) {
            total += qty
        });
    }

    $("#totalLpnsDispatched .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryLpnsDispatched(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiLpnsDispatched(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        var tmpl = $.templates("#tmpSummaryKpiLpnsDispatched");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersLpnsDispatchedFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiLpnsDispatched(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen LPNs Despachados');
    });
}

function pagKpiLpnsDispatched(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsDispatched').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersLpnsDispatchedFilter").val();
            var idLpnCode = $("#txtLpnLpnsDispatchedFilter").val();
            var startDate = $("#txtDateStartLpnsDispatchedFilter").val();
            var endDate = $("#txtDateEndLpnsDispatchedFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ddlWarehouseLpnsDispatchedFilter").val();

            userName = valueSelect(userName);
            idLpnCode = valueInput(idLpnCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryLpnsDispatched("tableKpiLpnsDispatched", userName, idLpnCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiLpnsDispatchedDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiLpnsDispatchedDetail');
    KpiLpnsDispatchedDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiLpnsDispatchedDetail(trackOutboundDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersLpnsDispatchedFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var idLpnCode = $("#txtLpnLpnsDispatchedFilter").val();

    if (idLpnCode == '') {
        idLpnCode = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        trackOutboundDate: trackOutboundDate,
        ownCode: ownCode,
        userName: userName,
        idLpnCode: idLpnCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };
     
    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiLpnsDispatchedDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableLpnsDispatchedDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle LPNs Despachados");
        }
    });
}

function TableLpnsDispatchedDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiLpnsDispatchedDetail");
        var html = tmpl.render(data);
        $("#tableKpiLpnsDispatchedDetail > tbody").empty().append(html);

        pagKpiLpnsDispatchedDetail(countReg, trackOutboundDate);
        $("#modalKpiLpnsDispatchedDetail").modal("show");
    }
}

function pagKpiLpnsDispatchedDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsDispatchedDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiLpnsDispatchedDetail(trackOutboundDate, page, pageSize());
        }
    });

}

function createExcelKpiLpnsDispatched(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: "Cantidad de LPNs",
        WorkHours: "Horas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var userName = $("#ctl00_MainContent_ddlUsersLpnsDispatchedFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsDispatchedFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersLpnsDispatchedFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsDispatchedFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - LPNs Despachados");
}
/* fin kpi lpns Dispatched */

/* inicio kpi M3 Dispatched */
function KpiM3Dispatched(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        userName: userName,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiM3Dispatched",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiM3Dispatched(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiM3Dispatched";
    var idCanvas = "chartKpiM3Dispatched";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiM3Dispatched").hide();
        $("#btnExcelKpiM3Dispatched").addClass("hidden");
        return;
    }

    $("#divPagKpiM3Dispatched").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiM3Dispatched").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    $("#divChartKpiM3Dispatched .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "M3 Despachados",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ratio"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiM3Dispatched');
    totalKpiM3Dispatched(data);
    configKpiSummaryM3Dispatched(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiM3Dispatched(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "M3").forEach(function (qty) {
            total += qty
        });
    }

    $("#totalM3Dispatched .values-total:eq(0) > span").html(numeral(total).format('0,0.000'));
}

function configKpiSummaryM3Dispatched(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiM3Dispatched(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        var tmpl = $.templates("#tmpSummaryKpiM3Dispatched");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 7;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersM3DispatchedFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiM3Dispatched(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen M3 Despachados');
    });
}

function pagKpiM3Dispatched(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiM3Dispatched').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersM3DispatchedFilter").val();
            var startDate = $("#txtDateStartM3DispatchedFilter").val();
            var endDate = $("#txtDateEndM3DispatchedFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseM3DispatchedFilter").val();

            userName = valueSelect(userName);
            idLpnCode = valueInput(idLpnCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryM3Dispatched("tableKpiM3Dispatched", userName, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiM3DispatchedDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiM3DispatchedDetail');
    KpiM3DispatchedDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiM3DispatchedDetail(trackOutboundDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersM3DispatchedFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        trackOutboundDate: trackOutboundDate,
        ownCode: ownCode,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiM3DispatchedDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableM3DispatchedDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle M3 Despachados");
        }
    });
}

function TableM3DispatchedDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiM3DispatchedDetail");
        var html = tmpl.render(data);
        $("#tableKpiM3DispatchedDetail > tbody").empty().append(html);

        pagKpiM3DispatchedDetail(countReg, trackOutboundDate);
        $("#modalKpiM3DispatchedDetail").modal("show");
    }
}

function pagKpiM3DispatchedDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiM3DispatchedDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiM3DispatchedDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiM3Dispatched(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQtyDispatch: "Cantidad de Items",
        M3: "Volumen",
        WorkHours: "Horas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var userName = $("#ctl00_MainContent_ddlUsersM3DispatchedFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseM3DispatchedFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQtyDispatch: value.ItemQtyDispatch,
            M3: numeral(value.M3).format('0,0.000'),
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersM3DispatchedFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseM3DispatchedFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - M3 Despachados");
}
/* fin kpi M3 Dispatched */

/* inicio kpi lpns sorted */
function KpiLpnsSorted(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (idLpnCode == '') {
        idLpnCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        userName: userName,
        idLpnCode: idLpnCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiLpnsSorted",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiLpnsSorted(data, idButton, userName, idLpnCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiLpnsSorted";
    var idCanvas = "chartKpiLpnsSorted";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiLpnsSorted").hide();
        $("#btnExcelKpiLpnsSorted").addClass("hidden");
        return;
    }

    $("#divPagKpiLpnsSorted").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiLpnsSorted").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    $("#divChartKpiLpnsSorted .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "LPNs Ordenados",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ratio"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiLpnsSorted');
    totalKpiLpnsSorted(data);
    configKpiSummaryLpnsSorted(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiLpnsSorted(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (qty) {
            total += qty
        });
    }

    $("#totalLpnsSorted .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryLpnsSorted(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiLpnsSorted(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        var tmpl = $.templates("#tmpSummaryKpiLpnsSorted");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersLpnsSortedFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiLpnsSorted(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen LPNs Ordenados');
    });
}

function pagKpiLpnsSorted(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsSorted').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersLpnsSortedFilter").val();
            var idLpnCode = $("#txtLpnLpnsSortedFilter").val();
            var startDate = $("#txtDateStartLpnsSortedFilter").val();
            var endDate = $("#txtDateEndLpnsSortedFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsSortedFilter").val();

            userName = valueSelect(userName);
            idLpnCode = valueInput(idLpnCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryLpnsSorted("tableKpiLpnsSorted", userName, idLpnCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiLpnsSortedDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiLpnsSortedDetail');
    KpiLpnsSortedDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiLpnsSortedDetail(trackOutboundDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersLpnsSortedFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var idLpnCode = $("#txtLpnLpnsSortedFilter").val();

    if (idLpnCode == '') {
        idLpnCode = null;
    }

    var param = {
        trackOutboundDate: trackOutboundDate,
        userName: userName,
        idLpnCode: idLpnCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiLpnsSortedDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableLpnsSortedDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle LPNs Ordenados");
        }
    });
}

function TableLpnsSortedDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiLpnsSortedDetail");
        var html = tmpl.render(data);
        $("#tableKpiLpnsSortedDetail > tbody").empty().append(html);

        pagKpiLpnsSortedDetail(countReg, trackOutboundDate);
        $("#modalKpiLpnsSortedDetail").modal("show");
    }
}

function pagKpiLpnsSortedDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsSortedDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiLpnsSortedDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiLpnsSorted(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: "Cantidad de LPNs",
        WorkHours: "Horas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var userName = $("#ctl00_MainContent_ddlUsersLpnsSortedFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsSortedFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersLpnsSortedFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsSortedFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - LPNs Distribuidos");
}
/* fin kpi lpns sorted */

/* inicio kpi units sorted */
function KpiUnitsSorted(userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (itemCode == '') {
        itemCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        userName: userName,
        itemCode: itemCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiUnitsSorted",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiUnitsSorted(data, idButton, userName, itemCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiUnitsSorted";
    var idCanvas = "chartKpiUnitsSorted";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiUnitsSorted").hide();
        $("#btnExcelKpiUnitsSorted").addClass("hidden");
        return;
    }

    $("#divPagKpiUnitsSorted").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiUnitsSorted").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    $("#divChartKpiUnitsSorted .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "Unidades Ordenados",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ratio"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiUnitsSorted');
    totalKpiUnitsSorted(data);
    configKpiSummaryUnitsSorted(idTable, userName, itemCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiUnitsSorted(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (qty) {
            total += qty
        });
    }

    $("#totalUnitsSorted .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryUnitsSorted(idTable, userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiUnitsSorted(userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        var tmpl = $.templates("#tmpSummaryKpiUnitsSorted");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersUnitsSortedFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiUnitsSorted(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Unidades Ordenadas');
    });
}

function pagKpiUnitsSorted(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiUnitsSorted').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersUnitsSortedFilter").val();
            var itemCode = $("#ctl00_MainContent_hidItemCodeUnitsSortedFilter").val();
            var startDate = $("#txtDateStartUnitsSortedFilter").val();
            var endDate = $("#txtDateEndUnitsSortedFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsSortedFilter").val();

            userName = valueSelect(userName);
            itemCode = valueInput(itemCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryUnitsSorted("tableKpiUnitsSorted", userName, itemCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiUnitsSortedDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiUnitsSortedDetail');
    KpiUnitsSortedDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiUnitsSortedDetail(trackOutboundDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersUnitsSortedFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var itemCode = $("#ctl00_MainContent_hidItemCodeUnitsSortedFilter").val();

    if (itemCode == '') {
        itemCode = null;
    }

    var param = {
        trackOutboundDate: trackOutboundDate,
        userName: userName,
        itemCode: itemCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiUnitsSortedDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableUnitsSortedDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Unidades Ordenadas");
        }
    });
}

function TableUnitsSortedDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiUnitsSortedDetail");
        var html = tmpl.render(data);
        $("#tableKpiUnitsSortedDetail > tbody").empty().append(html);

        pagKpiUnitsSortedDetail(countReg, trackOutboundDate);
        $("#modalKpiUnitsSortedDetail").modal("show");
    }
}

function pagKpiUnitsSortedDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiUnitsSortedDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiUnitsSortedDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiUnitsSorted(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: "Cantidad de Unidades",
        WorkHours: "Horas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var userName = $("#ctl00_MainContent_ddlUsersUnitsSortedFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsSortedFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersUnitsSortedFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsSortedFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - Unidades Distribuidos");
}
/* fin kpi units sorted */

/* inicio kpi lines sorted */
function KpiLinesSorted(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        userName: userName,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiLinesSorted",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiLinesSorted(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiLinesSorted";
    var idCanvas = "chartKpiLinesSorted";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiLinesSorted").hide();
        $("#btnExcelKpiLinesSorted").addClass("hidden");
        return;
    }

    $("#divPagKpiLinesSorted").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiLinesSorted").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    $("#divChartKpiLinesSorted .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "Líneas Ordenados",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ratio"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiLinesSorted');
    totalKpiLinesSorted(data);
    configKpiSummaryLinesSorted(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiLinesSorted(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (qty) {
            total += qty
        });
    }

    $("#totalLinesSorted .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryLinesSorted(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiLinesSorted(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        var tmpl = $.templates("#tmpSummaryKpiLinesSorted");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersLinesSortedFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiLinesSorted(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Líneas Ordenadas');
    });
}

function pagKpiLinesSorted(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLinesSorted').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersLinesSortedFilter").val();
            var startDate = $("#txtDateStartLinesSortedFilter").val();
            var endDate = $("#txtDateEndLinesSortedFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsSortedFilter").val();

            userName = valueSelect(userName);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryLinesSorted("tableKpiLinesSorted", userName, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiLinesSortedDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiLinesDispatchedDetail');
    KpiLinesSortedDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiLinesSortedDetail(trackOutboundDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersLinesSortedFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var param = {
        trackOutboundDate: trackOutboundDate,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiLinesSortedDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableLinesSortedDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Lineas Despachadas");
        }
    });
}

function TableLinesSortedDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiLinesSortedDetail");
        var html = tmpl.render(data);
        $("#tableKpiLinesSortedDetail > tbody").empty().append(html);

        pagKpiLinesSortedDetail(countReg, trackOutboundDate);
        $("#modalKpiLinesSortedDetail").modal("show");
    }
}

function pagKpiLinesSortedDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLinesSortedDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiLinesSortedDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiLinesSorted(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: "Cantidad de Lineas",
        WorkHours: "Horas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var userName = $("#ctl00_MainContent_ddlUsersLinesSortedFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseLinesSortedFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersLinesSortedFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseLinesSortedFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - Lineas Distribuidos");
}
/* fin kpi lines sorted */

/* inicio kpi fillrate */
function KpiFillRateDispatch(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (customerCode == '0') {
        customerCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        customerCode: customerCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiFillRateDispatch",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiFillRateDispatch(data, idButton, customerCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiFillRateDispatch";
    var idCanvas = "chartKpiFillRateDispatch";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiFillRateDispatch").hide();
        $("#btnExcelKpiFillRateDispatch").addClass("hidden");
        return;
    }

    $("#divPagKpiFillRateDispatch").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiFillRateDispatch").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    data = calculatePercentage(data, "ItemQtyDispatch", "ItemQtyOrder", "Ratio");
    $("#divChartKpiFillRateDispatch .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: [{
                label: "Cumplimiento Despachos",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                fill: false,
                data: _.pluck(data, "Ratio"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Tiempo'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "%"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiFillRateDispatch');
    totalKpiFillRateDispatch(data);
    configKpiSummaryFillRateDispatch(idTable, customerCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiFillRateDispatch(data) {
    var dispatchTotal = 0;
    var requestedTotal = 0;

    if (data.length > 0) {
        data.forEach(function (d) {
            dispatchTotal += d.ItemQtyDispatch;
            requestedTotal += d.ItemQtyOrder;
        });
    }

    $("#totalFillRateDispatch .values-total:eq(0) > span").html(numeral(dispatchTotal).format('0,0'));
    $("#totalFillRateDispatch .values-total:eq(1) > span").html(numeral(requestedTotal).format('0,0'));
}

function configKpiSummaryFillRateDispatch(idTable, customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiFillRateDispatch(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");
        data = calculatePercentage(data, "ItemQtyDispatch", "ItemQtyOrder", "Ratio");
        var tmpl = $.templates("#tmpSummaryKpiFillRateDispatch");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        customerCode = valueSelect(customerCode);
        var indexCustomerCode = 6;
        if (customerCode != null) {
            showColumnInTable(indexCustomerCode, idTable);
            setValueColumnInTable(indexCustomerCode, $("#ctl00_MainContent_ddlCustomerFillRateDispatchFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexCustomerCode, idTable);
        }

        pagKpiFillRateDispatch(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Cumplimiento de Despachos');
    });
}

function pagKpiFillRateDispatch(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateDispatch').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateDispatchFilter").val();
            var startDate = $("#txtDateStartFillRateDispatchFilter").val();
            var endDate = $("#txtDateEndFillRateDispatchFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateDispatchFilter").val();

            customerCode = valueSelect(customerCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryFillRateDispatch("tableKpiFillRateDispatch", customerCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiFillRateDispatchDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiFillRateDispatchDetail');
    KpiFillRateDispatchDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiFillRateDispatchDetail(trackOutboundDate, pageNumber, pageSize) {

    var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateDispatchFilter").val();

    if (customerCode == '0') {
        customerCode = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        trackOutboundDate: trackOutboundDate,
        customerCode: customerCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiFillRateDispatchDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableFillRateDispatchDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Cumplimiento de Despachos");
        }
    });
}

function TableFillRateDispatchDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiFillRateDispatchDetail");
        var html = tmpl.render(data);
        $("#tableKpiFillRateDispatchDetail > tbody").empty().append(html);

        pagKpiFillRateDispatchDetail(countReg, trackOutboundDate);
        $("#modalKpiFillRateDispatchDetail").modal("show");
    }
}

function pagKpiFillRateDispatchDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateDispatchDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiFillRateDispatchDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiFillRateDispatch(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQtyDispatch: "Cantidad Despachadas",
        ItemQtyOrder: "Cantidad Solicitadas",
        Ratio: "Ratio",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");
    data = calculatePercentage(data, "ItemQtyDispatch", "ItemQtyOrder", "Ratio");

    var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateDispatchFilter").val();
    customerCode = valueSelect(customerCode);

    if (customerCode != null) {
        headers.customerCode = "Cliente";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateDispatchFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQtyDispatch: value.ItemQtyDispatch,
            ItemQtyOrder: value.ItemQtyOrder,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (customerCode != null) {
            finalObj.customerCode = $("#ctl00_MainContent_ddlCustomerFillRateDispatchFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateDispatchFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - Cumplimiento de Despachos");
}
/* fin kpi fillrate */

/* inicio kpi order status */
function KpiOrderStatus(customerCode, ownCode, whsCode, startDate, endDate) {

    if (customerCode == '0') {
        customerCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        customerCode: customerCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiOrderStatus",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiOrderStatus(data, idButton, customerCode, startDate, endDate) {

    $("#divSimpleListOrderStatus").empty();
    $("#divCompleteListOrderStatus").empty();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#btnExcelKpiOrderStatus").addClass("hidden");
        return;
    }

    $("#btnExcelKpiOrderStatus").removeClass("hidden");

    var tmpl = $.templates("#tmpSummaryKpiSimpleListOrderStatus");
    var html = tmpl.render(data);
    $("#divSimpleListOrderStatus").append(html);

    var tmplCompleteList = $.templates("#tmpSummaryKpiCompleteListOrderStatus");
    var htmlCompleteList = tmplCompleteList.render(data);
    $("#divCompleteListOrderStatus").append(htmlCompleteList);

    if (isFullScreenOrderStatus() == true) {
        $("#divSimpleListOrderStatus").hide();
        $("#divCompleteListOrderStatus").show();
    } else {
        $("#divSimpleListOrderStatus").show();
        $("#divCompleteListOrderStatus").hide();
    }

    clearPagination('pagKpiOrderStatus');
    totalKpiOrderStatus(data);
}

function totalKpiOrderStatus(data) {
    var total = 0;

    if (data.length > 0) {
        data.forEach(function (d) {
            total += d.CountReg;
        });
    }

    $("#totalOrderStatus .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiOrderStatusSummaryFacade(nameTrackOutboundType, pageNumber, pageSize) {
    clearPagination('pagKpiOrderStatus');
    configKpiOrderStatusSummary(nameTrackOutboundType, pageNumber, pageSize);
}

function configKpiOrderStatusSummary(nameTrackOutboundType, pageNumber, pageSize) {

    var customerCode = $("#ctl00_MainContent_ddlCustomerOrderStatusFilter").val();

    if (customerCode == '0') {
        customerCode = null;
    }

    var startDate = $("#txtDateStartOrderStatusFilter").val();

    if (startDate == '') {
        startDate = null;
    }

    var endDate = $("#txtDateEndOrderStatusFilter").val();

    if (endDate == '') {
        endDate = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        nameTrackOutboundType: nameTrackOutboundType,
        customerCode: customerCode,
        ownCode: ownCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    var prom = Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiOrderStatusSummary",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );

    prom.done(function (data) { 
        var idTable = "tableKpiOrderStatus";
        data = data.d.Entities;

        if (data.length > 0) {
            var countReg = data[0].CountReg || 0;
            data = utilSetDateGeneric(data, "TrackOutboundDate");
            var tmpl = $.templates("#tmpSummaryKpiOrderStatus");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            var indexCustomerCode = 5;
            if (customerCode != null) {
                showColumnInTable(indexCustomerCode, idTable);
                setValueColumnInTable(indexCustomerCode, $("#ctl00_MainContent_ddlCustomerOrderStatusFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexCustomerCode, idTable);
            }

            pagKpiOrderStatus(countReg, idDispatchType);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Estado de Pedidos');
    });
}

function pagKpiOrderStatus(countReg, idDispatchType) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiOrderStatus').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            configKpiOrderStatusSummary(idDispatchType, page, pageSize());
        }
    });
}

function KpiOrderStatusDetail(outboundNumber) {

    var param = {
        outboundNumber: outboundNumber
    };

    var prom = Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiOrderStatusDetail",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );

    prom.done(function (data) {
        TableOrderStatusDetail(data.d.Entities);
    });

    prom.fail(function (error) {
        errorAjax('Detalle Estado de Pedidos');
    });
}

function TableOrderStatusDetail(data) {

    if (data.length > 0) {
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiOrderStatusDetail");
        var html = tmpl.render(data);
        $("#tableKpiOrderStatusDetail > tbody").empty().append(html);
        $("#modalKpiOrderStatusDetail").modal("show");
    }
}

/* fin kpi order status */

/* inicio kpi orders on time */
function KpiOrderOnTime(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (customerCode == '0') {
        customerCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        customerCode: customerCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiOrderOnTime",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiOrderOnTime(data, idButton, customerCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiOrderOnTime";
    var idCanvas = "chartKpiOrderOnTime";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiOrderOnTime").hide();
        $("#btnExcelKpiOrderOnTime").addClass("hidden");
        return;
    }

    $("#divPagKpiOrderOnTime").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiOrderOnTime").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    $("#divChartKpiOrderOnTime .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var datasetsData = [];

    var listOrdersOnTime = {
        label: "Ordenes Perfectas",
        backgroundColor: '#2babab',
        borderColor: '#2babab',
        data: _.pluck(data, "CountOrdersOnTime"),
        fill: false
    };

    var listOrdersDelayed = {
        label: "Ordenes con Problemas",
        backgroundColor: '#ff6384',
        borderColor: '#ff6384',
        data: _.pluck(data, "CountOrdersDelayed"),
        fill: false
    };

    datasetsData.push(listOrdersOnTime);
    datasetsData.push(listOrdersDelayed);

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: datasetsData
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Tiempo'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ordenes"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiOrderOnTime');
    totalKpiOrderOnTime(data);
    configKpiSummaryOrderOnTime(idTable, customerCode, ownCode, whsCode, startDate, endDate, 1, pageSize(), true);
}

function totalKpiOrderOnTime(data) {

    var sumOrdersOnTime = _.reduce(_.pluck(data, "CountOrdersOnTime"), function (memo, num) {
        return memo + num;
    }, 0);

    var sumOrdersDelayed = _.reduce(_.pluck(data, "CountOrdersDelayed"), function (memo, num) {
        return memo + num;
    }, 0);

    $("#totalOrderOnTime .values-total:eq(0) > span").html(numeral(sumOrdersOnTime).format('0,0'));
    $("#totalOrderOnTime .values-total:eq(1) > span").html(numeral(sumOrdersDelayed).format('0,0'));
}

function configKpiSummaryOrderOnTime(idTable, customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiOrderOnTime(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiOrderOnTime");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        customerCode = valueSelect(customerCode);
        var indexCustomerCode = 6;
        if (customerCode != null) {
            showColumnInTable(indexCustomerCode, idTable);
            setValueColumnInTable(indexCustomerCode, $("#ctl00_MainContent_ddlCustomerOrderOnTimeFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexCustomerCode, idTable);
        }

        pagKpiOrderOnTime(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Pedidos Perfectos');
    });
}

function pagKpiOrderOnTime(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiOrderOnTime').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var customerCode = $("#ctl00_MainContent_ddlCustomerOrderOnTimeFilter").val();
            var startDate = $("#txtDateStartOrderOnTimeFilter").val();
            var endDate = $("#txtDateEndOrderOnTimeFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseOrderOnTimeFilter").val();

            customerCode = valueSelect(customerCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryOrderOnTime("tableKpiOrderOnTime", customerCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiOrderOnTimeDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiOrderOnTimeDetail');
    KpiOrderOnTimeDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiOrderOnTimeDetail(trackOutboundDate, pageNumber, pageSize) {

    var customerCode = $("#ctl00_MainContent_ddlCustomerOrderOnTimeFilter").val();
    customerCode = valueSelect(customerCode);

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        trackOutboundDate: trackOutboundDate,
        customerCode: customerCode,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownCode: ownCode
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiOrderOnTimeDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableOrderOnTimeDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Pedidos Perfectos");
        }
    });
}

function TableOrderOnTimeDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiOrderOnTimeDetail");
        var html = tmpl.render(data);
        $("#tableKpiOrderOnTimeDetail > tbody").empty().append(html);

        pagKpiOrderOnTimeDetail(countReg, trackOutboundDate);
        $("#modalKpiOrderOnTimeDetail").modal("show");
    }
}

function pagKpiOrderOnTimeDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiOrderOnTimeDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiOrderOnTimeDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function ExcelKpiOrderOnTimeDetail(trackOutboundDateStart, trackOutboundDateEnd, pageNumber, pageSize) {

    var customerCode = $("#ctl00_MainContent_ddlCustomerOrderOnTimeFilter").val();
    customerCode = valueSelect(customerCode);

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    var whsCode = $("#ctl00_MainContent_ddlWarehouseOrderOnTimeFilter").val();

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        trackOutboundDateStart: trackOutboundDateStart,
        trackOutboundDateEnd: trackOutboundDateEnd,
        customerCode: customerCode,
        ownCode: ownCode,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ExcelKpiOrderOnTimeDetail",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createExcelKpiOrderOnTime(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        Status: "Estado",
        OutboundNumber: "Documento Salida",
        WhsName: "Bodega",
        UserWms: "Usuario",
        CustomerName: "Nombre Cliente",
        CustomerCode: "Rut Cliente",
        TrackOutboundDate: "Fecha"
    };

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseOrderOnTimeFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            Status: (value.Status == 'OT' ? "Perfecto" : "con Problemas"),
            OutboundNumber: value.OutboundNumber,
            WhsName: value.DmWarehouse.WhsName,
            UserWms: value.UserWms,
            CustomerName: value.CustomerName,
            CustomerCode: value.CustomerCode,
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseOrderOnTimeFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - Pedidos Perfectos");
}
/* fin kpi orders on time */

/* inicio kpi fillrate por orden */
function KpiFillRateOrdersDispatch(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (customerCode == '0') {
        customerCode = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    ownCode = valueSelect(ownCode);
    whsCode = valueSelect(whsCode);

    var param = {
        customerCode: customerCode,
        ownCode: ownCode,
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiFillRateOrdersDispatch",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiFillRateOrdersDispatch(data, idButton, customerCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiFillRateOrdersDispatch";
    var idCanvas = "chartKpiFillRateOrdersDispatch";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiFillRateOrdersDispatch").hide();
        $("#btnExcelKpiFillRateOrdersDispatch").addClass("hidden");
        return;
    }

    $("#divPagKpiFillRateOrdersDispatch").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiFillRateOrdersDispatch").removeClass("hidden");

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    $("#divChartKpiFillRateOrdersDispatch .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var datasetsData = [];

    var listOrdersOnTime = {
        label: "Ordenes Completas",
        backgroundColor: '#2babab',
        borderColor: '#2babab',
        data: _.pluck(data, "CountOrdersOnTime"),
        fill: false
    };

    var listOrdersDelayed = {
        label: "Ordenes Incompletas",
        backgroundColor: '#ff6384',
        borderColor: '#ff6384',
        data: _.pluck(data, "CountOrdersDelayed"),
        fill: false
    };

    datasetsData.push(listOrdersOnTime);
    datasetsData.push(listOrdersDelayed);

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(data, "TrackOutboundDate"),
            datasets: datasetsData
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Tiempo'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Ordenes"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiFillRateOrdersDispatch');
    totalKpiFillRateOrdersDispatch(data);
    configKpiSummaryFillRateOrdersDispatch(idTable, customerCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiFillRateOrdersDispatch(data) {

    var countOrdersFullDispatched = _.reduce(_.pluck(data, "CountOrdersOnTime"), function (memo, num) {
        return memo + num;
    }, 0);

    var countOrdersPartialDispatched = _.reduce(_.pluck(data, "CountOrdersDelayed"), function (memo, num) {
        return memo + num;
    }, 0);

    $("#totalFillRateOrdersDispatch .values-total:eq(0) > span").html(numeral(countOrdersFullDispatched).format('0,0'));
    $("#totalFillRateOrdersDispatch .values-total:eq(1) > span").html(numeral(countOrdersPartialDispatched).format('0,0'));
}

function configKpiSummaryFillRateOrdersDispatch(idTable, customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiFillRateOrdersDispatch(customerCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiFillRateOrdersDispatch");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        customerCode = valueSelect(customerCode);
        var indexCustomerCode = 6;
        if (customerCode != null) {
            showColumnInTable(indexCustomerCode, idTable);
            setValueColumnInTable(indexCustomerCode, $("#ctl00_MainContent_ddlCustomerFillRateOrdersDispatchFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexCustomerCode, idTable);
        }

        pagKpiFillRateOrdersDispatch(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Cumplimiento de Despachos por Orden');
    });
}

function pagKpiFillRateOrdersDispatch(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateOrdersDispatch').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateOrdersDispatchFilter").val();
            var startDate = $("#txtDateStartFillRateOrdersDispatchFilter").val();
            var endDate = $("#txtDateEndFillRateOrdersDispatchFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateOrdersDispatchFilter").val();

            customerCode = valueSelect(customerCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryFillRateOrdersDispatch("tableKpiFillRateOrdersDispatch", customerCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiFillRateDispatchOrdersDetailFacade(trackOutboundDate, pageNumber, pageSize) {
    clearPagination('pagKpiFillRateOrdersDispatchDetail');
    KpiFillRateOrdersDispatchDetail(trackOutboundDate, pageNumber, pageSize);
}

function KpiFillRateOrdersDispatchDetail(trackOutboundDate, pageNumber, pageSize) {

    var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateOrdersDispatchFilter").val();

    if (customerCode == '0') {
        customerCode = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        trackOutboundDate: trackOutboundDate,
        customerCode: customerCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiFillRateOrdersDispatchDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableFillRateOrdersDispatchDetail(data.d.Entities, trackOutboundDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Cumplimiento de Despachos por Orden");
        }
    });
}

function TableFillRateOrdersDispatchDetail(data, trackOutboundDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "TrackOutboundDate");

        var tmpl = $.templates("#tmpSummaryKpiFillRateOrdersDispatchDetail");
        var html = tmpl.render(data);
        $("#tableKpiFillRateOrdersDispatchDetail > tbody").empty().append(html);

        pagKpiFillRateOrdersDispatchDetail(countReg, trackOutboundDate);
        $("#modalKpiFillRateOrdersDispatchDetail").modal("show");
    }
}

function pagKpiFillRateOrdersDispatchDetail(countReg, trackOutboundDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateOrdersDispatchDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiFillRateOrdersDispatchDetail(trackOutboundDate, page, pageSize());
        }
    });
}

function createExcelKpiFillRateOrdersDispatch(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        Total: "Total Ordenes",
        CountOrdersOnTime: "Ordenes Completas",
        CountOrdersDelayed: "Ordenes Incompletas",
        TrackOutboundDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "TrackOutboundDate");

    var customerCode = $("#ctl00_MainContent_ddlCustomerFillRateOrdersDispatchFilter").val();
    customerCode = valueSelect(customerCode);

    if (customerCode != null) {
        headers.customerCode = "Cliente";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateOrdersDispatchFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            Total: value.TotalReg,
            CountOrdersOnTime: value.CountOrdersOnTime,
            CountOrdersDelayed: value.CountOrdersDelayed,
            TrackOutboundDate: value.TrackOutboundDate
        }

        if (customerCode != null) {
            finalObj.customerCode = $("#ctl00_MainContent_ddlCustomerFillRateOrdersDispatchFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateOrdersDispatchFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Salidas - Cumplimiento de Despachos por Orden");
}
/* fin kpi fillrate por orden */

function isFullScreenOrderStatus() {
    return $("#divSimpleListOrderStatus").closest(".flex-item").hasClass("fullScreen");
}

function setValuesFromPopUpUnitsSorted(code, description) {
    $("#ctl00_MainContent_hidItemCodeUnitsSortedFilter").val(code);
    $("#ctl00_MainContent_txtItemUnitsSortedFilter").val(description);
}

function clearTxtSearch() {
    $("#ctl00_MainContent_ucFilterItem_txtSearchValue").val('');
}

function txtItemsChange() {

    $("#ctl00_MainContent_txtItemUnitsSortedFilter").unbind("change");

    $("#ctl00_MainContent_txtItemUnitsSortedFilter").change(function (e) {
        e.preventDefault();
        if ($(this).val() == '') {
            $("#ctl00_MainContent_hidItemCodeUnitsSortedFilter").val('');
        }
    });
}

function maximizeFilter() {
    $(".maximizeFilter").click(function (e) {
        if ($(this).closest(".flex-item").find("canvas").length > 0 && $(this).closest(".flex-item").hasClass("fullScreen") == false) {
            if ($(this).attr('id') == 'maximizeKpiLpnsDispatched') {
                $("#tableKpiLpnsDispatched").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiM3Dispatched') {
                $("#tableKpiM3Dispatched").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiLpnsSorted') {
                $("#tableKpiLpnsSorted").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiUnitsSorted') {
                $("#tableKpiUnitsSorted").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiLinesSorted') {
                $("#tableKpiLinesSorted").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiFillRateDispatch') {
                $("#tableKpiFillRateDispatch").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiOrderStatus') {
                $("#tableKpiOrderStatus").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiOrderOnTime') {
                $("#tableKpiOrderOnTime").removeClass("hidden");
            }

        } else {
            $(".table-summary").addClass("hidden");
        }
    });
}