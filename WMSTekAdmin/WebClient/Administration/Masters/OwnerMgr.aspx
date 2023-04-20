<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="OwnerMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.OwnerMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content runat="server" ID="content1" Visible="true" ContentPlaceHolderID="MainContent">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("Owner_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("Owner_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" AutoGenerateColumns="False"
                                OnRowCreated="grdMgr_RowCreated" OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" AllowPaging="True" OnRowDataBound="grdMgr_RowDataBound"
                                EnableViewState="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="TradeName" HeaderText="Nombre Fantasía" AccessibleHeaderText="TradeName" />
                                    <asp:BoundField DataField="Address1" HeaderText="Dirección 1" AccessibleHeaderText="Address1" />
                                    <asp:BoundField DataField="Address2" HeaderText="Dirección 2" AccessibleHeaderText="Address2" />
                                    <asp:TemplateField HeaderText="País" AccessibleHeaderText="Country">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCountry" runat="server" Text='<%# Eval ("Country.Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="State">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblState" runat="server" Text='<%# Eval ("State.Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad" AccessibleHeaderText="City">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCity" runat="server" Text='<%# Eval ("City.Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:BoundField DataField="Phone1" HeaderText="Teléfono 1" AccessibleHeaderText="Phone1" />
                                    <asp:BoundField DataField="Phone2" HeaderText="Teléfono 2" AccessibleHeaderText="Phone2" />
                                    <asp:BoundField DataField="Fax1" HeaderText="Fax 1" AccessibleHeaderText="Fax1" />
                                    <asp:BoundField DataField="Fax2" HeaderText="Fax 2" AccessibleHeaderText="Fax2" />
                                    <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
                                    <asp:BoundField DataField="GLN" HeaderText="GLN" AccessibleHeaderText="GLN" />
                                    <asp:TemplateField HeaderText="Adm. Courier" AccessibleHeaderText="AllowCourier" SortExpression="AllowCourier">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkAllowCourier" runat="server" Checked='<%# Eval ( "AllowCourier" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CommandName="Delete" ToolTip="Eliminar" />
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
            <%-- Pop up Editar/Nuevo Owner --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlOwner" BackgroundCssClass="modalBackground" PopupDragHandleControlID="OwnerCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlOwner" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="OwnerCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Dueño" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Dueño" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIndex" runat="server" Value="-1" />
                         <ajaxToolkit:TabContainer runat="server" ID="tabOwner" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabGeneral">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divCode" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtCode" runat="server" ControlToValidate="txtCode"
	                                                  ErrorMessage="Código permite ingresar solo caracteres alfanuméricos." 
	                                                 ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
	                                                 ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divName" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtName" runat="server" MaxLength="60" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
                                                ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos." 
                                                 ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                                 ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divTradeName" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblTradeName" runat="server" Text="Nombre Fantasía" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtTradeName" runat="server" MaxLength="60" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvTradeName" runat="server" ControlToValidate="txtTradeName"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre de fantasía es requerido"/>
                                                <asp:RegularExpressionValidator ID="revtxtTradeName" runat="server" ControlToValidate="txtTradeName"
                                                ErrorMessage="Nombre de Fantasía permite ingresar solo caracteres alfanuméricos" 
                                                 ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                                 ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divAddress1" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblAddress1" runat="server" Text="Dirección 1" /></div>
                                            <div class="fieldLeft"><asp:TextBox ID="txtAddress1" runat="server" MaxLength="60" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtAddress1" runat="server" ControlToValidate="txtAddress1"
                                                    ErrorMessage="Dirección 1 permite ingresar solo caracteres alfanuméricos" 
                                                     ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ#_.-]*"
                                                     ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divAddress2" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblAddress2" runat="server" Text="Dirección 2" /></div>
                                            <div class="fieldLeft"><asp:TextBox ID="txtAddress2" runat="server" MaxLength="60" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvAddress2" runat="server" ControlToValidate="txtAddress2"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección 2 es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtAddress2" runat="server" ControlToValidate="txtAddress2"
                                                    ErrorMessage="Dirección 2 permite ingresar solo caracteres alfanuméricos" 
                                                    ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ#_.-]*"
                                                    ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divCountry" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblCountry" runat="server" Text="País"/></div>
                                            <div class="fieldLeft"><asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="País es requerido" InitialValue="-1" /></div>
                                        </div>
                                        <div id="divState" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblState" runat="server" Text="Región"/></div>
                                            <div class="fieldLeft"><asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged" />
                                            <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Región es requerida" InitialValue="-1" /></div>
                                        </div>
                                        <div id="divCity" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblCity" runat="server" Text="Ciudad"/></div>
                                            <div class="fieldLeft"><asp:DropDownList ID="ddlCity" runat="server" />
                                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Ciudad es requerido" InitialValue="-1" /></div>
                                        </div>
                                        <div id="divPhone1" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblPhone1" runat="server" Text="Teléfono 1" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPhone1" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rvfPhone1" runat="server" ControlToValidate="txtPhone1"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono 1 es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtPhone1" runat="server" ControlToValidate="txtPhone1"
                                                    ErrorMessage="Teléfono 1 permite ingresar solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>    
                                            </div>
                                        </div>
                                        <div id="divPhone2" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblPhone2" runat="server" Text="Teléfono 2" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPhone2" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rvfPhone2" runat="server" ControlToValidate="txtPhone2"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono 2 es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtPhone2" runat="server" ControlToValidate="txtPhone2"
                                                    ErrorMessage="Teléfono 2 debe permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divFax1" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblFax1" runat="server" Text="Fax 1" /></div>
                                            <div class="fieldLeft">                                
                                                <asp:TextBox ID="txtFax1" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvFax1" runat="server" ControlToValidate="txtFax1"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax 1 es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtFax1" runat="server" ControlToValidate="txtFax1"
                                                    ErrorMessage="Fax 1 debe permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divFax2" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblFax2" runat="server" Text="Fax 2" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFax2" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvFax2" runat="server" ControlToValidate="txtFax2"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax 2 es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtFax2" runat="server" ControlToValidate="txtFax2"
                                                    ErrorMessage="Fax 2 permite ingresar solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divEmail" runat="server" class="divControls">
                                            <div class="fieldRight"><asp:Label ID="lblEmail" runat="server" Text="E-mail" /></div>
                                            <div class="fieldLeft"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="150" />
                                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido" />                                
                                            <asp:RegularExpressionValidator ID="revMail" runat="server" ControlToValidate="txtEmail"
                                                ErrorMessage="Email Inválido" ValidationGroup="EditNew" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
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
                                        <div id="divCpurier" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="Courier" Text="Adm. Courier" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkAllowCourier" runat="server" Checked='<%# Eval ( "AllowCourier" ) %>' Enabled="true"
                                                    TabIndex="10" /></div>
                                        </div>
                                    </div>    
                                    <div class="divValidationSummary" >
                                        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false" CssClass="modalValidation"/>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>    
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabDocEntrada">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWhsInbound" runat="server" Text="Centro" />
                                            </div>
                                            <div class="fieldLeft">
                                                 <asp:DropDownList runat="server" ID="ddlWhsInbound" AutoPostBack="false">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvWhsInbound" runat="server" ControlToValidate="ddlWhsInbound"
                                                    InitialValue="-1" ValidationGroup="EditIn" Text=" * " ErrorMessage="Centro es requerido" />
                                            </div>
                                        </div>
                                    
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInboundType" runat="server" Text="Tipo Doc." />
                                            </div>
                                            <div class="fieldLeft">
                                                 <asp:DropDownList runat="server" ID="ddlInboundType" AutoPostBack="false">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvInboundType" runat="server" ControlToValidate="ddlInboundType"
                                                    InitialValue="-1" ValidationGroup="EditIn" Text=" * " ErrorMessage="Tipo Doc. Entrada es requerido" />
                                            </div>
                                        </div>
                                    
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLastInboundNumber" runat="server" Text="Correlativo" />
                                            </div>
                                            <div class="fieldLeft">                                
                                                <asp:TextBox ID="txtLastInboundNumber" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvLastInboundNumber" runat="server" ControlToValidate="txtLastInboundNumber"
                                                    ValidationGroup="EditIn" Text=" * " ErrorMessage="Correlativo Entrada es requerido" />
                                                <asp:RegularExpressionValidator ID="revLastInboundNumber" runat="server" ControlToValidate="txtLastInboundNumber"
                                                    ErrorMessage="Correlativo Entrada permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditIn" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>

                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblNumberLength" runat="server" Text="Largo" />
                                            </div>
                                            <div class="fieldLeft">                                
                                                <asp:TextBox ID="txtNumberLength" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvNumberLength" runat="server" ControlToValidate="txtNumberLength"
                                                    ValidationGroup="EditIn" Text=" * " ErrorMessage="Largo Entrada es requerido" />
                                                <asp:RegularExpressionValidator ID="revNumberLength" runat="server" ControlToValidate="txtNumberLength"
                                                    ErrorMessage="Largo Entrada permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditIn" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>

                                        <div  class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblIsCodePrefix" runat="server" Text="Usar Prefijo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkIsCodePrefix" runat="server" />
                                            </div>
                                             <div class="fieldRight">
                                                <asp:Button ID="btnAddNumberInboundOrde" runat="server" Text="Asignar" 
                                                    CausesValidation="true" ValidationGroup="EditIn" OnClick="btnAddNumberInboundOrde_Click" />
                                            </div>
                                        </div>
                                        
                                         <div  class="divControls">
                                         </div>

                                        <div class="textLeft">

                                            <asp:GridView ID="grdNumberInboundOrder" runat="server"   ShowFooter="false" 
                                                AllowPaging="false"
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdNumberInboundOrder_RowCreated" 
                                                OnRowDeleting="grdNumberInboundOrder_RowDeleting" 
                                                OnRowDataBound="grdNumberInboundOrder_RowDataBound"                                                
                                               CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                                <asp:HiddenField ID="hidWarehouseIdInbound" runat="server" Value='<%# Eval ( "Warehouse.Id" ) %>' />
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                   <%-- <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                     <asp:TemplateField AccessibleHeaderText="InboundTypeName" HeaderText="Tipo Doc.">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundTypeName" runat="server" Text='<%# Eval ("InboundType.Name") %>' />
                                                                <asp:HiddenField ID="hidInboundTypeCode" runat="server" Value='<%# Eval ( "InboundType.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="LastInboundNumber" HeaderText="Correlativo" AccessibleHeaderText="LastInboundNumber" />
                                                    <asp:BoundField DataField="NumberLength" HeaderText="Largo" AccessibleHeaderText="NumberLength" />
                                                    <asp:TemplateField HeaderText="Usar Prefijo" AccessibleHeaderText="IsCodePrefix">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:CheckBox ID="chkIsCodePrefix" runat="server" Checked='<%# ((int)Eval ( "IsCodePrefix" )== 0) ? false : true %>' Enabled="false" />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="false" CommandName="Delete" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" OnClientClick="return confirm('¿Desea eliminar correlativo entrada seleccionado?');" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    
                                    </div>
                                    <div class="divValidationSummary" >
                                        <asp:ValidationSummary ID="vsNumberInboundOrder" runat="server" ValidationGroup="EditIn" ShowMessageBox="false" CssClass="modalValidation"/>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                             <ajaxToolkit:TabPanel runat="server" ID="tabDocSalida">
                                <ContentTemplate>
                                     <div class="divCtrsFloatLeft">
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWhsOutbound" runat="server" Text="Centro" />
                                            </div>
                                            <div class="fieldLeft">
                                                 <asp:DropDownList runat="server" ID="ddlWhsOutbound" AutoPostBack="False">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvWhsOutbound" runat="server" ControlToValidate="ddlWhsOutbound"
                                                    InitialValue="-1" ValidationGroup="EditOut" Text=" * " ErrorMessage="Centro es requerido" />
                                            </div>
                                        </div>
                                    
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOutboundType" runat="server" Text="Tipo Doc." />
                                            </div>
                                            <div class="fieldLeft">
                                                 <asp:DropDownList runat="server" ID="ddlOutboundType" AutoPostBack="False">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvOutboundType" runat="server" ControlToValidate="ddlOutboundType"
                                                    InitialValue="-1" ValidationGroup="EditOut" Text=" * " ErrorMessage="Tipo Doc. Salida es requerido" />
                                            </div>
                                        </div>
                                    
                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLastOutboundNumber" runat="server" Text="Correlativo" />
                                            </div>
                                            <div class="fieldLeft">                                
                                                <asp:TextBox ID="txtLastOutboundNumber" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvLastOutboundNumber" runat="server" ControlToValidate="txtLastOutboundNumber"
                                                    ValidationGroup="EditOut" Text=" * " ErrorMessage="Correlativo Salida es requerido" />
                                                <asp:RegularExpressionValidator ID="revLastOutboundNumber" runat="server" ControlToValidate="txtLastOutboundNumber"
                                                    ErrorMessage="Correlativo Salida permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditOut" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>

                                        <div class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOutNumberLength" runat="server" Text="Largo" />
                                            </div>
                                            <div class="fieldLeft">                                
                                                <asp:TextBox ID="txtOutNumberLength" runat="server" MaxLength="20" Width="150" />
                                                <asp:RequiredFieldValidator ID="rfvOutNumberLength" runat="server" ControlToValidate="txtOutNumberLength"
                                                    ValidationGroup="EditOut" Text=" * " ErrorMessage="Largo Salida es requerido" />
                                                <asp:RegularExpressionValidator ID="revOutNumberLength" runat="server" ControlToValidate="txtOutNumberLength"
                                                    ErrorMessage="Largo Salida permite solo números" 
                                                    ValidationExpression="[0-99999999999]*" ValidationGroup="EditOut" Text=" * ">
                                                </asp:RegularExpressionValidator>
                                            </div>
                                        </div>

                                        <div  class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOutIsCodePrefix" runat="server" Text="Usar Prefijo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkOutIsCodePrefix" runat="server" />
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Button ID="btnAddNumberOutboundOrde" runat="server" Text="Asignar" CausesValidation="true" ValidationGroup="EditOut" 
                                                    OnClick="btnAddNumberOutboundOrde_Click" />
                                            </div>
                                        </div>

                                        <div  class="divControls">
                                        </div>

                                        <div class="textLeft">

                                            <asp:GridView ID="grdNumberOutboundOrder" runat="server"  ShowFooter="false" 
                                                AllowPaging="false"
                                                AutoGenerateColumns="False"
                                                OnRowDeleting="grdNumberOutboundOrder_RowDeleting" 
                                                OnRowDataBound="grdNumberOutboundOrder_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                                <asp:HiddenField ID="hidWarehouseIdOutbound" runat="server" Value='<%# Eval ( "Warehouse.Id" ) %>' />
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                  <%--                  <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                           </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                     <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Doc.">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundType.Name") %>' />
                                                                <asp:HiddenField ID="hidOutboundTypeCode" runat="server" Value='<%# Eval ( "OutboundType.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="LastOutboundNumber" HeaderText="Correlativo" AccessibleHeaderText="LastOutboundNumber" />
                                                    <asp:BoundField DataField="NumberLength" HeaderText="Largo" AccessibleHeaderText="NumberLength" />
                                                    <asp:TemplateField HeaderText="Usar Prefijo" AccessibleHeaderText="IsCodePrefix">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:CheckBox ID="chkIsCodePrefix" runat="server" Checked='<%#  ((int)Eval ( "IsCodePrefix" )== 0) ? false : true %>' Enabled="false" />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="false" CommandName="Delete" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" OnClientClick="return confirm('¿Desea eliminar correlativo salida seleccionado?');" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    
                                    </div>
                                     <div class="divValidationSummary" >
                                        <asp:ValidationSummary ID="vsNumberOutboundOrder" runat="server" ValidationGroup="EditOut" ShowMessageBox="false" CssClass="modalValidation"/>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                        </ajaxToolkit:TabContainer>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>
                    </div>    
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Owner --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Dueño?" Visible="false" />
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />    
    <asp:Label ID="lbltabDocEntrada" runat="server" Text="Correlativo Entrada" Visible="false" />  
    <asp:Label ID="lbltabDocSalida" runat="server" Text="Correlativo Salida" Visible="false" />  
    <asp:Label ID="lblExisteCorrelativo" runat="server" Text="Correlativo existente" Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
