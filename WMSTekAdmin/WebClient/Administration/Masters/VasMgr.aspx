<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="VasMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.VasMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("Vas_FindAll", "ctl00_MainContent_grdMgr");
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
        initializeGridDragAndDrop("Vas_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" ChildrenAsTriggers="true"  UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                DataKeyNames="Id" 
                                runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AllowPaging="True" 
                                EnableViewState="false"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <%--<asp:BoundField DataField="Id" HeaderText="ID Tipo Tarea" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="IdTaskType" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre Tipo Tarea" AccessibleHeaderText="TaskTypeName" />--%>
                                    <asp:TemplateField HeaderText="ID Vas" AccessibleHeaderText="IdVas">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdVas" runat="server" Text='<%# Eval ("Id") %>'/>
                                             </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
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
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="VasName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblVasName" runat="server" Text='<%# Eval ("Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="VasDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblVasDescription" runat="server" Text='<%# Eval ("Description") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("Customer.Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Cód. Cliente" AccessibleHeaderText="CustomerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("Customer.Code") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Status" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                      
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
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

                        <%-- Pop up Editar/Nuevo TaskType --%>
                        <div id="divModal" runat="server" visible="false">
                            <asp:Panel ID="pnlPanelPoUp" runat="server" CssClass="modalBox">
                               <%-- Encabezado --%>
                                <asp:Panel ID="OutboundCaption" runat="server" CssClass="modalHeader">
                                    <div class="divCaption">
                                        <asp:Label ID="lblNew" runat="server" Text="Nuevo Vas" Width="770px" />
                                        <asp:Label ID="lblEdit" runat="server" Text="Editar Vas" Width="770px" />
                                     <%--   <asp:ImageButton ID="ImageButton1" runat="server" OnClick="imgCloseNewEdit_Click" ToolTip="Cerrar" CssClass="closeButton"
                                            ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />--%>
                                    </div>
                                </asp:Panel>
                                <%-- Fin Encabezado --%>

                                <div class="modalBoxContent">
                                    <ajaxToolkit:TabContainer runat="server" ID="tabItemCustomer" ActiveTabIndex="0">
                                        <ajaxToolkit:TabPanel runat="server" ID="tabLayout">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                                    
                                                <div class="divCtrsFloatLeft">
                                                    <div id="divStatus" runat="server" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblStatus" runat="server" Text="Activo" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:CheckBox ID="chkStatus" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div id="divOwner" runat="server" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:DropDownList runat="server" ID="ddlOwner">
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                                        </div>
                                                    </div>        
                                                    <%-- Customer --%>                            
                                                    <div id="divCustomerCode" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblCustomerCode" Text="Cód. Cliente" runat="server" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtCustomerCode" runat="server"  />
                                                            <asp:ImageButton ID="imgBtnCustmerSearch" 
                                                                runat="server" 
                                                                Height="18px" 
                                                                ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                                OnClick="imgBtnCustmerSearch_Click" 
                                                                Width="18px" 
                                                                ToolTip="Buscar Cliente"/>
                                                            <%--<asp:RequiredFieldValidator ID="rfvCustomerCode" ControlToValidate="txtCustomerCode" ValidationGroup="EditNew"
                                                                runat="server" Text=" * " ErrorMessage="Código Cliente es requerido"></asp:RequiredFieldValidator>--%>
                                                        </div>
                                                    </div> 
                                                    <div id="divCustomerName" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblCustomerName" Text="Nombre Cliente" runat="server" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtCustomerName" runat="server"  Enabled="false"  />                                                
                                                        </div>
                                                    </div>
                                      
                                                    <div id="divName" runat="server" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtName" runat="server" MaxLength="20" Width="150" />
                                                            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido">
                                                            </asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div id="divDescription" runat="server" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblDescription" runat="server" Text="Descripcion" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtDescription" runat="server" MaxLength="500" Width="150" />
                                                            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripcion es requerido">
                                                            </asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="divValidationSummary" >
                                                    <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                                        ShowMessageBox="false" CssClass="modalValidation"/>
                                                </div>

                                                 <div style="clear: both">
                                                </div>
                                  
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                    </ajaxToolkit:TabContainer>

                                    <div id="divActions" runat="server" class="modalActions">
                                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"  ValidationGroup="EditNew" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCancel_Click" />
                                    </div>                           
                                </div>
                            </asp:Panel>
                        </div>
                        <%-- Pop up Editar/Nuevo TaskType --%>

                        <%-- Lookup Customers --%>
                        <div id="divLookupCustomer" runat="server" visible="false">
                            <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                            <!-- Boton 'dummy' para propiedad TargetControlID -->
                            <ajaxToolkit:ModalPopupExtender ID="mpLookupCustomer" runat="server" TargetControlID="btnDummy2"
                                PopupControlID="pnlLookupCustomer" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupCustomer"
                                Drag="true">
                            </ajaxToolkit:ModalPopupExtender>
                            <asp:Panel ID="pnlLookupCustomer" runat="server" CssClass="modalBox">
                                <asp:Panel ID="pnlHeadBarCustomer" runat="server" CssClass="modalHeader">
                                    <div class="divCaption">
                                        <asp:Label ID="lblAddCustomer" runat="server" Text="Buscar Cliente" />
                                        <asp:ImageButton ID="imgBtnCloseCustomerSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                            ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                                    </div>
                                    <div id="divPageGrdSearchCustomers" runat="server">
                                        <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                            <asp:ImageButton ID="btnFirstGrdSearchCustomers" runat="server" OnClick="btnFirstGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                            <asp:ImageButton ID="btnPrevGrdSearchCustomers" runat="server" OnClick="btnPrevGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                            Pág. 
                                            <asp:DropDownList ID="ddlPagesSearchCustomers" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchCustomersSelectedIndexChanged" SkinID="ddlFilter" /> 
                                            de 
                                            <asp:Label ID="lblPageCountSearchCustomers" runat="server" Text="" />
                                            <asp:ImageButton ID="btnNextGrdSearchCustomers" runat="server" OnClick="btnNextGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                            <asp:ImageButton ID="btnLastGrdSearchCustomers" runat="server" OnClick="btnLastGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="modalControls">
                                    <asp:HiddenField ID="hidCustomerId" runat="server" Value="-1" />
                                    <webUc:ucLookUpFilter ID="ucFilterCustomer" runat="server" />
                                    <div class="divCtrsFloatLeft">
                                        <div class="divLookupGrid">
                                            <asp:GridView ID="grdSearchCustomers" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchCustomers_RowCommand" AllowPaging="true"
                                                OnRowDataBound="grdSearchCustomers_RowDataBound"
                                                AutoGenerateColumns="False"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                                        SortExpression="Id" Visible="false" />
                                                    <asp:TemplateField AccessibleHeaderText="IdOwn" HeaderText="Id Dueño" Visible="false">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ("Owner.Id") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="OwnName" HeaderText="Dueño">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ("Owner.Name") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Código">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("Code") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("Name") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="imgBtnAddCustomer" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                                    Width="20px" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div style="clear: both" />
                                </div>
                            </asp:Panel>
                        </div>
                        <%-- FIN Lookup Customers --%>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />    
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />  
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>   
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>
    
   <%-- <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>



        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>--%>


   


    <%-- Modal Update Progress --%>
<%--    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprEditNew" />--%>
    <%-- FIN Modal Update Progress --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
     <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblCustomer" runat="server" Text="Cod. Cliente" Visible="false" /> 
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />
    <asp:Label ID="lblNameFilter" runat="server" Text="Nombre Vas" Visible="false" />
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este vas?" Visible="false" />
    <asp:Label ID="lblMessajeSelectedOwner" runat="server" Text="Debe seleccionar un Dueño." Visible="false" />
    <asp:Label ID="lblMessajeCustomer" runat="server" Text="Cliente ingresado no es válido." Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Vas" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>