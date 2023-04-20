<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.NotFound" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ERROR 404 - WMSTek</title>
    <link href="~/WebResources/Styles/WMSTekWeb.css" rel="stylesheet" type="text/css" />    
    <link href="~/WebResources/Styles/ModalPopup.css" rel="stylesheet" type="text/css" />
    <style>
        a:visited {
            color: darkblue !important;
        }
        a:active {
            color: blue !important;
        } 
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="divErrorPanel">
            <div class="divErrorBody">
                <div class="divErrorLevel">
                    <asp:Image id="imgErrorLevel" runat="server" ImageUrl="../WebResources/Images/Buttons/AlarmMessage/icon_warning.png" />
                </div>
                <div class="divErrorTitle">        
                    <asp:Label ID="lblTitle" runat="server" Text="Página No Encontrada" />
                </div> 
                <br />
                <div class="divErrorMessage">        
                    <asp:Label ID="lblMessage" runat="server" Text="Error 404" />
                </div>
                <div id="divErrorSolution" runat="server" class="divGenericErrorSolution">
                    <ul>
                        <li class="divErrorLink"><a href="/" title="Volver">Volver al Inicio</a></li>
                    </ul>
                </div>                    
             </div> 
        </div>
    </form>
</body>
</html>
