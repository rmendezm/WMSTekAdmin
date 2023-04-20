<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="DispatchConfirm.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.DispatchConfirm" %>

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
            initializeGridDragAndDrop('DispatchConfirm', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <style>
        #ctl00_ucMainFilterContent_txtFilterDocumentNumber{
            width: 80px !important;
        }

        #ctl00_ucMainFilterContent_ddlFilterDispatchType, #ctl00_ucMainFilterContent_ddlFilterOutboundType {
            width: 120px !important;
        }

        #ctl00_ucMainFilterContent_ddlFilterOwner, #ctl00_ucMainFilterContent_ddlFilterWarehouse {
            width: 115px !important;
        }
    </style>

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
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectDispatch">
	                                                <HeaderTemplate>
		                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectDispatch', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                </HeaderTemplate>
	                                                <itemtemplate>
	                                                    <asp:CheckBox ID="chkSelectDispatch" runat="server"/>
	                                                </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="ID" AccessibleHeaderText="ID" Visible="false" >
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblIdDispatch" runat="server" text='<%# Eval ( "Id" ) %>' />
                                                        </div>
                                                    </itemtemplate>
                                                 </asp:templatefield>

                                                <asp:templatefield HeaderText="Cód. CD" AccessibleHeaderText="WarehouseCode">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                        </div>
                                                    </itemtemplate>
                                                 </asp:templatefield>

                                                 <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "OutboundOrder.Owner.Code" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="Owner" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="N° Doc." AccessibleHeaderText="OutboundOrderNumber" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblOutboundOrder" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Tipo Doc" AccessibleHeaderText="OutboundTypeName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblTypeDoc" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Code" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>
                                                    
                                                <asp:templatefield HeaderText="Traza" AccessibleHeaderText="DispatchType" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblDispatchType" runat="server" text='<%# Eval ( "DispatchType" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Cliente" AccessibleHeaderText="Customer" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblCustomerName" runat="server" text='<%# Eval ( "Customer.Name" ) %>' />
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
                            <div id="divDetail" runat="server" visible="false" clas="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-5">
                                            <asp:Label ID="lblDeleteByLpn" runat="server">Borrar Por LPN</asp:Label>
                                            <asp:ImageButton ID="imbDeleteReceipt" runat="server" Enabled="true" Height="24px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_cancel.png" OnClick="imbDeleteReceipt_Click" ToolTip="Eliminar detalle por LPN" Visible="false" />
                                            <asp:Label ID="lblDeleteByLine" runat="server">Borrar Por Linea</asp:Label>
                                            <asp:ImageButton ID="imbDeleteReceiptByLine" runat="server" Enabled="true" Height="24px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" OnClick="imbDeleteReceiptByLine_Click" ToolTip="Eliminar detalle por Linea" Visible="false" />
                                        </div>
                                        <div class="col-md-5">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Documento : " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/> 
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnDataBound="grdDetail_DataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="false">  

                                                <Columns>

                                                    <asp:templatefield HeaderText="Seleccionar por LPN" AccessibleHeaderText="SelectDispatchDetail">
	                                                    <HeaderTemplate>
		                                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdDetail.ClientID %>', 'chkSelectDispatchDetail', this.checked)" id="chkAll" title="Seleccionar todos los LPN" />
	                                                    </HeaderTemplate>
	                                                    <itemtemplate>
	                                                        <asp:CheckBox ID="chkSelectDispatchDetail" runat="server"/>
	                                                    </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Seleccionar por Linea" AccessibleHeaderText="SelectDispatchDetailByLine">
	                                                    <HeaderTemplate>
		                                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdDetail.ClientID %>', 'chkSelectDispDetailByLine', this.checked)" id="chkAllByLine" title="Seleccionar todas las lineas" />
	                                                    </HeaderTemplate>
	                                                    <itemtemplate>
	                                                        <asp:CheckBox ID="chkSelectDispDetailByLine" runat="server"/>
	                                                    </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnCode" runat="server" text='<%# Eval ( "LPN.IdCode" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnType" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnType" runat="server" text='<%# Eval ( "LPN.LPNType.Name" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="LPN Contenedor" AccessibleHeaderText="IdLpnCodeContainer" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdLpnCodeContainer" runat="server" text='<%# Eval ( "IdLpnCodeContainer" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Tipo LPN Contenedor" AccessibleHeaderText="LpnTypeCodeContainer" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnTypeCodeContainer" runat="server" text='<%# Eval ( "LpnTypeCodeContainer" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="N° Línea" AccessibleHeaderText="LineNumber" ItemStyle-CssClass="LineNumber">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLineNumber" runat="server" text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="CodeName" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ShortName" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemName" runat="server" text='<%# Eval ( "Item.ShortName" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Descripción Item" AccessibleHeaderText="Description" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDescriptionItem" runat="server" text='<%# Eval ( "Item.Description" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblNameCategoryItem" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" ItemStyle-CssClass="text">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblFifo" runat="server" Text='<%# ((DateTime) Eval ("Fifo") > DateTime.MinValue)? Eval("Fifo", "{0:d}"):"" %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield> 

                                                    <asp:templatefield headertext="Expiración" accessibleHeaderText="Expiration" ItemStyle-CssClass="text">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblExpiration" runat="server" Text='<%# ((DateTime) Eval ("Expiration") > DateTime.MinValue)? Eval("Expiration", "{0:d}"):"" %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield> 

                                                    <asp:templatefield headertext="Fabricación" accessibleHeaderText="Fabrication" ItemStyle-CssClass="text">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblFabrication" runat="server" Text='<%# ((DateTime) Eval ("Fabrication") > DateTime.MinValue)? Eval("Fabrication", "{0:d}"):"" %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield> 

                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="Qty" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQty" runat="server" text='<%# Eval ( "Qty" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="N° Sello" AccessibleHeaderText="SealNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSealNumber" runat="server" text='<%# Eval ( "SealNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Ubicación Stage" AccessibleHeaderText="StageCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblStageCode" runat="server" text='<%# Eval ( "Stage.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdDispatchDetail" ItemStyle-CssClass="text" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdDispatchDetail" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">       
        <ContentTemplate>   
            <div id="divConfirmPrin" runat="server">
                <asp:Button ID="btnDialogDummy" runat="Server" Style="display: none" /> 
                <ajaxToolKit:ModalPopupExtender 
	                ID="modalPopUpDialog" runat="server" TargetControlID="btnDialogDummy" 
	                PopupControlID="pnlDialog"  
	                BackgroundCssClass="modalBackground" 
	                PopupDragHandleControlID="Caption" Drag="true" >
	            </ajaxToolKit:ModalPopupExtender>
	            <asp:Panel ID="pnlDialog" runat="server" CssClass="modalBox" Width="400px">    	
		            <%-- Encabezado --%>    			
		            <asp:Panel ID="DialogHeader" runat="server" CssClass="modalHeader">
			            <div class="divCaption">
			                <asp:Label ID="lblDialogTitle" runat="server" />			    
                        </div>
	                </asp:Panel>
            	    
                    <div id="divDialogPanel" class="divDialogPanel" runat="server">
                        <div class="divDialogMessage">
                            <asp:Image id="imgDialogIcon" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
                        </div>
                        <div id="divDialogMessage" runat="server" class="divDialogMessage">                          
                        </div>
                        <div id="divConfirm" runat="server" class="divDialogButtons">
                            <asp:Button ID="btnOk" runat="server" Text="   Sí   " OnClick="btnOk_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="   No   " OnClick="btnCancel_Click" />
                        </div> 
                    </div>                     
                 </asp:Panel>
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />  
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   
        </Triggers>
    </asp:UpdatePanel>

    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblFilterReferenceNumber" runat="server" Text="Doc. de Referencia" Visible="false" />
    <asp:Label ID="lblAdvancedFilter" runat="server" Text="Filtros" Visible="false" />
    <asp:Label ID="lblNoLpnSelected" runat="server" Text="Debe seleccionar al menos un LPN" Visible="false" />
    <asp:Label ID="lblNoConfiguredDispatchesType" runat="server" Text="Debe seleccionar al menos un LPN" Visible="false" />
    <asp:Label ID="lblFilterLPN" runat="server" Text="LPN" Visible="false" />
    <asp:Label ID="lblBtnSaveToolTip" runat="server" Text="Confirmar Despacho" Visible ="false"  />
    <asp:Label ID="lblConfirmDispatchHeader" runat="server" Text="Confirmar Despacho" Visible ="false"  />
    <asp:Label ID="lblConfirmDispatch" runat="server" Text="¿Desea Confirmar Despachos Seleccionados?" Visible ="false"  />
    <asp:Label ID="lblNotSelectedDispatch" runat="server" Text="No existen despachos seleccionados" Visible ="false"  />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
