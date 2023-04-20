<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="RptGeneric.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.Stock.RptGeneric" %>
<%@ Register TagPrefix="webUc" TagName="ucFilterReport" Src="~/Shared/FilterReport.ascx" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <webUc:ucFilterReport id="ucFilterReport" runat="server"/>
    <%-- Reporte --%>
    <div id="divReport" runat="server" visible="false">
        <rsweb:ReportViewer ID="rptViewStockByItem" runat="server" 
            ProcessingMode="Remote" AsyncRendering="true"
            Height="610px" Width="100%" 
            style="margin-right: 11px" >
            <ServerReport />
        </rsweb:ReportViewer>
   </div>
    <%-- FIN Reporte --%>
</asp:Content>


<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
