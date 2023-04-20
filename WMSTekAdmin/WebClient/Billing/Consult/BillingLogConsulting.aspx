<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BillingLogConsulting.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Billing.Consult.BillingLogConsulting" %>

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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
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
