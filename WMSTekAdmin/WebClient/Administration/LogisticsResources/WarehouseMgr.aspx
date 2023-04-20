<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="WarehouseMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.LogisticsResources.WarehouseMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("Warehouse_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("Warehouse_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView 
                                ID="grdMgr" 
                                runat="server" 
                                DataKeyNames="Id" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCreated="grdMgr_RowCreated" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" 
                                AllowPaging="True" 
                                EnableViewState="false"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        SortExpression="Id" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="ShortName" HeaderText="Nombre Corto" AccessibleHeaderText="ShortName" />
                                    <asp:BoundField DataField="Address1" HeaderText="Dirección 1" AccessibleHeaderText="Address1" />
                                    <asp:BoundField DataField="Address2" HeaderText="Dirección 2" AccessibleHeaderText="Address2" />
                                    <asp:TemplateField HeaderText="País" AccessibleHeaderText="Country">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCountry" runat="server" Text='<%# Eval ( "Country.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región" AccessibleHeaderText="State">
                                        <ItemTemplate>
                                            <asp:Label ID="lblState" runat="server" Text='<%# Eval ( "State.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comuna" AccessibleHeaderText="City">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCity" runat="server" Text='<%# Eval ( "City.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Phone1" HeaderText="Teléfono 1" AccessibleHeaderText="Phone1" />
                                    <asp:BoundField DataField="Phone2" HeaderText="Teléfono 2" AccessibleHeaderText="Phone2" />
                                    <asp:BoundField DataField="Fax1" HeaderText="Fax 1" AccessibleHeaderText="Fax1" />
                                    <asp:BoundField DataField="Fax2" HeaderText="Fax 2" AccessibleHeaderText="Fax2" />
                                    <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
                                    <asp:BoundField DataField="ZipCode" HeaderText="Cód. Postal" AccessibleHeaderText="ZipCode" />
                                    <asp:BoundField DataField="GLN" HeaderText="GLN" AccessibleHeaderText="GLN" />
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "CodStatus" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                    CausesValidation="false" CommandName="Edit" />
                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                    CausesValidation="false" CommandName="Delete" />
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
            <%-- Pop up Editar/Nuevo Centro de Distribución --%>
            <div id="divEditNew" runat="server" visible="false">            
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpeWarehouse" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlWarehouse" BackgroundCssClass="modalBackground" PopupDragHandleControlID="WarehouseCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlWarehouse" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="WarehouseCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Centro de Distribución" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Centro de Distribución" />
                            <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top" ToolTip="Cerrar" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />                        </div>
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
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
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
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="100" Width="80" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divShortName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblShortName" runat="server" Text="Nombre Corto" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtShortName" runat="server" MaxLength="10" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvShortName" runat="server" ControlToValidate="txtShortName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre corto es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtShortName" runat="server" ControlToValidate="txtShortName"
	                                     ErrorMessage="Nombre Corto permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divAddress1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAddress1" runat="server" Text="Dirección 1" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtAddress1" runat="server" MaxLength="100" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección 1 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtAddress1" runat="server" ControlToValidate="txtAddress1"
	                                     ErrorMessage="Dirección 1 permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divAddress2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAddress2" runat="server" Text="Dirección 2" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtAddress2" runat="server" MaxLength="100" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvAddress2" runat="server" ControlToValidate="txtAddress2"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección 2 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtAddress2" runat="server" ControlToValidate="txtAddress2"
	                                     ErrorMessage="Dirección 2 permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="^[a-zA-Z0-9 ñáéíóú]+$" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>    
                                        
                                </div>
                            </div>
                            <div id="divCountry" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCountry" runat="server" Text="País"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="País es requerido" />
                                </div>
                            </div>
                            <div id="divState" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblState" runat="server" Text="Región"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Región es requerido" /></div>
                            </div>
                            <div id="divCity" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCity" runat="server" Text="Comuna"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCity" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Comuna es requerido" /></div>
                            </div>
                            <div id="divPhone1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPhone1" runat="server" Text="Teléfono 1" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPhone1" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPhone1" runat="server" ControlToValidate="txtPhone1"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono 1 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtPhone1" runat="server" ControlToValidate="txtPhone1"
	                                     ErrorMessage="Teléfono 1 permite ingresar solo números" 
	                                     ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divPhone2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPhone2" runat="server" Text="Teléfono 2" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPhone2" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPhone2" runat="server" ControlToValidate="txtPhone2"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono 2 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtPhone2" runat="server" ControlToValidate="txtPhone2"
	                                     ErrorMessage="Teléfono 2 permite ingresar solo números" 
	                                     ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divFax1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFax1" runat="server" Text="Fax 1" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFax1" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvFax1" runat="server" ControlToValidate="txtFax1"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax 1 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtFax1" runat="server" ControlToValidate="txtFax1"
	                                     ErrorMessage="Fax 1 permite ingresar solo números" 
	                                     ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divFax2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFax2" runat="server" Text="Fax 2" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFax2" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvFax2" runat="server" ControlToValidate="txtFax2"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax 2 es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtFax2" runat="server" ControlToValidate="txtFax2"
	                                     ErrorMessage="Fax 2 permite ingresar solo números" 
	                                     ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divEmail" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEmail" runat="server" Text="E-mail" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido" />
                                    <asp:RegularExpressionValidator ID="revMail" runat="server" ControlToValidate="txtEmail"
                                        ErrorMessage="E-mail Inválido" ValidationGroup="EditNew" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divZipCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblZipCode" runat="server" Text="Cód. Postal" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtZipCode" runat="server" MaxLength="20" />
                                    <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="txtZipCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código Postal es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtZipCode" runat="server" ControlToValidate="txtZipCode"
	                                     ErrorMessage="Cód. Postal permite ingresar solo números" 
	                                     ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * ">
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
            <%-- FIN Pop up Editar/Nuevo Centro de Distribución --%>            
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Centro de Distribución?" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Corto" Visible="false" />    
    <asp:Label ID="lblGlnIsNotNumeric" runat="server" Text="Código GLN debe ser Numérico." Visible="false" />
    <asp:Label ID="lblGlnLengthInvalid" runat="server" Text="Largo del Código GLN debe ser 13." Visible="false" />
    <asp:Label ID="lblGlnCheckDigit" runat="server" Text="Dígito Verificador del Código GLN no es Válido." Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares--%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
