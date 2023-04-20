<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogDelete.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.DialogDelete" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>WMSTek</title>
    
        <script language="javascript" type="text/javascript">
            function btnClose_onclick(option) 
            {
                window.parent.execScript("deleteDialog(" + option + ")");
            }
        </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Está seguro de eliminar?
    </div>
        <input id="btkOk" type="button" value="Aceptar" language="javascript" onclick="return btn_onclick('true')" />&nbsp;
        <input id="btkCancel" type="button" value="Cancelar" language="javascript" onclick="return btn_onclick('false')" />
    </form>
</body>
</html>
