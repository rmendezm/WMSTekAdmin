<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Splitter.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Pruebas.Splitter" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" src="../WebResources/Javascript/graphics.js"></script>
<script type="text/javascript" src="../WebResources/Javascript/tooltip.js"></script>

<%--<img src= "../WebResources/Images/Map/rack.png" alt="Rack" usemap="#rackMap" />
--%>
<div id="divCanvas" style="position: relative" >
    <%--<img src= "../WebResources/Images/Map/fondo.png" />--%>
</div>

<%--<map name="rackMap">
<area shape="rect" coords ="10,10,210,110" onclick 
    onclick="Tip('Some text')" onmouseout="UnTip()"
/>

<area shape = "rect" coords ="100,0,200,50"
    onMouseOver="writeText('Ubicación 2')" 
    onmouseout="clearText()"
/>

<area shape ="rect" coords ="0,50,100,100"
    onMouseOver="writeText('Ubicación 3')"  
    onmouseout="clearText()"
/>

<area shape ="rect" coords ="100,50,200,100"
    onMouseOver="writeText('Ubicación 4')"  
    onmouseout="clearText()"
/>
</map>--%> 

<%--<p id="desc" style="background-color: Red">aca </p>--%>

<script type="text/javascript">
<!--

    var txtLoc001 = '<b>Ubicación 001</b><br/>Tarro Nescafé 250g<br/>Cantidad: 6';
    var txtLoc002 = '<b>Ubicación 002</b><br/>Cola de Mono<br/>Cantidad: 3';
    


   // var jg_doc = new jsGraphics(); // draw directly into document
    var jg = new jsGraphics("divCanvas");

    CargarHangar();

    jg.paint();    

//-->
</script>

<script type="text/javascript">
    function writeText(txt) {
        document.getElementById("desc").innerHTML = txt;
    }

    function clearText() {
        document.getElementById("desc").innerHTML = '';
    }
</script>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
        <div id="divHangar"></div>
      <%--<table id="tableHangar" cellpadding="0" cellspacing="0">
            <tr>
                <th>
                    ID
                </th>
                <th>
                    Código
                </th>
                <th>
                    Nombre
                </th>
                <th>
                    PositionX
                </th>
                <th>
                    PositionY
                </th>                
            </tr>
        </table>--%>
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
