<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KardexConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.KardexConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("Kardex_GetAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Kardex_GetAll", "ctl00_MainContent_grdMgr");
    }
</script>

     <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>          
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	                        <%-- Grilla Principal --%>         
                             <asp:GridView ID="grdMgr" 
                                runat="server" 
                                AllowPaging="True" 
                                OnRowCreated="grdMgr_RowCreated"
                                EnableViewState="False"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                    
                                 <Columns>
                                    <%--<asp:BoundField DataField="IdKardex" HeaderText="IdKardex" />--%>
                                    <asp:templatefield HeaderText="IdKardex" AccessibleHeaderText="IdKardex" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "IdKardex" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="Idwhs" HeaderText="IdCentro" />--%>
                                     <asp:templatefield HeaderText="IdCentro" AccessibleHeaderText="Idwhs" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblIdwhs" runat="server" text='<%# Eval ( "Idwhs" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                         
                                     <%--<asp:BoundField DataField="WhsName" HeaderText="Centro" />--%>
                                     <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "WhsName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="IdOwn" HeaderText="IdOwner" />--%>
                                     <asp:templatefield HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblIdOwn" runat="server" text='<%# Eval ( "IdOwn" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="OwnName" HeaderText="Owner" />--%>
                                     <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblOwnName" runat="server" text='<%# Eval ( "OwnName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="KardexCode" HeaderText="Tipo" />--%>
                                     <asp:templatefield HeaderText="Tipo" AccessibleHeaderText="KardexCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblKardexCode" runat="server" text='<%# Eval ( "KardexCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="MovementDate" HeaderText="Fecha" />--%>
                                     <asp:templatefield HeaderText="Fecha" AccessibleHeaderText="MovementDate" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblMovementDate" runat="server" text='<%# Eval ( "MovementDate" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="IdItem" HeaderText="IdItem" />--%>
                                     <asp:templatefield HeaderText="IdItem" AccessibleHeaderText="IdItem" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblIdItem" runat="server" text='<%# Eval ( "IdItem" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="ItemCode" HeaderText="Cod. Item" />--%>
                                     <asp:templatefield HeaderText="Cod. Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblItemCode" runat="server" text='<%# Eval ( "ItemCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="LongItemName" HeaderText="Item" />--%>
                                     <asp:templatefield HeaderText="Item" AccessibleHeaderText="LongItemName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblLongItemName" runat="server" text='<%# Eval ( "LongItemName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="IdCtgItem" HeaderText="IdCategoria" />--%>
                                    <%-- <asp:templatefield HeaderText="IdCategoria" AccessibleHeaderText="IdCtgItem" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblIdCtgItem" runat="server" text='<%# Eval ( "IdCtgItem" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>--%>
                         
                                     <%--<asp:BoundField DataField="CtgName" HeaderText="Categoria" />--%>
                                   <%--  <asp:templatefield HeaderText="Categoria" AccessibleHeaderText="CtgName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblCtgName" runat="server" text='<%# Eval ( "CtgName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>--%>
                         
                                     <%--<asp:BoundField DataField="ItemQtySign" HeaderText="Cantidad" />--%>
                                     <asp:templatefield HeaderText="Cantidad" AccessibleHeaderText="ItemQtySign" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblItemQtySign" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQtySign" )) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="BalanceQtyNew" HeaderText="Saldo" />--%>
                                     <asp:templatefield HeaderText="Saldo" AccessibleHeaderText="BalanceQtyNew" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblBalanceQtyNew" runat="server" text='<%# GetFormatedNumber(Eval ( "BalanceQtyNew" )) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="IdDocument" HeaderText="IdDocumento" />--%>
                                     <asp:templatefield HeaderText="IdDocumento" AccessibleHeaderText="IdDocument" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblIdDocument" runat="server" text='<%# Eval ( "IdDocument" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="DocumentNumber" HeaderText="Documento" />--%>
                                     <asp:templatefield HeaderText="Documento" AccessibleHeaderText="DocumentNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblDocumentNumber" runat="server" text='<%# Eval ( "DocumentNumber" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="DocumentTypeCode" HeaderText="Tipo Doc" />--%>
                                     <asp:templatefield HeaderText="Tipo Doc" AccessibleHeaderText="DocumentTypeCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblDocumentTypeCode" runat="server" text='<%# Eval ( "DocumentTypeCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="ReferenceDoc" HeaderText="Doc Referen" />--%>
                                     <asp:templatefield HeaderText="Doc Referen" AccessibleHeaderText="ReferenceDoc" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblReferenceDoc" runat="server" text='<%# Eval ( "ReferenceDoc" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:BoundField DataField="ReferenceDocTypeCode" HeaderText="Tipo Referen" />--%>
                                     <asp:templatefield HeaderText="Tipo Referen" AccessibleHeaderText="ReferenceDocTypeCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblReferenceDocTypeCode" runat="server" text='<%# Eval ( "ReferenceDocTypeCode" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                 </Columns>
                         
                              </asp:GridView>
                            <%-- FIN Grilla Principal --%>             
                        </div>
                        <div id="divReport" runat="server" visible="false">
                        <rsweb:ReportViewer ID="rptViewKardex" runat="server" ProcessingMode="Remote"
                            AsyncRendering="true" Height="610px" Width="100%" Style="margin-right: 11px">
                            <ServerReport />
                        </rsweb:ReportViewer>
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
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>             

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fifo" Visible="false" />   
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>
</asp:Content>
