/* MAPA BODEGA 2D 
  ===============
  
  Sistema de Coordenadas:
  ----------------------
  
  
(x,y)       WIDTH (x)
  o-------------------------
  |                         |
  |   HANGAR / LOCATION     | LENGHT (y)
  |                         |
  |                         |
   -------------------------\
   \                         \ HEIGHT (z)
    \                         \ 
     --------------------------
   
   
 (x,y)      WIDTH (x)
  o-------------------------
  |                         |
  |       MAP (FONDO)       |   HEIGHT (y)
  |                         |
  |                         |
   -------------------------

*/

var scale = 1;
var scalePreview = 1;
var currentDiv;
var currentColor;
var currentBorderColor;
var itemCode = '';
var hasItemCode = false;
var hasItemName = false;

//Nuevas Variables 22-07-2015
var hasMapFabricationDate = false;
var hasMapExpirationDate = false;
var hasMapFifoDate = false;
var hasMapLote = false;
var hasMapLPN = false;
var hasMapHoldLocation = false;
var hasMapCategory = false;

var firstClickEverOverDiv = false;
var lastIdDivClicked = null;

function GetLayoutMapClient(index) {

    if (index > 0) {
        tt_HideInit();
        ShowLockScreen();
        PageMethods.GetLayoutMap(index, OnGetLayoutMapClient);
    }
}
function ShowLockScreen() {
    var dis = document.getElementById("ctl00_MainContent_divBloqueoPantalla");
    dis.style.display = 'block';
}

function HideLockScreen() {
    var dis = document.getElementById("ctl00_MainContent_divBloqueoPantalla");
   dis.style.display = 'none';
}


function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}

function GetPreview() 
{
    GetMapPreview();
    GetColPreview();
}

function GetColPreview() 
{
    // Limpia dibujo anterior
    document.getElementById("divColPreview").innerHTML = '';

    // Column Detail Preview area
    previewWidth = document.getElementById("divColPreview").offsetWidth;
    previewHeight = document.getElementById("divColPreview").offsetHeight;

    // Border
    border = document.getElementById("ctl00_MainContent_tabMap2D_tabCol_txtColumnDetailBorder").value;
    borderColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpColumnDetailBorderColor")[0].value;
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpColumnDetailBackColor")[0].value;
    titleHeight = 18;

    drawRectangle(0, 0, previewWidth, previewHeight, borderColor, 0, '', "divColPreview", ' Fila: 2 Col: 3  Niveles: 5', 'left', 11, 'white', true, 2);

    // Title
    drawRectangle(210, 2, 18, 15, '#990000', 0, '', "divColPreview", 'X', 'center', 10, 'white', true, 2);
  
    // Background
    drawRectangle(border, Number(border) + titleHeight, previewWidth - (border * 2), previewHeight - (border * 2 + titleHeight), backColor, 0, '', "divColPreview", '');

    // Location details
    padding = document.getElementById("ctl00_MainContent_tabMap2D_tabCol_txtColumnDetailPadding").value;
    locPadding = document.getElementById("ctl00_MainContent_tabMap2D_tabCol_txtLocationDetailPadding").value;
    locBorder = document.getElementById("ctl00_MainContent_tabMap2D_tabCol_txtLocationDetailBorder").value;
    locBorderColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationDetailBorderColor")[0].value;
    locWidth = previewWidth - ((Number(border) + Number(padding) + Number(locBorder)) * 2);
    locHeight = 40;
    locSpace = 2;
    
    // Level 1
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationBackColor1")[0].value;
    drawRectangle(Number(border) + Number(padding), Number(border) + titleHeight + Number(padding) + 4 * (locHeight + locSpace), locWidth, locHeight - (locBorder * 2) - padding / 4, backColor, locBorder, locBorderColor, "divColPreview", 'Nivel 1', 'right', 11, 'black', false, locPadding);

    // Level 2
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationBackColor2")[0].value;
    drawRectangle(Number(border) + Number(padding), Number(border) + titleHeight + Number(padding) + 3 * (locHeight + locSpace), locWidth, locHeight - (locBorder * 2) - padding / 4, backColor, locBorder, locBorderColor, "divColPreview", 'Nivel 2', 'right', 11, 'black', false, locPadding);

    // Level 3
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationBackColor3")[0].value;
    drawRectangle(Number(border) + Number(padding), Number(border) + titleHeight + Number(padding) + 2 * (locHeight + locSpace), locWidth, locHeight - (locBorder * 2) - padding / 4, backColor, locBorder, locBorderColor, "divColPreview", 'Nivel 3', 'right', 11, 'black', false, locPadding);

    // Level 4
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationBackColor4")[0].value;
    drawRectangle(Number(border) + Number(padding), Number(border) + titleHeight + Number(padding) + (locHeight + locSpace), locWidth, locHeight - (locBorder * 2) - padding / 4, backColor, locBorder, locBorderColor, "divColPreview", 'Nivel 4', 'right', 11, 'black', false, locPadding);

    // Level 5
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabCol$cpLocationBackColor5")[0].value;
    drawRectangle(Number(border) + Number(padding), Number(border) + titleHeight + Number(padding), locWidth, locHeight - (locBorder * 2) - padding / 4, backColor, locBorder, locBorderColor, "divColPreview", 'Nivel 5', 'right', 11, 'black', false, locPadding);

}
    
function GetMapPreview() 
{
    // Limpia dibujo anterior
    document.getElementById("divMapPreview").innerHTML = '';

    // Map Preview area
    previewWidth = document.getElementById("divMapPreview").offsetWidth;
    previewHeight = document.getElementById("divMapPreview").offsetHeight;

    // Map
    margin = document.getElementById("ctl00_MainContent_tabMap2D_tabMap_txtMargin").value;
    backColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpBackColor")[0].value;
    
    drawRectangle(0, 0, previewWidth, previewHeight, backColor, 0, '', "divMapPreview", '');

    // Hangar
    hangarBackColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpHangarBackColor")[0].value;
    hangarBorder = document.getElementById("ctl00_MainContent_tabMap2D_tabMap_txtHangarBorder").value;
    hangarBorderColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpHangarBorderColor")[0].value;

    drawRectangle(margin, margin, previewWidth - (margin * 2 + hangarBorder * 2), previewHeight - (margin * 2 + hangarBorder * 2), hangarBackColor, hangarBorder, hangarBorderColor, "divMapPreview", '');

    // Calcula la escala a utilizar
    defaultMargin = 20;
    scalePreview = computeScale(previewWidth, previewHeight, margin, previewWidth - (defaultMargin * 2 + hangarBorder * 2), previewHeight - (defaultMargin * 2 + hangarBorder * 2), hangarBorder);

    // Columns
    colWidth = 41 * scalePreview;
    colHeight = 30 * scalePreview;
    colMargin = Number(margin) + 15;
    aisleWidth = 40;

    colBackColor5 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor5")[0].value;
    colBackColor4 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor4")[0].value;
    colBackColor3 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor3")[0].value;
    colBackColor2 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor2")[0].value;
    colBackColor1 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor1")[0].value;
    colBackColor0 = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColor0")[0].value;

    colBorder = document.getElementById("ctl00_MainContent_tabMap2D_tabMap_txtColumnBorder").value;
    colBorderColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBorderColor")[0].value;

    colActiveColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColorActive")[0].value;
    colActiveBorderColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBorderColorActive")[0].value;

    colItemColor = document.getElementsByName("ctl00$MainContent$tabMap2D$tabMap$cpColumnBackColorItem")[0].value;
    
    // Row 1
    drawRectangleLocPreview(colMargin, colMargin, colWidth, colHeight, colBackColor3, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth, colMargin, colWidth, colHeight, colBackColor5, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 2, colMargin, colWidth, colHeight, colBackColor4, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 3, colMargin, colWidth, colHeight, colItemColor, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);

    // Row 2
    drawRectangleLocPreview(colMargin, colMargin + colHeight, colWidth, colHeight, colBackColor1, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth, colMargin + colHeight, colWidth, colHeight, colBackColor2, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 2, colMargin + colHeight, colWidth, colHeight, colBackColor0, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 3, colMargin + colHeight, colWidth, colHeight, colBackColor2, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);

    // Row 3
    drawRectangleLocPreview(colMargin, colMargin + colHeight * 2 + aisleWidth, colWidth, colHeight, colBackColor5, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth, colMargin + colHeight * 2 + aisleWidth, colWidth, colHeight, colItemColor, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 2, colMargin + colHeight * 2 + aisleWidth, colWidth, colHeight, colBackColor3, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 3, colMargin + colHeight * 2 + aisleWidth, colWidth, colHeight, colBackColor4, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);

    // Row 4
    drawRectangleLocPreview(colMargin, colMargin + colHeight * 3 + aisleWidth, colWidth, colHeight, colBackColor0, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth, colMargin + colHeight * 3 + aisleWidth, colWidth, colHeight, colBackColor1, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 2, colMargin + colHeight * 3 + aisleWidth, colWidth, colHeight, colBackColor0, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);
    drawRectangleLocPreview(colMargin + colWidth * 3, colMargin + colHeight * 3 + aisleWidth, colWidth, colHeight, colItemColor, colActiveColor, colBorder, colBorderColor, colActiveBorderColor);

}

function OnGetLayoutMapClient(map) {

    var scaledBorder;

    currentDiv = null;

    // Recupera valores del filtro
    itemCode = document.getElementById("ctl00_ucMainFilterContent_txtFilterItem").value;
    itemName = document.getElementById("ctl00_ucMainFilterContent_txtFilterName").value;

    //Nuevo parametros de busqueda en el Mapa 22-07-2015
    //    mapFabricationDate = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapFabricationDate").value.trim();
    //    mapExpirationDate = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapExpirationDate").value.trim();
    //    mapFifoDate = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapFifoDate").value.trim();
    mapFabricationDate = $find("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_calMapFabricationDate")._selectedDate;
    mapExpirationDate = $find("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_calMapExpirationDate")._selectedDate;
    mapFifoDate = $find("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_calMapFifoDate")._selectedDate;
    mapLote = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapLote").value.trim();
    mapLPN = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapLPN").value.trim();
    mapHoldLocation = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_chkMapHoldLocation").checked;
    mapCategory = document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapCategory").value.trim();
    //formatDate = $find("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_calMapFabricationDate")._format;
    formatDate = "dd/MM/yyyy";
    mapFilterAdvanced = document.getElementById("ctl00_ucMainFilterContent_chkUseAdvancedFilter").checked;

    // Limpia dibujo anterior
    document.getElementById("ctl00_MainContent_divCanvas").innerHTML = '';
    document.getElementById("ctl00_MainContent_divCanvas").style.visibility = 'hidden';

    var h = (document.body.clientHeight - 40) + "px";
    var w = document.body.clientWidth + "px";
    document.getElementById("ctl00_MainContent_divCanvas").style.height = h;
    document.getElementById("ctl00_MainContent_divCanvas").style.width = w;
    
    // Calcula el tamaño del mapa
    map.Width = document.getElementById("ctl00_MainContent_divCanvas").offsetWidth;
    //map.Height = document.getElementById("divCanvas").offsetHeight - 67; // 67 = altura de la barra de tareas
    map.Height = (document.body.clientHeight - 70);

    // Calcula la escala a utilizar
    scale = computeScale(map.Width, map.Height, map.Margin, map.Hangar.Width, map.Hangar.Length, map.HangarBorder);


    // DIBUJA FONDO
    drawRectangle(0, 0, map.Width, map.Height, map.BackColor, 0, '', "ctl00_MainContent_divCanvas", '');


    // DIBUJA HANGAR
    scaledBorder = computeBorder(map.HangarBorder * scale);

    drawRectangle(map.Margin, map.Margin, map.Hangar.Width * scale - (scaledBorder * 2), map.Hangar.Length * scale - (scaledBorder * 2), map.HangarBackColor, scaledBorder, map.HangarBorderColor, "ctl00_MainContent_divCanvas", '');

    // DIBUJA UBICACIONES
    if (map.Hangar.Columns != null) {

        var defaultColor = null;
        if (IsCheckedLocationsMostUsedByItem()) {
            defaultColor = "#FFFFFF";
        }

        for (var i = 0; i < map.Hangar.Columns.length; i++) {

            scaledBorder = computeBorder(map.ColumnBorder * scale);

            locX = map.Margin + (map.Hangar.Columns[i].PositionX * scale) - scaledBorder;
            locY = map.Margin + (map.Hangar.Columns[i].PositionY * scale) - scaledBorder;

            drawRectangleLocation(
                                locX,                                           // Position X
                                locY,                                           // Position Y
                                map.Hangar.Columns[i].Width * scale,            // Width
                                map.Hangar.Columns[i].Length * scale,           // Height
                                defaultColor != null ? defaultColor : map.ColumnBackColor, // Background Color
                                map.ColumnBackColorActive,                      // Background Color (Mouse Over)
                                defaultColor != null ? defaultColor : map.ColumnBackColorItem, // Background Color (Item)
                                scaledBorder,                                   // Border
                                map.ColumnBorderColor,                          // Border Color
                                map.ColumnBorderColorActive,                    // Border Color (Mouse Over)
                                map.Hangar.Columns[i],                          // Location Details
                                map.ColumnDetail,                               // Column Detail Attributes
                                map.LocationDetail);                            // Location Detail Attributes

                     
        }
    }

    paintLocationsMostUsed(map);

    document.getElementById("ctl00_MainContent_divCanvas").style.visibility = 'visible';
    HideLockScreen();
}

function IsCheckedLocationsMostUsedByItem() {
    return $("#ctl00_ucMainFilterContent_chkLocationsMostUsedByItem").is(':checked');
}

function paintLocationsMostUsed(map) {

    var locations = map.LocationsMostUsedByItem;

    var isCheckedLocationsMostUsedByItem = IsCheckedLocationsMostUsedByItem();

    if (!isCheckedLocationsMostUsedByItem) {
        return;
    }

    if (locations) {
        if (locations.length > 0) {

            for (var i = 0; i < locations.length; i++) {
                var xPos = locations[i].LocSource.PositionX;
                var yPos = locations[i].LocSource.PositionY;

                var locationDiv = $('div[position-x=' + xPos + '][position-y=' + yPos + ']');
                var color = getColorByRange(locations[i].Range, map);

                if (locationDiv.length > 0) {
                    $(locationDiv).css({
                        "background-color": color
                    });

                    $(locationDiv).attr("most-used-location-color", color);
                }
                else {
                    console.warn("ubicacion " + locations[i].IdLocCodeSource + " no encontrada");
                }           
            }
        } else {

            if (isCheckedLocationsMostUsedByItem) {
                warningMessage('No se encontraron ubicaciones');
            }  
        }
    } else {

        if (isCheckedLocationsMostUsedByItem) {
            warningMessage('No se encontraron ubicaciones');
        }  
    }
}

function getColorByRange(range, map) {

    var color = '';

    switch (range.toLowerCase()) {
        case 'low':
            color = map.LowUsedLocation;
            break;
        case 'normallow':
            color = map.NormalLowUsedLocation;
            break;
        case 'normal':
            color = map.NormalUsedLocation;
            break;
        case 'normalhigh':
            color = map.NormalHighUsedLocation;
            break;
        case 'high':
            color = map.HighUsedLocation;
            break;
    }

    return color;
}


function ClearCanvas() {
    document.getElementById("ctl00_MainContent_divCanvas").innerHTML = '';

    var divText = document.createElement("div");
    divText.style.fontSize = "small";
    divText.style.height = "20px";
    divText.style.width = "800px";
    divText.style.color = "#993300";
    divText.style.backgroundColor = "#FFCC99";
    divText.style.position = "absolute";

    bold = document.createElement("b");    
    texto = document.createTextNode("Para ver resultados como mínimo debe seleccionar un centro, una bodega y hacer clic en la lupa para buscar");
    bold.appendChild(texto);
    divText.appendChild(bold);
    document.getElementById("ctl00_MainContent_divCanvas").appendChild(divText);

}

function computeScale(mapWidth, mapHeight, mapMargin, elementWidth, elementHeight, elementBorder) {
    avaliableWidth = mapWidth - (mapMargin * 2);
    avaliableHeight = mapHeight - (mapMargin * 2);

    difX = avaliableWidth / elementWidth;
    difY = avaliableHeight / elementHeight;

    if (difX < difY)
        return difX;
    else
        return difY;
}

function computeBorder(border) {
    border = border * scale;

    if (border < 1) border = 1;

    return border;
}

function drawRectangle(x, y, width, height, color, border, borderColor, target, text, textAlign, fontSize, fontColor, fontBold, fontPadding) {
    div = document.createElement("div");

    div.style.left = Math.floor(x) + "px";
    div.style.top = Math.floor(y) + "px";
    div.style.width = Math.floor(width) + "px";
    div.style.height = Math.floor(height) + "px";
    div.style.backgroundColor = color;
    div.style.position = "absolute";
    div.style.borderStyle = "solid";
    div.style.borderWidth = Math.floor(border) + "px";
    div.style.borderColor = borderColor;
    div.style.padding = 0;
    div.style.margin = 0;

    div.onmouseover = function() {
        if (currentDiv != null) {
            if (currentDiv != this.id) {
                document.getElementById(currentDiv).style.backgroundColor = currentColor;
                document.getElementById(currentDiv).style.borderColor = currentBorderColor;
            }
        }
    }

    if (text != '') {
        divText = document.createElement("div");
        textNode = document.createTextNode(text);

        divText.style.textAlign = textAlign;
        divText.style.color = fontColor;
        divText.style.verticalAlign = "top";
        divText.style.fontSize = fontSize;
        divText.style.padding = fontPadding;

        if (fontBold) {
            bold = document.createElement("b");
            bold.appendChild(textNode);
            divText.appendChild(bold);
        }
        else {
            divText.appendChild(textNode);
        }

        div.appendChild(divText);
    }

    document.getElementById(target).appendChild(div);
}

function drawRectangleLocPreview(x, y, width, height, color, colorActive, border, borderColor, borderColorActive) {
//Este No
    div = document.createElement("div");

    div.style.left = Math.floor(x) + "px";
    div.style.top = Math.floor(y) + "px";
    div.style.width = Math.floor(width) + "px";    
    div.style.height = Math.floor(height) + "px";
    div.style.backgroundColor = color;
    div.style.position = "absolute";
    div.style.borderStyle = "solid";
    div.style.borderWidth = Math.floor(border) + "px";
    div.style.borderColor = borderColor;
    div.style.padding = 0;
    div.style.margin = 0;

    locationColor = color;
    locationBorderColor = borderColor;

    div.onmouseover = function() 
    {
        // Vuelve el div anterior a su color original
        if (currentDiv != null) 
        {
            if (currentDiv != this.id) 
            {
                document.getElementById(currentDiv).style.backgroundColor = currentColor;
                document.getElementById(currentDiv).style.borderColor = currentBorderColor;
            }
        }

        // Salva el color del div actual   
        if (this.style.backgroundColor.toUpperCase() != colorActive.toUpperCase()) currentColor = this.style.backgroundColor;
        if (this.style.borderColor.toUpperCase() != borderColorActive.toUpperCase()) currentBorderColor = this.style.borderColor;

        // Resalta el color del div actual
        this.style.backgroundColor = colorActive;
        this.style.borderColor = borderColorActive;

        currentDiv = this.id;
    };

    div.onclick = function() 
    {
        this.style.backgroundColor = colorActive;
        this.style.borderColor = borderColorActive;
    }

    div.setAttribute("id", x + '-' + y);

    document.getElementById("divMapPreview").appendChild(div);
}

function drawRectangleLocation(x, y, width, height, color, colorActive, colorItem, border, borderColor, borderColorActive, column, columnAttributes, detailAttributes) {
    //Este no
    div = document.createElement("div");

    div.style.left = (Math.floor(x) + "px");
    div.style.top = (Math.floor(y) + "px");
    div.style.width = (Math.floor(width) + "px");
    div.style.height = (Math.floor(height) + "px");
    div.style.backgroundColor = color;
    div.style.position = "absolute";
    div.style.borderStyle = "solid";
    div.style.borderWidth = (Math.floor(border) + "px");
    div.style.borderColor = borderColor;
    div.style.padding = 0;
    div.style.margin = 0;
    div.setAttribute("position-x", column.PositionX);
    div.setAttribute("position-y", column.PositionY);

    // El color de fondo de la columna depende de la cantidad de ubicaciones con Stock
    div.style.backgroundColor = color[column.WithStock];

    //debugger;
    // Resalta las ubicaciones que contienen el item buscado
    if (itemCode != '' || itemName != '' || mapFabricationDate != null || mapExpirationDate != null
    || mapFifoDate != null || mapLote != '' || mapLPN != '' || mapHoldLocation == true || mapCategory != '') {
        for (var i = column.Locations.length - 1; i >= 0; i--) {
            if (column.Locations[i].ItemCode != null && column.Locations[i].ItemCode != '' && itemCode != '' && column.Locations[i].ItemCode.toUpperCase().search(itemCode.toUpperCase()) != -1) {
                hasItemCode = true;
            }

            if (column.Locations[i].ItemLongName != null && column.Locations[i].ItemLongName != '' && itemName != '' && column.Locations[i].ItemLongName.toUpperCase().search(itemName.toUpperCase()) != -1) {
                hasItemName = true;
            }

            //Nuevas Busquedas en Ubicaciones 22-07-2015 para el mapa
            if (mapFilterAdvanced && column.Locations[i].LotNumber != null && column.Locations[i].LotNumber != '' && mapLote != '' && column.Locations[i].LotNumber.toUpperCase().search(mapLote.toUpperCase()) != -1) {
                hasMapLote = true;
            }

            if (mapFilterAdvanced && column.Locations[i].Lpn != null && column.Locations[i].Lpn != '' && mapLPN != '' && column.Locations[i].Lpn.toUpperCase().search(mapLPN.toUpperCase()) != -1) {
                hasMapLPN = true;
            }

            if (mapFilterAdvanced && column.Locations[i].HoldCode != null && column.Locations[i].HoldCode != '' && mapHoldLocation) {
                hasMapHoldLocation = true;
            }

            if (mapFilterAdvanced && column.Locations[i].FabricationDate != null && column.Locations[i].FabricationDate != '' && mapFabricationDate != '' && mapFabricationDate != null
            && compareDate(column.Locations[i].FabricationDate, mapFabricationDate)) {
                hasMapFabricationDate = true;
            }

            if (mapFilterAdvanced && column.Locations[i].ExpirationDate != null && column.Locations[i].ExpirationDate != '' && mapExpirationDate != '' && mapExpirationDate != null
            && compareDate(column.Locations[i].ExpirationDate, mapExpirationDate)) {
                hasMapExpirationDate = true;
            }

            if (mapFilterAdvanced && column.Locations[i].FifoDate != null && column.Locations[i].FifoDate != '' && mapFifoDate != '' && mapFifoDate != null
            && compareDate(column.Locations[i].FifoDate, mapFifoDate)) {
                hasMapFifoDate = true;
            }

            if (mapFilterAdvanced && column.Locations[i].ItemCategory != null && column.Locations[i].ItemCategory != '' && column.Locations[i].ItemCategory.toUpperCase().search(mapCategory.toUpperCase()) != 1) {
                hasMapCategory = true;
            }

        }


        if ((hasItemCode && itemCode != '') || (hasItemName && itemName != '')) {
            div.style.backgroundColor = colorItem;
        }
        if ((hasMapLote && mapLote != '') || (hasMapLPN && mapLPN != '') || (hasMapHoldLocation && mapHoldLocation) || (hasMapCategory && mapCategory != '') ||
        (hasMapFabricationDate && mapFabricationDate != '') || (hasMapExpirationDate && mapExpirationDate != '') || (hasMapFifoDate && mapFifoDate != '')) {
            div.style.backgroundColor = colorItem;
        }

        hasItemCode = false;
        hasItemName = false;

        hasMapLote = false;
        hasMapLPN = false;
        hasMapHoldLocation = false;
        hasMapFabricationDate = false;
        hasMapExpirationDate = false;
        hasMapFifoDate = false;
        hasMapCategory = false;
    }

    locationColor = color;
    locationBorderColor = borderColor;

    div.onmouseover = function() {
        // Vuelve el div anterior a su color original
        if (currentDiv != null) {
            if (currentDiv != this.id) {
                document.getElementById(currentDiv).style.backgroundColor = currentColor;
                document.getElementById(currentDiv).style.borderColor = currentBorderColor;
            }
        }

        // Salva el color del div actual
        if (this.style.backgroundColor.toUpperCase() != colorActive.toUpperCase())
            currentColor = this.style.backgroundColor;

        if (this.style.borderColor.toUpperCase() != borderColorActive.toUpperCase()) 
            currentBorderColor = this.style.borderColor;

        // Resalta el color del div actual
        this.style.backgroundColor = colorActive;
        this.style.borderColor = borderColorActive;

        // ** Comentado el 12-08-2015 **
        // ** Esta funcion genera el detalle de las ubiciones en la Bodeda **
        Tip(getLocDetails(column, columnAttributes, detailAttributes), TITLE, getColumnDetails(column),
        TITLEBGCOLOR, '#FFCC99', BORDERWIDTH, 0, WIDTH, 280, HEIGHT, 1, CLOSEBTN, false, PADDING, 0, TITLEFONTCOLOR, '#000000');

        currentDiv = this.id;
    };

    div.onmouseout = function() {
        currentDiv = this.id;
        UnTip();        
    };

    div.onclick = function() {

        if (firstClickEverOverDiv == false) {
            lastIdDivClicked = this.id;
            firstClickEverOverDiv = true;
        }

        if (currentDiv != null) {
            if (currentDiv != this.id) {
                document.getElementById(currentDiv).style.backgroundColor = currentColor;
                document.getElementById(currentDiv).style.borderColor = currentBorderColor;
            }
        }

        // Salva el color del div actual   
        if (this.style.backgroundColor.toUpperCase() != colorActive.toUpperCase()) {
            var elem = $(this);
            var colorForLocationMostUsed = elem.attr("most-used-location-color");

            if (colorForLocationMostUsed) {
                currentColor = colorForLocationMostUsed;
            } else {
                currentColor = this.style.backgroundColor;
            } 
        }

        if (this.style.borderColor.toUpperCase() != borderColorActive.toUpperCase()) currentBorderColor = this.style.borderColor;

        // Resalta el color del div actual
        this.style.backgroundColor = colorActive;
        this.style.borderColor = borderColorActive;

        Tip(getLocDetails(column, columnAttributes, detailAttributes), TITLE, getColumnDetails(column), TITLEBGCOLOR, columnAttributes.BorderColor, WIDTH, -columnAttributes.MaxWidth);

        currentDiv = this.id;


        if (lastIdDivClicked != this.id) {

            var lastDiv = document.getElementById(lastIdDivClicked);

            document.getElementById(lastIdDivClicked).style.backgroundColor = lastDiv.getAttribute("data-backgroundcolor-default");
            document.getElementById(lastIdDivClicked).style.borderColor = lastDiv.getAttribute("data-bordercolor-default");

            var div = $(lastDiv);
            var mostUsedLocationColor = $(div).attr("most-used-location-color");

            if (mostUsedLocationColor) {
                $(div).css({
                    "background-color": mostUsedLocationColor
                });
            }

            lastIdDivClicked = this.id;
        }
    }
        
    
    div.setAttribute("id", x + '-' + y);
    div.setAttribute("data-backgroundColor-default", color[column.WithStock]);
    div.setAttribute("data-borderColor-default", borderColor);

    document.getElementById("ctl00_MainContent_divCanvas").appendChild(div);
}

//Compara si las fechas ingrsadas son iguales
function compareDate(date1, date2) {
    var result = false;
    var year1 = date1.getFullYear();
    var month1 = date1.getMonth() + 1;
    var day1 = date1.getDate();
    var year2 = date2.getFullYear();
    var month2 = date2.getMonth() + 1;
    var day2 = date2.getDate();

    if ((year1 == year2) && (month1 == month2) && (day1 == day2)) {
        result = true;
    }
    
    return result;
}

function getColumnDetails(column) {
    var cont = 0;
    var IdLocCodeAux = '';
    
    //Cuenta los niveles existentes
    for (var j = column.Locations.length - 1; j >= 0; j--) {
        if (IdLocCodeAux != column.Locations[j].IdCode) {
            cont = cont + 1;
        }
        IdLocCodeAux = column.Locations[j].IdCode;
    }

    return 'Fila:&nbsp;' + column.Row + '&nbsp;&nbsp;&nbsp;Col:&nbsp;' + column.Column + '&nbsp;&nbsp;&nbsp;Niveles:&nbsp;' + cont +  '&nbsp;&nbsp;&nbsp;Pasillo:&nbsp;'+ column.Aisle +'&nbsp;'; 
}

function getLocDetails(column, columnAttributes, detailAttributes) {
    var divWrapper = document.createElement("div");
    var divColumn = document.createElement("div");

    divColumn.style.backgroundColor = columnAttributes.BackColor[0];
    divColumn.style.margin = 0;
    divColumn.style.padding = (columnAttributes.Padding + "px");
    divColumn.style.borderStyle = "solid";
    divColumn.style.borderWidth = (columnAttributes.Border + "px");
    divColumn.style.borderColor = columnAttributes.BorderColor;
    divColumn.style.minHeight = (columnAttributes.MinHeight +"px");
    divColumn.style.maxHeight = (columnAttributes.MaxHeight +"px");
    //divColumn.style.maxHeight = "50px";

    divColumn.style.minWidth = (columnAttributes.MinWidth +"px");
    divColumn.style.maxWidth = (columnAttributes.MaxWidth +"px");
    divColumn.style.overflow = "auto";

    var IdLocCodeAux = '';

    // Recorre las ubicaciones de la posición actual
    for (var i = column.Locations.length - 1; i >= 0; i--) {

        if (IdLocCodeAux == column.Locations[i].IdCode) {

            divSpace = document.createElement("div");
//            divSpace.style.height = 5;
            divColumn.appendChild(divSpace)

            hr = document.createElement("hr");
            divText.appendChild(hr);
            divLocDetails.appendChild(divText);

            //LPN
            if (column.Locations[i].Lpn != "") {
                divText = document.createElement("div");
                texto = document.createTextNode('LPN: ' + column.Locations[i].Lpn);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }               

            // Code - Qty
            divText = document.createElement("div");
            texto = document.createTextNode('Cód: ' + column.Locations[i].ItemCode + ' - Cant: ' + column.Locations[i].ItemQty);
            divText.appendChild(texto);
            divLocDetails.appendChild(divText);

            // Name
            divText = document.createElement("div");
            divText.style.fontSize = "9px";
            texto = document.createTextNode(column.Locations[i].ItemLongName);
            divText.appendChild(texto);
            divLocDetails.appendChild(divText);

            // Category
            if (column.Locations[i].ItemCategory != "") {
                divText = document.createElement("div");
                texto = document.createTextNode('Categoría: ' + column.Locations[i].ItemCategory);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }

            // Lote - LPN
            if (column.Locations[i].LotNumber != "") {
                divText = document.createElement("div");
                texto = document.createTextNode('Lote: ' + column.Locations[i].LotNumber);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }
//            divText = document.createElement("div");
//            if (column.Locations[i].LotNumber != "") {
//                texto = document.createTextNode('Lote: ' + column.Locations[i].LotNumber + ' - LPN: ' + column.Locations[i].Lpn);
//            } else {
//                texto = document.createTextNode('LPN: ' + column.Locations[i].Lpn);
//            }
//            divText.appendChild(texto);
//            divLocDetails.appendChild(divText);


            // Fecha Elaboracion
            if (column.Locations[i].FabricationDate != "" && column.Locations[i].FabricationDate.getFullYear() > 1) {
                divText = document.createElement("div");
                texto = document.createTextNode('F. Elab: ' + column.Locations[i].FabricationDate.format(formatDate));
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }

            // Fecha Vencimiento
            if (column.Locations[i].ExpirationDate != "" && column.Locations[i].ExpirationDate.getFullYear() > 1) {
                divText = document.createElement("div");
                texto = document.createTextNode('F. Venc: ' + column.Locations[i].ExpirationDate.format(formatDate));
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }

            // Fecha FiFo
            if (column.Locations[i].FifoDate != "" && column.Locations[i].FifoDate.getFullYear() > 1) {
                divText = document.createElement("div");
                texto = document.createTextNode('F. Fifo: ' + column.Locations[i].FifoDate.format(formatDate));
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }

            // HoldCode
            if (column.Locations[i].HoldCode != "") {
                divText = document.createElement("div");
                texto = document.createTextNode('Ubic. Bloq.: ' + column.Locations[i].HoldCode);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }

            IdLocCodeAux = column.Locations[i].IdCode;


        } else {

            divLocDetails = document.createElement("div");

            divLocDetails.style.backgroundColor = detailAttributes.BackColor[column.Locations[i].Level - 1];
            divLocDetails.style.borderStyle = "solid";
            divLocDetails.style.borderWidth = (detailAttributes.Border + "px");
            divLocDetails.style.borderColor = detailAttributes.BorderColor;
            //divLocDetails.style.padding = "1px"; //detailAttributes.Padding;
            divLocDetails.style.padding = (detailAttributes.Padding + "px");     
            divLocDetails.style.minHeight = (detailAttributes.MinHeight + "px");
            //divLocDetails.style.minHeight = "20px";

            if ((column.Locations[i].Type == 'FKL' || column.Locations[i].Type == 'STG' || column.Locations[i].Type == 'TRUCK')) {

                divLocDetails.style.minHeight = (columnAttributes.MinHeight + "px");
                divLocDetails.style.maxHeight = (columnAttributes.MaxHeight + "px");              
            } else {
                divLocDetails.style.maxHeight = (detailAttributes.MaxHeight + "px");
            }
            
            divLocDetails.style.overflow = "auto";

            // Location Level
            divText = document.createElement("div");
            divText.style.textAlign = "right";
            divText.style.float = "right";
            texto = document.createTextNode('Nivel ' + column.Locations[i].Level);
            divText.appendChild(texto);
            divLocDetails.appendChild(divText);

            // Location Type - Code
            divText = document.createElement("div");
            bold = document.createElement("b");
            texto = document.createTextNode(column.Locations[i].Type + ' - ' + column.Locations[i].LocCode);
            bold.appendChild(texto);
            divText.appendChild(bold);
            hr = document.createElement("hr");
            divText.appendChild(hr);
            divLocDetails.appendChild(divText);

            // Detalles Item
            if (column.Locations[i].ItemCode != '') {

                //LPN
                if (column.Locations[i].Lpn != "") {
                    divText = document.createElement("div");
                    texto = document.createTextNode('LPN: ' + column.Locations[i].Lpn);
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }            
            
                // Code - Qty
                divText = document.createElement("div");
                texto = document.createTextNode('Cód: ' + column.Locations[i].ItemCode + ' - Cant: ' + column.Locations[i].ItemQty);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);

                // Name
                divText = document.createElement("div");
                divText.style.fontSize = "9px";
                texto = document.createTextNode(column.Locations[i].ItemLongName);
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);

                // Category
                if (column.Locations[i].ItemCategory != "") {
                    divText = document.createElement("div");
                    texto = document.createTextNode('Categoría: ' + column.Locations[i].ItemCategory);
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }

                // Lote - LPN
                
                if (column.Locations[i].LotNumber != "") {
                    //                    texto = document.createTextNode('Lote: ' + column.Locations[i].LotNumber + ' - LPN: ' + column.Locations[i].Lpn);
                    divText = document.createElement("div");
                    texto = document.createTextNode('Lote: ' + column.Locations[i].LotNumber);
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
//                } else {
//                    texto = document.createTextNode('LPN: ' + column.Locations[i].Lpn);
                }
                


                // Fecha Elaboracion
                if (column.Locations[i].FabricationDate != "" && column.Locations[i].FabricationDate.getFullYear() > 1) {
                    divText = document.createElement("div");
                    texto = document.createTextNode('F. Elab: ' + column.Locations[i].FabricationDate.format(formatDate));
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }

                // Fecha Vencimiento
                if (column.Locations[i].ExpirationDate != "" && column.Locations[i].ExpirationDate.getFullYear() > 1) {
                    divText = document.createElement("div");
                    texto = document.createTextNode('F. Venc: ' + column.Locations[i].ExpirationDate.format(formatDate));
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }

                // Fecha FiFo
                if (column.Locations[i].FifoDate != "" && column.Locations[i].FifoDate.getFullYear() > 1) {
                    divText = document.createElement("div");
                    texto = document.createTextNode('F. Fifo: ' + column.Locations[i].FifoDate.format(formatDate));
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }

                // HoldCode
                if (column.Locations[i].HoldCode != "") {
                    divText = document.createElement("div");
                    texto = document.createTextNode('Ubic. Bloq.: ' + column.Locations[i].HoldCode);
                    divText.appendChild(texto);
                    divLocDetails.appendChild(divText);
                }

                var cont = 0;
                for (var j = column.Locations.length - 1; j >= 0; j--) {
                    if (column.Locations[i].IdCode == column.Locations[j].IdCode) {
                        cont = cont + 1;
                    }
                }

                if (cont > 1) {
                    IdLocCodeAux = column.Locations[i].IdCode;
                }

            } else {
                divText = document.createElement("div");
                texto = document.createTextNode('** Sin Stock **');
                divText.appendChild(texto);
                divLocDetails.appendChild(divText);
            }
        }

        divColumn.appendChild(divLocDetails);

        if (IdLocCodeAux != column.Locations[i].IdCode) {
            // Separador
            divSpace = document.createElement("div");
            divSpace.style.height = 5;
            divSpace.style.backgroundColor = columnAttributes.BackColor;
        }

        try
        {
            if ((i > 0) && (divSpace != null)) {
                divColumn.appendChild(divSpace);
            }
        }
        catch(err)
        {}
    }
    //divWrapper.style.top = "200px";
    divWrapper.appendChild(divColumn);

    return divWrapper.innerHTML;
}


