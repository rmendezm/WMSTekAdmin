/* kpi fill rate vendor */
function KpiFillRateVendor(vendorCode, itemCode, whsCode, ownCode, pageNumber, pageSize) {

    vendorCode = valueSelect(vendorCode);
    itemCode = valueInput(itemCode);
    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

    var param = {
        vendorCode: vendorCode,
        itemCode: itemCode,
        whsCode: whsCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiFillRateVendor",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiFillRateVendor(data, idButton, vendorCode, itemCode, whsCode, ownCode) {
    var idTable = "tableKpiFillRateVendor";
    var idCanvas = "chartKpiFillRateVendor";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiFillRateVendor").hide();
        $("#btnExcelKpiFillRateVendor").addClass("hidden");
        return;
    }

    $("#divPagKpiFillRateVendor").show();
    showTotals(idButton, true);
    showButtonShowChart(idButton, true);
    $("#btnExcelKpiFillRateVendor").removeClass("hidden");

    data = utilSetDateGeneric(data, "ReceiptDate");
    data = calculatePercentage(data, "ItemQtyReceipt", "ItemQtyInbound", "FillRate");
    $("#divChartKpiFillRateVendor .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(data, "ReceiptDate"),
            datasets: [{
                label: "FillRate Proveedores",
                borderColor: 'rgb(255, 99, 132)',
                fill: false,
                data: _.pluck(data, "FillRate"),
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

    clearPagination('pagKpiFillRateVendor');
    totalKpiFillRateVendor(data);
    configKpiSummaryFillRateVendor(idTable, vendorCode, itemCode, whsCode, ownCode, 1, pageSize(), true);
}

function totalKpiFillRateVendor(data) {
    var totalItemQtyReceipt = _.reduce(_.pluck(data, "ItemQtyReceipt"), function (memo, num) {
        return memo + num;
    }, 0);

    var ItemQtyInbound = _.reduce(_.pluck(data, "ItemQtyInbound"), function (memo, num) {
        return memo + num;
    }, 0);

    $("#totalFillRate .values-total:eq(0) > span").html(numeral(totalItemQtyReceipt).format('0,0'));
    $("#totalFillRate .values-total:eq(1) > span").html(numeral(ItemQtyInbound).format('0,0'));
}

function configKpiSummaryFillRateVendor(idTable, vendorCode, itemCode, whsCode, ownCode, pageNumber, pageSize, firstTry) {

    var prom = KpiFillRateVendor(vendorCode, itemCode, whsCode, ownCode, pageNumber, pageSize);

    prom.done(function (data) {

        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = utilSetDateGeneric(data.d.Entities, "ReceiptDate");
            data = calculatePercentage(data, "ItemQtyReceipt", "ItemQtyInbound", "FillRate");
            var tmpl = $.templates("#tmpSummaryKpiFillRateVendor");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            vendorCode = valueSelect(vendorCode);
            var indexVendor = 8;
            if (vendorCode != null) {
                showColumnInTable(indexVendor, idTable);
                setValueColumnInTable(indexVendor, $("#ctl00_MainContent_ddlVendorFillRateFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexVendor, idTable);
            }

            itemCode = valueInput(itemCode);
            var indexItem = 6;
            if (itemCode != null) {    
                showColumnInTable(indexItem, idTable);
                setValueColumnInTable(indexItem, $("#ctl00_MainContent_txtItemFillRateFilter").val(), idTable);
            } else {
                hideColumnInTable(indexItem, idTable);
            }

            whsCode = valueSelect(whsCode);
            var indexWhs = 7;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWarehouseFillRateFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiFillRateVendor(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Fill Proveedores');
    });  
}

function pagKpiFillRateVendor(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateVendor').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var vendorCode = $("#ctl00_MainContent_ddlVendorFillRateFilter").val();
            var itemCode = $("#ctl00_MainContent_hidItemCodeFillRateFilter").val();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateFilter").val();

            vendorCode = valueSelect(vendorCode);
            itemCode = valueInput(itemCode);
            whsCode = valueSelect(whsCode);

            configKpiSummaryFillRateVendor("tableKpiFillRateVendor", vendorCode, itemCode, whsCode, page, pageSize(), false);
        }
    });
}

function KpiFillRateVendorDetailFacade(receiptDate, pageNumber, pageSize) {
    clearPagination('pagKpiFillRateVendorDetail');
    KpiFillRateVendorDetail(receiptDate, pageNumber, pageSize);
}

function KpiFillRateVendorDetail(receiptDate, pageNumber, pageSize) {

    var vendorCode = $("#ctl00_MainContent_ddlVendorFillRateFilter").val();
    vendorCode = valueSelect(vendorCode);

    var itemCode = $("#ctl00_MainContent_hidItemCodeFillRateFilter").val();
    itemCode = valueInput(itemCode);

    var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateFilter").val();
    whsCode = valueSelect(whsCode);

    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);

    var param = {
        receiptDate: receiptDate,
        vendorCode: vendorCode,
        itemCode: itemCode,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiFillRateVendorDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableFillRateVendorDetail(data.d.Entities, receiptDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Fill Proveedores");
        }
    });
}

function TableFillRateVendorDetail(data, receiptDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiFillRateVendorDetail");
        var html = tmpl.render(data);
        $("#tableKpiFillRateVendorDetail > tbody").empty().append(html);
        
        pagKpiFillRateVendorDetail(countReg, receiptDate);
        $("#modalKpiFillRateVendorDetail").modal("show");
    }
}

function pagKpiFillRateVendorDetail(countReg, receiptDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiFillRateVendorDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiFillRateVendorDetail(receiptDate, page, pageSize());
        }
    });
}

function createExcelKpiFillRateVendor(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        ItemQtyReceipt: "Suma Recibidos",
        ItemQtyInbound: "Suma Solicitados",
        FillRate: "Promedio",
        ReceiptDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "ReceiptDate");
    data = calculatePercentage(data, "ItemQtyReceipt", "ItemQtyInbound", "FillRate");

    var vendorCode = $("#ctl00_MainContent_ddlVendorFillRateFilter").val();
    var itemCode = $("#ctl00_MainContent_hidItemCodeFillRateFilter").val();
    var whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateFilter").val();
    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();

    vendorCode = valueSelect(vendorCode);
    itemCode = valueInput(itemCode);
    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

    if (vendorCode != null) {
        headers.vendorCode = "Proveedor";
    }

    if (itemCode != null) {
        headers.itemCode = "Item";
    }

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            ItemQtyReceipt: value.ItemQtyReceipt,
            ItemQtyInbound: value.ItemQtyInbound,
            FillRate: numeral(value.FillRate).format('0,0'), ReceiptDate: value.ReceiptDate
        }

        if (vendorCode != null) {
            finalObj.vendorCode = $("#ctl00_MainContent_ddlVendorFillRateFilter option:selected").text().trim();
        }

        if (itemCode != null) {
            finalObj.itemCode = $("#ctl00_MainContent_txtItemFillRateFilter").val();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseFillRateFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Recepción - FillRate Proveedores");
}

/* fin kpi fill rate vendor */

/* kpi rejection by vendor */
function KpiRejectionByVendor(vendorCode, whsCode, ownCode, pageNumber, pageSize) {

    vendorCode = valueSelect(vendorCode);
    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

    var param = {
        vendorCode: vendorCode,
        whsCode: whsCode,
        ownCode: ownCode,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "ChartKpiRejectionByVendor",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiRejectionByVendor(data, idButton, vendorCode, whsCode, ownCode) {
    var idTable = "tableKpiRejectionByVendor";
    var idCanvas = "chartKpiRejectionByVendor";
    $("#" + idCanvas).remove();

    if (data.length == 0) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiRejectionByVendor").hide();
        return;
    }

    $("#divPagKpiRejectionByVendor").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    var rejected = _.where(data, { Status: 'R' });
    var approved = _.where(data, { Status: 'A' });

    var ratioRejected = 0;
    if (data.length > 0) {
        ratioRejected = parseFloat(((rejected.length / data.length) * 100).toFixed(3));
    } 

    var ratioApproved = 0;
    if (data.length > 0) {
        ratioApproved = parseFloat(((approved.length / data.length) * 100).toFixed(3));
    }
    
    var finalData = [];
    finalData.push(ratioRejected);
    finalData.push(ratioApproved);

    $("#divChartKpiRejectionByVendor .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'pie',
        data: {
            datasets: [{
                data: finalData,
                backgroundColor: ['rgb(255, 99, 132)', 'rgb(21, 238, 115)']
            }],
            labels: [
                'Rechazados (%)',
                'Recepcionados (%)'
            ]
        },
        options: {
            responsive: true
        }
    });

    clearPagination('pagKpiRejectionByVendor');
    totalKpiRejectionByVendor(approved.length || 0, rejected.length || 0);
    configKpiSummaryRejectionByVendor(idTable, vendorCode, whsCode, ownCode, 1, pageSize(), true);
}

function totalKpiRejectionByVendor(approved, rejected) {
    $("#totalRejectionByVendor .values-total:eq(0) > span").html(numeral(approved).format('0,0'));
    $("#totalRejectionByVendor .values-total:eq(1) > span").html(numeral(rejected).format('0,0'));
}

function configKpiSummaryRejectionByVendor(idTable, vendorCode, whsCode, ownCode, pageNumber, pageSize, firstTry) {

    var prom = KpiRejectionByVendor(vendorCode, whsCode, ownCode, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = data.d.Entities;
            data = utilSetDateGeneric(data, "ReceiptDate");
            var tmpl = $.templates("#tmpSummaryKpiRejectionByVendor");
            var html = tmpl.render(data);
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            vendorCode = valueSelect(vendorCode);
            var indexVendor = 4;
            if (vendorCode != null) {
                showColumnInTable(indexVendor, idTable);
                setValueColumnInTable(indexVendor, $("#ctl00_MainContent_ddlVendorRejectionByVendorFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexVendor, idTable);
            }

            whsCode = valueSelect(whsCode);
            var indexWhs = 5;
            if (whsCode != null) {
                showColumnInTable(indexWhs, idTable);
                setValueColumnInTable(indexWhs, $("#ctl00_MainContent_ddlWarehouseRejectionByVendorFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexWhs, idTable);
            }

            pagKpiRejectionByVendor(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Rechazo por Proveedores');
    });  
}

function pagKpiRejectionByVendor(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiRejectionByVendor').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var vendorCode = $("#ctl00_MainContent_ddlVendorRejectionByVendorFilter").val();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseRejectionByVendorFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();

            vendorCode = valueSelect(vendorCode);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryRejectionByVendor("tableKpiRejectionByVendor", vendorCode, whsCode, ownCode, page, pageSize(), true);
        }
    });
}

function KpiRejectionByVendorDetailFacade(inboundNumber, pageNumber, pageSize) {
    clearPagination('pagKpiRejectionByVendorDetail');
    KpiRejectionByVendorDetail(inboundNumber, pageNumber, pageSize);
}

function KpiRejectionByVendorDetail(inboundNumber, pageNumber, pageSize) {

    var vendorCode = $("#ctl00_MainContent_ddlVendorRejectionByVendorFilter").val(); 
    vendorCode = valueSelect(vendorCode);

    var whsCode = $("#ctl00_MainContent_ddlWarehouseRejectionByVendorFilter").val();
    whsCode = valueSelect(whsCode);

    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);

    var param = {
        inboundNumber: inboundNumber,
        vendorCode: vendorCode,
        whsCode: whsCode,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiRejectionByVendorDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableRejectionByVendorDetail(data.d.Entities, inboundNumber);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Rechazo por Proveedores");
        }
    });
}

function TableRejectionByVendorDetail(data, inboundNumber) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiRejectionByVendorDetail");
        var html = tmpl.render(data);
        $("#tableKpiRejectionByVendorDetail > tbody").empty().append(html);

        pagKpiRejectionByVendorDetail(countReg, inboundNumber);
        $("#modalKpiRejectionByVendorDetail").modal("show");
    }
}

function pagKpiRejectionByVendorDetail(countReg, inboundNumber) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiRejectionByVendorDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiRejectionByVendorDetail(inboundNumber, page, pageSize());
        }
    });
}
/* fin kpi rejection by vendor */

/* kpi lpn received */
function KpiReceivedLPNs(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

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
            url: urlWS() + urlWSKPIs() + "ChartKpiReceivedLPNs",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiReceivedLPNs(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiReceivedLPNs";
    var idCanvas = "chartKpiReceivedLPNs";
    $("#" + idCanvas).remove();

    if (data.length == 0 || (data.length == 1 && data[0].Ratio == 0)) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiReceivedLPNs").hide();
        $("#btnExcelKpiReceivedLPNs").addClass("hidden");
        return;
    }

    $("#divPagKpiReceivedLPNs").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiReceivedLPNs").removeClass("hidden");

    data = utilSetDateGeneric(data, "ReceiptDate");
    $("#divChartKpiReceivedLPNs .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "ReceiptDate"),
            datasets: [{
                label: "LPN Recibidos",
                borderColor: 'rgb(0, 0, 255)',
                fill: false,
                data: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "Ratio"),
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
            },
            annotation: {
                annotations: [{
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: _.findWhere(data, { typeData: TypeDataEnum.AVG  }).Ratio,
                    borderColor: 'rgb(75, 192, 192)',
                    borderWidth: 4,
                    label: {
                        enabled: true,
                        content: 'Promedio ' + _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiReceivedLPNs');
    totalKpiReceivedLPNs(_.where(data, { typeData: TypeDataEnum.FILTERED }));
    configKpiSummaryReceivedLPNs(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize(), true);
}

function totalKpiReceivedLPNs(data) {
    var total = _.reduce(data, function (memo, d) {
        return memo + d.DividendCalculation;
    }, 0);
    $("#totalReceivedLPNs .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryReceivedLPNs(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize, firstTry) {

    var prom = KpiReceivedLPNs(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = utilSetDateGeneric(data.d.Entities, "ReceiptDate");
            var tmpl = $.templates("#tmpSummaryKpiReceivedLPNs");
            var html = tmpl.render(_.where(data, { typeData: TypeDataEnum.FILTERED }));
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            userName = valueSelect(userName);
            var indexUserName = 6;
            if (userName != null) {
                showColumnInTable(indexUserName, idTable);
                setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUserReceivedLPNsFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexUserName, idTable);
            }

            pagKpiReceivedLPNs(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen LPN Recibidos');
    }); 
}

function pagKpiReceivedLPNs(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedLPNs').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUserReceivedLPNsFilter").val();
            var startDate = $("#txtDateStartReceivedLPNsFilter").val();
            var endDate = $("#txtDateEndReceivedLPNsFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLpnsFilter").val();

            userName = valueSelect(userName);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryReceivedLPNs("tableKpiReceivedLPNs", userName, ownCode, whsCode, startDate, endDate, page, pageSize(), false);
        }
    });
}

function KpiFillReceivedLPNsFacade(receiptDate, pageNumber, pageSize) {
    clearPagination('pagKpiReceivedLPNsDetail');
    KpiReceivedLPNsDetail(receiptDate, pageNumber, pageSize);
}

function KpiReceivedLPNsDetail(receiptDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUserReceivedLPNsFilter").val();
    userName = valueSelect(userName);

    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);
    /// 
    var param = {
        receiptDate: receiptDate,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiReceivedLPNsDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableReceivedLPNsDetail(data.d.Entities, receiptDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle LPN Recibidos");
        }
    });
}

function TableReceivedLPNsDetail(data, receiptDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiReceivedLPNsDetail");
        var html = tmpl.render(data);
        $("#tableKpiReceivedLPNsDetail > tbody").empty().append(html);

        pagKpiReceivedLPNsDetail(countReg, receiptDate);
        $("#modalKpiReceivedLPNsDetail").modal("show");
    }
}

function pagKpiReceivedLPNsDetail(countReg, receiptDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedLPNsDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiReceivedLPNsDetail(receiptDate, page, pageSize());
        }
    });
}

function createExcelKpiReceivedLPNs(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        DividendCalculation: "Cantidad LPNs",
        DivisorCalculation: "Horas",
        Ratio: "Ratio",
        ReceiptDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "ReceiptDate");

    var userName = $("#ctl00_MainContent_ddlUserReceivedLPNsFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLpnsFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            DividendCalculation: value.DividendCalculation,
            DivisorCalculation: value.DivisorCalculation,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            ReceiptDate: value.ReceiptDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUserReceivedLPNsFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLpnsFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Recepción - LPN Recibidos");
}
/* end kpi lpn received */

/* kpi units received */
function KpiReceivedUnits(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

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
            url: urlWS() + urlWSKPIs() + "ChartKpiReceivedUnits",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiReceivedUnits(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiReceivedUnits";
    var idCanvas = "chartKpiReceivedUnits";
    $("#" + idCanvas).remove();

    if (data.length == 0 || (data.length == 1 && data[0].Ratio == 0)) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiReceivedUnits").hide();
        $("#btnExcelKpiReceivedUnits").addClass("hidden");
        return;
    }

    $("#divPagKpiReceivedUnits").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiReceivedUnits").removeClass("hidden");

    data = utilSetDateGeneric(data, "ReceiptDate");
    $("#divChartKpiReceivedUnits .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "ReceiptDate"),
            datasets: [{
                label: "Unidades Recibidas",
                borderColor: 'rgb(255, 128, 0)',
                fill: false,
                data: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "Ratio"),
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
            },
            annotation: {
                annotations: [{
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio,
                    borderColor: 'rgb(75, 192, 192)',
                    borderWidth: 4,
                    label: {
                        enabled: true,
                        content: 'Promedio ' + _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiReceivedUnits');
    totalKpiReceivedUnits(_.where(data, { typeData: TypeDataEnum.FILTERED }));
    configKpiSummaryReceivedUnits(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize(), true);
}

function totalKpiReceivedUnits(data) {
    var total = _.reduce(data, function (memo, d) {
        return memo + d.DividendCalculation;
    }, 0);

    $("#totalReceivedUnits .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryReceivedUnits(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize, firstTry) {

    var prom = KpiReceivedUnits(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = utilSetDateGeneric(data.d.Entities, "ReceiptDate");
            var tmpl = $.templates("#tmpSummaryKpiReceivedUnits");
            var html = tmpl.render(_.where(data, { typeData: TypeDataEnum.FILTERED }));
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            userName = valueSelect(userName);
            var indexUserName = 6;
            if (userName != null) {
                showColumnInTable(indexUserName, idTable);
                setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUserReceivedUnitsFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexUserName, idTable);
            }

            pagKpiReceivedUnits(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Unidades Recibidas');
    }); 
}

function pagKpiReceivedUnits(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedUnits').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUserReceivedUnitsFilter").val();
            var startDate = $("#txtDateStartReceivedUnitsFilter").val();
            var endDate = $("#txtDateEndReceivedUnitsFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedUnitsFilter").val();

            userName = valueSelect(userName);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryReceivedUnits("tableKpiReceivedUnits", userName, ownCode, whsCode, startDate, endDate, page, pageSize(), false);
        }
    });
}

function KpiFillReceivedUnitsFacade(receiptDate, pageNumber, pageSize) {
    clearPagination('pagKpiReceivedUnitsDetail');
    KpiReceivedUnitsDetail(receiptDate, pageNumber, pageSize);
}

function KpiReceivedUnitsDetail(receiptDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUserReceivedUnitsFilter").val();
    userName = valueSelect(userName);

    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);
    /// 
    var param = {
        receiptDate: receiptDate,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiReceivedUnitsDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableReceivedUnitsDetail(data.d.Entities, receiptDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Unidades Recibidas");
        }
    });
}

function TableReceivedUnitsDetail(data, receiptDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiReceivedUnitsDetail");
        var html = tmpl.render(data);
        $("#tableKpiReceivedUnitsDetail > tbody").empty().append(html);

        pagKpiReceivedUnitsDetail(countReg, receiptDate);
        $("#modalKpiReceivedUnitsDetail").modal("show");
    }
}

function pagKpiReceivedUnitsDetail(countReg, receiptDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedUnitsDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiReceivedUnitsDetail(receiptDate, page, pageSize());
        }
    });
}

function createExcelKpiReceivedUnits(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        DividendCalculation: "Cantidad Unidades",
        DivisorCalculation: "Horas",
        Ratio: "Ratio",
        ReceiptDate: "Fecha"
    };

    data = utilSetDateGeneric(data, "ReceiptDate");

    var userName = $("#ctl00_MainContent_ddlUserReceivedUnitsFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedUnitsFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            DividendCalculation: value.DividendCalculation,
            DivisorCalculation: value.DivisorCalculation,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            ReceiptDate: value.ReceiptDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUserReceivedUnitsFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedUnitsFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Recepción - Unidades Recibidas");
}
/* end kpi units received */

/* kpi lines received */
function KpiReceivedLines(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {

    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

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
            url: urlWS() + urlWSKPIs() + "ChartKpiReceivedLines",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiReceivedLines(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiReceivedLines";
    var idCanvas = "chartKpiReceivedLines";
    $("#" + idCanvas).remove();

    if (data.length == 0 || (data.length == 1 && data[0].Ratio == 0)) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiReceivedLines").hide();
        $("#btnExcelKpiReceivedLines").addClass("hidden");
        return;
    }

    $("#divPagKpiReceivedLines").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiReceivedLines").removeClass("hidden");

    data = utilSetDateGeneric(data, "ReceiptDate");
    $("#divChartKpiReceivedLines .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "ReceiptDate"),
            datasets: [{
                label: "Lineas Recibidas",
                borderColor: 'rgb(0, 255, 0)',
                fill: false,
                data: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "Ratio"),
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
            },
            annotation: {
                annotations: [{
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio,
                    borderColor: 'rgb(75, 192, 192)',
                    borderWidth: 4,
                    label: {
                        enabled: true,
                        content: 'Promedio ' + _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiReceivedLines');
    totalKpiReceivedLines(_.where(data, { typeData: TypeDataEnum.FILTERED }));
    configKpiSummaryReceivedLines(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize(), true);
}

function totalKpiReceivedLines(data) {
    var total = _.reduce(data, function (memo, d) {
        return memo + d.DividendCalculation;
    }, 0);
    $("#totalReceivedLines .values-total:eq(0) > span").html(numeral(total).format('0,0'));
}

function configKpiSummaryReceivedLines(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize, firstTry) {

    var prom = KpiReceivedLines(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = utilSetDateGeneric(data.d.Entities, "ReceiptDate");
            var tmpl = $.templates("#tmpSummaryKpiReceivedLines");
            var html = tmpl.render(_.where(data, { typeData: TypeDataEnum.FILTERED }));
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            userName = valueSelect(userName);
            var indexUserName = 6;
            if (userName != null) {
                showColumnInTable(indexUserName, idTable);
                setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUserReceivedLinesFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexUserName, idTable);
            }

            pagKpiReceivedLines(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen Lineas Recibidas');
    });
}

function pagKpiReceivedLines(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedLines').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUserReceivedLinesFilter").val();
            var startDate = $("#txtDateStartReceivedLinesFilter").val();
            var endDate = $("#txtDateEndReceivedLinesFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLinesFilter").val();

            userName = valueSelect(userName);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryReceivedLines("tableKpiReceivedLines", userName, ownCode, whsCode, startDate, endDate, page, pageSize(), false);           
        }
    });
}

function KpiFillReceivedLinesFacade(receiptDate, pageNumber, pageSize) {
    clearPagination('pagKpiReceivedLinesDetail');
    KpiReceivedLinesDetail(receiptDate, pageNumber, pageSize);
}

function KpiReceivedLinesDetail(receiptDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUserReceivedLinesFilter").val();
    userName = valueSelect(userName);

    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);

    var param = {
        receiptDate: receiptDate,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiReceivedLinesDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableReceivedLinesDetail(data.d.Entities, receiptDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle Lineas Recibidas");
        }
    });
}

function TableReceivedLinesDetail(data, receiptDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiReceivedLinesDetail");
        var html = tmpl.render(data);
        $("#tableKpiReceivedLinesDetail > tbody").empty().append(html);

        pagKpiReceivedLinesDetail(countReg, receiptDate);
        $("#modalKpiReceivedLinesDetail").modal("show");
    }
}

function pagKpiReceivedLinesDetail(countReg, receiptDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiReceivedLinesDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiReceivedLinesDetail(receiptDate, page, pageSize());
        }
    });
}

function createExcelKpiReceivedLines(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        DividendCalculation: "Cantidad Lineas",
        DivisorCalculation: "Horas",
        Ratio: "Ratio",
        ReceiptDate: "Fecha"
    };

    var userName = $("#ctl00_MainContent_ddlUserReceivedLinesFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLinesFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    data = utilSetDateGeneric(data, "ReceiptDate");

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            DividendCalculation: value.DividendCalculation,
            DivisorCalculation: value.DivisorCalculation,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            ReceiptDate: value.ReceiptDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUserReceivedLinesFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseReceivedLinesFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Recepción - Líneas Recibidas");
}
/* end kpi lines received */

/* kpi lpn stored */
function KpiStoredLPNs(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize) {
    if (userName == '0') {
        userName = null;
    }

    if (startDate == '') {
        startDate = null;
    }

    if (endDate == '') {
        endDate = null;
    }

    whsCode = valueSelect(whsCode);
    ownCode = valueSelect(ownCode);

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
            url: urlWS() + urlWSKPIs() + "ChartKpiStoredLPNs",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function createChartKpiStoredLPNs(data, idButton, userName, ownCode, whsCode, startDate, endDate) {
    var idTable = "tableKpiStoredLPNs";
    var idCanvas = "chartKpiStoredLPNs";
    $("#" + idCanvas).remove();

    if (data.length == 0 || (data.length == 1 && data[0].Ratio == 0)) {
        showMessage(defaultMessageWhenNoData());
        $("#" + idTable + " > tbody").empty();
        $("#" + idTable).addClass("hidden");
        showButtonShowChart(idButton, false);
        showTotals(idButton, false);
        $("#divPagKpiStoredLPNs").hide();
        $("#btnExcelKpiStoredLPNs").addClass("hidden");
        return;
    }

    $("#divPagKpiStoredLPNs").show();
    showButtonShowChart(idButton, true);
    showTotals(idButton, true);
    $("#btnExcelKpiStoredLPNs").removeClass("hidden");

    data = utilSetDateGeneric(data, "ReceiptDate");
    $("#divChartKpiStoredLPNs .divFilter").after(createCanvas(idCanvas));

    var ctx = document.getElementById(idCanvas).getContext('2d');

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "ReceiptDate"),
            datasets: [{
                label: "LPN Almacenados",
                borderColor: 'rgb(128, 0, 255)',
                fill: false,
                data: _.pluck(_.where(data, { typeData: TypeDataEnum.FILTERED }), "Ratio"),
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
            },
            annotation: {
                annotations: [{
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio,
                    borderColor: 'rgb(75, 192, 192)',
                    borderWidth: 4,
                    label: {
                        enabled: true,
                        content: 'Promedio ' + _.findWhere(data, { typeData: TypeDataEnum.AVG }).Ratio
                    }
                }]
            }
        }
    });

    clearPagination('pagKpiStoredLPNs');
    totalKpiStoredLPNs(_.where(data, { typeData: TypeDataEnum.FILTERED }).length || 0);
    configKpiSummaryStoredLPNs(idTable, userName, ownCode, whsCode, startDate, endDate, 1, pageSize(), true);
}

function totalKpiStoredLPNs(data) {
    $("#totalStoredLPNs .values-total:eq(0) > span").html(numeral(data).format('0,0'));
}

function configKpiSummaryStoredLPNs(idTable, userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize, firstTry) {

    var prom = KpiStoredLPNs(userName, ownCode, whsCode, startDate, endDate, pageNumber, pageSize);

    prom.done(function (data) {
        if (data.d.Entities.length > 0) {
            var countReg = data.d.Entities[0].CountReg || 0;
            data = utilSetDateGeneric(data.d.Entities, "ReceiptDate");
            var tmpl = $.templates("#tmpSummaryKpiStoredLPNs");
            var html = tmpl.render(_.where(data, { typeData: TypeDataEnum.FILTERED }));
            $("#" + idTable).removeClass("hidden");
            $("#" + idTable + " > tbody").empty().append(html);

            userName = valueSelect(userName);
            var indexUserName = 6;
            if (userName != null) {
                showColumnInTable(indexUserName, idTable);
                setValueColumnInTable(indexUserName, $("#ctl00_MainContent_ddlUserStoredLPNsFilter option:selected").text().trim(), idTable);
            } else {
                hideColumnInTable(indexUserName, idTable);
            }

            pagKpiStoredLPNs(countReg, firstTry);
        }
    });

    prom.fail(function (error) {
        errorAjax('Resumen LPN Almacenados');
    }); 

}

function pagKpiStoredLPNs(countReg, firstTry) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiStoredLPNs').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            var userName = $("#ctl00_MainContent_ddlUserStoredLPNsFilter").val();
            var startDate = $("#txtDateStartStoredLPNsFilter").val();
            var endDate = $("#txtDateEndStoredLPNsFilter").val();
            var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text();
            var whsCode = $("#ctl00_MainContent_ddlWarehouseStoredLPNsFilter").val();

            userName = valueSelect(userName);
            startDate = valueInput(startDate);
            endDate = valueInput(endDate);
            whsCode = valueSelect(whsCode);
            ownCode = valueSelect(ownCode);

            configKpiSummaryStoredLPNs("tableKpiStoredLPNs", userName, ownCode, whsCode, startDate, endDate, page, pageSize(), false);
        }
    });
}

function KpiFillStoredLPNsFacade(receiptDate, pageNumber, pageSize) {
    clearPagination('pagKpiStoredLPNsDetail');
    KpiStoredPNsDetail(receiptDate, pageNumber, pageSize);
}

function KpiStoredPNsDetail(receiptDate, pageNumber, pageSize) {

    var userName = $("#ctl00_MainContent_ddlUserStoredLPNsFilter").val();
    userName = valueSelect(userName);
    var ownName = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
    ownName = valueSelect(ownName);

    var param = {
        receiptDate: receiptDate,
        userName: userName,
        pageNumber: pageNumber,
        pageSize: pageSize,
        ownName: ownName
    };

    $.ajax({
        type: "POST",
        url: urlWS() + urlWSKPIs() + "ChartKpiStoredLPNsDetail",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            TableStoredLPNsDetail(data.d.Entities, receiptDate);
        },
        error: function (jqXHR, status, err) {
            errorAjax("Detalle LPN almacenados");
        }
    });
}

function TableStoredLPNsDetail(data, receiptDate) {

    if (data.length > 0) {
        var countReg = data[0].CountReg || 0;
        data = utilSetDateGeneric(data, "ReceiptDate");

        var tmpl = $.templates("#tmpKpiStoredLPNsDetail");
        var html = tmpl.render(data);
        $("#tableKpiStoredLPNsDetail > tbody").empty().append(html);

        pagKpiStoredLPNsDetail(countReg, receiptDate);
        $("#modalKpiStoredLPNsDetail").modal("show");
    }
}

function pagKpiStoredLPNsDetail(countReg, receiptDate) {

    if (countReg <= 0) {
        return;
    }

    var visiblePages = pageSize();
    var totalPages = Math.ceil(countReg / visiblePages);

    $('#pagKpiStoredLPNsDetail').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        initiateStartPageClick: false,
        first: 'Primer',
        prev: 'Previo',
        next: 'Siguiente',
        last: 'Ultimo',
        onPageClick: function (event, page) {
            KpiStoredPNsDetail(receiptDate, page, pageSize());
        }
    });
}

function createExcelKpiStoredLPNs(data) {

    if (data.length == 0) {
        return;
    }

    var headers = {
        DividendCalculation: "Cantidad LPNs",
        DivisorCalculation: "Horas",
        Ratio: "Ratio",
        ReceiptDate: "Fecha"
    };

    var userName = $("#ctl00_MainContent_ddlUserStoredLPNsFilter").val();
    userName = valueSelect(userName);

    if (userName != null) {
        headers.userName = "Usuario";
    }

    var ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner").val().trim();
    ownCode = valueSelect(ownCode);

    if (ownCode != null) {
        headers.ownCode = "Dueño";
    }

    var whsCode = $("#ctl00_MainContent_ddlWarehouseStoredLPNsFilter").val();
    whsCode = valueSelect(whsCode);

    if (whsCode != null) {
        headers.whsCode = "Bodega";
    }

    data = utilSetDateGeneric(data, "ReceiptDate");

    var finalData = [];
    $.each(data, function (index, value) {
        var finalObj = {
            DividendCalculation: value.DividendCalculation,
            DivisorCalculation: value.DivisorCalculation,
            Ratio: numeral(value.Ratio).format('0,0.000'),
            ReceiptDate: value.ReceiptDate
        }

        if (userName != null) {
            finalObj.userName = $("#ctl00_MainContent_ddlUserStoredLPNsFilter option:selected").text().trim();
        }

        if (ownCode != null) {
            finalObj.ownCode = $("#ctl00_ucMainFilterContent_ddlFilterOwner option:selected").text().trim();
        }

        if (whsCode != null) {
            finalObj.whsCode = $("#ctl00_MainContent_ddlWarehouseStoredLPNsFilter option:selected").text().trim();
        }

        finalData.push(finalObj);
    });

    exportCSVFile(headers, finalData, "Recepción - LPNs Almacenados");
}
/* end kpi lpn stored */

function setValuesFromPopUpFillRate(code, description) {
    $("#ctl00_MainContent_hidItemCodeFillRateFilter").val(code);
    $("#ctl00_MainContent_txtItemFillRateFilter").val(description);
}

function setValuesFromPopUpRejectionByVendor(code, description) {
    $("#ctl00_MainContent_hidItemCodeRejectionByVendor").val(code);
    $("#ctl00_MainContent_txtItemRejectionByVendor").val(description);
}

function clearTxtSearch() {
    $("#ctl00_MainContent_ucFilterItem_txtSearchValue").val('');
}

function txtItemsChange() {

    $("#ctl00_MainContent_txtItemFillRateFilter").unbind("change");
    $("#ctl00_MainContent_txtItemRejectionByVendor").unbind("change");

    $("#ctl00_MainContent_txtItemFillRateFilter").change(function (e) {
        e.preventDefault();
        if ($(this).val() == '') {
            $("#ctl00_MainContent_hidItemCodeFillRateFilter").val('');
        }
    });

    $("#ctl00_MainContent_txtItemRejectionByVendor").change(function (e) {
        e.preventDefault();
        if ($(this).val() == '') {
            $("#ctl00_MainContent_hidItemCodeRejectionByVendor").val('');
        }
    });

}

function maximizeFilter() {
    $(".maximizeFilter").click(function (e) {
        if ($(this).closest(".flex-item").find("canvas").length > 0 && $(this).closest(".flex-item").hasClass("fullScreen") == false) {
            if ($(this).attr('id') == 'maximizeKpiFillRateVendor') {
                $("#tableKpiFillRateVendor").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiRejectionByVendor') {
                $("#tableKpiRejectionByVendor").removeClass("hidden");
            } else if ($(this).attr('id') == 'maximizeKpiReceivedLPNs') {
                $("#tableKpiReceivedLPNs").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiReceivedUnits") {
                $("#tableKpiReceivedUnits").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiReceivedLines") {
                $("#tableKpiReceivedLines").removeClass("hidden");
            } else if ($(this).attr('id') == "maximizeKpiStoredLPNs") {
                $("#tableKpiStoredLPNs").removeClass("hidden");
            }

        } else {
            $(".table-summary").addClass("hidden");
        }
    });
}