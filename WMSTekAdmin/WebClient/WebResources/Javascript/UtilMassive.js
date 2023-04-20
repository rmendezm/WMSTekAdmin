function ValidateExtension() {

    var file = document.getElementById('ctl00_MainContent_uploadFile2').value;
    var lblError = document.getElementById("ctl00_MainContent_lblMensaje2");

    if (file == null || file == '') {
        alert('Seleccione el archivo a cargar.');
        return false;
    }

    var allowedFiles = [".xls", ".xlsx"];
    var regex = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(" + allowedFiles.join('|') + ")$");
    //(/\.(xls|xlsx)$/i
    if (regex.test(file)) {
        return true;
    } else {
        lblError.innerHTML = "Por favor, cargar archivos con extensiones: <b>" + allowedFiles.join(', ') + "</b>.";
        return false;
    }
}


function showProgress() {
    if (document.getElementById('ctl00_MainContent_uploadFile2').value.length > 0 && parseInt(document.getElementById('ctl00_MainContent_ddlOwnerLoad').value) > 0) {
        document.getElementById("ctl00_MainContent_divFondoPopupProgress").style.display = 'block';
        return true;
    } else {
        return false;
    }
}

function HideMessage() {
    document.getElementById("divFondoPopup").style.display = 'none';
    document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';
    return false;
}


function ShowMessage(title, message) {
    var position = (document.body.clientWidth - 400) / 2 + "px";
    document.getElementById("divFondoPopup").style.display = 'block';
    document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
    document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

    document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
    document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

    return false;
}