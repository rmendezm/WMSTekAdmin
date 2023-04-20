/* kpi warehouse utilization */
function KpiWarehouseUtilization(whsCode, startDate, endDate, pageNumber, pageSize) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiWarehouseUtilization",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiWarehouseUtilization(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiWarehouseUtilization";
    var idCanvas = "chartKpiWarehouseUtilization";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiWarehouseUtilization").hide();
        $("#btnExcelKpiWarehouseUtilization").addClass("hidden");
        return;
    }

    $("#divPagKpiWarehouseUtilization").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiWarehouseUtilization").removeClass("hidden");

    $("#divChartKpiWarehouseUtilization .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var finalData = [];
    var stockUsed = _.filter(data, function (row) {
        return row["IdLocCodeStock"] != null
    });
    var stockNoUsed = _.filter(data, function (row) {
        return row["IdLocCodeStock"] == null
    });

    var porcStockUsed = ((stockUsed.length / (stockUsed.length + stockNoUsed.length)) * 100).toFixed(3);
    var porcStockNoUsed = ((stockNoUsed.length / (stockUsed.length + stockNoUsed.length)) * 100).toFixed(3);

    finalData.push(porcStockUsed);
    finalData.push(porcStockNoUsed);

    var chart = new Chart(ctx, {
        type: 'pie',
        data: {
            datasets: [{
                data: finalData,
                backgroundColor: ['rgb(21, 238, 115)', 'rgb(255, 99, 132)']
            }],
            labels: [
                'Utilizados (%)',
                'No Utilizados (%)'
            ]
        },
        options: {
            responsive: true
        }
    });

    clearPagination('pagKpiWarehouseUtilization');
    totalKpiWarehouseUtilization(stockUsed.length, stockNoUsed.length);

    var prom = minAndMaxDates(whsCode, startDate, endDate);

    prom.done(function (data) {
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "MinDate");
        data = utilSetDateGeneric(data, "MaxDate");
        setMinAndMaxDates(data[0].MinDate, data[0].MaxDate);
        configKpiSummarytotalKpiWarehouseUtilization(idTable, whsCode, startDate, endDate, 1, pageSize());
    });

    prom.fail(function (error) {
        errorAjax('Obtener Fechas Utilizacion Bodega');
    });
}

function totalKpiWarehouseUtilization(stockUsed, stockNoUsed) {
    $("#totalWarehouseUtilization .values-total:eq(0) > span").html(numeral(stockUsed).format('0,0'));
    $("#totalWarehouseUtilization .values-total:eq(1) > span").html(numeral(stockNoUsed).format('0,0'));
}

function configKpiSummarytotalKpiWarehouseUtilization(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiWarehouseUtilization(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        var minAndMaxDates = getMinAndMaxDates();

        var tmpl = $.templates("#tmpSummaryKpiWarehouseUtilization");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        var indexMinDate = 4;
        showColumnInTable(indexMinDate, idTable);
        setValueColumnInTable(indexMinDate, minAndMaxDates.minDate, idTable);

        var indexMaxDate = 5;
        showColumnInTable(indexMaxDate, idTable);
        setValueColumnInTable(indexMaxDate, minAndMaxDates.maxDate, idTable);

        whsCode = valueSelect(whsCode);
        var indexWhs = 6;
        if (whsCode != null) {
            showColumnInTable(indexWhs, idTable);
            setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsWarehouseUtilizationFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexWhs, idTable);
        }

        pagKpiWarehouseUtilization(countReg);
    });

    prom.fail(function (error) {
        errorAjax('Resumen Utilizacion Bodega');
    });
}

function pagKpiWarehouseUtilization(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiWarehouseUtilization').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWhsWarehouseUtilizationFilter").val();
            var startDate = $("#txtDateStartWarehouseUtilizationFilter").val();
            var endDate = $("#txtDateEndWarehouseUtilizationFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummarytotalKpiWarehouseUtilization("tableKpiWarehouseUtilization", whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiWarehouseUtilizationDetailFacade(idLocCodeLocation, pageNumber, pageSize) {
    clearPagination('pagKpiWarehouseUtilizationDetail');
    KpiWarehouseUtilizationDetail(idLocCodeLocation, pageNumber, pageSize);
}

function KpiWarehouseUtilizationDetail(idLocCodeLocation, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsWarehouseUtilizationFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        idLocCodeLocation: idLocCodeLocation,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiWarehouseUtilizationDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableWarehouseUtilizationDetail(data.d.Entities, idLocCodeLocation);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Utilización de Bodegas");
        }
    });
}

function TableWarehouseUtilizationDetail(data, idLocCodeLocation) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiWarehouseUtilizationDetail");
        var html = tmpl.render(data);
        $("#tableKpiWarehouseUtilizationDetail > tbody").empty().append(html);

        pagKpiWarehouseUtilizationDetail(countReg, idLocCodeLocation);
        $("#modalKpiWarehouseUtilizationDetail").modal("show");
    }
}

function pagKpiWarehouseUtilizationDetail(countReg, idLocCodeLocation) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiWarehouseUtilizationDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiWarehouseUtilizationDetail(idLocCodeLocation, page, pageSize());
        }
    });
}

function minAndMaxDates(whsCode, startDate, endDate) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiWarehouseUtilizationMaxAndMinDate",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function setMinAndMaxDates(minDate, maxDate) {

    localStorage.removeItem("minDate");
    localStorage.removeItem("maxDate");
    localStorage.setItem("minDate", minDate);
    localStorage.setItem("maxDate", maxDate);
}

function getMinAndMaxDates() {

    var objDates = {};

    objDates.minDate = localStorage.getItem("minDate");
    objDates.maxDate = localStorage.getItem("maxDate");

    return objDates;
}

function createExcelKpiWarehouseUtilization(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        IdLocCodeStockUsed: "Ubicaciones Ocupadas",
        IdLocCodeStockNoUsed: "Ubicaciones Desocupadas"
    };

    var whsCode = $("#ctl00_MainContent_ddlWhsWarehouseUtilizationFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {};

        if (value.IdLocCodeStock != null) {
            finalObj.IdLocCodeStockUsed = value.IdLocCodeStock;
            finalObj.IdLocCodeStockNoUsed = " ";
        } else {
            finalObj.IdLocCodeStockUsed = " ";
            finalObj.IdLocCodeStockNoUsed = value.IdLocCodeLocation;
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsWarehouseUtilizationFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Utilización de Bodegas");
}
/* fin kpi warehouse utilization */

/* kpi stored stock */
function KpiStoredStock(whsCode, startDate, endDate, pageNumber, pageSize) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiStoredStock",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)            
        })
    );
}

function createChartKpiStoredStock(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiStoredStock";
    var idCanvas = "chartKpiStoredStock";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiStoredStock").hide();
        $("#btnExcelKpiStoredStock").addClass("hidden");
        return;
    }

    $("#divPagKpiStoredStock").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiStoredStock").removeClass("hidden");
    
    data = utilSetDateGeneric(data, "DateCreated");
    data = utilSetOnlyMonth(data, "DateCreated");
    $("#divChartKpiStoredStock .divFilter").after(createCanvas(idCanvas));
    
    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Stock Almacenado",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Price"),
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
                        labelString: "Costo Promedio ($)"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiStoredStock');
    totalKpiPriceAverage(data);
    configKpiSummaryStoredStock(idTable, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiPriceAverage(data) {
    var avg = 0;

    if (data.length > 0) {
        var total = 0
        _.pluck(data, "Price").forEach(function (price) {
            total += price
        });

        avg = total / data.length;
    } 

    $("#totalStoredStock .values-total:eq(0) > span").html(numeral(avg).format('0,0.000'));
}

function configKpiSummaryStoredStock(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiStoredStock(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        var countReg = data.d.Entities[0].CountReg || 0;
        data = data.d.Entities;
        data = utilSetDateGeneric(data, "DateCreated");
        var tmpl = $.templates("#tmpSummaryKpiStoredStock");
        var html = tmpl.render(data);
        $("#" + idTable).removeClass("hidden");
        $("#" + idTable + " > tbody").empty().append(html);

        whsCode = valueSelect(whsCode);
        var indexWhs = 4;
        if (whsCode != null) {
            showColumnInTable(indexWhs, idTable);
            setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsStoredStockFilter option:selected").text().trim(), idTable);
        } else {
            hideColumnInTable(indexWhs, idTable);
        }

        pagKpiStoredStock(countReg);

    });

    prom.fail(function (error) {
        errorAjax('Resumen Stock Almacenado');
    });
}

function pagKpiStoredStock(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiStoredStock').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateFilter").val();
            var startDate = $("#txtDateStartStoredStockFilter").val();
            var endDate = $("#txtDateEndStoredStockFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummaryStoredStock("tableKpiStoredStock", whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiStoredStockDetailFacade(dateCreated, pageNumber, pageSize) {
    clearPagination('pagKpiStoredStockDetail');
    KpiStoredStockDetail(dateCreated, pageNumber, pageSize);
}

function KpiStoredStockDetail(dateCreated, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsStoredStockFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        dateCreated: dateCreated,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiStoredStockDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableStoredStockDetail(data.d.Entities, dateCreated);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Stock Almacenado");
        }
    });
}

function TableStoredStockDetail(data, dateCreated) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiStoredStockDetail");
        var html = tmpl.render(data);
        $("#tableKpiStoredStockDetail > tbody").empty().append(html);

        pagKpiStoredStockDetail(countReg, dateCreated);
        $("#modalKpiStoredStockDetail").modal("show");
    }
}

function pagKpiStoredStockDetail(countReg, dateCreated) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiStoredStockDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiStoredStockDetail(dateCreated, page, pageSize());
        }
    });
}

function createExcelKpiStoredStock(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        Price: "Costo Promedio",
        DateCreated: "Fecha"
    };

    data = utilSetDateGeneric(data, "DateCreated");

    var whsCode = $("#ctl00_MainContent_ddlWhsStoredStockFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            Price: numeral(value.Price).format('0,0.000'),
            DateCreated: value.DateCreated
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsStoredStockFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Stock Almacenado");
}
/* fin kpi stored stock */

/* kpi sku per warehouse */
function KpiSkuPerWarehouse(whsCode, startDate, endDate, pageNumber, pageSize) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiSkuPerWarehouse",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiSkuPerWarehouse(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiSkuPerWarehouse";
    var idCanvas = "chartKpiSkuPerWarehouse";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiSkuPerWarehouse").hide();
        $("#btnExcelKpiSkuPerWarehouse").addClass("hidden");
        return;
    }

    $("#divPagKpiSkuPerWarehouse").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiSkuPerWarehouse").removeClass("hidden");

    data = utilSetDateGeneric(data, "DateCreated");
    data = utilSetOnlyMonth(data, "DateCreated");
    $("#divChartKpiSkuPerWarehouse .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Cantidad SKUs en CD",
                borderColor: 'rgb(255, 99, 132)',
                fill: false,
                data: _.pluck(data, "ItemQty"),
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
                        labelString: "Cant. SKU"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiSkuPerWarehouse');
    totalKpiItemQtySum(data);
    configKpiSummarySkuPerWarehouse(idTable, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiItemQtySum(data) {
    var total = 0

    if (data.length > 0) {      
        _.pluck(data, "ItemQty").forEach(function (price) {
            total += price
        });
    }

    $("#totalSkuPerWarehouse .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummarySkuPerWarehouse(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiSkuPerWarehouse(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = data.d.Entities;
            data = utilSetDateGeneric(data, "DateCreated");
            var tmpl = $.templates("#tmpSummaryKpiSkuPerWarehouse");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            whsCode = valueSelect(whsCode);
            var indexWhs = 4;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsSkuPerWarehouseFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiSkuPerWarehouse(countReg);
        }
    }); 

    prom.fail(function (error) {
        errorAjax('Resumen KPI Cantidad SKUs en CD');
    }); 
}

function pagKpiSkuPerWarehouse(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiSkuPerWarehouse').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWhsSkuPerWarehouseFilter").val();
            var startDate = $("#txtDateStartSkuPerWarehouseFilter").val();
            var endDate = $("#txtDateEndSkuPerWarehouseFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummarySkuPerWarehouse("tableKpiSkuPerWarehouse", whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiSkuPerWarehouseDetailFacade(dateCreated, pageNumber, pageSize) {
    clearPagination('pagKpiSkuPerWarehouseDetail');
    KpiSkuPerWarehouseDetail(dateCreated, pageNumber, pageSize);
}

function KpiSkuPerWarehouseDetail(dateCreated, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsSkuPerWarehouseFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        dateCreated: dateCreated,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiSkuPerWarehouseDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableSkuPerWarehouseDetail(data.d.Entities, dateCreated);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Cantidad SKUs en CD");
        }
    });
}

function TableSkuPerWarehouseDetail(data, dateCreated) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiSkuPerWarehouseDetail");
        var html = tmpl.render(data);
        $("#tableKpiSkuPerWarehouseDetail > tbody").empty().append(html);

        pagKpiSkuPerWarehouseDetail(countReg, dateCreated);
        $("#modalKpiSkuPerWarehouseDetail").modal("show");
    }
}

function pagKpiSkuPerWarehouseDetail(countReg, dateCreated) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiSkuPerWarehouseDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiSkuPerWarehouseDetail(dateCreated, page, pageSize());
        }
    });
}

function createExcelKpiSkuPerWarehouse(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQty: "Sumatoria Cantidad SKUs",
        DateCreated: "Fecha"
    };

    data = utilSetDateGeneric(data, "DateCreated");

    var whsCode = $("#ctl00_MainContent_ddlWhsSkuPerWarehouseFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQty: numeral(value.ItemQty).format('0,0'),
            DateCreated: value.DateCreated
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsSkuPerWarehouseFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Cantidad SKUs en CD");
}
/* fin kpi sku per warehouse */

/* kpi cubicMeters stored per warehouse */
function KpiCubicMetersStoredPerWarehouse(whsCode, startDate, endDate, pageNumber, pageSize) {
    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiCubicMetersStoredPerWarehouse",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiCubicMetersStoredPerWarehouse(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiCubicMetersStoredPerWarehouse";
    var idCanvas = "chartKpiCubicMetersStoredPerWarehouse";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiCubicMetersStoredPerWarehouse").hide();
        $("#btnExcelKpiCubicMetersStoredPerWarehouse").addClass("hidden");
        return;
    }

    $("#divPagKpiCubicMetersStoredPerWarehouse").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiCubicMetersStoredPerWarehouse").removeClass("hidden");

    data = utilSetDateGeneric(data, "DateCreated");
    data = utilSetOnlyMonth(data, "DateCreated");
    $("#divChartKpiCubicMetersStoredPerWarehouse .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Metros Cúbicos Almacenados en CD",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "M3"),
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
                        labelString: "M3"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiCubicMetersStoredPerWarehouse');
    totalKpiCubicMetersStoredPerWarehouse(data);
    configKpiSummaryCubicMetersStoredPerWarehouse(idTable, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiCubicMetersStoredPerWarehouse(data) {

    var total = 0

    if (data.length > 0) {
        _.pluck(data, "M3").forEach(function (M3) {
            total += M3
        });
    }

    $("#totalCubicMetersStoredPerWarehouse .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryCubicMetersStoredPerWarehouse(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiCubicMetersStoredPerWarehouse(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = data.d.Entities;
            data = utilSetDateGeneric(data, "DateCreated");
            data = setFormatNumber(data, "M3");
            var tmpl = $.templates("#tmpSummaryKpiCubicMetersStoredPerWarehouse");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            whsCode = valueSelect(whsCode);
            var indexWhs = 4;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsCubicMetersStoredPerWarehouseFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiCubicMetersStoredPerWarehouse(countReg);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Metros Cúbicos Almacenados en CD');
    }); 
}

function pagKpiCubicMetersStoredPerWarehouse(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiCubicMetersStoredPerWarehouse').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWhsCubicMetersStoredPerWarehouseFilter").val();
            var startDate = $("#txtDateStartCubicMetersStoredPerWarehouseFilter").val();
            var endDate = $("#txtDateEndCubicMetersStoredPerWarehouseFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummaryCubicMetersStoredPerWarehouse("tableKpiSkuPerWarehouse", whsCode, startDate, endDate, page, pageSize());
        }
    });

}

function KpiCubicMetersStoredPerWarehouseDetailFacade(dateCreated, pageNumber, pageSize) {
    clearPagination('pagKpiCubicMetersStoredPerWarehouseDetail');
    KpiCubicMetersStoredPerWarehouseDetail(dateCreated, pageNumber, pageSize);
}

function KpiCubicMetersStoredPerWarehouseDetail(dateCreated, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsCubicMetersStoredPerWarehouseFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        dateCreated: dateCreated,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiCubicMetersStoredPerWarehouseDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableCubicMetersStoredPerWarehouseDetail(data.d.Entities, dateCreated);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Metros Cúbicos Almacenados en CD");
        }
    });
}

function TableCubicMetersStoredPerWarehouseDetail(data, dateCreated) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiCubicMetersStoredPerWarehouseDetail");
        var html = tmpl.render(data);
        $("#tableKpiCubicMetersStoredPerWarehouseDetail > tbody").empty().append(html);

        pagKpiCubicMetersStoredPerWarehouseDetail(countReg, dateCreated);
        $("#modalKpiCubicMetersStoredPerWarehouseDetail").modal("show");
    }
}

function pagKpiCubicMetersStoredPerWarehouseDetail(countReg, dateCreated) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiCubicMetersStoredPerWarehouseDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiCubicMetersStoredPerWarehouseDetail(dateCreated, page, pageSize());
        }
    });
}

function createExcelKpiCubicMetersStoredPerWarehouse(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        M3: "Sumatoria M3 Almacenados",
        DateCreated: "Fecha"
    };

    data = utilSetDateGeneric(data, "DateCreated");

    var whsCode = $("#ctl00_MainContent_ddlWhsCubicMetersStoredPerWarehouseFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            M3: numeral(value.M3).format('0,0.000'),
            DateCreated: value.DateCreated
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsCubicMetersStoredPerWarehouseFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Cantidad Metros Cúbicos en CD");
}
/* fin kpi cubicMeters stored per warehouse */

/* kpi accuracy per lpn */
function KpiAccuracyPerLpn(whsCode, startDate, endDate, pageNumber, pageSize) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiAccuracyPerLpn",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiAccuracyPerLpn(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiAccuracyPerLpn";
    var idCanvas = "chartKpiAccuracyPerLpn";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiAccuracyPerLpn").hide();
        $("#btnExcelKpiAccuracyPerLpn").addClass("hidden");
        return;
    }

    $("#divPagKpiAccuracyPerLpn").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiAccuracyPerLpn").removeClass("hidden");

    data = utilSetDateGeneric(data, "DateCreated");
    data = utilSetOnlyMonth(data, "DateCreated");
    $("#divChartKpiAccuracyPerLpn .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Precisión por LPN",
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

    clearPagination('pagKpiAccuracyPerLpn');
    totalKpiAccuracyPerLpn(data);
    configKpiSummaryAccuracyPerLpn(idTable, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiAccuracyPerLpn(data) {

    var totalItemQtyMov = 0;
    var totalItemQtyInv = 0;

    if (data.length > 0) {
        (data).forEach(function (row) {
            totalItemQtyMov += row.ItemQtyMov;
            totalItemQtyInv += row.ItemQtyInv;
        });
    }

    $("#totalKpiAccuracyPerLpn .values-total:eq(0) > span").html(numeral(totalItemQtyMov).format('0,0'));
    $("#totalKpiAccuracyPerLpn .values-total:eq(1) > span").html(numeral(totalItemQtyInv).format('0,0'));
}

function configKpiSummaryAccuracyPerLpn(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiAccuracyPerLpn(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = data.d.Entities;
            data = utilSetDateGeneric(data, "DateCreated");
            var tmpl = $.templates("#tmpSummaryKpiAccuracyPerLpn");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            whsCode = valueSelect(whsCode);
            var indexWhs = 6;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsAccuracyPerLpnFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiAccuracyPerLpn(countReg);
        }
    }); 

    prom.fail(function (error) {
        errorAjax('Resumen Precisión por LPN');
    }); 
}

function pagKpiAccuracyPerLpn(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiAccuracyPerLpn').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLpnFilter").val();
            var startDate = $("#txtDateStartAccuracyPerLpnFilter").val();
            var endDate = $("#txtDateEndAccuracyPerLpnFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummaryAccuracyPerLpn("tableKpiAccuracyPerLpn", whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiAccuracyPerLpnDetailFacade(dateCreated, pageNumber, pageSize) {
    clearPagination('pagKpiAccuracyPerLpnDetail');
    KpiAccuracyPerLpnDetail(dateCreated, pageNumber, pageSize);
}

function KpiAccuracyPerLpnDetail(dateCreated, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLpnFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        dateCreated: dateCreated,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiAccuracyPerLpnDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableAccuracyPerLpnDetail(data.d.Entities, dateCreated);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Precisión por LPN");
        }
    });
}

function TableAccuracyPerLpnDetail(data, dateCreated) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiAccuracyPerLpnDetail");
        var html = tmpl.render(data);
        $("#tableKpiAccuracyPerLpnDetail > tbody").empty().append(html);

        pagKpiAccuracyPerLpnDetail(countReg, dateCreated);
        $("#modalKpiAccuracyPerLpnDetail").modal("show");
    }
}

function pagKpiAccuracyPerLpnDetail(countReg, dateCreated) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiAccuracyPerLpnDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiAccuracyPerLpnDetail(dateCreated, page, pageSize());
        }
    });
}

function createExcelKpiAccuracyPerLpn(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQtyMov: "Sumatoria Cantidad Unidades Fisicas LPN",
        ItemQtyInv: "Sumatoria Cantidad Unidades Sistematicas LPN",
        Ratio: "Ratio",
        DateCreated: "Fecha"
    };

    data = utilSetDateGeneric(data, "DateCreated");

    var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLpnFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQtyMov: value.ItemQtyMov,
            ItemQtyInv: value.ItemQtyInv,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            DateCreated: value.DateCreated
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLpnFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Precisión por LPN");
}
/* fin kpi accuracy per lpn */

/* kpi accuracy per location */
function KpiAccuracyPerLocation(whsCode, startDate, endDate, pageNumber, pageSize) {

    if (whsCode == '0') {
        whsCode = null;
    }

    if (startDate == '') {
        startDate = null;
    } else {
        startDate = '01/' + startDate;
    }

    if (endDate == '') {
        endDate = null;
    } else {
        endDate = lastDayMonth('01/' + endDate);
    }

    var param = {
        whsCode: whsCode,
        startDate: startDate,
        endDate: endDate,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiAccuracyPerLocation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiAccuracyPerLocation(data, idButton, whsCode, startDate, endDate) {
    var idTable = "tableKpiAccuracyPerLocation";
    var idCanvas = "chartKpiAccuracyPerLocation";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiAccuracyPerLocation").hide();
        $("#btnExcelKpiAccuracyPerLocation").addClass("hidden");
        return;
    }

    $("#divPagKpiAccuracyPerLocation").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiAccuracyPerLocation").removeClass("hidden");

    data = utilSetDateGeneric(data, "DateCreated");
    data = utilSetOnlyMonth(data, "DateCreated");
    $("#divChartKpiAccuracyPerLocation .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Precisión por Ubicación",
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

    clearPagination('pagKpiAccuracyPerLocation');
    totalKpiAccuracyPerLocation(data);
    configKpiSummaryAccuracyPerLocation(idTable, whsCode, startDate, endDate, 1, pageSize());
}

function totalKpiAccuracyPerLocation(data) {

    var totalItemQtyMov = 0;
    var totalItemQtyInv = 0;

    if (data.length > 0) {
        (data).forEach(function (row) {
            totalItemQtyMov += row.ItemQtyMov;
            totalItemQtyInv += row.ItemQtyInv;
        });
    }

    $("#totalKpiAccuracyPerLocation .values-total:eq(0) > span").html(numeral(totalItemQtyMov).format('0,0'));
    $("#totalKpiAccuracyPerLocation .values-total:eq(1) > span").html(numeral(totalItemQtyInv).format('0,0'));
}

function configKpiSummaryAccuracyPerLocation(idTable, whsCode, startDate, endDate, pageNumber, pageSize) {

    var prom = KpiAccuracyPerLocation(whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = data.d.Entities;
            data = utilSetDateGeneric(data, "DateCreated");
            var tmpl = $.templates("#tmpSummaryKpiAccuracyPerLocation");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            whsCode = valueSelect(whsCode);
            var indexWhs = 6;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWhsAccuracyPerLocationFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiAccuracyPerLocation(countReg);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Precisión por Ubicación');
    }); 
}

function pagKpiAccuracyPerLocation(countReg) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiAccuracyPerLocation').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLocationFilter").val();
            var startDate = $("#txtDateStartAccuracyPerLocationFilter").val();
            var endDate = $("#txtDateEndAccuracyPerLocationFilter").val();

            whsCode = valueSelect(whsCode);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);

            configKpiSummaryAccuracyPerLocation("tableKpiAccuracyPerLocation", whsCode, startDate, endDate, page, pageSize());
        }
    });
}

function KpiAccuracyPerLocationDetailFacade(dateCreated, pageNumber, pageSize) {
    clearPagination('pagKpiAccuracyPerLocationDetail');
    KpiAccuracyPerLocationDetail(dateCreated, pageNumber, pageSize);
}

function KpiAccuracyPerLocationDetail(dateCreated, pageNumber, pageSize) {

    var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLocationFilter").val();

    if (whsCode == '0') {
        whsCode = null;
    }

    var param = {
        dateCreated: dateCreated,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiAccuracyPerLocationDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableAccuracyPerLocationDetail(data.d.Entities, dateCreated);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Precisión por Ubicación");
        }
    });
}

function TableAccuracyPerLocationDetail(data, dateCreated) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "DateCreated");

        var tmpl = $.templates("#tmpSummaryKpiAccuracyPerLocationDetail");
        var html = tmpl.render(data);
        $("#tableKpiAccuracyPerLocationDetail > tbody").empty().append(html);

        pagKpiAccuracyPerLocationDetail(countReg, dateCreated);
        $("#modalKpiAccuracyPerLocationDetail").modal("show");
    }
}

function pagKpiAccuracyPerLocationDetail(countReg, dateCreated) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiAccuracyPerLocationDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiAccuracyPerLocationDetail(dateCreated, page, pageSize());
        }
    });
}

function createExcelKpiAccuracyPerLocation(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQtyMov: "Sumatoria Cantidad Unidades Fisicas Ubicación",
        ItemQtyInv: "Sumatoria Cantidad Unidades Sistematicas Ubicación",
        Ratio: "Ratio",
        DateCreated: "Fecha"
    };

    data = utilSetDateGeneric(data, "DateCreated");

    var whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLocationFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQtyMov: value.ItemQtyMov,
            ItemQtyInv: value.ItemQtyInv,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            DateCreated: value.DateCreated
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWhsAccuracyPerLocationFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Inventario - Precisión por Ubicación");
}
/* fin kpi accuracy per location */

function maximizeFilter() {
    $(".maximizeFilter").click(function (e) {
        if ($(this).closest(".flex-item").find("canvas").length > 0 && $(this).closest(".flex-item").hasClass("fullScreen") == false) {
            if ($(this).attr('id') == 'maximizeKpiWarehouseUtilization') {
                $("#tableKpiWarehouseUtilization").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiStoredStock') {
                $("#tableKpiStoredStock").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiSkuPerWarehouse') {
                $("#tableKpiSkuPerWarehouse").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiCubicMetersStoredPerWarehouse") {
                $("#tableKpiCubicMetersStoredPerWarehouse").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiAccuracyPerLpn") {
                $("#tableKpiAccuracyPerLpn").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiAccuracyPerLocation") {
                $("#tableKpiAccuracyPerLocation").removeClass("hidden");
            }

        } else {
            $(".table-summary").addClass("hidden");
        }
    });
}

function setFormatNumber(data, nameColumn) {

    _.pluck(data, nameColumn).forEach(function (value, i) {
        data[i][nameColumn] = numeral(value).format('0.000')
    });

    return data;
}