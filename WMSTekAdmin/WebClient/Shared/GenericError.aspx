<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.GenericError" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ERROR - WMSTek</title>
    
    <!-- Hoja de estilo principal -->   
    <link href="~/WebResources/Styles/WMSTekWeb.css" rel="stylesheet" type="text/css" />    
    <link href="~/WebResources/Styles/ModalPopup.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
            <asp:ScriptManager ID="smGenericError" runat="server" EnablePartialRendering="true" EnableScriptGlobalization="True" />
            
        <%--Esto lo he comentado por que se ve mal con el encabezado + el encabezado del master page--%>            
        <%--Hay que fijarse si desde otro lado se ve igual el masterpage--%> 
        <%--Encabezado --%>
        <%--<div id="header">
                   <div id="logo">
                        <img src="../WebResources/Images/Header/img_logo_cabecera.png" alt="logo" /></div>
                   <div id="gradiente">
                   </div>  
               </div>--%>

	    <%--Información mostrada al usuario--%>
        <div class="divErrorPanel">
            <div class="divErrorBody">
                <div class="divErrorLevel">
                    <asp:Image id="imgErrorLevel" runat="server" ImageUrl="../WebResources/Images/Buttons/AlarmMessage/icon_warning.png" />
                </div>
                <div class="divErrorTitle">        
                    <asp:Label ID="lblTitle" runat="server" />
                </div> 
                <br />
                <div class="divErrorMessage">        
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
                <div id="divErrorSolution" runat="server" class="divGenericErrorSolution">
                    <ul>
                        <li class="divErrorLink"><a href="javascript:history.go(-1)">Reintetar</a></li>
                        <li class="divErrorLink"><a href="/" title="Volver">Volver al Inicio</a></li>
                        <li class="divErrorNoLink">Si el problema persiste, póngase en contacto con el Administrador.</li>
                    </ul>
                </div>                    
                <div id="divErrorViewDetails" runat="server" class="divErrorViewDetails">
                    <asp:Label ID="lblViewDetails" runat="server" Text="Ver detalles" />
                </div>
             </div> 
         <br />   
         
        <%--Información adicional --%>
       <ajaxToolkit:CollapsiblePanelExtender ID="cpeErrorDetails" runat="Server"
            TargetControlID="pnlErrorDetails"
            ExpandControlID="divErrorViewDetails"
            CollapseControlID="divErrorViewDetails" 
            Collapsed="True"
            SuppressPostBack="False"
            TextLabelID="lblViewDetails"
            ExpandedText="Ocultar detalles"
            CollapsedText="Ver detalles"            
             />
          
        <asp:Panel id="pnlErrorDetails" Visible="true" class="divErrorAditionalInfo" runat="server">
            <div class="ErrorDetailsCaption">Información Adicional</div>      
            <div> 
                <b><asp:Label ID="lblExMesasgeTitle" runat="server" Text="Message"/></b><br />
                <asp:Label ID="lblExMessage" runat="server" /> <br /><br />
                
                <b><asp:Label ID="lblExSourceTitle" runat="server" Text="Source"/></b><br /> 
                <asp:Label ID="lblExSource" runat="server" /> <br /><br />
                
                <b><asp:Label ID="lblExStackTraceTitle" runat="server" Text="StackTrace"/></b><br /> 
                <asp:Label ID="lblExStackTrace" runat="server" /> 
            </div>          
        </asp:Panel>   
        
   <%-- Mensajes de Confirmacion y Auxiliares --%>    
        <asp:Label id="lblNoInfo" runat="server"  Text="No hay información adicional disponible. " Visible="false" />  	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>                    
    </form>
</body>
</html>
