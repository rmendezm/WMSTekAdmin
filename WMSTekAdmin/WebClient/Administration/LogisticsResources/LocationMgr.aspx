<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
CodeBehind="LocationMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.LogisticsResources.LocationMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
//    function resizeDivPrincipal() {
//        var h = document.body.clientHeight + "px";
//        var w = document.body.clientWidth + "px";
//        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
//        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;       
//    }

    window.onresize = SetDivs; 

    //    var prm = Sys.WebForms.PageRequestManager.getInstance();
    //    prm.add_endRequest(resizeDivPrincipal);

    $(document).ready(function () {
        initializeGridWithNoDragAndDrop(true);

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
        initializeGridWithNoDragAndDrop(true);
    }
</script>
<%--
<div id="divMainPrincipal" runat="server">--%>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <%-- Grilla Principal --%>
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" >
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="IdCode" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCreated="grdMgr_RowCreated" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" 
                                AllowPaging="True" EnableViewState="False" AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="IdCode" HeaderText="Ubicación" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="IdLocCode" />
                                    <asp:TemplateField HeaderText="Cód. Centro" AccessibleHeaderText="WarehouseCode">
                                        <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblShortWhsName" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="LocCode" />
                                    <asp:TemplateField HeaderText="Cód. Bodega" AccessibleHeaderText="HangarCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCodHangar" runat="server" Text='<%# Eval ( "Hangar.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bodega" AccessibleHeaderText="Hangar">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblHangarName" runat="server" Text='<%# Eval ( "Hangar.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fila" AccessibleHeaderText="RowLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "Row" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "Column" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "Level" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pasillo" AccessibleHeaderText="Aisle">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblAisle" runat="server" Text='<%# Eval ( "Aisle" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>                                
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="LocTypeCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLocTypeCode" runat="server" Text='<%# Eval ( "Type.LocTypeCode" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo PTL" AccessibleHeaderText="PtlTypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPtlTypeName" runat="server" Text='<%# Eval ( "PtlType.PtlTypeName" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Description" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comparte" AccessibleHeaderText="SharedItem">
                                        <ItemTemplate>
                                            <center>                                    
                                                <asp:CheckBox ID="chkSharedItem" runat="server" Checked='<%# Eval ( "SharedItem" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Usa LPN" AccessibleHeaderText="OnlyLPN">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkOnlyLPN" runat="server" Checked='<%# Eval ( "OnlyLPN" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
                                               </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño Único" AccessibleHeaderText="DedicatedOwner">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkDedicatedOwner" runat="server" Checked='<%# Eval ( "DedicatedOwner" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bloqueo" AccessibleHeaderText="HoldCode">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblHoldCode" runat="server" Text='<%# Eval("Reason.Name") %>'></asp:Label>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ruta Picking" AccessibleHeaderText="PickingFlow">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPickingFlow" runat="server" Text='<%# Eval ( "PickingFlow" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ruta Almacenaje" AccessibleHeaderText="PutawayFlow">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPutawayFlow" runat="server" Text='<%# ((int)Eval("PutawayFlow") == -1) ? " " : Eval("PutawayFlow")%>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPNs Max" AccessibleHeaderText="CapacityLPN">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCapacityLPN" runat="server" Text='<%# ((int)Eval("CapacityLPN") == -1) ? " " : Eval("CapacityLPN")%>' />
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Unid Max" AccessibleHeaderText="CapacityUnit">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCapacityUnit" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("CapacityUnit") == -1) ? " " : Eval("CapacityUnit"))%>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largo" AccessibleHeaderText="Length">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLength" runat="server" Text='<%# ((decimal)Eval("Length") == -1) ? " " : Eval("Length")%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ancho" AccessibleHeaderText="Width">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWidth" runat="server" Text='<%# ((decimal)Eval("Width") == -1) ? " " : Eval("Width")%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" AccessibleHeaderText="Height">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblHeight" runat="server" Text='<%# ((decimal)Eval("Height") == -1) ? " " : Eval("Height")%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="Volume">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblVolume" runat="server" Text='<%# ((decimal)Eval("Volume") == -1) ? " " : Eval("Volume")%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWeight" runat="server" Text='<%# ((decimal)Eval("Weight") == -1) ? " " : Eval("Weight")%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pos. X" AccessibleHeaderText="PositionX">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPositionX" runat="server" Text='<%# ((int)Eval("PositionX") == -1) ? " " : Eval("PositionX")%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pos. Y" AccessibleHeaderText="PositionY">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPositionY" runat="server" Text='<%# ((int)Eval("PositionY") == -1) ? " " : Eval("PositionY")%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pos. Z" AccessibleHeaderText="PositionZ">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPositionZ" runat="server" Text='<%# ((int)Eval("PositionZ") == -1) ? " " : Eval("PositionZ")%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bloqueo Inventario" AccessibleHeaderText="LockInventory">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkLockInventory" runat="server" Checked='<%# Eval ( "LockInventory" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. Razón" AccessibleHeaderText="ReasonCodeLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblReasonCodeLoc" runat="server" Text='<%# Eval ( "ReasonCodeLoc" ) %>' />
                                            </div>
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
        
    <%-- Pop up Editar/Nueva Ubicación --%>        
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlLocation" BackgroundCssClass="modalBackground" PopupDragHandleControlID="LocationCaption"
                    Drag="true" Enabled="true" >
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel ID="pnlLocation" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="LocationCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Ubicación" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Ubicación" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>    
                    <div class="modalControls">                
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIndex" runat="server" Value="-1" />
                        <ajaxToolkit:TabContainer runat="server" ID="tabLocation" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabGeneral">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divStatus" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblStatus" runat="server" Text="Activo" /></div>
                                            <div class="fieldLeft"><asp:CheckBox ID="chkStatus" runat="server" TabIndex="9" /></div>
                                        </div>                                       
                                        <div id="divWarehouse" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist."></asp:Label></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlWarehouse" runat="server" AutoPostBack="True" 
                                                    onselectedindexchanged="ddlWarehouse_SelectedIndexChanged" />
                                                <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro es requerido" />
                                            </div>
                                        </div>
                                        <div id="divHangar" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHangar" runat="server" Text="Bodega" />
                                            </div>
                                            <asp:DropDownList ID="ddlHangar" runat="server" Width="150px" TabIndex="2" />
                                            <asp:RequiredFieldValidator ID="rfvHangar" runat="server" ControlToValidate="ddlHangar"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Bodega es requerido" />
                                        </div>
                                        
                                        <div id="divOwner" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                            </div>
                                            <asp:DropDownList ID="ddlOwner" runat="server" Width="200px" TabIndex="14" />
                                            <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                        </div>  
                                        <div id="divLocTypeCode" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblLocTypeCode" runat="server" Text="Tipo" /></div>
                                            <asp:DropDownList ID="ddlLocTypeCode" runat="server" Width="150px" 
                                                TabIndex="10" AutoPostBack="True" 
                                                OnSelectedIndexChanged="ddlLocTypeCode_SelectedIndexChanged" />
                                            <asp:RequiredFieldValidator ID="rfvLocTypeCode" runat="server" ControlToValidate="ddlLocTypeCode"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido" />
                                        </div>
                                        
                                        <div id="divRowLoc" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblRowLoc" runat="server" Text="Fila" />
                                            </div>
                                            <asp:TextBox ID="txtRowLoc" runat="server" MaxLength="3" TabIndex="3" 
                                                Width="40px" />
                                            <asp:RequiredFieldValidator ID="rfvRowLoc" runat="server" ControlToValidate="txtRowLoc"
                                                ValidationGroup="EditNew" Text="*" ErrorMessage="Fila es requerido" />
                                            <asp:RangeValidator ID="rvRowLoc" runat="server" ControlToValidate="txtRowLoc" ErrorMessage="Fila no contiene un número válido"
                                                MaximumValue="999" MinimumValue="1" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            <asp:RequiredFieldValidator ID="rfvGeneratecode" runat="server" 
                                                ControlToValidate="txtRowLoc" 
                                                ErrorMessage="Debe ingresar la fila para generar el código" 
                                                ValidationGroup="GenerateCode">*</asp:RequiredFieldValidator>
                                        </div>
                                        <div id="divColumnLoc" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblColumnLoc" runat="server" Text="Columna" />
                                            </div>
                                            <asp:TextBox ID="txtColumnLoc" runat="server" MaxLength="2" Width="40px" 
                                                TabIndex="4" />
                                            <asp:RequiredFieldValidator ID="rfvColumnLoc" runat="server" ControlToValidate="txtColumnLoc"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna es requerido" />
                                            <asp:RangeValidator ID="rvColumnLoc" runat="server" ControlToValidate="txtColumnLoc"
                                                ErrorMessage="Columna no contiene un número válido" MaximumValue="99" MinimumValue="1"
                                                ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            <asp:RequiredFieldValidator ID="rfvGeneratecode0" runat="server" 
                                                ControlToValidate="txtColumnLoc" 
                                                ErrorMessage="Debe ingresar la columna para generar el código" 
                                                ValidationGroup="GenerateCode">*</asp:RequiredFieldValidator>
                                        </div>
                                        <div id="divLevelLoc" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLevelLoc" runat="server" Text="Nivel" />
                                            </div>
                                            <asp:TextBox ID="txtLevelLoc" runat="server" MaxLength="2" Width="40px" 
                                                TabIndex="5" />
                                            <asp:RequiredFieldValidator ID="rfvLevelLoc" runat="server" ControlToValidate="txtLevelLoc"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel es requerido" />
                                            <asp:RangeValidator ID="rvLevelLoc" runat="server" ControlToValidate="txtLevelLoc"
                                                ErrorMessage="Nivel no contiene un número válido" MaximumValue="99" MinimumValue="1"
                                                ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            <asp:RequiredFieldValidator ID="rfvGeneratecode1" runat="server" 
                                                ControlToValidate="txtLevelLoc" 
                                                ErrorMessage="Debe ingresar el nivel  para generar el código" 
                                                ValidationGroup="GenerateCode">*</asp:RequiredFieldValidator>
                                            
                                        </div>
                                        <div id="divSugerido" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                &nbsp;&nbsp;
                                            </div>
                                            <asp:CheckBox ID="chkGenerateCod" runat="server" AutoPostBack="True" OnCheckedChanged="chkGenerateCod_CheckedChanged"
                                                Text="Cód. Sugerido" TabIndex="5" TextAlign="Left" ValidationGroup="GenerateCode"/>
                                        </div>
                                        <div id="divLocCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLocCode" runat="server" Text="Código" />
                                            </div>
                                            <asp:TextBox ID="txtLocCode" runat="server" MaxLength="20" Width="100px" TabIndex="7" />
                                            <asp:RequiredFieldValidator ID="rfvLocCode" runat="server" ControlToValidate="txtLocCode" ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                        </div>
                                        <div id="divAisle" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblAisle" runat="server" Text="Pasillo" /></div>
                                            <asp:TextBox ID="txtAisle" runat="server" MaxLength="2" Width="20px" TabIndex="8" />
                                            <asp:RequiredFieldValidator ID="rfvAisle" runat="server" ControlToValidate="txtAisle"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Pasillo es requerido" />
                                            <asp:RegularExpressionValidator ID="revAisle" runat="server" 
                                                    ControlToValidate="txtAisle"
                                                    ErrorMessage="Pasillo permite ingresar solo caracteres alfanuméricos" 
                                                    ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
                                                    ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator> 
                                            <%--<asp:RangeValidator ID="rvAisle" runat="server" ControlToValidate="txtAisle" ErrorMessage="Pasillo no contiene un número válido"
                                                MaximumValue="99" MinimumValue="1" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>--%>
                                        </div>
                                        
                                        <div id="divDescription" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                            <asp:TextBox ID="txtDescription" runat="server" MaxLength="60" Width="150px" TabIndex="11" />
                                            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" />
                                        </div>
                                        <div id="divSharedItem" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSharedItem" runat="server" Text="Comparte" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkSharedItem" runat="server" TabIndex="12" /></div>
                                        </div>
                                        <div id="divOnlyLPN" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOnlyLPN" runat="server" Text="Usa LPN" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkOnlyLPN" runat="server" TabIndex="13" AutoPostBack="True" OnCheckedChanged="chkOnlyLPN_CheckedChanged" /></div>
                                        </div>
                                        <div id="divDedicatedOwner" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDedicatedOwner" runat="server" Text="Dueño Pr" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkDedicatedOwner" runat="server" TabIndex="15" /></div>
                                        </div>
                                        <div id="divLockInventory" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLockInventory" runat="server" Text="Bloq. Inventario" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkLockInventory" runat="server" TabIndex="16" /></div>
                                        </div>
                                    </div>
                                    <div class="divValidationSummary" >
                                        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"
                                            ShowMessageBox="True" CssClass="modalValidation"/>
                                    </div>    
                                    <div style="clear: both"></div>                                 
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            
                            <ajaxToolkit:TabPanel runat="server" ID="tabDetails">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divHoldCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHoldCode" runat="server" Text="Bloqueo" />
                                            </div>
                                            <asp:DropDownList ID="ddlHoldCode" runat="server" Width="90" />
                                        </div>
                                        <div id="divPickingFlow" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPickingFlow" runat="server" Text="Ruta Picking" />
                                            </div>
                                            <asp:TextBox ID="txtPickingFlow" runat="server" MaxLength="6" Width="80" TabIndex="16" onkeypress="return OnlyNumber(event,'Integer')"/>
                                            <asp:RequiredFieldValidator ID="rfvPickingFlow" runat="server" ControlToValidate="txtPickingFlow"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Ruta Picking es requerido" />
                                            <asp:RangeValidator ID="rvPickingFlow" runat="server" ControlToValidate="txtPickingFlow"
                                                ErrorMessage="Ruta Picking no contiene un número válido" MaximumValue="999999"
                                                MinimumValue="1" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divPutawayFlow" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPutawayFlow" runat="server" Text="Ruta Almacenaje" />
                                            </div>
                                            <asp:TextBox ID="txtPutawayFlow" runat="server" MaxLength="6" Width="80" TabIndex="17" onkeypress="return OnlyNumber(event,'Integer')"/>
                                            <asp:RequiredFieldValidator ID="rfvPutawayFlow" runat="server" ControlToValidate="txtPutawayFlow"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Ruta Almacenaje es requerido" />
                                            <asp:RangeValidator ID="rvPutawayFlow" runat="server" ControlToValidate="txtPutawayFlow"
                                                ErrorMessage="Ruta Almacenaje no contiene un número válido" MaximumValue="999999"
                                                MinimumValue="1" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divCapacityLPN" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCapacityLPN" runat="server" Text="LPNs máx." />
                                            </div>
                                            <asp:TextBox ID="txtCapacityLPN" runat="server" MaxLength="3" Width="80" TabIndex="18" onkeypress="return OnlyNumber(event,'Integer')"  />
                                            <asp:RequiredFieldValidator ID="rfvCapacityLPN" runat="server" ControlToValidate="txtCapacityLPN"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="LPNs máximos es requerido" />
                                            <asp:RangeValidator ID="rvCapacityLPN" runat="server" ControlToValidate="txtCapacityLPN"
                                                ErrorMessage="LPNs máx. no contiene un número válido" MaximumValue="999" MinimumValue="1"
                                                ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divCapacityUnit" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCapacityUnit" runat="server" Text="Unid máx." />
                                            </div>
                                            <asp:TextBox ID="txtCapacityUnit" runat="server" MaxLength="13" Width="90" TabIndex="19" onkeypress="return OnlyNumber(event,'Decimal')" />
                                            <asp:RequiredFieldValidator ID="rfvCapacityUnit" runat="server" ControlToValidate="txtCapacityUnit"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Unidades máximas es requerido" />
                                            <asp:RangeValidator ID="rvCapacityUnit" runat="server" ControlToValidate="txtCapacityUnit" ErrorMessage="Unid máx. no contiene un número válido, ej 99999999,9999"
                                                MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                        </div>
                                        <div id="divLength" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLength" runat="server" Text="Largo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLength" runat="server" MaxLength="13" TabIndex="20" Width="90" onkeypress="return OnlyNumber(event,'Decimal')" />
                                                <asp:RequiredFieldValidator ID="rfvLength" runat="server" ControlToValidate="txtLength"
                                                    ValidationGroup="EditNew" Text=" * " />
                                                <asp:RangeValidator ID="rvLength" runat="server" ControlToValidate="txtLength" ErrorMessage="Largo no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divWidth" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWidth" runat="server" Text="Ancho" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWidth" runat="server" MaxLength="13" TabIndex="21" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth"
                                                    ValidationGroup="EditNew" Text=" * " />
                                                <asp:RangeValidator ID="rvWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Ancho no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divHeight" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHeight" runat="server" Text="Alto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtHeight" runat="server" MaxLength="13" TabIndex="22" Width="120" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvHeight" runat="server" ControlToValidate="txtHeight"
                                                    ValidationGroup="EditNew" Text=" * " />
                                                <asp:RangeValidator ID="rvHeight" runat="server" ControlToValidate="txtHeight" ErrorMessage="Alto no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divVolume" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVolume" runat="server" Text="Volumen" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVolume" runat="server" MaxLength="13" TabIndex="23" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvVolume" runat="server" ControlToValidate="txtVolume"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Volumen es requerido" />
                                                <asp:RangeValidator ID="rvVolume" runat="server" ControlToValidate="txtVolume" ErrorMessage="Volumen no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divWeight" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWeight" runat="server" Text="Peso" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWeight" runat="server" MaxLength="13" TabIndex="24" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvWeight" runat="server" ControlToValidate="txtWeight"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Peso es requerido" />
                                                <asp:RangeValidator ID="rvWeight" runat="server" ControlToValidate="txtWeight" ErrorMessage="Peso no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divPositionX" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPositionX" runat="server" Text="Pos. X" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPositionX" runat="server" MaxLength="6" Width="90" TabIndex="25"  onkeypress="return OnlyNumber(event,'Integer')" />
                                                <asp:RequiredFieldValidator ID="rfvPositionX" runat="server" ControlToValidate="txtPositionX"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición X es requerido" />
                                                <asp:RangeValidator ID="rvPosX" runat="server" ControlToValidate="txtPositionX" ErrorMessage="Posición X no contiene un número válido"
                                                    MaximumValue="999999" MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*
                                                </asp:RangeValidator>
                                                
                                            </div>
                                        </div>
                                        <div id="divPositionY" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPositionY" runat="server" Text="Pos. Y" MaxLength="30" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPositionY" runat="server" MaxLength="6" Width="90" TabIndex="26" onkeypress="return OnlyNumber(event,'Integer')" />
                                                <asp:RequiredFieldValidator ID="rfvPositionY" runat="server" ControlToValidate="txtPositionY"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición Y es requerido" />
                                                <asp:RangeValidator ID="rvPosY" runat="server" ErrorMessage="Posición Y no contiene un número válido"
                                                    ControlToValidate="txtPositionY" MaximumValue="999999" MinimumValue="0" Type="Integer"
                                                    ValidationGroup="EditNew">*
                                                </asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divPositionZ" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPositionZ" runat="server" Text="Pos. Z" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPositionZ" runat="server" MaxLength="6" Width="90" TabIndex="27" onkeypress="return OnlyNumber(event,'Integer')" />
                                                <asp:RequiredFieldValidator ID="rfvPositionZ" runat="server" ControlToValidate="txtPositionZ"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición Z es requerido" />
                                                <asp:RangeValidator ID="rvPosZ" runat="server" ControlToValidate="txtPositionZ" ErrorMessage="Posición Z no contiene un número válido"
                                                    MaximumValue="999999" MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                            </div>
                                        </div>

                                        <div id="divPtlType" runat="server" class="divControls" visible="false">
                                            <div class="fieldRight"><asp:Label ID="lblPtlType" runat="server" Text="Tipo PTL" /></div>
                                            <asp:DropDownList ID="ddlPtlType" runat="server" Width="150px" TabIndex="10" />                                           
                                        </div>    

                                        <div id="divReasonCodeLoc" runat="server" class="divControls" visible="false">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblReasonCodeLoc" runat="server" Text="Razón" />
                                            </div>
                                            <asp:DropDownList ID="ddlReasonCodeLoc" runat="server" Width="200px" TabIndex="28" />
                                        </div>
                                    </div>
                                    <div style="clear: both"></div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            
                            <ajaxToolkit:TabPanel ID="tabWorkZones" runat="server" Visible="false">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlWorkZones" runat="server" />
                                            <asp:Button ID="btnAddWorkZone" runat="server" Text="Asignar" OnClick="btnAddWorkZone_Click" />
                                        </div>
                                    
                                        <div class="textLeft">
                                            <asp:Label ID="Label2" runat="server" Text="Zonas Asignadas:" />
                                            <br />
                                            <%-- Grilla de WorkZones asignadas al Item --%>
                                            <asp:GridView ID="grdWorkZones" runat="server" DataKeyNames="Id" ShowFooter="false"
                                                OnRowDeleting="grdWorkZones_RowDeleting" OnRowCreated="grdWorkZones_RowCreated"
                                                AllowPaging="False" ShowHeader="false"
                                                onrowdatabound="grdWorkZones_RowDataBound"
                                                AutoGenerateColumns="False"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                                    <asp:BoundField DataField="Name" HeaderText="Zona" AccessibleHeaderText="Name" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <%-- FIN Grilla de WorkZones asignadas al Item --%>
                                        </div>                                     
                                    
                                    </div>
                                    <div style="clear: both"></div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            
                            <ajaxToolkit:TabPanel ID="tabAssignPrinter" runat="server" Visible="false">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="div1" runat="server" class="divControls">
                                            <div class="fieldLeft">
                                                <asp:Label ID="lblProceso" runat="server" Text="Proceso" />
                                                <asp:DropDownList ID="ddlWmsProcess" runat="server"  />
                                            </div>
                                            
                                        </div>
                                        <div id="div2" runat="server" class="divControls">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlPrinters" runat="server" />
                                                <asp:Button ID="btnAddPrinters" runat="server" Text="Asignar" OnClick="btnAddPrinters_Click" />
                                            </div>
                                        </div>
                                        <div class="textLeft">
                                            <asp:Label ID="Label1" runat="server" Text="Impresoras Asignadas:" />
                                            <br />
                                            <%-- Grilla de Impresoras asignadas la ubicacion --%>
                                            <asp:GridView ID="grdPrinters" runat="server" DataKeyNames="Id" ShowFooter="false"
                                                OnRowDeleting="grdPrinters_RowDeleting" 
                                                OnRowCreated="grdPrinters_RowCreated"
                                                AllowPaging="False" ShowHeader="True"
                                                onrowdatabound="grdPrinters_RowDataBound"
                                                AutoGenerateColumns="False"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                                    <asp:BoundField DataField="Name" HeaderText="Impresora" AccessibleHeaderText="Name" />
                                                    <asp:BoundField DataField="WmsProcessName" HeaderText="Proceso" AccessibleHeaderText="WmsProcessName" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDeletePrinter" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <%-- FIN Grilla de Impresoras asignadas a la ubicacion --%>
                                        </div> 
                                    </div>
                                    <div style="clear: both"></div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            
                        </ajaxToolkit:TabContainer>
                        <%--Mensaje de advertencia--%>
                        <div id="divWarning" class="modalValidation" runat="server" visible="false">
                            <asp:Label ID="lblError" runat="server" Text="Hay un error en el ingreso de los datos, por favor revise e intente denuevo. Los datos no fueron almacenados"> </asp:Label>
                            <asp:Label ID="lblError2" Visible="false" runat="server" Text="Para generar el código debe seleccionar Centro Distr. <br/> y Bodega e ingresar valores numéricos en Fila, Columna y Nivel"> </asp:Label>
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
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
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
     <%-- FIN Pop up Editar/Nueva Ubicación --%>
     
     <%-- Pop up Crear Ubicaciones Por Rango--%>
     
    <asp:UpdatePanel ID="upNewRange" runat="server" UpdateMode="Conditional">
         <ContentTemplate>
            <div id="divNewRange" runat="server" visible="false">
                <asp:Button ID="btnDummyRange" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalRangePopUp" runat="server" TargetControlID="btnDummyRange"
                    PopupControlID="pnlLocationRange" BackgroundCssClass="modalBackground" PopupDragHandleControlID="LocationCaptionRange"
                    Drag="true" Enabled="true" >
                </ajaxToolkit:ModalPopupExtender>
                
                <asp:Panel ID="pnlLocationRange" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="LocationCaptionRange" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNewRange" runat="server" Text="Creación Ubicaciones por Rango" />
                            <asp:ImageButton ID="btnCloseRange" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <ajaxToolkit:TabContainer runat="server" ID="tabLocationRange" Height="350" Width="350" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabGeneralRange" HeaderText="General">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divStatusRange" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblStatusRange" runat="server" Text="Activo" /></div>
                                            <div class="fieldLeft"><asp:CheckBox ID="chkStatusRange" runat="server" TabIndex="0" /></div>
                                        </div>  
                                        <div id="divWarehouseRange" runat="server" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="lblWarehouseRange" runat="server" Text="Centro Dist."></asp:Label>
                                                </div>
                                                <div class="fieldLeft">
                                                    <asp:DropDownList ID="ddlWarehouseRange" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlWarehouseRange_SelectedIndexChanged" TabIndex ="1"/>
                                                    <asp:RequiredFieldValidator ID="rfvWarehouseRange" runat="server" ControlToValidate="ddlWarehouseRange"
                                                        InitialValue="-1" ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Centro es requerido" />
                                                </div>
                                            </div>
                                        <div id="divHangarRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHangarRange" runat="server" Text="Bodega" />
                                            </div>
                                            <asp:DropDownList ID="ddlHangarRange" runat="server" Width="150px" TabIndex="2" />
                                            <asp:RequiredFieldValidator ID="rfvHangarRange" runat="server" ControlToValidate="ddlHangarRange"
                                                InitialValue="-1" ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Bodega es requerido" />
                                        </div>
                                        <div id="divOwnerRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwnerRange" runat="server" Text="Dueño" />
                                            </div>
                                            <asp:DropDownList ID="ddlOwnerRange" runat="server" Width="200px" TabIndex="3" />
                                            <asp:RequiredFieldValidator ID="rfvOwnerRange" runat="server" ControlToValidate="ddlOwnerRange"
                                                InitialValue="-1" ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Dueño es requerido" />
                                        </div>  
                                        <div id="divLocTypeCodeRange" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblLocTypeCodeRange" runat="server" Text="Tipo" /></div>
                                            <asp:DropDownList ID="ddlLocTypeCodeRange" runat="server" Width="150px" TabIndex="4" AutoPostBack="true" OnSelectedIndexChanged="ddlLocTypeCodeRange_SelectedIndexChanged" />
                                            <asp:RequiredFieldValidator ID="rfvLocTypeCodeRange" runat="server" ControlToValidate="ddlLocTypeCodeRange"
                                                InitialValue="-1" ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Tipo es requerido" />
                                        </div>
                                        <div id="divRowLocRange" runat="server" class="divControls">
                                            <div class="fieldLeft">
                                                <asp:Label ID="lblRowLocRangeFrom" runat="server" Text="Fila: Desde" CssClass="fieldRight" />
                                                <asp:TextBox ID="txtRowLocRangeFrom" runat="server" MaxLength="3" TabIndex="5" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvRowLocRangeFrom" runat="server" ControlToValidate="txtRowLocRangeFrom" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Fila Desde es requerido" />
                                                <asp:RangeValidator ID="rvRowLocRangeFrom" runat="server" ControlToValidate="txtRowLocRangeFrom" ErrorMessage="Fila Desde no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblRowLocRangeTo" runat="server" Text="Hasta" CssClass="fieldLeft" />
                                                <asp:TextBox ID="txtRowLocRangeTo" runat="server" MaxLength="3" TabIndex="6" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvRowLocRangeTo" runat="server" ControlToValidate="txtRowLocRangeTo" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Fila Hasta es requerido" />
                                                <asp:RangeValidator ID="rvRowLocRangeTo" runat="server" ControlToValidate="txtRowLocRangeTo" ErrorMessage="Fila Hasta no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                                <asp:CompareValidator ID="cvRowLocRangeTo" runat="server" ControlToValidate="txtRowLocRangeTo" ControlToCompare="txtRowLocRangeFrom" Operator="GreaterThanEqual" Type="Integer" ErrorMessage="Hilera Hasta, debe ser mayor o igual a Nivel Inicial">*</asp:CompareValidator>
                                            </div>
                                        </div>
                                        <div id="divColumnLocRange" runat="server" class="divControls">
                                            <div class="fieldLeft">
                                                <asp:Label ID="lblColumnLocRangeFrom" runat="server" Text="Columna: Desde" CssClass="fieldRight" />
                                                <asp:TextBox ID="txtColumnLocRangeFrom" runat="server" MaxLength="3" TabIndex="7" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvColumnLocRangeFrom" runat="server" ControlToValidate="txtColumnLocRangeFrom" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Columna Desde es requerido" />
                                                <asp:RangeValidator ID="rvColumnLocRangeFrom" runat="server" ControlToValidate="txtColumnLocRangeFrom" ErrorMessage="Columna Desde no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblColumnLocRangeTo" runat="server" Text="Hasta" CssClass="fieldLeft" />
                                                <asp:TextBox ID="txtColumnLocRangeTo" runat="server" MaxLength="3" TabIndex="8" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvColumnLocRangeTo" runat="server" ControlToValidate="txtColumnLocRangeTo" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Columna Hasta es requerido" />
                                                <asp:RangeValidator ID="rvColumnLocRangeTo" runat="server" ControlToValidate="txtColumnLocRangeTo" ErrorMessage="Columna Hasta no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                                <asp:CompareValidator ID="cvColumnLocRangeTo" runat="server" ControlToValidate="txtColumnLocRangeTo" ControlToCompare="txtColumnLocRangeFrom" Operator="GreaterThanEqual" Type="Integer" ErrorMessage="Columna Hasta, debe ser mayor o igual a Nivel Inicial">*</asp:CompareValidator>
                                            </div>
                                        </div>
                                        <div id="divLevelLocRange" runat="server" class="divControls">
                                            <div class="fieldLeft">
                                                <asp:Label ID="lblLevelLocRangeFrom" runat="server" Text="Nivel: Desde" CssClass="fieldRight" />
                                                <asp:TextBox ID="txtLevelLocRangeFrom" runat="server" MaxLength="3" TabIndex="9" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvLevelLocRangeFrom" runat="server" ControlToValidate="txtLevelLocRangeFrom" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Nivel Desde es requerido" />
                                                <asp:RangeValidator ID="rvLevelLocRangeFrom" runat="server" ControlToValidate="txtColumnLocRangeFrom" ErrorMessage="Nivel Desde no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLevelLocRangeTo" runat="server" Text="Hasta" CssClass="fieldLeft" />
                                                <asp:TextBox ID="txtLevelLocRangeTo" runat="server" MaxLength="3" TabIndex="10" Width="40px" />
                                                <asp:RequiredFieldValidator ID="rfvLevelLocRangeTo" runat="server" ControlToValidate="txtLevelLocRangeTo" ValidationGroup="EditNewRange" Text="*" ErrorMessage="Nivel Hasta es requerido" />
                                                <asp:RangeValidator ID="rvLevelLocRangeTo" runat="server" ControlToValidate="txtLevelLocRangeTo" ErrorMessage="Nivel Hasta no contiene un número válido" MaximumValue="999" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                                <asp:CompareValidator ID="cvLevelLocRangeTo" runat="server" ControlToValidate="txtLevelLocRangeTo" ControlToCompare="txtLevelLocRangeFrom" Operator="GreaterThanEqual" Type="Integer" ErrorMessage="Nivel Hasta, debe ser mayor o igual a Nivel Inicial" ValidationGroup="EditNewRange">*</asp:CompareValidator>
                                            </div>
                                        </div>
                                        <div id="divAisleRange" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblAisleRange" runat="server" Text="Pasillo" /></div>
                                            <asp:TextBox ID="txtAisleRange" runat="server" MaxLength="2" Width="20px" TabIndex="11" />
                                            <asp:RequiredFieldValidator ID="rfvAisleRange" runat="server" ControlToValidate="txtAisleRange"
                                                ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Pasillo es requerido" />
                                            <asp:RegularExpressionValidator ID="revAisleRange" runat="server" 
                                                    ControlToValidate="txtAisleRange"
                                                    ErrorMessage="Pasillo permite ingresar solo caracteres alfanuméricos" 
                                                    ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                                    ValidationGroup="EditNewRange" Text=" * "></asp:RegularExpressionValidator>    
                                            <%--<asp:RangeValidator ID="rvAisleRange" runat="server" ControlToValidate="txtAisleRange" ErrorMessage="Pasillo no contiene un número válido"
                                                MaximumValue="99" MinimumValue="1" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>--%>
                                        </div>
                                        <div id="divDescriptionRange" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblDescriptionRange" runat="server" Text="Descripción" /></div>
                                            <asp:TextBox ID="txtDescriptionRange" runat="server" MaxLength="60" Width="150px" TabIndex="12" />
                                            <asp:RequiredFieldValidator ID="rfvDescriptionRange" runat="server" ControlToValidate="txtDescriptionRange"
                                                ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Descripción es requerido" />
                                        </div>
                                        <div id="divSharedItemRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSharedItemRange" runat="server" Text="Comparte" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkSharedItemRange" runat="server" TabIndex="13" /></div>
                                        </div>
                                        <div id="divOnlyLPNRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOnlyLPNRange" runat="server" Text="Usa LPN" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkOnlyLPNRange" runat="server" TabIndex="14" AutoPostBack="True" OnCheckedChanged="chkOnlyLPNRange_CheckedChanged" /></div>
                                        </div>
                                        <div id="divDedicatedOwnerRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDedicatedOwnerRange" runat="server" Text="Dueño Pr" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkDedicatedOwnerRange" runat="server" TabIndex="15" /></div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabDetailsRange" HeaderText="Detalles">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                    
                                        <div id="divPickingFlowRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPickingFlowRange" runat="server" Text="Ruta Picking(Inicial)" />
                                            </div>
                                            <asp:TextBox ID="txtPickingFlowRange" runat="server" MaxLength="6" Width="80" TabIndex="16" onkeypress="return OnlyNumber(event,'Integer')"/>
                                            <asp:RequiredFieldValidator ID="rfvPickingFlowRange" runat="server" ControlToValidate="txtPickingFlowRange"
                                                ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Ruta Picking(Inicial) es requerido" />
                                            <asp:RangeValidator ID="rvPickingFlowRange" runat="server" ControlToValidate="txtPickingFlowRange"
                                                ErrorMessage="Ruta Picking(Inicial) no contiene un número válido" MaximumValue="999999"
                                                MinimumValue="1" ValidationGroup="EditNewRange" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divPutawayFlowRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPutawayFlowRange" runat="server" Text="Ruta Almacenaje(Inicial)" />
                                            </div>
                                            <asp:TextBox ID="txtPutawayFlowRange" runat="server" MaxLength="6" Width="80" TabIndex="17" onkeypress="return OnlyNumber(event,'Integer')"/>
                                            <asp:RequiredFieldValidator ID="rfvPutawayFlowRange" runat="server" ControlToValidate="txtPutawayFlowRange"
                                                ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Ruta Almacenaje(Inicial) es requerido" />
                                            <asp:RangeValidator ID="rvPutawayFlowRange" runat="server" ControlToValidate="txtPutawayFlowRange"
                                                ErrorMessage="Ruta Almacenaje(Inicial) no contiene un número válido" MaximumValue="999999"
                                                MinimumValue="1" ValidationGroup="EditNewRange" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divCapacityLPNRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCapacityLPNRange" runat="server" Text="LPNs máx." />
                                            </div>
                                            <asp:TextBox ID="txtCapacityLPNRange" runat="server" MaxLength="3" Width="80" TabIndex="18" onkeypress="return OnlyNumber(event,'Integer')"  />
                                            <asp:RequiredFieldValidator ID="rfvCapacityLPNRange" runat="server" ControlToValidate="txtCapacityLPNRange"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="LPNs máximos es requerido" />
                                            <asp:RangeValidator ID="rvCapacityLPNRange" runat="server" ControlToValidate="txtCapacityLPNRange"
                                                ErrorMessage="LPNs máx. no contiene un número válido" MaximumValue="999" MinimumValue="1"
                                                ValidationGroup="EditNewRange" Type="Integer">*</asp:RangeValidator>
                                        </div>
                                        <div id="divCapacityUnitRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCapacityUnitRange" runat="server" Text="Unid máx." />
                                            </div>
                                            <asp:TextBox ID="txtCapacityUnitRange" runat="server" MaxLength="13" Width="90" TabIndex="19" onkeypress="return OnlyNumber(event,'Decimal')" />
                                            <asp:RequiredFieldValidator ID="rfvCapacityUnitRange" runat="server" ControlToValidate="txtCapacityUnitRange"
                                                ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Unidades máximas es requerido" />
                                            <asp:RangeValidator ID="rvCapacityUnitRange" runat="server" ControlToValidate="txtCapacityUnitRange" ErrorMessage="Unid máx. no contiene un número válido, ej 99999999,9999"
                                                MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                        </div>
                                        <div id="divLengthRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLengthRange" runat="server" Text="Largo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLengthRange" runat="server" MaxLength="13" TabIndex="20" Width="90" onkeypress="return OnlyNumber(event,'Decimal')" />
                                                <asp:RequiredFieldValidator ID="rfvLengthRange" runat="server" ControlToValidate="txtLengthRange"
                                                    ValidationGroup="EditNewRange" Text=" * " />
                                                <asp:RangeValidator ID="rvLengthRange" runat="server" ControlToValidate="txtLengthRange" ErrorMessage="Largo no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divWidthRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWidthRange" runat="server" Text="Ancho" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWidthRange" runat="server" MaxLength="13" TabIndex="21" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvWidthRange" runat="server" ControlToValidate="txtWidthRange"
                                                    ValidationGroup="EditNewRange" Text=" * " />
                                                <asp:RangeValidator ID="rvWidthRange" runat="server" ControlToValidate="txtWidthRange" ErrorMessage="Ancho no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divHeightRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHeightRange" runat="server" Text="Alto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtHeightRange" runat="server" MaxLength="13" TabIndex="22" Width="120" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvHeightRange" runat="server" ControlToValidate="txtHeightRange"
                                                    ValidationGroup="EditNew" Text=" * " />
                                                <asp:RangeValidator ID="rvHeightRange" runat="server" ControlToValidate="txtHeightRange" ErrorMessage="Alto no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divVolumeRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVolumeRange" runat="server" Text="Volumen" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVolumeRange" runat="server" MaxLength="13" TabIndex="23" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvVolumeRange" runat="server" ControlToValidate="txtVolumeRange"
                                                    ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Volumen es requerido" />
                                                <asp:RangeValidator ID="rvVolumeRange" runat="server" ControlToValidate="txtVolumeRange" ErrorMessage="Volumen no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divWeightRange" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWeightRange" runat="server" Text="Peso" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWeightRange" runat="server" MaxLength="13" TabIndex="24" Width="90" onkeypress="return OnlyNumber(event,'Decimal')"  />
                                                <asp:RequiredFieldValidator ID="rfvWeightRange" runat="server" ControlToValidate="txtWeightRange"
                                                    ValidationGroup="EditNewRange" Text=" * " ErrorMessage="Peso es requerido" />
                                                <asp:RangeValidator ID="rvWeightRange" runat="server" ControlToValidate="txtWeightRange" ErrorMessage="Peso no contiene un número válido, ej 99999999,9999"
                                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNewRange" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                        </ajaxToolkit:TabContainer>
                    <%--Mensaje de advertencia--%>
                        <div id="divWarningRange" class="modalValidation" runat="server" visible="false">
                            <asp:Label ID="lblErrorRange" runat="server" Text="Hay un error en el ingreso de los datos, por favor revise e intente denuevo. Los datos no fueron almacenados"> </asp:Label>
                            <asp:Label ID="lblErrorRange2" Visible="false" runat="server" Text="Para generar el código debe seleccionar Centro Distr. <br/> y Bodega e ingresar valores numéricos en Fila, Columna y Nivel"> </asp:Label>
                        </div>
                        <div id="divActionsRange" runat="server" class="modalActions">
                            <asp:Button ID="btnSaveRange" runat="server" OnClick="btnSaveRange_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNewRange" />
                            <asp:Button ID="btnCancelRange" runat="server" Text="Cancelar" />
                        </div>     
                    </div>
                </asp:Panel>
            </div>
         </ContentTemplate>
         <Triggers>
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
         </Triggers>
     </asp:UpdatePanel>
     
    <asp:UpdateProgress ID="uprNewRange" runat="server" AssociatedUpdatePanelID="upNewRange" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress22" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    
    <webUc:UpdateProgressOverlayExtender ID="muprNewRange" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprNewRange" />
      
    <%-- FIN Pop up Crear Ubicaciones Por Rango--%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Ubicación?" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Ubicación" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblDefaultText" runat="server" Text="Seleccione..." Visible="false" />
    
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />    
    <asp:Label ID="lbltabDetails" runat="server" Text="Detalles" Visible="false" />    
    <asp:Label ID="lbltabWorkZones" runat="server" Text="Zonas" Visible="false" />  
    <asp:Label ID="lbltabAssignPrinters" runat="server" Text="Impresoras" Visible="false" />  
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>   
                  
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
