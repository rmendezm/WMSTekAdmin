<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="RptConsolidateTruck.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.Dispatch.RptConsolidateTruck" %>

<%@ Register TagPrefix="webUc" TagName="ucFilterReport" Src="~/Shared/FilterReport.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            if ($("#ctl00_MainContent_divReport").length > 0) {
                var h = document.body.clientHeight + "px";
                var w = document.body.clientWidth + "px";
                var h1 = (document.body.clientHeight - 100) + "px";
                var w2 = (document.body.clientWidth - 10) + "px";
                document.getElementById("ctl00_MainContent_divReport").style.height = h;
                document.getElementById("ctl00_MainContent_divReport").style.width = w;

                document.getElementById("ctl00_MainContent_rptViewConsolidate").style.height = h1;
                document.getElementById("ctl00_MainContent_rptViewConsolidate").style.width = w2;
            }
        }
    
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        $(function () {
            if ($("#ctl00_divMainFilter").html().trim() == "") {
                $("#ctl00_divMainFilter").remove();
            }
        });

    </script>

    <style>
        #ctl00_MainContent_divReport{
            overflow: auto;
        }

            #ctl00_divTop {
            margin-top: 0px;          
        }
    </style>

    <webUc:ucFilterReport ID="ucFilterReport" runat="server" />    
     
    <%-- Reporte --%>     
    <div id="divReport" runat="server" visible="false"  style="width: 100%; height: 100%; margin: 0px; margin-bottom: 0px"> 

        <rsweb:ReportViewer ID="rptViewConsolidate" runat="server" ProcessingMode="Remote" ShowPrintButton="true" 
            AsyncRendering="true"   ExportContentDisposition="OnlyHtmlInline" SizeToReportContent="true" >
            <ServerReport ReportServerUrl="" />
        </rsweb:ReportViewer>
    
    </div>  


    <%-- FIN Reporte --%>
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <asp:Label ID="lblCodeTruck" runat="server" Text="Patente Camión" Visible="false"></asp:Label>
    <asp:Label ID="lblDate" runat="server" Text="Fecha" Visible="false"></asp:Label>
</asp:Content>