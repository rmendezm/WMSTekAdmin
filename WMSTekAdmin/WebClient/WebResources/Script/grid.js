//metodos principales
function initializeGridDragAndDrop(queryName, idTable, idPage, showFooter) {

    if (!idPage) {
        idPage = null;
    }

    idTable = getIdTable(idTable);

    $(idTable + " > thead > tr > th").addClass("acceptDAD");

    //borra clase drag and drop en cabecera con checkbox que selecciona todos los checkbox de la grilla
    if ($(idTable + " > thead > tr > th > input[type='checkbox']").length > 0) {
        $(idTable + " > thead > tr > th > input[type='checkbox']").parent().removeClass("acceptDAD");
    }

    $(idTable).dragtable({ dragaccept: '.acceptDAD' }, {
        persistState: function (table) {

            persistChangeOrder(table, queryName, idPage);
        }
    });

    maxWidthHeightGrid();

    $(idTable).tableHeadFixer();

    borderRounded();
    colorHeaderGrid();

    var prom = getConfigurationColumnsGrid(queryName);

    prom.done(function (data) {

        if (data.d.Errors) {
            console.warn("queryName: " + queryName + " Mensaje: " + data.d.Errors.Message);
            hideColumnsByDB(idTable, data.d.Configuration, showFooter);         
        } else if (data.d.Configuration == null) {
            console.warn("queryName " + queryName + " null");
            $(idTable + " > thead > tr > th").removeClass("acceptDAD");
        } else {
            //Debe ir en este orden
            addFilterShowHideColumns(data.d.Configuration, idTable, queryName, idPage);
            SetDivs();
            hideColumnsByDB(idTable, data.d.Configuration, showFooter);
        }
    });

    prom.fail(function (xhr, status) {
        console.error('error getConfigurationColumnsGrid');
    });

}

function initializeGridWithNoDragAndDrop(flag) {

    borderRoundedWithNoDragAndDrop();
    colorHeaderGridWithNoDragAndDrop();
    maxWidthHeightGridNoDragAndDrop();

    removeFooterClass("table-NoDragAndDrop");

    if (flag) {
        if (flag == true) {
            $(".table-NoDragAndDrop").tableHeadFixer();
        }
    }
}

function getIdTable(idTable) {
    if (idTable.substring(0, 1) != "#") {
        idTable = "#" + idTable;
    }

    return idTable;
}

function persistChangeOrder(table, queryName, idPage) {
    var idTable = "#" + $(table.el).attr("id");
    var thDisplacedToLeft1 = $(idTable + " thead th:nth-child(" + table.startIndex + ")");
    var thDropped = $(idTable + " thead th:nth-child(" + table.endIndex + ")");
    var auxIndex;
    var objColumnModified = new ColumnModified();

    if (table.endIndex > table.startIndex) {
        auxIndex = table.endIndex - 1;
    } else {
        auxIndex = table.endIndex + 1;
    }

    var thDisplacedToLeft2 = $(idTable + " thead th:nth-child(" + auxIndex + ")");

    var checkboxes = $(idTable).closest("div.container").find("div.row:first-child input.filter-show-hide[type='checkbox']");

    if (checkboxes.length == 0) {
        console.log("No hay checkbox para guiar el ordenamiento");
        return;
    } 

    //console.log("cant checkboxes " + checkboxes.length);

    $.each(checkboxes, function (i, elem) {
        var textCheckbox = $(elem).attr("data-field-name");

        var thDisplacedToLeft1Abbr = thDisplacedToLeft1.attr("abbr");
        var thDisplacedToLeft2Abbr = thDisplacedToLeft2.attr("abbr");

        //Drag and drop de izquierda a derecha y obtener max range para actualizar
        if (table.endIndex > table.startIndex && textCheckbox == thDisplacedToLeft2Abbr) {

            //busqueda del checkbox correspondiente al head de columna seleccionada ("el que se mueve por efecto del dragg")
            var chkOrderMinRange = checkboxes.filter(function () { 
                return $(this).attr("data-field-name") == thDisplacedToLeft1Abbr;
            });

            var chkOrderMinRangeValue = $(chkOrderMinRange).attr("data-order");
            //En el caso que se deje la columna en posicion 1, va a buscar por el campo "acciones", el cual no tiene checkbox
            if (!chkOrderMinRangeValue) {
                chkOrderMinRangeValue = 1;
            }

            objColumnModified.fieldName = thDropped.attr("abbr");
            objColumnModified.fieldOrderMinRange = chkOrderMinRangeValue;
            objColumnModified.fieldOrderMaxRange = $(elem).attr("data-order");
            objColumnModified.typeDragAndDrop = TypeDragAndDropEnum.RightToLeft;

            //Drag and drop de derecha a izquierda
        } else if (table.startIndex > table.endIndex && textCheckbox == thDisplacedToLeft1Abbr) {

            //busqueda del chechbox correspondiente al head de columna seleccionado 
            var chkOrderMinRange = checkboxes.filter(function () { 
                return $(this).attr("data-field-name") == thDisplacedToLeft2Abbr;
            });

            var chkOrderMinRangeValue = $(chkOrderMinRange).attr("data-order");

            if (!chkOrderMinRangeValue) {
                console.log("no se encontro checkbox de " + thDisplacedToLeft2Abbr);
            }

            objColumnModified.fieldName = thDropped.attr("abbr");
            objColumnModified.fieldOrderMinRange = chkOrderMinRangeValue;
            objColumnModified.fieldOrderMaxRange = $(elem).attr("data-order");
            objColumnModified.typeDragAndDrop = TypeDragAndDropEnum.LeftToRight;
        } 
    });

    if (objColumnModified.fieldOrderMinRange && objColumnModified.fieldOrderMaxRange && objColumnModified.fieldName) {
        var p = updateOrderColumns(objColumnModified.fieldOrderMinRange, objColumnModified.fieldOrderMaxRange, objColumnModified.typeDragAndDrop, objColumnModified.fieldName, queryName, idPage);

        p.done(function (data) {

        });

        p.fail(function (xhr, status) {
            console.error('error updateOrderColumns');
        });
    } else {
        console.log("no hubo ordenamiento");
    }
}

function hideColumnsByDB(idTable, data, showFooter) {

    $.each($(idTable + " > thead > tr > th.acceptDAD"), function (i, elem) {

        var fieldName = $(elem).attr("abbr");
        var objSelected = _.findWhere(data, { FieldName: fieldName });

        //Por si la query en CfgEntityProperty no tiene todos los campos
        if (!objSelected) {
            return;
        }

        if (objSelected.VisibleGrid == false) {
            var index = getIndexColumn(idTable, $(elem).text());
            hideSpecificColumn(idTable, index); 
        }
    });

    if (!showFooter) {
        removeFooter(idTable);
    }
}

function removeFooter(idTable) {

    idTable = getIdTable(idTable);

    if ($(idTable + " > tbody > tr:last table").length > 0) {
        $(idTable + " > tbody > tr:last").hide();
    }
}

function removeFooterClass(idClass) {
    $.each($("." + idClass), function (i, elem) {
        removeFooter($(elem).attr("id"));
    });
}

//Agrega div de checkbox
function addFilterShowHideColumns(data, idTable, queryName, idPage) {

    if ($(idTable).closest("div.container").find("div.height-filter").length > 0) {
        $(idTable).closest("div.container").find("div.row.row-height-filter").remove();
    }

    var divRow =
    $('<div/>', {
        'class': 'row row-height-filter'
    });

    $(idTable).closest("div.container").prepend(divRow);

    var divCol12 =
    $('<div/>', {
        'class': 'col-md-12 height-filter',
    });

    divRow.append(divCol12);

    $.each($(idTable + " > thead > tr > th"), function (i, elem) {

        if (!$(elem).attr("abbr")) {
            console.warn("indice: " + i);
            console.warn(elem);
            return;
        }

        //Primera columna "Acciones" no se toma en cuenta
        if ($(elem).attr("abbr").toUpperCase() == "ACTIONS") {
            return;
        }

        var fieldName = $(elem).attr("abbr");
        var objSelected = _.findWhere(data, { FieldName: fieldName });
        
        //Por si la query en CfgEntityProperty no tiene todos los campos
        if (!objSelected) {
            console.warn("addFilterShowHideColumns fieldName " + fieldName + " objeto null en queryName " + queryName);
            return;
        }

        var label =
        $('<label/>', {
            'class': 'checkbox-inline'
        });

        divCol12.append(label);

        var checkbox;
        if (objSelected.VisibleGrid == false) {
            checkbox =
            $('<input/>', {
                'type': 'checkbox',
                'class': 'filter-show-hide',
                'data-value': i + 2,
                'data-order': objSelected.FieldOrder,
                'data-field-name': objSelected.FieldName
            });
        }
        else {
            checkbox =
            $('<input/>', {
                'type': 'checkbox',
                'class': 'filter-show-hide',
                'data-value': i + 2,
                'checked': true,
                'data-order': objSelected.FieldOrder,
                'data-field-name': objSelected.FieldName
            });
        }

        label.append(checkbox);
        label.append($(elem).html().trim());
    });

    clickFilterShowHide(idTable, queryName, idPage);
}

//Accion al hacer click en un checkbox
function clickFilterShowHide(idTable, queryName, idPage) {
    $(idTable).closest("div.container").find(".filter-show-hide").click(function () {

        var checked = $(this).is(':checked');
        var text = $(this).parent().text();
        var index = -1;

        if (checked == true) {

            index = getIndexColumn(idTable, text);

            if (index != -1) {
                showSpecificColumn(idTable, index);
            }

        } else {
            index = getIndexColumn(idTable, text);

            if (index != -1) {
                hideSpecificColumn(idTable, index);
            }
        }

        var fieldName = $(idTable + " thead th:nth-child(" + index + ")").attr("abbr");
        var p = updateVisibilityColumn(checked, fieldName, queryName, idPage);

        p.done(function (data) {

        });

        p.fail(function (xhr, status) {
            console.error('error updateVisibilityColumn');
        });

    });
}


function hideSpecificColumn(idTable, index) {
    $(idTable + " thead th:nth-child(" + index + ")").hide();
    $(idTable + " tbody td:nth-child(" + index + ")").hide();
}

function showSpecificColumn(idTable, index) {
    $(idTable + " thead th:nth-child(" + index + ")").show();
    $(idTable + " tbody td:nth-child(" + index + ")").show();
}

function maxWidthHeightGrid() {
    //solo funciona con un div padre inmediato de la tabla
    $(".table-dragAndDrop").closest('div').addClass("froze-header-grid").addClass("table-responsive");
}

function maxWidthHeightGridNoDragAndDrop() {
    //solo funciona con un div padre inmediato de la tabla
    $(".table-NoDragAndDrop").closest('div').addClass("froze-header-grid").addClass("table-responsive");
}

function borderRounded() {
    //Para tabla con bordes redondeados
    $(".table-dragAndDrop").css("border-collapse", "separate");
}

function borderRoundedWithNoDragAndDrop() {
    //Para tabla con bordes redondeados
    $(".table-NoDragAndDrop").css("border-collapse", "separate");
}

//Agrega clase para color en cabecera de grilla
function colorHeaderGrid() {
    $(".table-dragAndDrop > thead > tr > th").css("background-color", "").addClass("header-grid");
}

function colorHeaderGridWithNoDragAndDrop() {
    $.each($(".table-NoDragAndDrop"), function (i, elem) {

        if ($(elem).find("thead").length > 0) {
            $(elem).find("thead > tr > th").css("background-color", "").addClass("header-grid");
        } else {
            $(elem).find("tbody > tr:nth-child(1) > th").css("background-color", "").addClass("header-grid");
        }
    });
}

function getIndexColumn(idTable, text) {
    var index = -1;

    $.each($(idTable + " thead th"), function (i, elem) {
        if ($(elem).text() == text) {
            index = $(elem).index() + 1;
            return false;
        }
    });

    return index;
}

//Ajax
function getConfigurationColumnsGrid(nameQuery) {

    var param = {
        nameQuery: nameQuery
    };

    return Q(
    $.ajax({
        type: "POST",
        url: urlWS() + urlWSGrid() + "GetConfigurationColumnsGrid",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param)
    }));
}

function updateVisibilityColumn(visibilityGrid, fieldName, queryName, idPage) {
 
    console.log("idPage " + idPage);

    var param = {
        visibilityGrid: visibilityGrid,
        fieldName: fieldName,
        queryName: queryName,
        idPage: idPage
    };

    return Q(
    $.ajax({
        type: "POST",
        url: urlWS() + urlWSGrid() + "UpdateVisibilityColumn",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param)
    }));
}

function updateOrderColumns(fieldOrderMinRange, fieldOrderMaxRange, typeDragAndDrop, fieldName, queryName, idPage) {

    var param = {
        fieldOrderMinRange: fieldOrderMinRange,
        fieldOrderMaxRange: fieldOrderMaxRange,
        typeDragAndDrop: typeDragAndDrop,
        fieldName: fieldName,
        queryName: queryName,
        idPage: idPage
    };

    return Q(
    $.ajax({
        type: "POST",
        url: urlWS() + urlWSGrid() + "UpdateOrderColumns",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param)
    }));
}

function ColumnModified(fieldName, fieldOrderMinRange, fieldOrderMaxRange, typeDragAndDrop) {
    this.fieldName = fieldName;
    this.fieldOrderMinRange = fieldOrderMinRange;
    this.fieldOrderMaxRange = fieldOrderMaxRange;
    this.typeDragAndDrop = typeDragAndDrop;
}

TypeDragAndDropEnum = {
    RightToLeft: 1,
    LeftToRight: 2
}
