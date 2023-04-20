<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XML.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Pruebas.XML" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnSendInbound" runat="server" Text="Enviar Inbound" onclick="btnSendInbound_Click" />
        
        <asp:Label ID="lblWsMessage" runat="server" />
    </div>
    </form>
</body>
</html>
