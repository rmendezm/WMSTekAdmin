<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="HangarMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.LogisticsResources.HangarMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="MainContent">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("hangar_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("hangar_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCreated="grdMgr_RowCreated" OnRowDeleting="grdMgr_RowDeleting" 
                                AllowPaging="True" EnableViewState="False" 
                                OnRowEditing="grdMgr_RowEditing"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="GLN" HeaderText="GLN" AccessibleHeaderText="GLN" />
                                    <asp:TemplateField HeaderText="Pos. X" AccessibleHeaderText="PositionX">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblPositionX" runat="server" Text='<%# ((int)Eval("PositionX") == -1) ? " " : Eval("PositionX")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pos. Y" AccessibleHeaderText="PositionY">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblPositionY" runat="server" Text='<%# ((int)Eval("PositionY") == -1) ? " " : Eval("PositionY")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pos. Z" AccessibleHeaderText="PositionZ">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblPositionZ" runat="server" Text='<%# ((int)Eval("PositionZ") == -1) ? " " : Eval("PositionZ")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largo" AccessibleHeaderText="Length">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblLength" runat="server" Text='<%# ((decimal)Eval("Length") == -1) ? " " : Eval("Length")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ancho" AccessibleHeaderText="Width">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblWidth" runat="server" Text='<%# ((decimal)Eval("Width") == -1) ? " " : Eval("Width")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" AccessibleHeaderText="Height">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblHeight" runat="server" Text='<%# ((decimal)Eval("Height") == -1) ? " " : Eval("Height")%>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="CodStatus">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                    Enabled="false" />
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
                                </Columns>
                            </asp:GridView>
                        </div>
                        <%-- FIN Grilla Principal --%>    
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
    
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%> 
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
                        
            <%-- Pop up Editar/Nuevo Hangar --%>
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" /><!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpeHangar" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlHangar" BackgroundCssClass="modalBackground" PopupDragHandleControlID="hangarCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlhangar" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="hangarCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Bodega" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Bodega" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>                        
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                            <div id="divCodStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCodStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkCodStatus" runat="server" /></div>
                            </div>
                            
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist."></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouse" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro Dist. es requerido" />
                                </div>
                            </div>
                                                            
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="80" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requrido" />
                                     <asp:RegularExpressionValidator ID="revtxtCode" runat="server" ControlToValidate="txtCode"
	                                     ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="60" Width="80" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>

                            <div id="divGLN" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblGLN" runat="server" Text="GLN" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtGLN" runat="server"  MaxLength="13" />
                                     <asp:RegularExpressionValidator ID="revtxtGLN" runat="server" 
                                        ControlToValidate="txtGLN" ErrorMessage="GLN permite ingresar solo números" ValidationGroup="EditNew" 
                                        ValidationExpression="[0-9999999999999]*" Text="*">
                                   </asp:RegularExpressionValidator>
                                </div>                       
                            </div>

                            <div id="divPositionX" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPositionX" runat="server" Text="Pos. X" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPositionX" runat="server" MaxLength="5" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPositionX" runat="server" ControlToValidate="txtPositionX"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición X es requerido" />
                                    <asp:RangeValidator ID="rvPosX" runat="server" ControlToValidate="txtPositionX" ErrorMessage="Posición X no contiene un número válido"
                                        MaximumValue="65535" MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*
                                    </asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divPositionY" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPositionY" runat="server" Text="Pos. Y" MaxLength="30" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPositionY" runat="server" MaxLength="5" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPositionY" runat="server" ControlToValidate="txtPositionY"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición Y es requerido" />
                                    <asp:RangeValidator ID="rvPosY" runat="server" ErrorMessage="Posición Y no contiene un número válido"
                                        ControlToValidate="txtPositionY" MaximumValue="65535" MinimumValue="0" Type="Integer"
                                        ValidationGroup="EditNew">*
                                    </asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divPositionZ" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPositionZ" runat="server" Text="Pos. Z" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPositionZ" runat="server" MaxLength="5" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPositionZ" runat="server" ControlToValidate="txtPositionZ"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Posición Z es requerido" />
                                    <asp:RangeValidator ID="rvPosZ" runat="server" ControlToValidate="txtPositionZ" ErrorMessage="Posición Z no contiene un número válido"
                                        MaximumValue ="65535" MinimumValue="0" ValidationGroup="EditNew" Type="Integer" Text = "*"></asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divLength" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLength" runat="server" Text="Largo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLength" runat="server" MaxLength="19" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvLength" runat="server" ControlToValidate="txtLength"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Largo es requerido" />
                                    <asp:RangeValidator ID="rvLength" runat="server" ControlToValidate="txtLength" Text=" * " ErrorMessage="Largo no contiene un número válido"
                                        MaximumValue="9999999999" MinimumValue="0" SetFocusOnError="True" Type="Double" ValidationGroup="EditNew">*</asp:RangeValidator>
                                </div>
                                
                            </div>
                            <div id="divWidth" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWidth" runat="server" Text="Ancho" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWidth" runat="server" MaxLength="19" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure2" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho es requerido" />
                                    <asp:RangeValidator ID="rvWidth" runat="server" ControlToValidate="txtWidth" CultureInvariantValues="True"
                                        Display="Dynamic" ErrorMessage="Ancho no contiene un número válido" MaximumValue="9999999999"
                                        MinimumValue="0" SetFocusOnError="True" Type="Double" ValidationGroup="EditNew">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divHeight" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHeight" runat="server" Text="Altura" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtHeight" runat="server" MaxLength="19" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure3" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvHeight" runat="server" ControlToValidate="txtHeight"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto es requerido" />
                                    <asp:RangeValidator ID="rvAltura" runat="server" ControlToValidate="txtHeight" CultureInvariantValues="True"
                                        ErrorMessage="Altura no contiene un número válido" MaximumValue="9999999999"
                                        MinimumValue="0" SetFocusOnError="True" Type="Double" ValidationGroup="EditNew">*</asp:RangeValidator>
                                </div>
                            </div>
                         </div>
                         <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"
                                ShowMessageBox="false" CssClass="modalValidation"/>
                         </div>                        
                        <%--Mensaje de advertencia--%>
                        <div id="divWarning" class="modalValidation" runat="server" visible="false">
                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Hay un error en el ingreso de los datos, por favor revise e intente denuevo. Los datos no se guardaron"
                                Visible="false" />
                            <asp:Label ID="lblErrorCode" runat="server" ForeColor="Red" Text="El código ya existe, por favor ingrese otro"
                                Visible="false" />
                       </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                            
                        </div>
                    </div> 
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Modal Update Progress --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Bodega?" Visible="false" />

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
