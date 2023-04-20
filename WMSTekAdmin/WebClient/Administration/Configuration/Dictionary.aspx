<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dictionary.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Configuration.Dictionary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <div>
        <asp:PlaceHolder ID="MainContentPanel" runat="server"></asp:PlaceHolder>
        <asp:Button ID="btnCreateDictionary" runat="server" Text="Crear Diccionario" onclick="btnCreateDictionary_Click" />
        <br />
            <asp:PlaceHolder ID="phFrames" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
