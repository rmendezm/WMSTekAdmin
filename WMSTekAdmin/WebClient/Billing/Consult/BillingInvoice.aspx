<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BillingInvoice.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Billing.BillingInvoice" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

<script type="text/javascript">

    window.onresize = SetDivs;

    function ShowProgress() {
        
        var rowsGrdMgr = $("[id*=grdMgr] td").closest("tr").length;

        if (rowsGrdMgr > 1) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 100);
            return true;

        } else {
            //Poner mesaje de validacion
            return false;
        }
    }
    

    $(document).ready(function () {
        initializeGridDragAndDrop("BillingLogConsult_GetByFilter", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("BillingLogConsult_GetByFilter", "ctl00_MainContent_grdMgr");
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
                                OnRowDataBound="grdMgr_RowDataBound"
                                EnableViewState="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkInvoiceConfirm', this.checked)" id="chkAll" title="Seleccionar todos" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkInvoiceConfirm" runat="server" Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />

                                    <asp:TemplateField HeaderText="Id. Transaccion" AccessibleHeaderText="IdBillingTransaction" SortExpression="IdBillingTransaction" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdTransaction" runat="server" Text='<%# Eval ( "IdBillingTransaction" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre Transaccion" AccessibleHeaderText="BillingTransactionName" SortExpression="BillingTransactionName" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblTransactionName" runat="server" Text='<%# Eval ( "BillingTransactionName" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id. Tipo Cobro" AccessibleHeaderText="IdBillingType" SortExpression="IdBillingType" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdBillingType" runat="server" Text='<%# Eval ( "IdBillingType" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo Cobro" AccessibleHeaderText="BillingTypeName" SortExpression="BillingTypeName" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblBillingTypenName" runat="server" Text='<%# Eval ( "BillingTypeName" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Facturado" AccessibleHeaderText="Invoiced">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkinvoiced" runat="server" Checked='<%# Eval ( "Invoiced" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="dateEntry" HeaderText="Fecha Entrada" AccessibleHeaderText="dateEntry" />
                        
                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblQty" runat="server" Text='<%# Eval("Qty") %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Folio" AccessibleHeaderText="BillingFolio" SortExpression="BillingFolio">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBillingFolio" runat="server" Text='<%# ((int) Eval("BillingFolio") == -1 )? "":Eval("BillingFolio") %>' />
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
                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentDialog$btnOk" EventName="Click" />         
                     <%--<asp:PostBackTrigger ControlID="btnExcel" />--%>
                  </Triggers>
                </asp:UpdatePanel>
             </div>
        </div>
    </div>            
    
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" >
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>

    <asp:UpdatePanel ID="upMarvalData" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <%-- Pop up datos especificos Marval --%>
            <div id="divMarvalData" runat="server" visible="false">            
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnl" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Seleccionar" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>                    
                    </asp:Panel>
                    <div class="modalControls" >
                        <div class="divCtrsFloatLeft">                            
                            <div id="divCompany" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblCompany" runat="server" Text="Compañia" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCompany" runat="server" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCompany" runat="server" ControlToValidate="ddlCompany"
                                    ValidationGroup="SpecialData" Text=" * "  ErrorMessage="Compañia es requerida" InitialValue="-1" />
                                </div>
                            </div>

                            <div id="divZone" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblZone" runat="server" Text="Area/Etapa/Zona" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlZone" runat="server" OnSelectedIndexChanged="ddlZone_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvZone" runat="server" ControlToValidate="ddlZone"
                                    ValidationGroup="SpecialData" Text=" * "  ErrorMessage="Zona es requerida" InitialValue="-1" />
                                </div>
                            </div>

                            <div id="divBusiness" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblBusiness" runat="server" Text="Negocio/Representado" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlBusiness" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rvfBusiness" runat="server" ControlToValidate="ddlBusiness"
                                    ValidationGroup="SpecialData" Text=" * "  ErrorMessage="Negocio es requerido" InitialValue="-1" />
                                </div>
                            </div>

                            <div id="divLorry" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblLorry" runat="server" Text="Máquina/Camión" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlLorry" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rvfLorry" runat="server" ControlToValidate="ddlLorry"
                                    ValidationGroup="SpecialData" Text=" * "  ErrorMessage="Camión es requerido" InitialValue="-1" />
                                </div>
                            </div>

                            <div id="divDepartment" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblDepartment" runat="server" Text="Departmento" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlDepartment" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rvfDepartment" runat="server" ControlToValidate="ddlDepartment"
                                    ValidationGroup="SpecialData" Text=" * "  ErrorMessage="Departamento es requerido" InitialValue="-1" />
                                </div>
                            </div>

                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="SpecialData"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="SpecialData" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                     </div>   
                </asp:Panel>
            </div>
            <%-- FIN Pop up datos especificos Marval --%>
       </ContentTemplate>
    </asp:UpdatePanel> 

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprMarvalData" runat="server" AssociatedUpdatePanelID="upMarvalData" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprMarvalData" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprMarvalData" />    
    <%-- FIN Modal Update Progress --%>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Facturación de Cobros" Visible="false" />
    <asp:Label ID="lblErrorDate" runat="server" Text="Las fechas no son válidas, fecha hasta es menor a la fecha desde." Visible="false" />
    <asp:Label ID="lblErrorValidContract" runat="server" Text="Dueño no cuenta con un contrato vigente." Visible="false" />
    <asp:Label ID="lblErrorNotLog" runat="server" Text="No existen cobros." Visible="false" />
    <asp:Label ID="lblErrorNotLogConfirm" runat="server" Text="No existen cobros a facturar." Visible="false" />
    <asp:Label ID="lblGenerateExcel" runat="server" Text="Generar Facturación de Cobros." Visible="false" />
    <asp:Label ID="lblToolTipConfirmInvoiced" runat="server" Text="Confirmar Facturación" Visible="false" />
    <asp:Label ID="lblMessageConfirmInvoiced" runat="server" Text="¿Esta Seguro de Confirmar la Facturación?" Visible="false" />
    <asp:Label ID="lblMustSelectAnyInvoice" runat="server" Text="Debe seleccionar al menos una factura" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  

    <%-- Div Bloquea Pantalla al Momento de Realizar Carga Masiva --%>
    <div id="divFondoPopupProgress" class="loading" align="center">
        Generando Planilla de Facturación <br />Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>

    <iframe id="iframe" runat="server" style="visibility:hidden"></iframe>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
