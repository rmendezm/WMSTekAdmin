﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="RptHistoricItemsPacked.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.RptHistoricItemsPacked" %>

<%@ Register TagPrefix="webUc" TagName="ucFilterReport" Src="~/Shared/FilterReport.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        $(function () {
            if ($("#ctl00_divMainFilter").html().trim() == "") {
                $("#ctl00_divMainFilter").remove();
            }
        });

        function resizeDiv() {
            if ($("#ctl00_MainContent_divReport").length > 0) {
                var body = document.body.clientHeight - 70;
                var h = body + "px";
                var w = document.body.clientWidth + "px";
                var h1 = (document.body.clientHeight - 100) + "px";
                var w2 = (document.body.clientWidth - 10) + "px";
                document.getElementById("<%=divReport.ClientID %>").style.height = h;
                document.getElementById("<%=divReport.ClientID %>").style.width = w;

                document.getElementById("<%=rptViewReceiptHistoric.ClientID %>").style.height = h1;
                document.getElementById("<%=rptViewReceiptHistoric.ClientID %>").style.width = w2;
            }
        }

        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

    </script>
    <style>
        #ctl00_MainContent_divReport{
            overflow: auto;
        }
    </style>
    <webUc:ucFilterReport ID="ucFilterReport" runat="server" />
    <%-- Reporte --%>
    <div id="divReport" runat="server" visible="false">
        <rsweb:ReportViewer ID="rptViewReceiptHistoric" runat="server" ProcessingMode="Remote" 
            AsyncRendering="true" Height="100%" Width="100%" SizeToReportContent="true" Style="margin-right: 11px" >
        </rsweb:ReportViewer>
    </div>
    <asp:Label ID="lblError2" Visible="False" runat="server" ForeColor="Red" 
        Text="" Font-Size="Small" />
    <div id="divWarning" runat="server" class="modalValidation" visible="false">
        <asp:Label ID="lblTitle1" runat="server" ForeColor="#4682B4" Text="Embalados" Font-Size="20pt" Font-Bold="True" />
        <br />    
        <asp:Label ID="lblError" runat="server" ForeColor="Black" Text="No se han encontrado registros" />
    </div>
    <%-- FIN Reporte --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
<%--    <asp:Label ID="lblCodeVirtual" runat="server" Visible="false" Text="Recepción"></asp:Label>
    <asp:Label ID="lblDescription" runat="server" Visible="false" Text="Doc. Ref."></asp:Label>--%>
</asp:Content>