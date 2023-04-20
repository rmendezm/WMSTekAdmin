/* Selecciona todos los checkboxes en una grilla */
/* --------------------------------------------- */

// toggle state of the checkbox between selected and not selected!
function toggleCheckBoxes(gvId, chkId, isChecked) {
    var checkboxes = getCheckBoxesFrom(document.getElementById(gvId), chkId);

    for (i = 0; i <= checkboxes.length - 1; i++) 
    {
        if (checkboxes[i].disabled != true)
        {
            checkboxes[i].checked = isChecked;
        }
    }
}

// get all the checkboxes from the container control
function getCheckBoxesFrom(gv, chkId) {

    var checkboxesArray = new Array();
    var inputElements = gv.getElementsByTagName("input");
    if (inputElements.length == 0) null;

    for (i = 0; i <= inputElements.length - 1; i++) {

        if (isCheckBox(inputElements[i]) && inputElements[i].name.indexOf(chkId) > -1) {

            checkboxesArray.push(inputElements[i]);
        }
    }

    return checkboxesArray;
}

// checks if the elements is a checkbox or not
function isCheckBox(element) {
    return element.type == "checkbox";
}

/* FIN Selecciona todos los checkboxes en una grilla */


/* Tamaño de un elemento */

function setHeight(ctlId, value) {
    //alert(control.style.height);
    control = document.getElementById(ctlId);
    control.style.height = value;
    //alert(control.style.height);
}

/* Efectos sobre la Grilla*/
/* ---------------------- */
/* Controla color de las filas de la grilla para los eventos del mouse */

    var gridViewCtl = null;
    var curSelRow = null;
    var curDetailSelRow = null;
    var selectedRowColor = '#dbfbfb';
    var selectedOverRowColor = '#FAC65C';
    var overRowColor = '#FAFAFA';
    var overRowColorAlt = '#DDECFF';

    function getSelectedRow(rowIdx, gridViewCtlId) 
    {
        getGridViewControl(gridViewCtlId);
        if (null != gridViewCtl) 
        {
            return gridViewCtl.rows[rowIdx];
        }
        return null;
    }

    function getGridViewControl(gridViewCtlId) 
    {
        gridViewCtl = document.getElementById(gridViewCtlId);
    }

    /* On Click */
    function gridViewOnclick(rowIdx, gridViewCtlId) {

        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (curSelRow != null) {
            curSelRow.style.backgroundColor = selRow.originalstyle;
        }

        if (null != selRow) {
            curSelRow = selRow;
            $("#" + gridViewCtlId + " tr").css("background-color", "");
            $(selRow).css("background-color", selectedRowColor);
        }
    }

    function gridViewDetailOnclick(rowIdx, gridViewCtlId) {

        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (curDetailSelRow != null) {
            curDetailSelRow.style.backgroundColor = selRow.originalstyle;
        }

        if (null != selRow) {
            curDetailSelRow = selRow;
            curDetailSelRow.style.backgroundColor = selectedRowColor;
        }
    }
        

    /* On Mouse Over */
    function gridViewOnmouseover(rowIdx, gridViewCtlId) 
    {
        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (null != selRow) 
        {
            if (curSelRow != null && curSelRow == selRow) 
            {
                selRow.originalstyle = selRow.style.backgroundColor;
                selRow.style.backgroundColor = selectedOverRowColor;
            }
            else 
            {
                selRow.originalstyle = selRow.style.backgroundColor;

                if (isEven(row))
                    selRow.style.backgroundColor = overRowColor;
                else
                    selRow.style.backgroundColor = overRowColorAlt;
            }
        }
    }

    function gridViewDetailOnmouseover(rowIdx, gridViewCtlId) {
        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (null != selRow) {
            if (curDetailSelRow != null && curDetailSelRow == selRow) {
                selRow.originalstyle = selRow.style.backgroundColor;
                selRow.style.backgroundColor = selectedOverRowColor;
            }
            else {
                selRow.originalstyle = selRow.style.backgroundColor;

                if (isEven(row))
                    selRow.style.backgroundColor = overRowColor;
                else
                    selRow.style.backgroundColor = overRowColorAlt;
            }
        }
    }    

    /* On Mouse Out */
    function gridViewOnmouseout(rowIdx, gridViewCtlId) {

        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (null != selRow) {
            if (curSelRow != null && curSelRow == selRow) {
                curSelRow.style.backgroundColor = selectedRowColor;
            }
            else {
                selRow.style.backgroundColor = selRow.originalstyle;
            }
        }
    }

    function gridViewDetailOnmouseout(rowIdx, gridViewCtlId) {

        row = parseInt(rowIdx) + 1;
        var selRow = getSelectedRow(row, gridViewCtlId);

        if (null != selRow) {
            if (curDetailSelRow != null && curDetailSelRow == selRow) {
                curDetailSelRow.style.backgroundColor = selectedRowColor;
            }
            else {
                selRow.style.backgroundColor = selRow.originalstyle;
            }
        }
    }    
    /* FIN Efectos sobre la Grilla*/

    /* Botones Activar / Desactivar Filtro Avanzado */
    function toggleButton(activate, btnId) 
    {
        btn = document.getElementById(btnId);
        //btnInactivateAdvancedFilter = document.getElementById("btnInactivateAdvancedFilter");

        alert(btnId);
        if (activate) 
        {
            btn.style.display = 'none';
         //   btnInactivateAdvancedFilter.style.display = 'inline';
        }
        else 
        {
            btn.style.display = 'inline';
        //    btnInactivateAdvancedFilter.style.display = 'none';        
        }
    }    
    
    function isEven(num) 
    {
        return !(num % 2);
    }
    
/**
* Color Tools Javascript
*
* Contiene varias clases para manipular colores en Javascript.
* Basado en la versión 0.5 de Color Tools PHP por
* Jack Sleight - www.reallyshiny.com
*
* @author Pedro Fuentes <pedro.fuentes@oxus.cl>
* @version 0.1
* @license Creative Commons Attribution-ShareAlike 2.5 License
*/

colorTools = {

/**
* Convert Hex code string to RGB array
* @param string hex hexadecimal color code
* @return array
*/
hexToRgb : function(hex){

regex	= hex.replace('#', '');
rgb			= new Array();

rgb['r']	= colorTools.hexdec(regex.substr(0,2));
rgb['g']	= colorTools.hexdec(regex.substr(2,2));
rgb['b']	= colorTools.hexdec(regex.substr(4,2));

return rgb;

},

/**
* Convert RGB array to Hex code string
* @param array rgb
* @return string
*/
rgbToHex : function(rgb){
return sprintf('%02x', rgb['r']) + sprintf('%02x', rgb['g']) + sprintf('%02x', rgb['b']);
},

/**
* Create gradient from one color to another
* @param col1 string
* @param col2 string
* @param steps integer
* @return array
*/
gradient : function(col1, col2, steps){
gradient	= new Array();
step		= new Array();

col1 = colorTools.hexToRgb(col1);
col2 = colorTools.hexToRgb(col2);

step['r'] = (col1['r'] - col2['r']) / (steps - 1);
step['g'] = (col1['g'] - col2['g']) / (steps - 1);
step['b'] = (col1['b'] - col2['b']) / (steps - 1);

for(i = 0; i <= steps; i++) {

color		= new Array();

color['r']	= Math.round(col1['r'] - (step['r'] * i));
color['g']	= Math.round(col1['g'] - (step['g'] * i));
color['b']	= Math.round(col1['b'] - (step['b'] * i));

gradient[i]	= colorTools.rgbToHex(color);

}

return gradient;
},

/**
* Create a text gradient from one color to another
* @param col1 string
* @param col2 string
* @param str string
* @return array
*/
textGradient : function(col1, col2, str){
gradient	= new Array();
step		= new Array();

col1 = colorTools.hexToRgb(col1);
col2 = colorTools.hexToRgb(col2);

step['r']	= (col1['r'] - col2['r']) / (str.length - 1);
step['g']	= (col1['g'] - col2['g']) / (str.length - 1);
step['b']	= (col1['b'] - col2['b']) / (str.length - 1);

var res			= '';

for(i = 0; i <= str.length; i++) {

color		= new Array();

color['r']	= Math.round(col1['r'] - (step['r'] * i));
color['g']	= Math.round(col1['g'] - (step['g'] * i));
color['b']	= Math.round(col1['b'] - (step['b'] * i));

res	= res + '<span style="color: #' + colorTools.rgbToHex(color) + '">' + str.charAt(i) + '';

}

return res;
},

/**
* Invert a color
* @param col string
* @return string
*/
invert : function(col){
var col1	= colorTools.hexToRgb(col);
var col2	= new Array();

col2['r']	= 255 - col1['r'];
col2['g']	= 255 - col1['g'];
col2['b']	= 255 - col1['b'];

return colorTools.rgbToHex(col2);

},

/**
* Convert Hexadecimal to Decimal
* @param h hexadecimal
* @return int
*/
hexdec: function(h) {
    return parseInt(h, 16);
}

}

/**
*
*  Javascript sprintf
*  http://www.webtoolkit.info/
*
*
**/

sprintfWrapper = {

    init: function() {

        if (typeof arguments == "undefined") { return null; }
        if (arguments.length < 1) { return null; }
        if (typeof arguments[0] != "string") { return null; }
        if (typeof RegExp == "undefined") { return null; }

        var string = arguments[0];
        var exp = new RegExp(/(%([%]|(\-)?(\+|\x20)?(0)?(\d+)?(\.(\d)?)?([bcdfosxX])))/g);
        var matches = new Array();
        var strings = new Array();
        var convCount = 0;
        var stringPosStart = 0;
        var stringPosEnd = 0;
        var matchPosEnd = 0;
        var newString = '';
        var match = null;

        while (match = exp.exec(string)) {
            if (match[9]) { convCount += 1; }

            stringPosStart = matchPosEnd;
            stringPosEnd = exp.lastIndex - match[0].length;
            strings[strings.length] = string.substring(stringPosStart, stringPosEnd);

            matchPosEnd = exp.lastIndex;
            matches[matches.length] = {
                match: match[0],
                left: match[3] ? true : false,
                sign: match[4] || '',
                pad: match[5] || ' ',
                min: match[6] || 0,
                precision: match[8],
                code: match[9] || '%',
                negative: parseInt(arguments[convCount]) < 0 ? true : false,
                argument: String(arguments[convCount])
            };
        }
        strings[strings.length] = string.substring(matchPosEnd);

        if (matches.length == 0) { return string; }
        if ((arguments.length - 1) < convCount) { return null; }

        var code = null;
        var match = null;
        var i = null;

        for (i = 0; i < matches.length; i++) {

            if (matches[i].code == '%') { substitution = '%' }
            else if (matches[i].code == 'b') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(2));
                substitution = sprintfWrapper.convert(matches[i], true);
            }
            else if (matches[i].code == 'c') {
                matches[i].argument = String(String.fromCharCode(parseInt(Math.abs(parseInt(matches[i].argument)))));
                substitution = sprintfWrapper.convert(matches[i], true);
            }
            else if (matches[i].code == 'd') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)));
                substitution = sprintfWrapper.convert(matches[i]);
            }
            else if (matches[i].code == 'f') {
                matches[i].argument = String(Math.abs(parseFloat(matches[i].argument)).toFixed(matches[i].precision ? matches[i].precision : 6));
                substitution = sprintfWrapper.convert(matches[i]);
            }
            else if (matches[i].code == 'o') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(8));
                substitution = sprintfWrapper.convert(matches[i]);
            }
            else if (matches[i].code == 's') {
                matches[i].argument = matches[i].argument.substring(0, matches[i].precision ? matches[i].precision : matches[i].argument.length)
                substitution = sprintfWrapper.convert(matches[i], true);
            }
            else if (matches[i].code == 'x') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                substitution = sprintfWrapper.convert(matches[i]);
            }
            else if (matches[i].code == 'X') {
                matches[i].argument = String(Math.abs(parseInt(matches[i].argument)).toString(16));
                substitution = sprintfWrapper.convert(matches[i]).toUpperCase();
            }
            else {
                substitution = matches[i].match;
            }

            newString += strings[i];
            newString += substitution;

        }
        newString += strings[i];

        return newString;

    },

    convert: function(match, nosign) {
        if (nosign) {
            match.sign = '';
        } else {
            match.sign = match.negative ? '-' : match.sign;
        }
        var l = match.min - match.argument.length + 1 - match.sign.length;
        var pad = new Array(l < 0 ? 0 : l).join(match.pad);
        if (!match.left) {
            if (match.pad == "0" || nosign) {
                return match.sign + pad + match.argument;
            } else {
                return pad + match.sign + match.argument;
            }
        } else {
            if (match.pad == "0" || nosign) {
                return match.sign + match.argument + pad.replace(/0/g, ' ');
            } else {
                return match.sign + match.argument + pad;
            }
        }
    }
}

sprintf = sprintfWrapper.init;