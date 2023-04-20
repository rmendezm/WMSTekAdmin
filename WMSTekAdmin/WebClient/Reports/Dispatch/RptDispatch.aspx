<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="RptDispatch.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.Dispatch.RptDispatch" %>

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

                document.getElementById("<%=rptViewDispatch.ClientID %>").style.height = h1;
                document.getElementById("<%=rptViewDispatch.ClientID %>").style.width = w2;
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

         #ctl00_divTop {
         margin-top: 0px;          
        }
    </style>

    <webUc:ucFilterReport ID="ucFilterReport" runat="server" />
    <%-- Reporte --%>
    <div id="divReport" runat="server" visible="false">
        <rsweb:ReportViewer ID="rptViewDispatch" runat="server" ProcessingMode="Remote" AsyncRendering="false" SizeToReportContent="true"
            Height="100%" Width="100%" Style="margin-right: 11px">
            <ServerReport />
        </rsweb:ReportViewer>
    </div>
    <div id="divWarning" runat="server" class="modalValidation" visible="false">
        <asp:Label ID="lblTitle1" runat="server" ForeColor="#4682B4" Text="Despacho" Font-Size="20pt" Font-Bold="True" />
        <br />
        <asp:Label ID="lblError" runat="server" ForeColor="Black" Text="No se han encontrado registros" />
    </div>
    <%-- FIN Reporte --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
