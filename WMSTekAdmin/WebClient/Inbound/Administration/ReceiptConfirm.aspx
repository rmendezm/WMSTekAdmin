<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReceiptConfirm.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Administration.ReceiptConfirm" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    //$("div.container:last div.row:first").remove();
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('ReceiptDetail_ById', gridDetail);
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
            var extraSpace = 160;

            var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
        }
    </script>

    <div id="divMainPrincipal" runat="server" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50" HeightDefault="500">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <%-- Grilla Principal --%>
                                        <div id="divGrid" runat="server" visible="true" onresize="SetDivs();" >
                                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id"  AutoGenerateColumns = "false"
                                                AllowPaging="True" EnableViewState="false" 
                                                OnRowCreated="grdMgr_RowCreated" 
                                                OnRowDataBound="grdMgr_RowDataBound"
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <%--<asp:CheckBox ID="chkSelectAll" runat="server" onclick="CheckAllEmp(this);" />--%>
                                                            <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkReceiptConfirm', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkReceiptConfirm" runat="server" Visible="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" ReadOnly="True"
                                                        SortExpression="Id" AccessibleHeaderText="Id" ItemStyle-Wrap="false">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "InboundOrder.Owner.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "InboundOrder.Owner.Name" ) %>' />
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Doc. Entrada" AccessibleHeaderText="InboundOrderNumber"
                                                        SortExpression="InboundOrder">
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundOrder" runat="server" Text='<%# Eval ( "InboundOrder.Number" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Tipo Recepción" AccessibleHeaderText="ReceiptTypeName" SortExpression="ReceiptTypeName"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReceiptTypeName" runat="server" Text='<%# Eval ( "ReceiptTypeName" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cód. Tipo Recep." AccessibleHeaderText="ReceiptTypeCode" SortExpression="ReceiptTypeCode"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReceiptTypeCode" runat="server" Text='<%# Eval ( "ReceiptTypeCode" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Proveedor" AccessibleHeaderText="Vendor" SortExpression="Vendor"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ( "InboundOrder.Vendor.Name" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Fecha Recepcion" AccessibleHeaderText="ReceiptDate" SortExpression="ReceiptDate">
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblReceiptDate" runat="server" Text='<%# ((DateTime) Eval ("ReceiptDate")) %>' />
                                                               </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>    
                                                    <asp:TemplateField HeaderText="Doc.Referencia" AccessibleHeaderText="ReferenceDoc" SortExpression="ReferenceDoc"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReferenceDoc" runat="server" Text='<%# Eval ( "ReferenceDoc" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Tipo Doc.Referencia" AccessibleHeaderText="ReferenceDocTypeName" SortExpression="ReferenceDocTypeName"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReferenceDocTypeName" runat="server" Text='<%# Eval ( "ReferenceDocTypeName" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                        
                                                    <asp:TemplateField HeaderText="Id.Ubicación" AccessibleHeaderText="IdLocationStage" SortExpression="IdLocationStage"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdLocationStage" runat="server" Text='<%# Eval ( "Stage.IdCode" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Usuario" AccessibleHeaderText="UserWms" SortExpression="UserWms"
                                                        HeaderStyle-HorizontalAlign="Left">
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblUserWms" runat="server" Text='<%# Eval ( "UserWms" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Observación" AccessibleHeaderText="SpecialField1">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "InboundOrder.SpecialField1" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <%-- FIN Grilla Principal --%>
                            
                                                        
                                        <%-- FIN Panel Cerrar Documento --%>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />  
                                        <%--<asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> --%>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" /> 
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" />   
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$imbDeleteReceipt" EventName="Click" /> 
                            
                                        <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> --%>            
                                    </Triggers>
                                </asp:UpdatePanel>
                                <%-- FIN Barra de Estado --%>
                            </div>
                        </div>
                    </div>  

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$upGrid"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop"
                    CssClass="updateProgress" TargetControlID="uprGrid" />
                    
                    <%-- FIN Modal Update Progress --%>
                 </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>     
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">           
                                 <%--   grilla detalle inicio--%>
                                 <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">       
                                        <ContentTemplate>                        
                                            <div id="divDetail" runat="server" visible="true" class="divGridDetailScroll">            
                                              <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                  <asp:ImageButton ID="imbDeleteReceipt" runat="server" Enabled="true" Height="24px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_cancel.png" OnClick="imbDeleteReceipt_Click" ToolTip="Eliminar detalle" Visible="false" />
	                                            <asp:Label ID="lblGridDetail" runat="server" visible="false" Text="Detalle Recepción: " />
	                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                              </div>
	                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
	                                                DataKeyNames="Id" EnableViewState="false"
	                                                OnRowCreated="grdDetail_RowCreated" 
                                                    OnRowDataBound="grdDetail_RowDataBound"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                 <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdDetail.ClientID %>','chkReceiptDetailConfirm', this.checked)" id="chkAllDetail" title="Seleccionar todos" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkReceiptDetailConfirm" runat="server" Visible="true" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                                                                SortExpression="Id" AccessibleHeaderText="Id" Visible="False" />
                     <%--                                       <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber"
                                                                SortExpression="LineNumber" ItemStyle-HorizontalAlign="Center" />
                                                            <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea" AccessibleHeaderText="LineCode" ItemStyle-CssClass="text"
                                                                SortExpression="LineCode" ItemStyle-HorizontalAlign="Center" />--%>
                                                            <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode"  ItemStyle-CssClass="text">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                        
                                                        <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                                            
                                                            <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="ItemDescription"
                                                                SortExpression="ItemDescription">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem" SortExpression="CategoryItemName">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                        
                                                            <asp:TemplateField HeaderText="Recibido" AccessibleHeaderText="Received" SortExpression="Received">
                                                               <ItemTemplate>
                                                                   <asp:Label ID="lblReceived" runat="server" Text='<%# GetFormatedNumber(Eval ("Received")) %>' />
                                                               </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <%--<asp:BoundField DataField="Received" HeaderText="Recibido" AccessibleHeaderText="Received"
                                                                SortExpression="Received" />--%>
                                            
                                                            <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" SortExpression="LotNumber">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ("LotNumber") %>' />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate">
                                                                <ItemStyle Wrap="false" />
                                                                <itemtemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                                        </div>
                                                                    </center>    
                                                            </itemtemplate>
                                                            </asp:templatefield>              
                                        
                                                            <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                                                <ItemStyle Wrap="false" />
                                                                <itemtemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                                        </div>
                                                                    </center>    
                                                            </itemtemplate>
                                                            </asp:templatefield> 
                                        
                                                            <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                                                <ItemStyle Wrap="false" />
                                                                <itemtemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                                       </div>
                                                                    </center>    
                                                            </itemtemplate>
                                                            </asp:templatefield>    
                                    
                                                            <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price" SortExpression="Price">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblPrice" runat="server" Text=' <%# ((decimal) Eval ("Price") == -1 )?" ": Eval ("Price") %>' />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                                           
                                                            <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblIdLpnCode" runat="server" Text=' <%# Eval ("LPN.IdCode") %>' />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                            </asp:GridView>
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
                       
                                           <%-- <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> --%>
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" />   
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
                                            <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   --%>
                                            <asp:AsyncPostBackTrigger ControlID="imbDeleteReceipt" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                 <%--fin grilla detalle--%>
                            </div>
                        </div>  
                    </div>
                    <%--<asp:Button ID="Button1" runat="server" Text="nada" />--%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
        
        
        <%-- Panel Cerrar Auditoria --%>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">       
        <ContentTemplate>   
            <div id="divMessaje" runat="server" visible="false" class="divItemDetails" >
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpMessaje" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlMessaje" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlMessaje" runat="server" CssClass="modalBox" Width="430px" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitleMessaje" runat="server" Text="Motivo Confirmación" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    
                    <div class="modalControls">
                        <div id="div3" style="width: 100%; margin-bottom: 6px;height: 100px;" >
                            <asp:HiddenField ID="hidIdReceiptMotive" runat="server" Value="-1" />
                            
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDoc" runat="server" Text="Recepción: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:Label ID="lblInboundOrder" runat="server" /></b>
                                </div>
                            </div>
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label1" runat="server" Text="Tipo Recepción: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:Label ID="lblReceiptType" runat="server" /></b>
                                </div>
                            </div>
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTypeRejection" runat="server" Text="Tipo Rechazo: " />
                                </div>
                                <div class="fieldLeft">
                                    <b> <asp:DropDownList ID="ddlTypeRejection" runat="server"></asp:DropDownList></b>
                                    <asp:RequiredFieldValidator ID="rfvTypeRejection" runat="server" InitialValue="-1"
                                        Text=" * " ErrorMessage="Tipo Rechazo es requerido" ControlToValidate="ddlTypeRejection" />
                                </div>
                            </div>
                            <div id="divMotiveRejection" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMotiveRejection" runat="server" Text="Motivo Rechazo: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:DropDownList ID="ddlMotiveRejection" runat="server"></asp:DropDownList></b>
                                    <asp:RequiredFieldValidator ID="rfvMotiveRejection" runat="server" InitialValue="-1"
                                         Text=" * " ErrorMessage="Motivo Rechazo es requerido" ControlToValidate="ddlMotiveRejection" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="divCtrsFloatLeft">       
                    </div>
                    <div style="clear:both" />                       
                    <div id="Div1" runat="server" class="modalActions">
                        <asp:Button ID="btnSaveConfirm" runat="server"  Text="Aceptar" CausesValidation="true" 
                          OnClick="btnSaveConfirm_Click" />
                    </div>                        
                </asp:Panel>
            </div>
            
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" />   
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   
        </Triggers>
        </asp:UpdatePanel>
        <%-- FIN Panel Cerrar Auditoria --%>
        
        
    </div>  
    
    
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Documento?" Visible="false" />
    <asp:Label ID="lblDetailOrder" runat="server" Text="¿Ver detalle" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Recepción" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Button ID="btnDetail" runat="server" Text="" OnClick="btnDetail_Click" />
    <asp:Label ID="lblConfirmReceipt" runat="server" Text="¿Desea Confirmar Recepciones Seleccionadas?" Visible ="false"  />
    <asp:Label ID="lblConfirmedReceipt" runat="server" Text="Recepciones Confirmadas: " Visible ="false"  />
    <asp:Label ID="lblConfirmReceiptHeader" runat="server" Text="Confirmar Recepción" Visible ="false"  />
    <asp:Label ID="lblBtnSaveToolTip" runat="server" Text="Confirmar Recepción" Visible ="false"  />
    <asp:Label ID="lblNotSelectedReceipt" runat="server" Text="No existen recepciones seleccionadas" Visible ="false"  />
    <asp:HiddenField ID="hdIndexGrd" runat="server" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    
    <script type="text/javascript" language="javascript">
        function ViewDetail(grd) {
            var index = grd.parentElement.parentElement.parentElement.parentElement.rowIndex;
            var btnDetail = document.getElementById("ctl00$MainContent$btnDetail");
            document.getElementById('ctl00$MainContent$hdIndexGrd').value = index-1;
            
            btnDetail.click();
            return false;
        }
        function CheckAllEmp(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdMgr.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }
        function resizeDivPrincipal() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
        }
    </script>
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
