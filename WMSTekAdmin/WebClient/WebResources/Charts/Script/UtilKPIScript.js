function showMessage(message) {
    $("#ctl00_StatusContent_ucStatus_lblStatusBar").html(message);
}

function clearMessage() {
    $("#ctl00_StatusContent_ucStatus_lblStatusBar").html('');
}

function defaultMessageWhenNoData() {
    return "No se encontraron resultados para su búsqueda";
}

function DatePicker() {
    $('.datepicker').datepicker({
        format: "dd/mm/yyyy",
        autoclose: true,
        language: "es",
        todayHighlight: true,
        endDate: '0d',
    }).on('changeDate', function (e) {
        //validateDate($(e.currentTarget));
    });
}

function DatepickerMonth() {
    $(".datepickerMonth").datepicker({
        format: "mm/yyyy",
        startView: "months",
        minViewMode: "months",
        autoclose: true,
        language: "es"
    });
}

function utilSetDateGeneric(data, column, willBeDateVanillaJs) {

    $.each(data, function (index, value) {

        var d = new Date();
        var ms = parseInt(value[column].toString().replace("/Date(", "").replace(")/", ""));
        d.setTime(ms);

        if (willBeDateVanillaJs) {
            value[column] = d;
        } else {
            value[column] = moment(d).format("DD/MM/YYYY");
        }        
    });

    return data;
}

function formatDate(date) {
    var d = new Date();
    var ms = parseInt(date.toString().replace("/Date(", "").replace(")/", ""));
    d.setTime(ms);
    return moment(d).format("DD/MM/YYYY");
}

function utilSetOnlyMonth(data, column) {
    $.each(data, function (index, value) {
        value[column] = value[column].toString().substring(3);
    });

    return data;
}

function removeDivs() {
    $("#ctl00_ucMainFilterContent_divFilterWarehouse").hide();
    $("#ctl00_ucMainFilterContent_upSearch").hide();
}

function urlWSKPIs() {
    return "/App_Resource/WebClient_KPI.dll/WebClient_KPI.wsKPIs.asmx/";
}

TypeDataEnum = {
    FILTERED: -1,
    AVG: 1
}

function executeKpis() {
    $.each($(".updateIcon"), function (key, value) {
        $(this).trigger("click")
    });
}

function filterVisibility() {

    $(".minimizeFilter").click(function (e) {
        var div = $(this).parent().parent().next(".divFilter");

        if (div.hasClass("filterVisibility") == true) {
            div.removeClass("filterVisibility");
        } else {
            div.addClass("filterVisibility");
        }
    });
}

function fullScreen() {

    $(".maximizeFilter").click(function (e) {
        var div = $(this).parent().parent().parent().parent();
        
        if (div.hasClass("fullScreen") == true) {
            div.removeClass("fullScreen");
            div.find(".kpiSummary").addClass("hidden");
            div.find(".row-total").addClass("hidden");
            div.find(".showChart").addClass("hidden");
            div.find(".print").addClass("hidden");
        } else {
            div.addClass("fullScreen");
            div.find(".kpiSummary").removeClass("hidden"); 

            var canvasCount = div.find("canvas").length;
            if (canvasCount > 0) {
                div.find(".showChart").removeClass("hidden");
                div.find(".row-total").removeClass("hidden");
                div.find(".print").removeClass("hidden");
            }
        }
    });
}

function calculateHeight() {
    var body = parseInt($("body").css("height").replace("px", ""));
    var filter = parseInt($(".mainFilterPanel").css("height").replace("px", ""));
    var statusBar = parseInt($(".statusBarPanel").css("height").replace("px", ""));

    var total = body - filter - statusBar - 55;

    $(".container").css("height", total + "px");
}

function lastDayMonth(date) {
    return new moment(date, 'DD/MM/YYYY').add(1, 'M').subtract(1, 'day').format("DD/MM/YYYY");
}

function createCanvas(id) {
    var canvas =
        $('<canvas/>', {
            'id': id
        });

    return canvas;
}


function hideColumnInTable(index, idTable) {
    $("#" + idTable + " > tbody td:nth-child(" + index + ")").css("display", "none");
    $("#" + idTable + " > thead th:nth-child(" + index + ")").css("display", "none");
}

function showColumnInTable(index, idTable) {
    $("#" + idTable + " > tbody td:nth-child(" + index + ")").css("display", "");
    $("#" + idTable + " > thead th:nth-child(" + index + ")").css("display", "");
}

function setValueColumnInTable(index, value, idTable) {
    $("#" + idTable + " > tbody td:nth-child(" + index + ")").html(value);
}

function valueInput(value) {
    if (value == '') {
        value = null;
    }

    return value;
}

function valueSelect(value) {
    if (value == '0' || value == 'Seleccione' || value == '(Todos)') {
        value = null;
    }

    return value;
}

function pageSize() {
    return 10;
}

function calculatePercentage(data, dividend, divisor, columnResult) {
    $.each(data, function (i, elem) {
        if (elem[divisor] == 0) {
            elem[columnResult] = 0;
        } else {
            elem[columnResult] = ((elem[dividend] / elem[divisor]) * 100).toFixed(3);
        }
    });

    return data;
}

function clearPagination(id) {
    if ($('#' + id).data("twbs-pagination")) {
        $('#' + id).twbsPagination('destroy');
    }
}

function buttonsFilter() {
    $(".flex-container-item button").click(function (e) {
        e.preventDefault();

        var hasFullScreen = $(this).closest(".flex-item").hasClass("fullScreen");
        var canvas = $(this).closest(".flex-item").find("canvas");

        if ($(this).hasClass("showChart")) {

            if (canvas.length == 0) {
                $(this).hide();
            } else {
                $(this).show();

                var isCanvasHidden = canvas.hasClass("hidden");

                if (isCanvasHidden == true) {
                    canvas.removeClass("hidden");
                    $(this).find(".fa-eye").hide();
                    $(this).find(".fa-eye-slash").show();
                } else {
                    canvas.addClass("hidden");
                    $(this).find(".fa-eye").show();
                    $(this).find(".fa-eye-slash").hide();
                }
            }
        } else if ($(this).hasClass("maximizeFilter")) {

            if (hasFullScreen == true) {
                $(this).find(".fa-search-minus").show();
                $(this).find(".fa-search-plus").hide();
            } else {
                $(this).find(".fa-search-minus").hide();
                $(this).find(".fa-search-plus").show();
            }
        } else if ($(this).hasClass("print")) {        

            if (canvas.length > 0) {

                var image = document.getElementById(canvas.attr("id")).toDataURL("image/jpg");

                saveAs(image, nameFile(canvas.attr("id")));
            }
        }
    });
}

function nameFile(idCanvas) {
    var extension = ".jpg";
    var fileName = idCanvas.replace("chart", "");
    var random = moment(new Date()).format("DDMMYYYYHHmmss");

   
    return fileName + "-" + random + extension;
}

function saveAs(uri, filename) {
    var link = document.createElement('a');
    if (typeof link.download === 'string') {
        link.href = uri;
        link.download = filename;

        //Firefox requires the link to be in the body
        document.body.appendChild(link);

        //simulate click
        link.click();

        //remove the link when done
        document.body.removeChild(link);
    } else {
        window.open(uri);
    }
}

function showButtonShowChart(idButton, visible) {
    var button = $("#" + idButton).closest(".flex-item").find(".showChart");

    var isFullScreen = $("#" + idButton).closest(".flex-item").hasClass("fullScreen");
    if (isFullScreen == true) {
        button.removeClass("hidden");
    }
    
    var iconShow = button.find(".fa-eye"); 
    var iconHide = button.find(".fa-eye-slash"); 

    if (visible == true) {
        button.show();
    } else {
        button.hide();
    }
}

function showTotals(idButton, visible) {
    var divTotals = $("#" + idButton).closest(".flex-item").find(".row-total");
    var isFullScreen = $("#" + idButton).closest(".flex-item").hasClass("fullScreen");

    if (isFullScreen == true) {
        if (visible == true) {
            divTotals.removeClass("hidden");
        } else {
            divTotals.addClass("hidden");
        }
    } 
}

function showButtonPrint(idButton, visible) {
    var button = $("#" + idButton).closest(".flex-item").find(".print");
    var isFullScreen = $("#" + idButton).closest(".flex-item").hasClass("fullScreen");

    if (isFullScreen == true) {
        if (visible == true) {
            button.removeClass("hidden");
        } else {
            button.addClass("hidden");
        }
    } else {
        button.addClass("hidden");
    }
}

function errorAjax(message) {
    $.toast({
        heading: 'Error',
        text: 'Problemas en ' + message,
        showHideTransition: 'fade',
        icon: 'error',
        position: 'bottom-right'
    });
}

function warningMessage(message) {
    $.toast({
        heading: 'Advertencia',
        text: message,
        showHideTransition: 'fade',
        icon: 'warning',
        position: 'bottom-right'
    });
}

function GetParameterStartDate() {
    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSKPIs() + "GetParameterStartDate",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        })
    );
}

function GetParameterStartDateFacade() {
    var prom = GetParameterStartDate();

    prom.done(function (data) {

        if (data != null) {
            var finalDate = formatDate(data.d);

            $('.startDate.datepicker').val(finalDate);
            $('.startDate.datepickerMonth').val(finalDate.substring(3));
            
        } else {
            warningMessage('Fecha úlima actualización no encontrada');
        }

        //if (typeof executeKpis !== 'undefined') {
        //    executeKpis();
        //} 
    });

    prom.fail(function (data) {
        errorAjax('Obtener parámetros');
    });
}