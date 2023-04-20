<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="UserMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Profile.UserMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        initializeGridDragAndDrop("User_FindAll_Base", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("User_FindAll_Base", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <%-- Grilla Principal --%>
                    <div id="divGrid" runat="server" onresize="SetDivs();" >
                        <asp:GridView ID="grdMgr" 
                            runat="server" 
                            DataKeyNames="Id" 
                            OnRowDataBound="grdMgr_RowDataBound"
                            OnRowDeleting="grdMgr_RowDeleting"
                            OnRowEditing="grdMgr_RowEditing" 
                            OnPageIndexChanging="grdMgr_PageIndexChanging"
                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                            OnRowCreated="grdMgr_RowCreated"
                            AllowPaging="True" 
                            EnableViewState="False" 
                            AutoGenerateColumns="False"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                    SortExpression="Id" AccessibleHeaderText="IdUserWms" />
                                <asp:BoundField DataField="UserName" HeaderText="Usuario" AccessibleHeaderText="UserName" />
                                <asp:BoundField DataField="FirstName" HeaderText="Nombre" AccessibleHeaderText="FirstName" />
                                <asp:BoundField DataField="LastName" HeaderText="Apellido" AccessibleHeaderText="LastName" />
                                <asp:BoundField DataField="WorkPhone" HeaderText="Tel. Trabajo" AccessibleHeaderText="WorkPhone" />
                                <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
                                <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="CodStatus">
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "CodStatus" ) %>'
                                                Enabled="false" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Idioma" AccessibleHeaderText="Language">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLanguage" runat="server" Text='<%# Eval ( "Language.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="MobilePhone" HeaderText="Celular" AccessibleHeaderText="MobilePhone" />
                                <asp:BoundField DataField="HousePhone" HeaderText="Tel. Particular" AccessibleHeaderText="HousePhone" />
                                <asp:BoundField DataField="UserInternalCode" HeaderText="Cód. Interno" AccessibleHeaderText="UserInternalCode"
                                    ItemStyle-HorizontalAlign="Center">
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Capataz" AccessibleHeaderText="Foreman">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">    
                                            <asp:Label ID="lblForeman" runat="server" Text='<%# String.Concat(Eval ("Foreman.FirstName"), " ", Eval ("Foreman.LastName")) %>'
                                            Width="120px" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Comment" HeaderText="Comentarios" AccessibleHeaderText="Comment" />
                                <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                    <ItemTemplate>
                                        <div style="width: 60px">
                                            <center>
                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                    CommandName="Edit" ToolTip="Editar"/>
                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                    CommandName="Delete" ToolTip="Eliminar"/>
                                            </center>
                                        </div>
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
        
        <%-- Pop up Editar/Nuevo Usuario --%>
        <div id="divEditNew" runat="server" visible="false">        
            <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
            <!-- Boton 'dummy' para propiedad TargetControlID -->
            <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                PopupControlID="pnlUser" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                Drag="true">
            </ajaxToolkit:ModalPopupExtender>
            <%-- Encabezado --%>
            <asp:Panel ID="pnlUser" runat="server" CssClass="modalBox">
                <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                    <div class="divCaption">
                        <asp:Label ID="lblNew" runat="server" Text="Nuevo Usuario" />
                        <asp:Label ID="lblEdit" runat="server" Text="Editar Usuario" />
                        <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar" />
                    </div>
                </asp:Panel>
                <%-- Fin Encabezado --%>
                <div class="modalControls">        
                    <div runat="server" class="divCtrsFloatLeft" > 
                    <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                    <ajaxToolkit:TabContainer runat="server" ID="tabUser" ActiveTabIndex="0" Height="350px" Width="100%">
                        <ajaxToolkit:TabPanel runat="server" ID="tabLayout" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div id="divCodStatus" runat="server" class="divControls">
                                        <div class="fieldRight"><asp:Label ID="lblCodStatus" runat="server" Text="Activo" /></div>
                                        <div class="fieldLeft"><asp:CheckBox ID="chkCodStatus" runat="server" /></div>
                                    </div>

                                    <div id="divTypeUser" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblTypeUser" runat="server" Text="Tipo" />
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlTypeUser" runat="server" Width="100px" TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="ddlTypeUser_SelectedIndexChanged">
                                                <asp:ListItem Value="1">Bodega</asp:ListItem>
                                                <asp:ListItem Value="2">Web Service</asp:ListItem>
                                            </asp:DropDownList>

                                        </div>
                                    </div>

                                    <div id="divUserName" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblUserName" runat="server" Text="Usuario" />
                                        </div>
                                        <asp:TextBox ID="txtUserName" runat="server" MaxLength="30" Width="150px" TabIndex="1" />
                                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Usuario es requerido" />
                                        <asp:RegularExpressionValidator ID="revUserName" runat="server" ControlToValidate="txtUserName"
                                            ErrorMessage="Usuario permite ingresar solo caracteres alfanuméricos" ValidationExpression="[a-zA-Z0-9 ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
                                            ValidationGroup="EditNew" Text=" * ">
                                        </asp:RegularExpressionValidator>                                        
                                    </div>
                                    <div id="divFirstName" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblFirstName" runat="server" Text="Nombre" />
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtFirstName" runat="server" MaxLength="30" Width="150px" TabIndex="5" />
                                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                        <asp:RegularExpressionValidator ID="revFirstName" runat="server" ControlToValidate="txtFirstName"
                                            ErrorMessage="Debe ingresar solo letras de la A - Z o a - z" ValidationExpression="[a-zA-Z ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
                                            ValidationGroup="EditNew" Text=" * ">
                                        </asp:RegularExpressionValidator>                                                   
                                                </div>
                                    </div>
                                    <div id="divLastName" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblLastName" runat="server" Text="Apellido" />
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtLastName" runat="server" MaxLength="30" Width="150px" TabIndex="6" />
                                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Apellido es requerido" />
                                            <asp:RegularExpressionValidator ID="revLastName" runat="server" ControlToValidate="txtLastName"
                                                ErrorMessage="Debe ingresar solo letras de la A - Z o a - z" ValidationExpression="[a-zA-Z ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
                                                ValidationGroup="EditNew" Text=" * ">
                                            </asp:RegularExpressionValidator>                                                     
                                        </div>
                                    </div>
                                    <div id="divWorkPhone" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblWorkPhone" runat="server" Text="Tel. Trabajo" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtWorkPhone" runat="server" MaxLength="30" Width="150px" TabIndex="7" />
                                            <asp:RequiredFieldValidator ID="rfvWorkPhone" runat="server" ControlToValidate="txtWorkPhone"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Tel. Trabajo es requerido" />
                                            <asp:RegularExpressionValidator ID="revWorkPhone" runat="server" ControlToValidate="txtWorkPhone"
                                                ErrorMessage="Tel. Trabajo permite ingresar solo números" ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * " />
                                        </div>
                                    </div>
                                    <div id="divEmail" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblEmail" runat="server" Text="E-mail" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="30" Width="150px" TabIndex="8" />
                                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido" />
                                            <asp:RegularExpressionValidator ID="revMail" runat="server" 
                                                ControlToValidate="txtEmail" ErrorMessage="Email Inválido" ValidationGroup="EditNew" 
                                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
                                           </asp:RegularExpressionValidator>                                    
                                        </div>
                                    </div>
                                    <div id="divLanguage" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblLanguage" runat="server" Text="Idioma" />
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlLanguage" runat="server" Width="250px" TabIndex="9" />
                                            <asp:RequiredFieldValidator ID="rfvLanguage" runat="server" ControlToValidate="ddlLanguage"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Idioma es requerido" />
                                        </div>
                                    </div>
                                    <div id="divMobilePhone" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblMobilePhone" runat="server" Text="Celular" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtMobilePhone" runat="server" MaxLength="30" Width="150px" TabIndex="10" />
                                            <asp:RequiredFieldValidator ID="rfvMobilePhone" runat="server" ControlToValidate="txtMobilePhone" ValidationGroup="EditNew" Text=" * " ErrorMessage="Celular es requerido" />
                                            <asp:RegularExpressionValidator ID="revMobilePhone" runat="server" ControlToValidate="txtMobilePhone"
                                                ErrorMessage="Celular permite ingresar solo números" ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * " />
                                        </div>
                                    </div>
                                    <div id="divHousePhone" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblHousePhone" runat="server" Text="Tel. Particular" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtHousePhone" runat="server" MaxLength="30" Width="150px" TabIndex="11" />
                                            <asp:RequiredFieldValidator ID="rfvHousePhone" runat="server" ControlToValidate="txtHousePhone" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tel. Particular es requerido" />
                                            <asp:RegularExpressionValidator ID="revHousePhone" runat="server" ControlToValidate="txtHousePhone"
                                                ErrorMessage="Tel. Particular permite ingresar solo números" ValidationExpression="[0-9]*" ValidationGroup="EditNew" Text=" * " />
                                        </div>
                                    </div>
                                    <div id="divUserInternalCode" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblUserInternalCode" runat="server" Text="Cód. Interno" />
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtUserInternalCode" runat="server" MaxLength="30" Width="150px"
                                                TabIndex="12" />
                                            <asp:RequiredFieldValidator ID="rfvUserInternalCode" runat="server" ControlToValidate="txtUserInternalCode"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Cód. Interno es requerido" />
                                        </div>
                                    </div>
                                    <div id="divForeman" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblForeman" runat="server" Text="Capataz" /></div>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlForeman" runat="server" Width="250px" TabIndex="13" />
                                            <%--<asp:RequiredFieldValidator ID="rfvForeman" runat="server" ControlToValidate="ddlForeman"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Capataz es requerido" />--%>
                                        </div>
                                    </div>
                                    <div id="divComment" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblComment" runat="server" Text="Comentarios" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtComment" runat="server" MaxLength="300" Width="300px" Rows="3"
                                                TextMode="MultiLine" TabIndex="14" />
                                            <%--<asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="txtComment" ErrorMessage="Comentarios es requerido" ValidationGroup="EditNew" Text=" * " />--%>
                                        </div>
                                    </div>
                                </div>
                                
                                <div style="clear: both"></div>                                 
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tabWarehouse" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlWarehouse" runat="server" />
                                                <%--<asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro es requerido" />     --%>                                       
                                            <asp:Button ID="btnAddWarehouse" runat="server" Text="Asignar" OnClick="btnAddWarehouse_Click" />
                                        </div>   
                                        <div class="textLeft"> 
                                            <asp:Label ID="Label2" runat="server" Text="Centros Asignados:" />
                                            <br />
                                             <div style="overflow: auto; height: 300px; width: 250px;" >
                                                <asp:GridView ID="grdWarehouses" runat="server" ForeColor="#333333" OnRowDeleting="grdWarehouses_RowDeleting"
                                                DataKeyNames="Id" OnRowCommand="grdWarehouses_RowCommand"
                                                    EnableTheming="false"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop"
                                                    >
                                                <Columns>
                                                    <asp:BoundField DataField="Id" ShowHeader="False" Visible="False" />
                                                    <asp:BoundField DataField="ShortName" HeaderText="Centro" AccessibleHeaderText="ShortName" />
                                                    <asp:TemplateField HeaderText="Por Defecto">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:CheckBox ID="chkIsDefaultWarehouse" runat="server" Checked='<%# Eval ( "IsDefault" ) %>'
                                                                    Enabled="false" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Acción">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar"/>
                                                                <asp:ImageButton ID="btnIsDefault" runat="server" CausesValidation="False" CommandName="Select"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"  ToolTip="Por Defecto"/>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            </div>
                                            <asp:TextBox ID="txtDefaultWhs" runat="server" Text="0" Visible="false" />
                                            <asp:RequiredFieldValidator ID="rfvDefaultWhs" runat="server" ControlToValidate="txtDefaultWhs" InitialValue="-1" ValidationGroup="EditNew" ErrorMessage="Debe seleccionar una Centro por defecto" Text=" * "  />
                                        </div>
                                    </div>
                                </div>
                                <div style="clear: both"></div>   
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tabWorkZone" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlZone" runat="server" />
                                           <%-- <asp:RequiredFieldValidator ID="rfvZone" runat="server" ControlToValidate="ddlZone"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="La Zona es requerida" /> --%>
                                            <asp:Button ID="btnAddZone" runat="server" Text="Asignar" OnClick="btnAddZone_Click" />                                        
                                        </div>
                                        <div class="textLeft"> 
                                            <asp:Label ID="Label3" runat="server" Text="Zonas Asignadas:" />
                                            <br />
                                            <div style="overflow: auto; height: 300px; width: 200px;" >
                                                <asp:GridView ID="grdWorkZone" runat="server" ForeColor="#333333" DataKeyNames="Id"
                                                OnRowDeleting="grdWorkZone_RowDeleting"
                                                    EnableTheming="false"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" ShowHeader="False" Visible="False" />
                                                    <asp:BoundField DataField="Name" HeaderText="Zona" AccessibleHeaderText="Name" />
                                                    <asp:TemplateField HeaderText="Acción">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDeleteZone" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar Zona"/>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div style="clear: both"></div>   
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tabPrinter" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                           <asp:DropDownList ID="ddlPrinters" runat="server" />
                                           <asp:Button ID="btnAddPrinter" runat="server" Text="Asignar" OnClick="btnAddPrinter_Click" />
                                        <%--   <asp:RequiredFieldValidator ID="rfvPrinter" runat="server" ControlToValidate="ddlPrinters"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="La Impresora es requerida" />          --%>                                  
                                        </div>
                                        <div class="textLeft">                                         
                                            <asp:Label ID="Label4" runat="server" Text="Impresoras Asignadas:" />
                                            <br />
                                             <div style="overflow: auto; height: 300px; width: 250px;" >
                                                <asp:GridView ID="grdPrinter" runat="server" ForeColor="#333333" DataKeyNames="Id"
                                                    OnRowCommand="grdPrinter_RowCommand" OnRowDeleting="grdPrinter_RowDeleting"
                                                    EnableTheming="false"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" ShowHeader="False" Visible="False" />
                                                        <asp:BoundField DataField="Name" HeaderText="Impresora" AccessibleHeaderText="Name" />
                                                        <asp:TemplateField HeaderText="Por Defecto">
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:CheckBox ID="chkIsDefaultPrinter" runat="server" Checked='<%# Eval ( "IsDefault" ) %>'
                                                                        Enabled="false" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Acción">
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar"/>
                                                                    <asp:ImageButton ID="btnIsDefaultPrinter" runat="server" CausesValidation="False"
                                                                        CommandName="Select" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" ToolTip="Por Defecto"/>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                             </div>
                                            <asp:TextBox ID="txtDefaultPrinter" runat="server" Text="0" Visible="false" />
                                            <asp:RequiredFieldValidator ID="rfvDefaultPrinter" runat="server" ControlToValidate="txtDefaultPrinter" InitialValue="-1" ValidationGroup="EditNew" ErrorMessage="Debe seleccionar una Impresora por defecto" Text=" * "  />
                                        </div>
                                    </div>
                                </div>
                                <div style="clear: both"></div>   
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tabOwners" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlOwners" runat="server" />
<%--                                           <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwners"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="El Owner es requerido" />    --%>                                        
                                            <asp:Button ID="btnAddOwner" runat="server" Text="Asignar" OnClick="btnAddOwner_Click" />  
                                        </div>
                                        <div class="textLeft">   
                                            <asp:Label ID="Label5" runat="server" Text="Dueños Asignados:" />
                                            <br />                  
                                             <div style="overflow: auto; height: 300px; width: 250px;" >                                                                                                  
                                                <asp:GridView ID="grdOwner" runat="server" ForeColor="#333333" DataKeyNames="Id"
                                                OnRowCommand="grdOwner_RowCommand" OnRowDeleting="grdOwner_RowDeleting"
                                                    EnableTheming="false"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" ShowHeader="False" Visible="False" />
                                                    <asp:BoundField DataField="Name" HeaderText="Dueño" AccessibleHeaderText="Name" />
                                                    <asp:TemplateField HeaderText="Por Defecto">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:CheckBox ID="chkIsDefaultOwner" runat="server" Checked='<%# Eval ( "IsDefault" ) %>'
                                                                    Enabled="false" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Acción">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDeleteOwner" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar"/>
                                                                <asp:ImageButton ID="btnIsDefaultOwner" runat="server" CausesValidation="False" CommandName="Select"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" ToolTip="Por Defecto"/>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            </div>
                                            <asp:TextBox ID="txtDefaultOwner" runat="server" Text="0" Visible="false" />
                                            <%--<asp:RequiredFieldValidator ID="rfvDefaultOwner" runat="server" ControlToValidate="txtDefaultOwner" InitialValue="-1" ValidationGroup="EditNew" ErrorMessage="Debe seleccionar un Dueño por defecto" Text=" * "  />--%>
                                            
                                        </div>
                                    </div>
                                </div>
                                <div style="clear: both"></div> 
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tabVendors" Width="100%">
                            <ContentTemplate>
                                <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlVendors" runat="server" />
                                            <asp:Button ID="btnAddVendor" runat="server" Text="Asignar" OnClick="btnAddVendor_Click" />  
                                        </div>
                                        <div class="textLeft"> 
                                            <asp:Label ID="Label1" runat="server" Text="Proveedores Asignados:" />
                                            <br />  
                                            <div style="overflow: auto; height: 300px; width: 250px;" > 
                                                <asp:GridView ID="grdVendor" runat="server" ForeColor="#333333" DataKeyNames="Id"
						                            OnRowCommand="grdVendor_RowCommand" OnRowDeleting="grdVendor_RowDeleting"
							                            EnableTheming="false"
							                            AutoGenerateColumns="false"
							                            CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop">
						                            <Columns>
							                            <asp:BoundField DataField="Id" ShowHeader="False" Visible="False" />
							                            <asp:BoundField DataField="Name" HeaderText="Proveedor" AccessibleHeaderText="Name" />
							                            <asp:TemplateField HeaderText="Acción">
								                            <ItemTemplate>
									                            <center>
										                            <asp:ImageButton ID="btnDeleteVendor" runat="server" CausesValidation="False" CommandName="Delete"
											                            ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar"/>
									                            </center>
								                            </ItemTemplate>
							                            </asp:TemplateField>
						                            </Columns>
					                            </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div style="clear: both"></div> 
                             </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                    </div>
                    <%--Mensaje de advertencia--%>
                    <div  class="divCtrsFloatLeft">
                        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                            ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>  
                    <div id="divWarning" class="modalValidation" runat="server" visible="false">
                        <asp:Label ID="lblErrorWhs" runat="server" Text="* Debe seleccionar un centro" Visible="false"> </asp:Label>    
                        <asp:Label ID="lblErrorOwn" runat="server" Text="* Debe seleccionar un dueño"  Visible="false"></asp:Label>  
                        <asp:Label ID="lblErrorCDAsig"    runat="server" Text="* Debe asignar un centro por defecto"  Visible="false"></asp:Label> 
                        <asp:Label ID="lblErrorPrintAsig" runat="server" Text="* Debe asignar una impresora por defecto"  Visible="false"></asp:Label>   
                        <asp:Label ID="lblErrorOwnerAsig" runat="server" Text="* Debe seleccionar un dueño por defecto"  Visible="false"></asp:Label>      
                        <asp:Label ID="lblErrorLenguageAsig" runat="server" Text="* Debe seleccionar el idioma del usuario"  Visible="false"></asp:Label> 
                        <asp:Label ID="lblErrorMustSelectAnOwner" runat="server" Text="* Debe seleccionar un dueño"  Visible="false"></asp:Label>  
                    </div>
                    <div id="divActions" runat="server" class="modalActions">
                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                            ValidationGroup="EditNew" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                    </div>                  
                    <%-- Fin Mensajes de error --%>
                </div>
            </asp:Panel>
            </div>
            <%-- FINPop up Editar/Nuevo Usuario --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Usuario?" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Usuario" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Apellido" Visible="false" />    
    
    
    <asp:Label ID="lbltabLayout" runat="server" Text="Datos Generales" Visible="false" />    
    <asp:Label ID="lbltabWarehouse" runat="server" Text="Centro" Visible="false" />    
    <asp:Label ID="lbltabWorkZone" runat="server" Text="Zona" Visible="false" />    
    <asp:Label ID="lbltabPrinter" runat="server" Text="Impresora" Visible="false" />    
    <asp:Label ID="lbltabOwners" runat="server" Text="Dueño" Visible="false" />   
    <asp:Label ID="lblTabVendors" runat="server" Text="Proveedor" Visible="false" /> 
    <asp:Label ID="lblWarehouseAsig" runat="server" Text="Centro se encuentra asignado." Visible="false" />
    <asp:Label ID="lblMessajeUserBaseWhs" runat="server" Text="Primero debe asociar un centro de distribución al usuario " Visible="false" />
    <asp:Label ID="lblInfoMessaje" runat="server" Text="Mensaje de Información" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
