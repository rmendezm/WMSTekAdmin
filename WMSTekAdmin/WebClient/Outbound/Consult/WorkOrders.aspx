<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="WorkOrders.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.WorkOrders" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            //clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('B2BAdministration', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function setDivsAfter() {
            //var heightDivBottom = $("#__hsMasterDetailRD").height();
            //var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetail > .container > .row:first-child").height();
            //var heightStatusBarPanel = $(".statusBarPanel").height();
            //var extraSpace = 100;
            //var divDetailFilter = $("#divFilterDetail").height();

            //var totalHeight = heightDivBottom - heightLabelsBottom - heightStatusBarPanel - extraSpace - divDetailFilter;
            //$("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");

            //var heightDivTop = $("#hsMasterDetail_LeftP_Content").height();
            //var heightDivPrinter = $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_divPrintLabel").height();
            //var totalHeightTop = heightDivTop - heightDivPrinter;
            //$("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr").parent().css("max-height", totalHeightTop + "px");
        }

        function ShowProgress2() {

            if ($("#ctl00_MainContent_txtKeyField").val() != "" && $("#ctl00_MainContent_txtValueField").val() != "") {
                loadingAjaxStart();
            }
        }

        function ShowMessage(title, message) {

            var position = (document.body.clientWidth - 400) / 2 + "px";
            document.getElementById("divFondoPopup").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

            document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
            document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

            return false;
        }

        function HideMessage() {
            document.getElementById("divFondoPopup").style.display = 'none';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

            return false;
        }
    </script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                                    <ContentTemplate>
                                            <asp:GridView ID="grdMgr" 
                                                DataKeyNames="Id" 
                                                runat="server" 
                                                OnRowCreated="grdMgr_RowCreated"
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                                AllowPaging="True" 
                                                OnRowDataBound="grdMgr_RowDataBound"
                                                AutoGenerateColumns="false"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "InboundOrder.Owner.Name" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Nº Doc Salida" AccessibleHeaderText="OutboundNumber">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "InboundOrder.OutboundOrder.Number" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Tipo Doc Salida" AccessibleHeaderText="OutboundTypeName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "InboundOrder.OutboundOrder.OutboundType.Name" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Emisión Salida" AccessibleHeaderText="OutboundEmissionDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOutboundEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("InboundOrder.OutboundOrder.EmissionDate") > DateTime.MinValue) ? Eval("InboundOrder.OutboundOrder.EmissionDate", "{0:d}") : "" %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Nº Doc Entrada" AccessibleHeaderText="InboundNumber">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundNumber" runat="server" Text='<%# Eval ( "InboundOrder.Number" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Tipo Doc Entrada" AccessibleHeaderText="InboundTypeName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundTypeName" runat="server" Text='<%# Eval ( "InboundOrder.InboundType.Name" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Emisión Entrada" AccessibleHeaderText="InboundEmissionDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("InboundOrder.EmissionDate") > DateTime.MinValue) ? Eval("InboundOrder.EmissionDate", "{0:d}") : "" %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Tipo Recepción" AccessibleHeaderText="ReceiptTypeName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReceiptTypeName" runat="server" Text='<%# Eval ( "ReceiptTypeName" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Recepción" AccessibleHeaderText="ReceiptDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReceiptDate" runat="server" Text='<%# ((DateTime) Eval ("ReceiptDate") > DateTime.MinValue) ? Eval("ReceiptDate", "{0:d}") : "" %>'></asp:Label>
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

                                <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid">
                                    <ProgressTemplate>
                                        <div class="divProgress" style="z-index: 2147483647 !important;">
                                            <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                            </div>
                        </div>
                    </div>
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="120">
                <Content>   
                     <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-3">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Documento: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>  
                                        </div>
                                    </div>
                                    <div class="row">
                                         <div class="col-md-12">
                                             <asp:GridView ID="grdDetail" 
                                                 runat="server" 
                                                 SkinID="grdDetail"
                                                 DataKeyNames="Id" 
                                                 EnableViewState="True" 
                                                 AutoGenerateColumns="False"
                                                 OnRowCreated="grdDetail_RowCreated"
                                                 OnRowDataBound="grdDetail_RowDataBound"
                                                 CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                 EnableTheming="false"
                                                 AllowPaging="false">  

                                                    <Columns>

                                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Item.Description" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="N° Linea" AccessibleHeaderText="LineNumber">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLineNumber" runat="server" Text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Cant. Recibida" AccessibleHeaderText="Received">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblReceived" runat="server" Text='<%# Eval ( "Received" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "LPN.IdCode" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Categoria" AccessibleHeaderText="CtgName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue) ? Eval("FifoDate", "{0:d}") : "" %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Fecha Fabricación" AccessibleHeaderText="FabricationDate">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue) ? Eval("FabricationDate", "{0:d}") : "" %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Fecha Expiración" AccessibleHeaderText="ExpirationDate">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue) ? Eval("ExpirationDate", "{0:d}") : "" %>'></asp:Label>
                                                                </div>
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
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                        </Triggers>
                     </asp:UpdatePanel>
                </Content>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
