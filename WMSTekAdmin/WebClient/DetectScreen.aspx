<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetectScreen.aspx.cs" Inherits="Binaria.WMSTek.WebClient.DetectScreen" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<script type="text/javascript" language="javascript">
    // Detecta la resolución de pantalla del cliente
    // Se modifica para posteriores versiones de internet explorer donde fallaba el javaScript
    //    var screenX = "&screenX=" + screen.width;
    //    var screenY = "&screenY=" + screen.height;

    //    var screenX = "&screenX=" + 1200;
    //    var screenY = "&screenY=" + 850;
    var screenX = "&screenX=" + window.screen.availWidth;
    var screenY = "&screenY=" + window.screen.availHeight;
    window.location.href = "DetectScreen.aspx?action=set" + screenX + screenY;

</script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>


