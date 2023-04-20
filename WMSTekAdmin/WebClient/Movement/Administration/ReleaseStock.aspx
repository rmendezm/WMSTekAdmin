<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="ReleaseStock.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Administration.ReleaseStock" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("GetStockWithHoldCodeByFilters", "ctl00_MainContent_grdMgr");

        Sys.Application.add_init(appl_init);
    });

    function appl_init() {
        var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
        pgRegMgr.add_beginRequest(beforeAsyncPostBack);
        pgRegMgr.add_endRequest(afterAsyncPostBack);
    }

    function beforeAsyncPostBack() {

    }

    function afterAsyncPostBack() {
        initializeGridDragAndDrop("GetStockWithHoldCodeByFilters", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >  
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowEditing="grdMgr_RowEditing" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AllowPaging="True" 
                                EnableViewState="False" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:Templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                           <asp:Label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' ></asp:Label>
                                        </ItemTemplate>
                                     </asp:Templatefield>
                                         
                                    <asp:Templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                           <asp:Label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' ></asp:Label>
                                        </ItemTemplate>
                                     </asp:Templatefield>
                         
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:Templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield>         
                        
                                    <asp:Templatefield headertext="Nº Doc" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield>    
                        
                                    <asp:Templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFifo" runat="server" text='<%# Eval ("FifoDate") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield>                                                       
                                                                
                                    <asp:Templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield>
            
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                             

                                    <asp:Templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield>                                           
                                                               
                                   <asp:Templatefield headertext="Cantidad" accessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' ></asp:Label>
                                            </right>    
                                        </ItemTemplate>
                                    </asp:Templatefield>
                        
                                    <asp:TemplateField HeaderText="Cód. Bloqueo" AccessibleHeaderText="HoldCode" SortExpression="HoldCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHoldCode" runat="server" Text='<%# Eval("Hold") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                         
                                     <asp:TemplateField HeaderText="Bloqueo Stock" AccessibleHeaderText="HoldName" SortExpression="HoldName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHoldName" runat="server" Text='<%# Eval("HoldName") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <%--<asp:TemplateField HeaderText="Bloqueo Stock" AccessibleHeaderText="HoldName" SortExpression="HoldName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEstado" runat="server" Text='<%# Eval("Hold") %>' Visible = "false" />
                                            <asp:DropDownList ID="ddlEstado" runat="server"  AutoPostBack="true"  OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged" >
                                            </asp:DropDownList>                            
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                        
                                     <asp:Templatefield headertext="Lote" accessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text" />
                                    </asp:Templatefield>            
                        
                         
                                    <asp:Templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:label ID="lblFabricationDate" runat="server"  
                                                text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' ></asp:Label>
                                            </center>    
                                        </ItemTemplate>
                                    </asp:Templatefield> 
                           
                                    <asp:Templatefield headertext="Fifo" accessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                        <ItemTemplate>
                                            <asp:label ID="lblFifoDate" runat="server"  text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield> 
                        
                                      <asp:Templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <ItemTemplate>
                                            <asp:label ID="lblExpiration" runat="server"  text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:Templatefield> 
                        
                                    <asp:TemplateField HeaderText="Lpn" AccessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpn" runat="server" Text='<%# Eval("Lpn.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:BoundField DataField="IdLpnCodeContainer" HeaderText="Lpn Contenedor" AccessibleHeaderText="IdLpnCodeContainer" />
                                    <asp:BoundField DataField="LpnTypeCodeContainer" HeaderText="Tipo Lpn Contenedor" AccessibleHeaderText="LpnTypeCodeContainer" />
                                            
                                    <asp:TemplateField HeaderText="Ubicacion" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                        
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" ToolTip="Cambio Bloqueo" />
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
                        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                    </Triggers>    
                </asp:UpdatePanel>
            </div>
        </div>
    </div>  
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Grilla Principal --%>

    <%-- Pop up Editar Estado Stock --%>                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlVendor" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="VendorCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Stock" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">  
                            <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                          <%--Warehouse  --%>
                            <div id="divId" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdStock" runat="server" Text="ID."></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtId" runat="server" Text=""  Enabled="false"/>                     
                                </div>
                            </div>  
                            
                            <%--Warehouse  --%>
                            <div id="divWarehouseCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist."></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWarehouseCode" runat="server" Text="" Enabled="false"/>                     
                                </div>
                            </div>   
                            
                             <%-- Owner --%>
                            <div id="divOwnerCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtOwnerCode" runat="server" Text="" Enabled="false"/>                      
                                </div>
                            </div>  
                            
                             <%-- Location --%>
                            <div id="divLocation" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLocation" runat="server" Text="Ubicación"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLocation" runat="server" Text="" Enabled="false"/>                      
                                </div>
                            </div> 
                            
                            <%-- Lpn --%>
                            <div id="divIdLpnCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdLpnCodeNew" runat="server" Text="Lpn"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtIdLpnCode" runat="server" Text="" Enabled="false"/>                      
                                </div>
                            </div> 
                            
                             <%-- Item --%>
                            <div id="divItemCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblItemCode" runat="server" Text="Cód. Item"></asp:Label>
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtItemCode" runat="server" Text="" Enabled="false"/>                      
                                </div>
                            </div> 
                            
                            <%-- Lot --%>
                            <div id="divLotNumber" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLot" runat="server" Text="Lote"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLotNumber" runat="server" Text="" Enabled="false"/>                      
                                </div>
                            </div> 
                            
                            <%-- HoldCode --%>                            
                            <div id="divHoldCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHoldCode" runat="server" Text="Bloqueo Stock" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:DropDownList runat="server" ID="ddlHoldCode"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvHoldCode" runat="server" ControlToValidate="ddlHoldCode"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Estado Stock" />                                                              
                                </div>
                            </div>                    
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>                       
                        </div>                        
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
      </Triggers>
    </asp:UpdatePanel>  

    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Pop up Editar/Nuevo impresora --%>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblTitle" runat="server" Text="Liberar Stock" Visible="false" />
    <asp:Label id="lblLpn" runat="server" Text="Código Lpn" Visible="false" />
    <asp:Label id="lblReleaseStock" runat="server" Text="Liberar Stock" Visible="false" />
    <asp:Label id="lblFilterNameLpn" runat="server" Text="LPN" Visible="false" />
    <asp:Label id="lblFilterNameLot" runat="server" Text="Lote" Visible="false" />
    <asp:Label id="lblFilterNameReasonCode" runat="server" Text="Tipo Bloqueo" Visible="false" />
    <%--FIN Mensajes de Confirmacion y Auxiliares --%>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
