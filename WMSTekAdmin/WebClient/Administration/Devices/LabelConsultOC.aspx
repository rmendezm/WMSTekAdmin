<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="LabelConsultOC.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.LabelConsultOC" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language='Javascript'>   
    function resizeDivCharts() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }

    window.onresize = resizeDivCharts;

    function clearFilterDetail(gridDetail) {
        if ($("#" + gridDetail).length == 0) {
            if ($("div.container").length == 2) {
                $("div.container:last div.row-height-filter").remove();
            }
        }
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdDetail';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('InBoundLabel_GetAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
    }

    function setDivsAfter() {
        var heightDivBottom = $("#__hsMasterDetailRD").height();
        var heightDetailChkFilter = $("#__hsMasterDetailRD .row-height-filter").height();
        var heightTitle = $("#ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_divDetailTitle").height();
        var heightCombos = $("#__hsMasterDetailRD .divCtrsFloatLeft").height();
        var extraSpace = 140;

        var totalHeight = heightDivBottom - heightDetailChkFilter - heightTitle - heightCombos - extraSpace;

        $("#ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
    }
</script> 

    <style>
        .divPrintLabel{
            height: 0 !important;
        }
    </style>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
           <TopPanel ID="topPanel" HeightMin="50">
             <Content>
                 <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                        
                    
                                <asp:GridView ID="grdMgr" runat="server" OnRowCreated="grdMgr_RowCreated"
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                                AllowPaging="True" EnableViewState="false"
                                                OnRowDataBound="grdMgr_RowDataBound"
                                                AutoGenerateColumns="false"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Id Inbound Order" AccessibleHeaderText="IdInboundOrder">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdInboundOrder" runat="server" Text='<%# Eval ( "IdInboundOrder" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Whs" AccessibleHeaderText="IdWhs">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "IdWhs" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Cod. CD" AccessibleHeaderText="WhsCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWhsCode" runat="server" Text='<%# Eval("WhsCode") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Nombre Corto CD" AccessibleHeaderText="ShortWhsName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblShortWhsName" runat="server" Text='<%# Eval ( "ShortWhsName" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "WhsName" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Id Own" AccessibleHeaderText="IdOwn">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval("IdOwn") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cod Dueño" AccessibleHeaderText="OwnCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnCode" runat="server" Text='<%# Eval ( "OwnCode" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "OwnName" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Nombre Comercial" AccessibleHeaderText="TradeName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblTradeName" runat="server" Text='<%# Eval("TradeName") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="N° Doc" AccessibleHeaderText="InboundNumber">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblInboundNumber" runat="server" Text='<%# Eval ( "InboundNumber" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id InBound Type" AccessibleHeaderText="IdInboundType">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdInboundType" runat="server" Text='<%# Eval ( "IdInboundType" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Tipo Doc" AccessibleHeaderText="InboundTypeCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblInboundTypeCode" runat="server" Text='<%# Eval("InboundTypeCode") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Comentario" AccessibleHeaderText="OrderComment">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOrderComment" runat="server" Text='<%# Eval ( "OrderComment" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Vendor" AccessibleHeaderText="IdVendor">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdVendor" runat="server" Text='<%# Eval ( "IdVendor" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Proveedor" AccessibleHeaderText="VendorName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ( "VendorName" ) %>'></asp:Label>
                                                            </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cod Proveedor" AccessibleHeaderText="VendorCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblVendorCode" runat="server" Text='<%# Eval ( "VendorCode" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="DateExpected">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblDateExpected" runat="server" Text='<%# Eval("DateExpected") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Emision" AccessibleHeaderText="EmissionDate">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblEmissionDate" runat="server" Text='<%# Eval ( "EmissionDate" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblExpirationDate" runat="server" Text='<%# Eval ( "ExpirationDate" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="Status">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id TrackInboundType" AccessibleHeaderText="IdTrackInboundType">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdTrackInboundType" runat="server" Text='<%# Eval ( "IdTrackInboundType" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Nom TrackInboundType" AccessibleHeaderText="NameTrackInboundType">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblNameTrackInboundType" runat="server" Text='<%# Eval ( "NameTrackInboundType" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Id OnBound Salida" AccessibleHeaderText="IdOutboundOrder">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdOutboundOrder" runat="server" Text='<%# Eval("IdOutboundOrder") %>'></asp:Label>
                                                            </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundNumber" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Es Asn" AccessibleHeaderText="IsAsn">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIsAsn" runat="server" Text='<%# Eval ( "IsAsn" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="% Lpn Inspeccion" AccessibleHeaderText="PercentLpnInspection">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblPercentLpnInspection" runat="server" Text='<%# Eval("PercentLpnInspection") %>'></asp:Label>
                                                            </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="% QA" AccessibleHeaderText="PercentQA">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblPercentQA" runat="server" Text='<%# Eval ( "PercentQA" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ShiftNumber" AccessibleHeaderText="ShiftNumber">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblShiftNumber" runat="server" Text='<%# Eval ( "ShiftNumber" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField1" AccessibleHeaderText="SpecialField1">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "SpecialField1" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="SpecialField2" AccessibleHeaderText="SpecialField2">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval("SpecialField2") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField3" AccessibleHeaderText="SpecialField3">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "SpecialField3" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField4" AccessibleHeaderText="SpecialField4">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "SpecialField4" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DateCreated" AccessibleHeaderText="DateCreated">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblDateCreated" runat="server" Text='<%# Eval ( "DateCreated" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UserCreated" AccessibleHeaderText="UserCreated">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ( "UserCreated" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DateModified" AccessibleHeaderText="DateModified">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblDateModified" runat="server" Text='<%# Eval ( "DateModified" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UserModified" AccessibleHeaderText="UserModified">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblUserModified" runat="server" Text='<%# Eval ( "UserModified" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />                        
                                </Triggers>
                            </asp:UpdatePanel>
                            <%-- FIN Panel Grilla Principal --%>
                            </div>
                        </div>
                    </div>
                </Content>
            </TopPanel>
            <BottomPanel ID="bottomPanel" HeightMin="50">
                <Content>
                    <%-- Panel Grilla Detalle --%>
                    <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                        <div ID="divPrintLabel" runat="server" class="divPrintLabel" visible="false">
                            <div class="divCtrsFloatLeft">
                                <div id="div1" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPrinter" runat="server" Text="Impresora" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:UpdatePanel ID="updPrinter" runat="server">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="ddlPrinters" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPrinters_Change" />
                                                <asp:DropDownList ID="ddlLabelSize" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvLabelSize" runat="server" ControlToValidate="ddlLabelSize" InitialValue="" ValidationGroup="valPrint" Text=" * " ErrorMessage="Tipo Etiqueta es requerido" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>    
                            </div>
                            <div class="divCtrsFloatLeft">
                                <%--<asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation"/>--%>
                                <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label>                              
                            </div> 
                        </div>
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                    <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                                    <asp:Label ID="lblNroDoc" runat="server" Text="" />
                                </div>
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server"
                                                        OnRowCreated="grdDetail_RowCreated" 
                                                        OnSelectedIndexChanged="grdDetail_SelectedIndexChanged"
                                                        OnRowEditing="grdMgr_RowEditing1" 
                                                        EnableViewState="false"
                                                        AllowPaging="False"
                                                        SkinID="grdDetail" 
                                                        AutoGenerateColumns="False"
                                                        OnRowDataBound="grdDetail_RowDataBound"
                                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                        EnableTheming="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Seleccions">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSeleccion" Checked='<%# Eval ( "Selected" ) %>' runat="server" ToolTip="Selección Imprimir"/>  
                                                            <%--AutoPostBack="True" OnCheckedChanged="chkSeleccion_CheckedChanged"--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Inbound Order Detail" AccessibleHeaderText="IdInboundDetail">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdInboundDetail" runat="server" Text='<%# Eval ( "IdInboundDetail" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Inbound Order" AccessibleHeaderText="IdInboundOrder">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdInboundOrder" runat="server" Text='<%# Eval ( "IdInboundOrder" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Número Línea" AccessibleHeaderText="LineNumber">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLineNumber" runat="server" Text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Codigo Linea" AccessibleHeaderText="LineCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLineCode" runat="server" Text='<%# Eval ( "LineCode" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "IdItem" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Description" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cod. Item" AccessibleHeaderText="ItemCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "ItemCode" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Id Categoria Linea" AccessibleHeaderText="IdCtgItem">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# Eval ( "IdCtgItem" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Nombre Categoría" AccessibleHeaderText="CtgName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "CtgName" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Solicitado" AccessibleHeaderText="ItemQty">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemQty" runat="server" Text='<%# Eval ( "ItemQty" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Recibido" AccessibleHeaderText="Received">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReceived" runat="server" Text='<%# Eval ( "Received" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="Status">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval ( "Status" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Linea Comentario" AccessibleHeaderText="LineComment">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLineComment" runat="server" Text='<%# Eval ( "LineComment" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:label ID="Label1" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# Eval ( "ExpirationDate" ) %>'></asp:Label>
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="FabricationDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# Eval ( "FabricationDate" ) %>'></asp:Label>
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Numero Lote" AccessibleHeaderText="LotNumber">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Código LPN" AccessibleHeaderText="LpnCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLpnCoder" runat="server" Text='<%# Eval ( "LpnCode" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblPrice" runat="server" Text='<%# Eval ( "Price" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="lblSpecialField1" AccessibleHeaderText="SpecialField1">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "SpecialField1" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField2" AccessibleHeaderText="SpecialField2">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval("SpecialField2") %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField3" AccessibleHeaderText="SpecialField3">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "SpecialField3" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SpecialField4" AccessibleHeaderText="SpecialField4">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">                                                
                                                                <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "SpecialField4" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DateCreated" AccessibleHeaderText="DateCreated">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblDateCreated" runat="server" Text='<%# Eval ( "DateCreated" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UserCreated" AccessibleHeaderText="UserCreated">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ( "UserCreated" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DateModified" AccessibleHeaderText="DateModified">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblDateModified" runat="server" Text='<%# Eval ( "DateModified" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UserModified" AccessibleHeaderText="UserModified">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblUserModified" runat="server" Text='<%# Eval ( "UserModified" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                        </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="Lote">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtLote"  Text='<%# Eval("LotNumber") %>' Visible='<%# Eval ( "UseLot" ) %>' Enabled='<%# Eval ( "Selected" ) %>' Width="80" MaxLength="20" runat="server"  ></asp:TextBox>
                                                           <%--<asp:RequiredFieldValidator ID="rfvLotRequired"  runat="server" ControlToValidate="txtLote" ValidationGroup="valPrint" Text=" * " ErrorMessage="Lote es requerido." />--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField> 
                                                     <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="Fabricacion">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtFabricacion"  Visible='<%# Eval ( "UseLot" ) %>' Text='<%# DisplayDate(Eval("FabricationDate")) %>' Width="70" runat="server" Enabled="false"></asp:TextBox>
                                                            <asp:ImageButton ID="imbFabricacion" runat="server" ImageUrl="/WebResources\Images\Buttons\Filter\cal_date_picker.gif"  />
                                                            <ajaxToolkit:CalendarExtender ID="CalExtFabricacion" runat="server" TargetControlID="txtFabricacion" Format="dd/MM/yyyy" PopupButtonID="imbFabricacion">
                                                            </ajaxToolkit:CalendarExtender>
                                                            <%--<asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtFabricacion"
	                                                             ErrorMessage="Formato no Válido" Text=" * " ValidationGroup="EditNew" 
	                                                             ValidationExpression="^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$" >
                                                            </asp:RegularExpressionValidator> --%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="Vencimiento">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtVencimiento" Visible='<%# Eval ( "UseLot" ) %>' Text='<%# DisplayDate(Eval("ExpirationDate")) %>' Width="70" runat="server" Enabled="False"></asp:TextBox>
                                                            <asp:ImageButton ID="imbVencimiento" runat="server" ImageUrl="/WebResources\Images\Buttons\Filter\cal_date_picker.gif"  />
                                                            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtVencimiento" Format="dd/MM/yyyy" PopupButtonID="imbVencimiento">
                                                            </ajaxToolkit:CalendarExtender>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:templatefield headertext="Copias" accessibleHeaderText="ItemQtyCopyas">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtCopias" Text='<%#((Decimal) Eval("ItemQty")>0)? Eval("ItemQty", "{0:0}"):"" %>' Enabled='<%# Eval ( "Selected" ) %>'  runat="server" MaxLength="5" Width="55"></asp:TextBox>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>   
                                                    <asp:TemplateField HeaderText="Agregar" AccessibleHeaderText="Agregar">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 60px">
                                                                    <asp:ImageButton ID="btnEdit" Enabled='<%# Eval ( "Selected" ) %>' Visible='<%# Eval ( "Selected" ) %>' runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add_on.png"
                                                                        CausesValidation="false" CommandName="Edit" ToolTip="Agregar"/>
                                                                     <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                                                        ShowMessageBox="false" CssClass="modalValidation"/>
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>           
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />                        
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$ctl01$grdDetail" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$ctl01$grdDetail" EventName="RowEditing" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <%-- FIN Panel Grilla Detalle --%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>                
        </spl:HorizontalSplitter>       
    </div>    

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblValidateLabelType" runat="server" Text="Tipo etiqueta no seleccionado" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
