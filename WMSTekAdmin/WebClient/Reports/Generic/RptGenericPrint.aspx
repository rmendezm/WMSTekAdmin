<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptGenericPrint.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.RptGenericPrint" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>WMSTek - Impresión Reporte</title>
</head>
<body>
    <form id="form1" runat="server" title="WMSTek - Impresión Reporte">
    <div>
    <%-- Reporte --%>
    <div id="divReport" runat="server" visible="false">
        <rsweb:ReportViewer ID="rptViewPrint" runat="server" 
            ProcessingMode="Remote" AsyncRendering="true"
            Height="610px" Width="100%" 
            style="margin-right: 11px" >
        </rsweb:ReportViewer>
   </div>
    <%-- FIN Reporte --%>
    </div>
    </form>
</body>
</html>
