<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ReserveStockLog.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stocks.Consult.ReserveStockLog" %>

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
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('ReserveStockLog', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
            initializeGridWithNoDragAndDrop();
        }

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetail > .container > .row:first-child").height();
            var heightStatusBarPanel = $(".statusBarPanel").height();
            var extraSpace = 100;
            var divDetailFilter = $("#divFilterDetail").height();

            var totalHeight = heightDivBottom - heightLabelsBottom - heightStatusBarPanel - extraSpace - divDetailFilter;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");

            var heightDivTop = $("#hsMasterDetail_LeftP_Content").height();
            var heightDivPrinter = $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_divPrintLabel").height();
            var totalHeightTop = heightDivTop - heightDivPrinter;
            $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr").parent().css("max-height", totalHeightTop + "px");
        }

    </script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>

                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            EnableViewState="False"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">

                                                <Columns>

                                                    <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Cliente" AccessibleHeaderText="CustomerName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblCustomer" runat="server" text='<%# Eval ( "NameCustomer" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="ItemCode" AccessibleHeaderText="ItemCode" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblItemCode" runat="server" text='<%# Eval ( "ItemCode" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Item" AccessibleHeaderText="Item" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblItem" runat="server" text='<%# Eval ( "ShortNameItem" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Qty" AccessibleHeaderText="Qty" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Reserve") < 0 )? 0 : Eval ("Reserve")) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

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
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="120">
                <Content>                
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:Label ID="lblReserveStockSelected" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:Label ID="lblReserveStockSelected2" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">

                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnRowCreated="grdDetail_RowCreated"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="true"
                                                OnPageIndexChanging="grdDetail_PageIndexChanging">  

                                                <Columns>

                                                    <asp:templatefield HeaderText="Tipo" AccessibleHeaderText="Type" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblType" runat="server" text='<%# Eval ( "Type.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Documento" AccessibleHeaderText="Outbound" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutbound" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Cantidad" AccessibleHeaderText="Qty" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Qty") < 0 )? 0 : Eval ("Qty")) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Usuario Creador" AccessibleHeaderText="UserCreated" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblUserCreated" runat="server" text='<%# Eval ( "UserCreated" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Fecha Creación" AccessibleHeaderText="DateCreated" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblDateCreated" runat="server" text='<%# Eval ( "DateCreated" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Usuario Modificación" AccessibleHeaderText="UserModified" Visible="false" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblUserModified" runat="server" text='<%# Eval ( "UserModified" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Fecha Modificación" AccessibleHeaderText="DateModified" Visible="false" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblDateModified" runat="server" text='<%# ((DateTime) Eval ("DateModified") > DateTime.MinValue)? Eval("DateModified", "{0:d}"):"" %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                </Columns>

                                                <PagerSettings NextPageText="Siguiente" PreviousPageText="Anterior" Mode="NumericFirstLast" PageButtonCount="4"  FirstPageText="Primero" LastPageText="Último"/>
                                                <pagerstyle CssClass="pagination-ys" />

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
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" /> 
    <asp:Label id="lblCustomer" runat="server" Text="Nombre Cliente" Visible="false" /> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
