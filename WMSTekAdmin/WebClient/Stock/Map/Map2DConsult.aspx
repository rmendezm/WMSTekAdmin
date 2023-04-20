<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Map2DConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stocks.Map2DConsult" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type= "text/javascript" >
    
//    $(document).ready(function(){
//        ClearCanvas();
//    });

    function cleanControlsMap() {
        document.getElementById("ctl00_ucMainFilterContent_txtFilterItem").value = "";
        document.getElementById("ctl00_ucMainFilterContent_txtFilterName").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapFabricationDate").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapExpirationDate").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapFifoDate").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapLote").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapLPN").value = "";
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_chkMapHoldLocation").checked = false;
        document.getElementById("ctl00_ucMainFilterContent_Tabs_tabMapaBodega_txtMapCategory").value = "";
        
    }

    function EndRequestHandler(sender, args) {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divCanvas").style.height = h;
        document.getElementById("ctl00_MainContent_divCanvas").style.width = w;
    }
    
</script>

    <style>
        @media (max-width: 850px) {
            #ctl00_MainContent_lblMsg, #ctl00_MainContent_divMsg {
                display: none;
            }
        }
    </style>

    <script type="text/javascript" src="../../WebResources/Javascript/wz_tooltip.js"></script>
    <script src="../../WebResources/Javascript/Map2D.js" type="text/javascript"></script>

    <div id="divBloqueoPantalla" runat="server" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 1101; width: 100%; height: 100%; background-color: White; color: #ffffff;
        filter: Alpha(Opacity=60); -moz-opacity: 0.2; text-align: center; vertical-align: middle;">
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;         
        <img alt="Procesando" src="../../WebResources/Images/Buttons/icon_progress.gif" />        
    </div>

    <div id="divMsg" runat="server" style="position:absolute;width:800px;height:20px; background-color: #FFCC99; color: #993300; font-weight: bold; font-size: small;" >
        <asp:Label ID="lblMsg" runat="server" Visible="true" Text="Para ver resultados como mínimo debe seleccionar un centro, una bodega y hacer clic en la lupa para buscar"></asp:Label>
    </div>
    
        <div id="divCanvas" runat="server"  style="overflow: auto; position:relative; z-index:0;"  />

     
   
    <%-- Mensajes de advertencia y auxiliares --%>
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Largo" Visible="false" />    
    <asp:Label ID="lblFilterItemRequired" runat="server" Text="Debe ingresar un item" Visible="false" />   
    <asp:Label ID="lblFilterDateRequired" runat="server" Text="Debe ingresar fecha inicio y fin" Visible="false" />   
    <asp:Label ID="lblFilterDate" runat="server" Text="Inicio" Visible="false" />
    <%-- FIN Mensajes de advertencia y auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
