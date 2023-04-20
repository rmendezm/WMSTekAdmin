<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BillingLogMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Billing.BillingLogMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("BillingLog_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("BillingLog_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" AutoGenerateColumns="False" AllowPaging="True" 
                                OnRowCreated="grdMgr_RowCreated" 
                                OnRowEditing="grdMgr_RowEditing"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                EnableViewState="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner" SortExpression="Owner" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cód. Centro Dist." AccessibleHeaderText="WhsCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWhsCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Facturado" AccessibleHeaderText="Invoiced">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkInvoiced" runat="server" Checked='<%# Eval ( "Invoiced" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Contrato" AccessibleHeaderText="IdBillingContract" SortExpression="IdBillingContract" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdBillingContract" runat="server" Text='<%# Eval ( "BillingContract.Id" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contrato" AccessibleHeaderText="BillingContractDescription" SortExpression="BillingContractDescription" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblBillingContractDescription" runat="server" Text='<%# Eval ( "BillingContract.Description" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Transaccion" AccessibleHeaderText="IdBillingTransaction" SortExpression="IdBillingTransaction" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdBillingTransaction" runat="server" Text='<%# Eval ( "BillingTransaction.Id" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Transaccion" AccessibleHeaderText="BillingTransactionName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblBillingTransactionName" runat="server" Text='<%# Eval ("BillingTransaction.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Transaccion" AccessibleHeaderText="BillingTransactionType">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblBillingTransactionType" runat="server" Text='<%# ((String) Eval ("BillingTransaction.Type") == "N")? "Normal": ((String) Eval ("BillingTransaction.Type") == "C")? "Cada Vez":((String) Eval ("BillingTransaction.Type") == "A")?"Adicional":"Diaria" %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Desc. Transaccion" AccessibleHeaderText="BillingTransactionAddName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblBillingTransactionAddName" runat="server" Text='<%# Eval("BillingTransactionAddName") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Valor Tipo Contrato" AccessibleHeaderText="BillingTypeContractValue" SortExpression="BillingTypeContractValue" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblBillingTypeContractValue" runat="server" Text='<%# Eval ( "BillingTypeContract.Value" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Tipo Cobro" AccessibleHeaderText="IdType" SortExpression="IdType" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdType" runat="server" Text='<%# Eval ( "BillingType.Id" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Cobro" AccessibleHeaderText="TypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblTypeName" runat="server" Text='<%# Eval ("BillingType.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Moneda Cobro" AccessibleHeaderText="IdBillingMoney">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblIdBillingMoney" runat="server" Text='<%# Eval ("BillingMoney.Id") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Moneda Cobro" AccessibleHeaderText="BillingMoneyValue">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblBillingMoneyValue" runat="server" Text='<%# Eval ("BillingMoney.Description") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Qty" HeaderText="Cantidad" AccessibleHeaderText="Qty" />
                        
                                    <asp:TemplateField HeaderText="Entrada/Salida" AccessibleHeaderText="DocumentInOut">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblDocumentInOut" runat="server" Text='<%# ((String) Eval ("DocumentInOut") == "IN")? "Entrada":"Salida" %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="DocumentType" HeaderText="Tipo Documento" AccessibleHeaderText="DocumentType" />
                                    <asp:BoundField DataField="DocumentNumber" HeaderText="Nro.Documento" AccessibleHeaderText="DocumentNumber" />
                                    <asp:BoundField DataField="ReferenceDocNumber" HeaderText="Ref.Nro.Documento" AccessibleHeaderText="ReferenceDocNumber" />
                                    <asp:BoundField DataField="UserName" HeaderText="Usuario" AccessibleHeaderText="UserName" />

                                    <asp:templatefield headertext="Fecha" accessibleHeaderText="DateEntry">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblDateEntry" runat="server" text='<%# ((DateTime) Eval ("DateEntry") > DateTime.MinValue)? Eval("DateEntry", "{0:d}"):"" %>' />
                                                </div>
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CommandName="Edit" ToolTip="Editar" />
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
            <%-- Pop up Editar/Nuevo BillingLog --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlBillingLog" BackgroundCssClass="modalBackground" PopupDragHandleControlID="BillingLogCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlBillingLog" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Cobro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Cobro" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">

                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwner" AutoPostBack="true" OnSelectedIndexChanged="ddlOwner_SelectedIndexChanged" AppendDataBoundItems="true" />
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>

                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist." />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlWarehouse" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro Dist. es requerido" />
                                </div>
                            </div>

                            <div id="divContract" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblContract" runat="server" Text="Contrato" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlContract" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvContract" runat="server" ControlToValidate="ddlContract"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Contrato es requerido" />
                                </div>
                            </div>
                                       
                            <div id="divTransaction" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTransaction" runat="server" Text="Transacción" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlTransaction" AutoPostBack="false" Enabled="false" />
                                    <asp:RequiredFieldValidator ID="rfvTransaction" runat="server" ControlToValidate="ddlTransaction"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Transacción es requerida" />
                                </div>
                            </div>

                            <div id="divTransactionAdd" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblTransactionAdd" runat="server" Text="Desc. Transacción" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtTransactionAdd" runat="server" MaxLength="30"/>
                                <asp:RequiredFieldValidator ID="rfvTransactionAdd" runat="server" ControlToValidate="txtTransactionAdd" ValidationGroup="EditNew" Text=" * " ErrorMessage="Desc. Transacción es requerido" /></div>
                            </div>

                            <div id="divType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblType" runat="server" Text="Tipo Cobro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlType" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="ddlType"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Cobro es requerido" />
                                </div>
                            </div>

                            <div id="divMoney" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMoney" runat="server" Text="Moneda" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlMoney" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvMoney" runat="server" ControlToValidate="ddlMoney"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Moneda es requerido" />
                                </div>
                            </div>

                            <div id="divDocumentInOut" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDocumentInOut" runat="server" Text="In/Out" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlDocumentInOut" AutoPostBack="true" OnSelectedIndexChanged="ddlDocumentInOut_OnSelectedIndexChanged" AppendDataBoundItems="true" />
                                    <%--<asp:RequiredFieldValidator ID="rfvDocumentInOut" runat="server" ControlToValidate="ddlDocumentInOut"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="In/Out es requerido" />--%>
                                </div>
                            </div>

                            <div id="divDocumentType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDocumentType" runat="server" Text="Tipo Documento" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlDocumentType" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvDocumentType" runat="server" ControlToValidate="ddlDocumentType"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Documento es requerido" />
                                </div>
                            </div>

                            <div id="divDocumentNumber" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblDocumentNumber" runat="server" Text="Nro.Documento" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtDocumentNumber" runat="server" MaxLength="30"/>
                                <asp:RequiredFieldValidator ID="rfvDocumentNumber" runat="server" ControlToValidate="txtDocumentNumber" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nro.Documento es requerido" /></div>
                            </div>

                            <%-- Start Date --%>  
                            <div id="divStartDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStartDate" runat="server" Text="Inicio" /></div>
                                
                                <%-- Date --%>  
                                <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">  
    	                            <tr>
	                                    <td rowspan="2">              
                                            <asp:TextBox ID="txtStartDate" runat="server" Width="70px" />
                                    
                                            <ajaxToolkit:CalendarExtender ID="calStartDate" 
                                 CssClass="CalMaster"
                                    runat="server" 
                                    Enabled="true" 
                                    FirstDayOfWeek="Sunday" 
                                    TargetControlID="txtStartDate"
                                    PopupButtonID="txtStartDate" Format="dd-MM-yyyy" />
                                            <asp:requiredfieldvalidator ID="rfvStartDate" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtStartDate" display="Static" ErrorMessage="Fecha es requerido"/>&nbsp;&nbsp;
                                            <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Fecha debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                            <%-- Hours --%>  
                                            <asp:TextBox ID="txtStartDateHours" runat="server" Width="20" MaxLength="2" Visible="false"/>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="btnMoreStartDateHours" Visible ="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>                        
                                        <td rowspan="2">
                                            <asp:RangeValidator Type="Integer" ID="rangeStartDateHours" ControlToValidate="txtStartDateHours" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Hora debe estar entre 0 y 23" MinimumValue="0" MaximumValue="23" />&nbsp;&nbsp;
                                            <%-- Minutes --%>  
                                            <asp:TextBox ID="txtStartDateMinutes" runat="server" Width="20px" MaxLength="2" Visible="false" />
                                        </td>                        
                                        <td>
                                            <asp:ImageButton ID="btnMoreStartDateMinutes" Visible = "false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>   
                                        <td rowspan="2" >
                                            <asp:RangeValidator Type="Integer" ID="rangeStartDateMinutes" ControlToValidate="txtStartDateMinutes" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Minutos debe estar entre 0 y 59" MinimumValue="0" MaximumValue="59" />
                                        </td>                        
                                     </tr>
                                     <tr >
                                        <td >
                                            <asp:ImageButton ID="btnLessStartDateHours" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                        <td >
                                            <asp:ImageButton ID="btnLessStartDateMinutes" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                     </tr> 
                                  </table>
                                </div>
                            </div>

                            <div id="divValue" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblValue" runat="server" Text="Valor" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtValue" runat="server" MaxLength="30"/>
                                    <asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" ValidationGroup="EditNew" Text=" * " ErrorMessage="Valor es requerido" />
                                    <asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ingrese valores numericos" Type="Double" />
                                </div>
                            </div>

                            <div id="divQty" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblQty" runat="server" Text="Cantidad" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtQty" runat="server" MaxLength="30"/>
                                    <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQty" ValidationGroup="EditNew" Text=" * " ErrorMessage="Cantidad es requerido" />
                                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="txtQty" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ingrese valores numericos" Type="Double" />
                                </div>
                            </div>
                            
                        </div>    
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
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
    <asp:Label ID="lblStatusName" runat="server" Text="Facturado" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Contrato" Visible="false" />
    <asp:Label ID="lblDescription" runat="server" Text="Transacción" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Fecha" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
        
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
