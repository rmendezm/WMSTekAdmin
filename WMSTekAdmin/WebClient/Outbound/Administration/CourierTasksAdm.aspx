<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="CourierTasksAdm.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.CourierTasksAdm" %>

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

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('GetDispatchSpecialHeaderABCDin', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>  
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnRowEditing="grdMgr_RowEditing"
                                            OnRowDeleting="grdMgr_RowDeleting"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="N° Doc" AccessibleHeaderText="OutboundOrderNumber" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblNumberDocumentBound" runat="server" text='<%# Eval ( "NumberDocumentBound" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="% Avance" AccessibleHeaderText="PercCompletion" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblPercCompletion" runat="server" text='<%# Eval ( "PercCompletion" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Estado" AccessibleHeaderText="NameTrackTaskQueue" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblNameTrackTaskQueue" runat="server" text='<%# Eval ( "TrackTaskQueue.NameTrackTaskQueue" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:TemplateField ShowHeader="False" HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center" AccessibleHeaderText="Actions">
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="width: 160px">
                                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png" CausesValidation="false" CommandName="Edit" />
                                                                <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" CausesValidation="false" CommandName="Delete" />
                                                            </div>
                                                        </center>
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
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="uprSelectedOrders" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrders" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrders" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
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
                                        <div class="col-md-9">
                                            <asp:Button ID="btnApiDetail" OnClick="btnApiDetail_Click" runat="server" Text="Detalle desde Enviame" CssClass="pull-right btn-detail" />
                                        </div>
                                    </div>
                                     <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="false">

                                                <Columns>

                                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnCode" runat="server" text='<%# Eval ( "LPN.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Cód. Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemShortName" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemShortName" runat="server" text='<%# Eval ( "Item.ShortName" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="Qty" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQty" runat="server" text='<%# Eval ( "Qty" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Categoria Item" AccessibleHeaderText="CategoryItemName" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCategoryItemName" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="Fifo">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFifo" runat="server" Text='<%# ((DateTime) Eval ("Fifo") > DateTime.MinValue)? Eval("Fifo", "{0:d}"):"" %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Expiración" AccessibleHeaderText="Expiration">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblExpiration" runat="server" Text='<%# ((DateTime) Eval ("Expiration") > DateTime.MinValue)? Eval("Expiration", "{0:d}"):"" %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="Fabrication">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFabrication" runat="server" Text='<%# ((DateTime) Eval ("Fabrication") > DateTime.MinValue)? Eval("Fabrication", "{0:d}"):"" %>' />
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

    <asp:UpdatePanel ID="upShowDetails" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divShowDetails" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpShowDetails" runat="server" TargetControlID="btnDummy"
                    BehaviorID="BIShowDetails" PopupControlID="pnlShowDetails" BackgroundCssClass="modalBackground"
                    PopupDragHandleControlID="ShowDetailsCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlShowDetails" runat="server" CssClass="modalBox" Style="display: none;">

                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Detalles" />
                        </div>
                    </asp:Panel>

                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">

                            <div id="divCustomer" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCustomer" runat="server" Text="Courier" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtCustomer" runat="server" Width="150px" Enabled="false" />
                                </div>
                            </div>

                            <div id="divAddress" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAddress" runat="server" Text="Dirección" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtAddress" runat="server" Width="300px" Enabled="false" />
                                </div>
                            </div>

                            <div id="divCity" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCity" runat="server" Text="Ciudad/Comuna" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtCity" runat="server" Width="150px" Enabled="false" />
                                </div>
                            </div>

                            <div id="divCarrier" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCarrier" runat="server" Text="Courier" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtCarrier" runat="server" Width="150px" Enabled="false" />
                                </div>
                            </div>

                            <div id="divStatusName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatusName" runat="server" Text="Último Estado" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtStatusName" runat="server" Width="150px" Enabled="false" />
                                </div>
                            </div>

                            <div id="divTracking" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTracking" Text="Historial" runat="server" />
                                </div>
                                <div class="fieldLeft">                 
                                    <asp:GridView ID="grdTracking" runat="server"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false"
                                        AutoGenerateColumns="False">
                                        <Columns>
                                            <asp:TemplateField HeaderText="status_id" ItemStyle-CssClass="text" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIdStatus" runat="server" text='<%# Eval ( "status_id" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Track" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblName" runat="server" text='<%# Eval ( "name" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Comentarios" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComment" runat="server" text='<%# Eval ( "comment" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Fecha Creación" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreatedAt" runat="server" text='<%# Eval ( "created_at" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <div id="divLabel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLabel" runat="server" Text="Ver Etiqueta" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:HyperLink ID="linkLabel" runat="server" Target="_blank" Text="Link Etiqueta" CssClass="linkDecoration"></asp:HyperLink>
                                </div>
                            </div>

                            <div id="divManifest" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblManifest" runat="server" Text="Ver Manifesto" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:HyperLink ID="linkManfiest" runat="server" Target="_blank" Text="Link Manifesto" CssClass="linkDecoration"></asp:HyperLink>
                                </div>
                            </div>

                        </div>
                        <div style="clear: both"></div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCerrar_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprShowDetails" runat="server" AssociatedUpdatePanelID="upShowDetails" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprShowDetails" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprShowDetails" />

    <asp:Label ID="lblConfirmCancel" runat="server" Text="¿Desea cancelar esta tarea?" Visible="false" />
    <asp:Label ID="lblRetryCancel" runat="server" Text="¿Desea reintentar esta tarea?" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
