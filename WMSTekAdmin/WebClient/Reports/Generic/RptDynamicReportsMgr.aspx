<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="RptDynamicReportsMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.RptDynamicReportsMgr" Title="Untitled Page" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucFilterReport" Src="~/Shared/FilterReport.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">

    $(function () {
        
    });

    function getSearchParameters() {
        var prmstr = window.location.search.substr(1);
        return prmstr != null && prmstr != "" ? transformToAssocArray(prmstr) : {};
    }

    function transformToAssocArray(prmstr) {
        var params = {};
        var prmarr = prmstr.split("&");
        for (var i = 0; i < prmarr.length; i++) {
            var tmparr = prmarr[i].split("=");
            params[tmparr[0]] = tmparr[1];
        }
        return params;
    }

    function loadReportFromSSRS(report) {

        var params = getSearchParameters();

        var result = Object.keys(params).map(function (key) {
            return [key, params[key]];
        }); 

        var reportParams = '';

        for (var i = 0; i < result.length; i++) { 
            reportParams += '&' + result[i][0] + '=' + result[i][1];
        } 

        var urlReport = report + reportParams;
        $("#frmReports").attr('src', urlReport);
    }

    function resizeDiv() {
        if ($("#ctl00_MainContent_divReport").length > 0) {
            var body = document.body.clientHeight - 70;
            var h = body + "px";
            var w = document.body.clientWidth + "px";
            var h1 = (document.body.clientHeight - 100) + "px";
            var w2 = (document.body.clientWidth - 10) + "px";
            document.getElementById("<%=divReport.ClientID %>").style.height = h;
                document.getElementById("<%=divReport.ClientID %>").style.width = w;

                document.getElementById("<%=rptViewDynamic.ClientID %>").style.height = h1;
            document.getElementById("<%=rptViewDynamic.ClientID %>").style.width = w2;
        }
    }

    window.onresize = resizeDiv;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);
</script>

    <style>
        #ctl00_divTop {
            margin-top: 0px !important;
        }

        .mainFilterPanel {
	        top: 0px !important;
        }

        #ctl00_ucTaskBarContent_divTaskBar {
	        visibility: hidden;
        }

        #ctl00_MainContent_divReport {
            overflow: auto !important;
        }

        #frmReports {
            margin-right: 11px; 
            width: 100%; 
            height: 500px;
        }
    </style>


    <webUc:ucFilterReport id="ucFilterReport" runat="server"/>

    <%-- Reporte --%>
    <div id="divReport" class="divControls" runat="server" visible="false" style="margin-left: 0px; margin-top:2px"  >
        <rsweb:ReportViewer ID="rptViewDynamic" runat="server" 
            ProcessingMode="Remote" AsyncRendering="false" SizeToReportContent="true" Height="100%" Width="100%" Style="margin-right: 11px" >      
        </rsweb:ReportViewer>
    </div>

    <div class="col-md-12">
         <iframe id="frmReports" title="Reportes"></iframe> 
    </div>
    <%-- FIN Reporte --%>
        
    <%-- Warning --%>
    <div id="divWarning" runat="server" class="divControls" visible="false" style="margin-left: 0px; margin-top:5px" >
        <asp:Label ID="lblTitleWarning" runat="server" ForeColor="#4682B4" Text="Recepción" Font-Size="20pt" Font-Bold="True" />
        <br />        
        <asp:Label ID="lblError" runat="server" ForeColor="Black" Text="No se han encontrado registros" />
    </div>
    <%-- FIN Warning --%>

    <asp:Label ID="lblTitleInfo" runat="server" Visible="false" Text="Reportes"></asp:Label>
    <asp:Label ID="lblCodeVirtual" runat="server" Visible="false" Text="Recepción"></asp:Label>
    <asp:Label ID="lblDescription" runat="server" Visible="false" Text="Doc. Ref."></asp:Label>
    <asp:Label ID="lblCodeTruck" runat="server" Text="Patente Camión" Visible="false"></asp:Label>
    <asp:Label ID="lblDate" runat="server" Text="Fecha" Visible="false"></asp:Label>
    <asp:Label ID="lblReceipt" runat="server" Text="Recepción" Visible="false"></asp:Label>  
    <asp:Label ID="lblErrorReceipt" runat="server" Text="Recepción no existe" Visible="false"></asp:Label>    
    <asp:Label ID="lblErrorReceiptFormat" Visible="false" runat="server" Text="El formato de recepción debe ser numérico" />
    <asp:Label ID="lblDispatch" runat="server" Visible="false" Text="Despacho"></asp:Label>
    <asp:Label ID="lblErrorDispatch" runat="server" Visible="false" Text="Despacho no existe"></asp:Label>
    <asp:Label ID="lblErrorReceptionYear" runat="server" Visible="false" Text="Año debe estar entre 2012 y 2030"></asp:Label>
    <asp:Label ID="lblErrorFormatYear" Visible="false" runat="server" Text="Año debe ser Numérico" />
    
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
