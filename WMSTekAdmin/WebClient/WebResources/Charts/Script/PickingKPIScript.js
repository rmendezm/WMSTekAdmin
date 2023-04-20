/* inicio kpi lpns productivity */
function KpiLpnsProductivity(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

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
            url: urlWS() + urlWSKPIs() + "ChartKpiLpnsProductivity",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiLpnsProductivity(data, idButton, userName, idLpnCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiLpnsProductivity";
    var idCanvas = "chartKpiLpnsProductivity";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiLpnsProductivity").hide();
        showButtonPrint(idButton, false);
        $("#btnExcelKpiLpnsProductivity").addClass("hidden");
        return;
    }

    showButtonPrint(idButton, true);
    $("#divPagKpiLpnsProductivity").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiLpnsProductivity").removeClass("hidden");

    data = utilSetDateGeneric(data, "EndTime");
    $("#divChartKpiLpnsProductivity .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "EndTime"),
            datasets: [{
                label: "Productividad LPNs",
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

    clearPagination('pagKpiLpnsProductivity');
    totalKpiLpnsProductivity(data);
    configKpiSummaryLpnsProductivity(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiLpnsProductivity(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (price) {
            total += price
        });
    }

    $("#totalLpnsProductivity .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryLpnsProductivity(idTable, userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiLpnsProductivity(userName, idLpnCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "EndTime");
        var tmpl = $.templates("#tmpSummaryKpiLpnsProductivity");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersLpnsProductivityFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiLpnsProductivity(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Productividad LPNs');
    });
}

function pagKpiLpnsProductivity(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsProductivity').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersLpnsProductivityFilter").val();
            var idLpnCode = $("#txtLpnLpnsProductivityFilter").val();
            var startDate = $("#txtDateStartLpnsProductivityFilter").val();
            var endDate = $("#txtDateEndLpnsProductivityFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsProductivityFilter").val();

            userName = valueSelect(userName);
            idLpnCode = valueInput(idLpnCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryLpnsProductivity("tableKpiLpnsProductivity", userName, idLpnCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiLpnsProductivityDetailFacade(endTime, pageNumber, pageSize) {
    clearPagination('pagKpiLpnsProductivityDetail');
    KpiLpnsProductivityDetail(endTime, pageNumber, pageSize);
}

function KpiLpnsProductivityDetail(endTime, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersLpnsProductivityFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var idLpnCode = $("#txtLpnLpnsProductivityFilter").val();

    if (idLpnCode == '') {
        idLpnCode = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        endTime: endTime,
        userName: userName,
        idLpnCode: idLpnCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiLpnsProductivityDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableLpnsProductivityDetail(data.d.Entities, endTime);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Productividad LPNs");
        }
    });
}

function TableLpnsProductivityDetail(data, endTime) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "EndTime");

        var tmpl = $.templates("#tmpSummaryKpiLpnsProductivityDetail");
        var html = tmpl.render(data);
        $("#tableKpiLpnsProductivityDetail > tbody").empty().append(html);

        pagKpiLpnsProductivityDetail(countReg, endTime);
        $("#modalKpiLpnsProductivityDetail").modal("show");
    }
}

function pagKpiLpnsProductivityDetail(countReg, endTime) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLpnsProductivityDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiLpnsProductivityDetail(endTime, page, pageSize());
        }
    });
}

function createExcelKpiLpnsProductivity(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: 'Cantidad de LPNs', 
        WorkHours: "Horas",
        Ratio: "Ratio",
        EndTime: "Fecha"
    };

    var userName = $("#ctl00_MainContent_ddlUsersLpnsProductivityFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseLpnsProductivityFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    data = utilSetDateGeneric(data, "EndTime");

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            EndTime: value.EndTime
        }

        finalObj.ownCode = ownCode;

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlWarehouseLpnsProductivityFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLpnsFilter option:selected").text().trim();
        }

        finalData.push(finalObj);  
    });

    exportCSVFile(headers, finalData, "Picking - Productividad LPNs");
}

/* fin kpi lpns productivity */

/* inicio kpi units productivity */
function KpiUnitsProductivity(userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (itemCode == '0') {
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
            url: urlWS() + urlWSKPIs() + "ChartKpiUnitsProductivity",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiUnitsProductivity(data, idButton, userName, itemCode, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiUnitsProductivity";
    var idCanvas = "chartKpiUnitsProductivity";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiUnitsProductivity").hide();
        showButtonPrint(idButton, false);
        $("#btnExcelKpiUnitsProductivity").addClass("hidden");
        return;
    }

    showButtonPrint(idButton, true);
    $("#divPagKpiUnitsProductivity").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiUnitsProductivity").removeClass("hidden");

    data = utilSetDateGeneric(data, "EndTime");
    $("#divChartKpiUnitsProductivity .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "EndTime"),
            datasets: [{
                label: "Productividad Unidades",
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

    clearPagination('pagKpiUnitsProductivity');
    totalKpiUnitsProductivity(data);
    configKpiSummaryUnitsProductivity(idTable, userName, itemCode, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiUnitsProductivity(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (price) {
            total += price
        });
    }

    $("#totalUnitsProductivity .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryUnitsProductivity(idTable, userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiUnitsProductivity(userName, itemCode, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "EndTime");
        var tmpl = $.templates("#tmpSummaryKpiUnitsProductivity");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersUnitsProductivityFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiUnitsProductivity(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Productividad Unidades');
    });
}

function pagKpiUnitsProductivity(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiUnitsProductivity').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersUnitsProductivityFilter").val();
            var itemCode = $("#ctl00_MainContent_ddlItemUnitsProductivityFilter").val();
            var startDate = $("#txtDateStartLpnsProductivityFilter").val();
            var endDate = $("#txtDateEndLpnsProductivityFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsProductivityFilter").val();

            userName = valueSelect(userName);
            itemCode = valueSelect(itemCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryLpnsProductivity("tableKpiUnitsProductivity", userName, itemCode, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiUnitsProductivityDetailFacade(endTime, pageNumber, pageSize) {
    clearPagination('pagKpiUnitsProductivityDetail');
    KpiUnitsProductivityDetail(endTime, pageNumber, pageSize);
}

function KpiUnitsProductivityDetail(endTime, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersUnitsProductivityFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var itemCode = $("#ctl00_MainContent_ddlItemUnitsProductivityFilter").val();

    if (itemCode == '0') {
        itemCode = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        endTime: endTime,
        userName: userName,
        itemCode: itemCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiUnitsProductivityDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableUnitsProductivityDetail(data.d.Entities, endTime);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Productividad Unidades");
        }
    });
}

function TableUnitsProductivityDetail(data, endTime) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "EndTime");

        var tmpl = $.templates("#tmpSummaryKpiUnitsProductivityDetail");
        var html = tmpl.render(data);
        $("#tableKpiUnitsProductivityDetail > tbody").empty().append(html);

        pagKpiUnitsProductivityDetail(countReg, endTime);
        $("#modalKpiUnitsProductivityDetail").modal("show");
    }
}

function pagKpiUnitsProductivityDetail(countReg, endTime) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiUnitsProductivityDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiUnitsProductivityDetail(endTime, page, pageSize());
        }
    });
}

function createExcelKpiUnitsProductivity(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: 'Cantidad de Unidades',
        WorkHours: "Horas",
        Ratio: "Ratio",
        EndTime: "Fecha"
    };

    data = utilSetDateGeneric(data, "EndTime");

    var userName = $("#ctl00_MainContent_ddlUsersUnitsProductivityFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsProductivityFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            TotalReg: value.TotalReg,
            WorkHours: value.WorkHours,
            Ratio: numeral(value.Ratio).format('0,0.000'), EndTime:
            value.EndTime
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUsersUnitsProductivityFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseUnitsProductivityFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Picking - Productividad Unidades");
}
/* fin kpi units productivity */

/* inicio kpi lines productivity */
function KpiLinesProductivity(userName, line, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (line == '') {
        line = null;
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
        line: line,
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
            url: urlWS() + urlWSKPIs() + "ChartKpiLinesProductivity",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiLinesProductivity(data, idButton, userName, line, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiLinesProductivity";
    var idCanvas = "chartKpiLinesProductivity";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiLinesProductivity").hide();
        showButtonPrint(idButton, false);
        $("#btnExcelKpiLinesProductivity").addClass("hidden");
        return;
    }

    showButtonPrint(idButton, true);
    $("#divPagKpiLinesProductivity").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiLinesProductivity").removeClass("hidden");

    data = utilSetDateGeneric(data, "EndTime");
    $("#divChartKpiLinesProductivity .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "EndTime"),
            datasets: [{
                label: "Productividad Lineas",
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

    clearPagination('pagKpiLinesProductivity');
    totalKpiLinesProductivity(data);
    configKpiSummaryLinesProductivity(idTable, userName, line, ownCode, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiLinesProductivity(data) {
    var total = 0

    if (data.length > 0) {
        _.pluck(data, "TotalReg").forEach(function (price) {
            total += price
        });
    }

    $("#totalLinesProductivity .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryLinesProductivity(idTable, userName, line, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiLinesProductivity(userName, line, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "EndTime");
        var tmpl = $.templates("#tmpSummaryKpiLinesProductivity");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        userName = valueSelect(userName);
        var indexUserName = 6;
        if (userName != null) {
            showColumnInTable(indexUserName, idTable);
            setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUsersLinesProductivityFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexUserName, idTable);
        }

        pagKpiLinesProductivity(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Productividad Lineas');
    });
}

function pagKpiLinesProductivity(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLinesProductivity').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUsersLinesProductivityFilter").val();
            var line = $("#txtLineLinesProductivityFilter").val();
            var startDate = $("#txtDateStartLinesProductivityFilter").val();
            var endDate = $("#txtDateEndLinesProductivityFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseLinesProductivityFilter").val();

            userName = valueSelect(userName);
            line = valueInput(line);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            ownCode = valueSelect(ownCode);
            whsCode = valueSelect(whsCode)

            configKpiSummaryLinesProductivity("tableKpiLinesProductivity", userName, line, ownCode, whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiLinesProductivityDetailFacade(endTime, pageNumber, pageSize) {
    clearPagination('pagKpiLinesProductivityDetail');
    KpiLinesProductivityDetail(endTime, pageNumber, pageSize);
}

function KpiLinesProductivityDetail(endTime, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUsersLinesProductivityFilter").val();

    if (userName == '0') {
        userName = null;
    }

    var line = $("#txtLineLinesProductivityFilter").val();

    if (line == '') {
        line = null;
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
    ownCode = valueSelect(ownCode);

    var param = {
        endTime: endTime,
        userName: userName,
        line: line,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiLinesProductivityDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableLinesProductivityDetail(data.d.Entities, endTime);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Productividad Lineas");
        }
    });
}

function TableLinesProductivityDetail(data, endTime) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "EndTime");

        var tmpl = $.templates("#tmpSummaryKpiLinesProductivityDetail");
        var html = tmpl.render(data);
        $("#tableKpiLinesProductivityDetail > tbody").empty().append(html);

        pagKpiLinesProductivityDetail(countReg, endTime);
        $("#modalKpiLinesProductivityDetail").modal("show");
    }
}

function pagKpiLinesProductivityDetail(countReg, endTime) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiLinesProductivityDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiLinesProductivityDetail(endTime, page, pageSize());
        }
    });
}

function createExcelKpiLinesProductivity(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        TotalReg: 'Cantidad de Lineas',
        WorkHours: "Horas",
        Ratio: "Ratio",
        EndTime: "Fecha"
    };

    data = utilSetDateGeneric(data, "EndTime");

    var userName = $("#ctl00_MainContent_ddlUsersLinesProductivityFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseLinesProductivityFilter").val();
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
            EndTime: value.EndTime
        }

        if (userName != null) {
            headers.userName = $("#ctl00_MainContent_ddlUsersLinesProductivityFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseLinesProductivityFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Picking - Productividad Lineas");
}
/* fin kpi lines productivity */

function maximizeFilter() {
    $(".maximizeFilter").click(function (e) {
        if ($(this).closest(".flex-item").find("canvas").length > 0 && $(this).closest(".flex-item").hasClass("fullScreen") == false) {
            if ($(this).attr('id') == 'maximizeKpiLpnsProductivity') {
                $("#tableKpiLpnsProductivity").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiUnitsProductivity') {
                $("#tableKpiUnitsProductivity").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiLinesProductivity') {
                $("#tableKpiLinesProductivity").removeClass("hidden");
            } 

        } else {
            $(".table-summary").addClass("hidden");
        }
    });
}