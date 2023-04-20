<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"  CodeBehind="ReleaseOrderPendingMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.ReleaseOrderPendingMgr" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" Assembly="Flan.Controls" Namespace="Flan.Controls"  %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    function resizeDiv() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;    
    }

    window.onresize = resizeDiv;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);


    function showProgress() {
        //var prueba = document.getElementById("ctl00_ucMainFilterContent_rfvSummary");
        //var attr = $(this).attr('data-val-required');
        var error = false;
        $(".error").each(function () {
            if ($(this).css("visibility") == "visible") {
                error = true;
            }
        });

        if (error == false) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);

                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 10);
            return true;
        } else {
            return false;
        }
    }

    function hideProgress() {

        var loading = $(".modalLoading");
        loading.hide();
        var loading = $(".loading");
        loading.hide();
    }

</script>

<style  type="text/css">
    .divRightHeader {
        font-family: Verdana, Helvetica, Sans-Serif;
        font-weight: normal;
        font-size: 11px; 
        float:right;
        padding: 0px;
        padding-top: 0px;
        padding-right: 30px;
    }

    .container{
             margin-top: 0px !important;
         }

    #ctl00_MainContent_pnlPendingOrders{
        z-index: 10000001 !important;
    }
    .loading{
       z-index: 50000001 !important;
    }
    .modalLoading{
       z-index: 50000000 !important;
     }
</style>

    <div style="width:100%;height:100%;margin:0px;margin-bottom:80px" runat="server" id="divMainPrincipal">
        <spl:Splitter LiveResize="false" CookieDays="0" ID="hsVertical" runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
            <LeftPanel ID="leftPanel" WidthDefault="200" WidthMin="100">
                <Content>
                    <%-- Ordenes Selecciondadas --%>                             
                                            
                        <asp:UpdatePanel ID="upSelectedOrdersHeader" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="divGridTitleDispatch" runat="server">
                                <div class="divCenter">
                                    <asp:Label ID="lblSelectedOrders" runat="server" Text="Pedidos Seleccionados" />
                                </div>                                        
                                <asp:ImageButton ID="btnRemove" runat="server" onclick="btnRemove_Click" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_remove_dis.png" Enabled="false" ToolTip="Remover"/> 
                                <asp:ImageButton ID="btnReprocess" runat="server" onclick="btnReprocess_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png" ToolTip="Simular"/> 
                                        
                                <div class="divRightHeader">
                                    <asp:Label ID="lblSelectedOrdersCount" runat="server" Text="Total:" />
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnRelease" EventName="Click" />      
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentDialog$btnOk" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$ctl00$ctl01$btnReleasePopUp" EventName="Click" />                                                                  
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnAddToSelected" EventName="Click" />
                        </Triggers>     
                        </asp:UpdatePanel>  
                        <asp:UpdateProgress ID="uprSelectedOrdersHeader" runat="server" AssociatedUpdatePanelID="upSelectedOrdersHeader" DisplayAfter="20" DynamicLayout="true">
                            <ProgressTemplate>
                                <div class="divProgress">
                                    <asp:ImageButton ID="imgProgress0" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" ControlToOverlayID="divMainPrincipal" CssClass="updateProgress" TargetControlID="uprSelectedOrdersHeader" />  
                                                          
                        <asp:UpdatePanel ID="upSelectedOrders" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>                                        
                                <div id="divSelected" runat="server">                                                
                                                            
                                        <%-- Grilla de Ordenes Selecciondadas --%>   
                                        <asp:GridView ID="grdSelected" runat="server"
                                        OnRowCreated="grdSelected_RowCreated"
                                        OnRowCommand="grdSelected_RowCommand"   
                                        OnSelectedIndexChanged="grdSelected_SelectedIndexChanged"
                                        AllowPaging="False"
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdSelected_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false">
                                        <Columns>

                                        <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                        <asp:TemplateField ShowHeader="False">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdSelected.ClientID %>', 'chkRemoveOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>                                                     
                                            <ItemTemplate> 
                                                <center>
                                                <div style="width:20px">
	                                                <asp:CheckBox ID="chkRemoveOrder" runat="server" />
                                                </div>	                        
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                            
                                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id" />

                                            <asp:templatefield headertext="En otra Sim." accessibleHeaderText="InOtherSimulation" SortExpression="InOtherSimulation">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkInSimulation" runat="server" checked='<%# Eval ( "InOtherSimulation" ) %>' Enabled="false"/>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:templatefield>
                                            
                                            <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.TradeName" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField HeaderText="Nº Doc." ItemStyle-Wrap="false" DataField="Number" 
                                                AccessibleHeaderText="OutboundNumber" >
                                            </asp:BoundField>

                                            <asp:BoundField DataField="Priority" HeaderText="Prioridad" 
                                                AccessibleHeaderText="Priority" ItemStyle-Wrap="false">
                                            </asp:BoundField>
                                                    
                                            <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" ItemStyle-Wrap="false">
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'/>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>    
                                                                
                                            <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" ItemStyle-Wrap="false">
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'/>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>    

                                            <asp:TemplateField HeaderText="Compl." AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "FullShipment" ) %>' Enabled="false" />
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>
                                                    
                                            <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                <ItemTemplate> 
                                                    <center>
                                                    <div style="width:60px;margin:0px; padding: 0px;">
	                                                    <asp:ImageButton ID="btnUp" runat="server" 
	                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_up.png" 
	                                                        onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_up_on.png';"
                                                            onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_up.png';"
	                                                        CausesValidation="false" CommandName="Up" ToolTip="Mover Arriba"/>
	                                                    <asp:ImageButton ID="btnDown" runat="server" 
	                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_down.png" 
	                                                        onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_down_on.png';"
                                                            onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_down.png';"
	                                                        CausesValidation="false" CommandName="Down" ToolTip="Mover Abajo"/>
                                                    </div>	                        
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>    
                                                    
                                            <asp:TemplateField AccessibleHeaderText="CompliancePct"  HeaderText="% Satisf.">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblCompliancePct" runat="server" text='<%# Eval ( "CompliancePct" ) %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>   
                                            
                                            <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
	                                            <itemtemplate>
		                                            <div style="word-wrap: break-word;">
			                                            <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "Branch.Name" ) %>'/>
		                                            </div>  
	                                            </itemtemplate>
                                            </asp:TemplateField> 
                                                                
                                        </Columns>
                                        </asp:GridView>
                                            
                                        <%-- Detalle --%>
                                        <div id="divSelectedDetail" runat="server" visible="false" class="divGridDetail">
                                            <div id="divSelectedDetailTitle" runat="server" class="divGridDetailTitle">
	                                            <asp:Label ID="lblSelectedGridDetail" runat="server" Text="Detalle Doc: " />
	                                            <asp:Label ID="lblSelectedNroDoc" runat="server" Text=""/>
                                            </div>
                                    	            
                                            <asp:GridView ID="grdSelectedDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="False" 
                                                AllowPaging="False"
                                                OnRowCreated="grdSelectedDetail_RowCreated" 
                                                AutoGenerateColumns="false"
                                                OnRowDataBound="grdSelectedDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                               
                                                <Columns>
                                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                                         
                                                    <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber" 
                                                        ItemStyle-HorizontalAlign="Center" >
                                                                    
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea"  AccessibleHeaderText="LineCode"
                                                        ItemStyle-HorizontalAlign="Center" >
                                                    </asp:BoundField>
                                                            
                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'/>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                           
                                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                      
                                                                                                                   
                                                    <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'/>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                            
                                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgItem">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'/>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                        
                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQty" )) %>'/>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"/>--%>
                                                    <asp:BoundField DataField="CompliancePct" HeaderText="% Satisf." AccessibleHeaderText="CompliancePct"/>
                                                    </Columns>
                                            </asp:GridView>
	                                    </div>
                                        <%-- FIN Detalle --%>	
                                 
                                    </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnRelease" EventName="Click" />   
                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentDialog$btnOk" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$ctl00$ctl01$btnReleasePopUp" EventName="Click" />                                                                        
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnAddToSelected" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$btnRemove" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$btnReprocess" EventName="Click" />
                            </Triggers>        
                        </asp:UpdatePanel>        
                                
                        <asp:UpdateProgress ID="uprSelectedOrders" runat="server" AssociatedUpdatePanelID="upSelectedOrders" DisplayAfter="20" DynamicLayout="true">
                            <ProgressTemplate>
                                <div class="divProgress">
                                    <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrders" runat="server" ControlToOverlayID="divSelected" CssClass="updateProgress" TargetControlID="uprSelectedOrders" />
                                                                                     
                    <%-- FIN Ordenes Selecciondadas --%>                                                      
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>            
            </LeftPanel>
            <RightPanel>
                <Content>                                    
                     <%-- Simulación de Liberación de Pedidos --%>    
                     <asp:UpdatePanel ID="upSimulacion" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                           <%-- Simulación --%>   
                           <div id="divSimulation" runat="server">
                                 <div class="divGridTitleDispatch">
                                    <div class="divCenter"><asp:Label ID="lblSimulationTitle" runat="server" Text="Asignación" /></div>
                                    <asp:ImageButton ID="btnReleasePopUp" runat="server" Enabled="false" onclick="btnReleasePopUp_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_release_dis.png" ToolTip="Liberar"/>	                                                            
                                    <asp:ImageButton ID="btnExportExcel" runat="server" Enabled="false" onclick="btnExportExcel_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel_dis.png" ToolTip="Exportar a Excel"/> 
                                 </div>
                          
                                <%-- Grilla Simulación --%>   
                                <asp:GridView ID="grdSimulate" runat="server" 
                                    OnRowCreated="grdSimulate_RowCreated"
                                    OnSelectedIndexChanged="grdSimulate_SelectedIndexChanged" 
                                    EnableViewState="False" 
                                    AllowPaging="False" 
                                    AutoGenerateColumns="false"
                                    OnRowDataBound="grdSimulate_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                     
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Code" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "OutboundOrder.Owner.TradeName" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   
                                    <asp:TemplateField AccessibleHeaderText="OutboundNumber"  HeaderText="Nº Doc.">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField AccessibleHeaderText="Priority"  HeaderText="Prioridad">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblPriority" runat="server" text='<%# ((int)Eval("OutboundOrder.Priority") == -1) ? " " : Eval("OutboundOrder.Priority")%>' />
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                    
                                    
                                    <asp:TemplateField HeaderText="Compl." AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                        <itemtemplate>
                                            <center>
                                                <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "OutboundOrder.FullShipment" ) %>' Enabled="false" />
                                            </center>    
                                        </itemtemplate>
                                    </asp:TemplateField>
                                   
                                    <asp:TemplateField AccessibleHeaderText="CompliancePct"  HeaderText="% Satisf.">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblCompliancePct" runat="server" text='<%# Eval ( "CompliancePct" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                    
                                    <asp:TemplateField AccessibleHeaderText="Weight"  HeaderText="Peso Total Items">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblWeightItem" runat="server" text='<%# Eval ( "Weight" , "{0:n3}") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>       
                                    
                                    <asp:TemplateField AccessibleHeaderText="Volume"  HeaderText="Volumen Total Items">
                                        <ItemTemplate >
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblVolumeItem" runat="server" text='<%# Eval ( "Volume" , "{0:n3}") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    
                                    <asp:TemplateField AccessibleHeaderText="QtyLpnVolume"  HeaderText="LPN por Vol.">
                                        <ItemTemplate >
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblQtyLpnVolume" runat="server" text='<%# Eval ( "QtyLpnVolume" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField AccessibleHeaderText="QtyLpnWeight"  HeaderText="LPN por Peso">
                                        <ItemTemplate >
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblQtyLpnWeigth" runat="server" text='<%# Eval ( "QtyLpnWeight" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>   
                                    
                                    <asp:TemplateField AccessibleHeaderText="LocProposal"  HeaderText="Andén Propuesto">
                                        <ItemTemplate >
                                            <div style="word-wrap: break-word;">                                            
                                                <asp:label ID="lblLocProporsal" runat="server" text='<%# Eval ( "LocProposal" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>           

                                 </Columns>
                                </asp:GridView>

                              <%-- Detalle Simulación --%>     
                              <div id="divDetailSim" runat="server" class="divGridDetail" visible="false">
                                      <div id="divDetailSimTitle" runat="server" class="divGridDetailTitle">
                                        <asp:Label ID="lblGridDetailSim"  runat="server" Text="Detalle Simulación: "/>
                                        <asp:Label ID="lblNroDocSim" runat="server" Text=""/>
                                      </div>
                        	            
                                      <asp:GridView ID="grdDetailSim" runat="server" SkinID="grdDetail"
                                            DataKeyNames="Id" 
                                            EnableViewState="False" 
                                            AllowPaging="False"
                                            OnRowCreated="grdDetailSim_RowCreated" 
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdDetailSim_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                            EnableTheming="false">
                                   
                                            <Columns>
                                               <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "OutboundDetail.Item.Code" ) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                               
                                                <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("OutboundDetail.Item.LongName") %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>                      
                                                                                                       
                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "OutboundDetail.Item.Description" ) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                
                                                <asp:TemplateField HeaderText="Solicitado" AccessibleHeaderText="ProposalQty">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblProposalQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ProposalQty" )) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Stock" AccessibleHeaderText="TotalQty">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblTotalQty" runat="server" text='<%# GetFormatedNumber(Eval ( "TotalQty" )) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField DataField="ProposalQty" HeaderText="Solicitado" AccessibleHeaderText="ProposalQty"/>
                                                <asp:BoundField DataField="TotalQty" HeaderText="Stock" AccessibleHeaderText="TotalQty"/>--%>
                                                
                                                <asp:TemplateField AccessibleHeaderText="CompliancePct"  HeaderText="% Satisf.">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "CompliancePct" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>   
                                                
                                                <asp:TemplateField AccessibleHeaderText="ExistItemCustomRules"  HeaderText="Reglas">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblExistItemCustomRules" runat="server" text='<%# (int)Eval( "ExistItemCustomRules" ) == 1 ? "SI": "NO" %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                                        
                                             </Columns>
                                       </asp:GridView>
                                     </div>               
                              <%-- FIN Detalle Simulación --%>  
                           </div>
                        </ContentTemplate>
                         <Triggers>
                             <asp:PostBackTrigger  ControlID="btnExportExcel"/>
                         </Triggers>
                    </asp:UpdatePanel>
                    
                    <asp:UpdateProgress ID="uprSimulacion" runat="server" AssociatedUpdatePanelID="upSimulacion" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprSimulacion" runat="server" ControlToOverlayID="divSimulation" CssClass="updateProgress" TargetControlID="uprSimulacion" />
                    
                     <%-- FIN Simulación de Liberación de Pedidos --%>        
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </RightPanel>                            
        </spl:Splitter>  
    </div>   

    <%-- pop up Pedidos Pendientes --%>
     <asp:UpdatePanel ID="upPendingOrders" runat="server" UpdateMode="Conditional" >
         <ContentTemplate>
             <div id="divPendingOrders" runat="server" visible="false">
                 <asp:Button ID="btnDummyPendingOrders" runat="Server" Style="display: none" />
                 <ajaxToolKit:ModalPopupExtender 
	                    ID="mpPendingOrders" runat="server" TargetControlID="btnDummyPendingOrders" 
	                    PopupControlID="pnlPendingOrders"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="panelCaptionPendingOrders" Drag="true" >
	             </ajaxToolKit:ModalPopupExtender>

                 <asp:Panel ID="pnlPendingOrders" runat="server" CssClass="modalBox">
                     <asp:Panel ID="panelCaptionPendingOrders" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitlePendingOrders" runat="server" Text="Pedidos Pendientes" />
                            <asp:ImageButton ID="ImageButtonPendingOrders" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                     </asp:Panel>

                     <div id="divCtrs1" style="height: 520px; width: 920px; " class="modalControls">
                         <div class="modalBoxContent">  
                             <div class="divCenter"><asp:Label ID="lblPendingOrders" runat="server" Text="Pedidos Pendientes" /></div>
                             <asp:ImageButton ID="btnAddToSelected" runat="server" onclick="btnAddToSelected_Click" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" ToolTip="Agregar Seleccionadas"/>	 
                             <div><asp:Label ID="lblPendingOrdersCount" runat="server" Text="Total:" /></div>
                             <div id="divPageGrdMgr" runat="server">
                                <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                    <asp:ImageButton ID="btnFirstgrdMgr" runat="server" OnClick="btnFirstgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                    <asp:ImageButton ID="btnPrevgrdMgr" runat="server" OnClick="btnPrevgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                    Pág. 
                                    <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSelectedIndexChanged" SkinID="ddlFilter" /> 
                                    de 
                                    <asp:Label ID="lblPageCount" runat="server" Text="" />
                                    <asp:ImageButton ID="btnNextgrdMgr" runat="server" OnClick="btnNextgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                    <asp:ImageButton ID="btnLastgrdMgr" runat="server" OnClick="btnLastgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                </div>
                             </div>

                             <%-- Grilla Principal --%>   
                             <asp:GridView ID="grdMgr" runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                AllowPaging="true" 
                                EnableViewState="False"
                                AutoGenerateColumns="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>

                                <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions" >
                                    <HeaderTemplate>
                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                    </HeaderTemplate>                
                                    <ItemTemplate> 
                                        <center>
                                        <div style="width:20px">
	                                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                                        </div>	                        
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                               
                                <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id" />

                                <asp:templatefield headertext="En Otra Sim." accessibleHeaderText="InOtherSimulation" SortExpression="InOtherSimulation">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:CheckBox ID="chkInSimulation" runat="server" checked='<%# Eval ( "InOtherSimulation" ) %>' Enabled="false"/>
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>
                                                
                                <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                    <itemtemplate>
                                        <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                    </itemtemplate>
                                    </asp:templatefield>
                                                                 
                                <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </div>                                                        
                                    </itemtemplate>
                                    </asp:templatefield>
                                                 
                                <asp:templatefield HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblWarehouseTargeteCode" runat="server" text='<%# Eval ( "WarehouseTarget.Code" ) %>' />
                                        </div>
                                    </itemtemplate>
                                    </asp:templatefield>
                                                                 
                                <asp:templatefield HeaderText="Centro Dist. Destino" AccessibleHeaderText="WarehouseTarget">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblWarehouseTarget" runat="server" text='<%# Eval ( "WarehouseTarget.ShortName" ) %>' />
                                        </div>
                                    </itemtemplate>
                                    </asp:templatefield>                 

                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'/>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.Name" ) %>'/>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField HeaderText="N° Doc." ItemStyle-Wrap="false" DataField="Number" 
                                    AccessibleHeaderText="OutboundNumber" >
                                </asp:BoundField>
                                               
                                <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                                
                                <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Doc.">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundType.Name") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                                
                                <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                        </center>    
                                </itemtemplate>
                                </asp:templatefield>
                                                
                                <asp:BoundField DataField="ReferenceNumber" HeaderText="N° Ref." AccessibleHeaderText="ReferenceNumber">
                                </asp:BoundField>
                                                
                                <asp:BoundField DataField="LoadCode" HeaderText="Cód. Carga" 
                                    ItemStyle-Wrap="false"  AccessibleHeaderText="LoadCode">

                                </asp:BoundField>
                                <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" 
                                    AccessibleHeaderText="LoadSeq" ItemStyle-Wrap="false">

                                </asp:BoundField>
                                <asp:BoundField DataField="Priority" HeaderText="Prioridad" 
                                    AccessibleHeaderText="Priority" ItemStyle-Wrap="false">
                                </asp:BoundField>
                                                
                                <asp:templatefield headertext="Liberación Autom." accessibleHeaderText="InmediateProcess">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:CheckBox ID="chkInmediateProcess" runat="server" checked='<%# Eval ( "InmediateProcess" ) %>' Enabled="false"/>
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>
                                                
                                <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="ExpectedDate" SortExpression="ExpectedDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblExpectedDate" runat="server" Text='<%# ((DateTime) Eval ("ExpectedDate") > DateTime.MinValue)? Eval("ExpectedDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>                         
                                <asp:TemplateField HeaderText="Emisión" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>                        
                                <asp:TemplateField HeaderText="Salida" AccessibleHeaderText="ShipmentDate" SortExpression="ShipmentDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblShipmentDate" runat="server" Text='<%# ((DateTime) Eval ("ShipmentDate") > DateTime.MinValue)? Eval("ShipmentDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>  
                                <asp:TemplateField HeaderText="Cancelación" AccessibleHeaderText="CancelDate" SortExpression="CancelDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCancelDate" runat="server" Text='<%# ((DateTime) Eval ("CancelDate") > DateTime.MinValue)? Eval("CancelDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>  
                                <asp:BoundField DataField="CancelUser" HeaderText="Usuario Cancelación" AccessibleHeaderText="CancelUser"/>
                                                
                                <asp:TemplateField HeaderText="Traza" accessibleHeaderText="OutboundTrack" SortExpression="OutboundTrack">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblOutboundTrack" runat="server" text='<%# Eval ( "LatestOutboundTrack.Type.Name" ) %>' />
                                        </div>
                                    </itemtemplate>
                                    </asp:TemplateField>
                                                
                                <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>


                                </asp:TemplateField>                
                                <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>


                                </asp:TemplateField>    


                                <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "Branch.Name" ) %>'/>
                                        </div>  
                                    </itemtemplate>
                                </asp:TemplateField>          
                                                
                                                
                                <asp:BoundField DataField="DeliveryAddress1" HeaderText="Dirección Entrega" AccessibleHeaderText="DeliveryAddress1"/>
                                <asp:BoundField DataField="DeliveryAddress2" HeaderText="Dirección Entrega Opc." AccessibleHeaderText="DeliveryAddress2"/>
                                                
                                <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "CountryDelivery.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>


                                </asp:TemplateField>
                                                
                                <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblStateDeliveryName" runat="server" text='<%# Eval ( "StateDelivery.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>


                                </asp:TemplateField>         
                                                
                                <asp:TemplateField HeaderText="Comuna Entrega" AccessibleHeaderText="CityDelivery" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCityDeliveryName" runat="server" text='<%# Eval ( "CityDelivery.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>


                                </asp:TemplateField>                         
                                                
                                <asp:BoundField DataField="DeliveryPhone" HeaderText="Teléfono" AccessibleHeaderText="DeliveryPhone"/>
                                <asp:BoundField DataField="DeliveryEmail" HeaderText="E-mail" AccessibleHeaderText="DeliveryEmail"/>
                                                
                                <asp:TemplateField HeaderText="Compl." AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "FullShipment" ) %>' Enabled="false" />
                                        </center>    
                                    </itemtemplate>
                                </asp:TemplateField>
                                                
                                <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCarrierCode" runat="server" text='<%# Eval ( "Carrier.Code" ) %>'/>
                                                </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:TemplateField>  
                                                
                                <asp:BoundField DataField="RouteCode" HeaderText="Ruta" AccessibleHeaderText="RouteCode"/>
                                <asp:BoundField DataField="Plate" HeaderText="Patente" AccessibleHeaderText="Plate"/>
                                <asp:BoundField DataField="Invoice" HeaderText="N° Factura" AccessibleHeaderText="Invoice"/>
                                <asp:BoundField DataField="FactAddress1" HeaderText="Dirección Factura" AccessibleHeaderText="FactAddress1"/>
                                <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Dirección Factura Opc." />
                                                                                                        
                                <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CountryFact" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCountryFactName" runat="server" text='<%# Eval ( "CountryFact.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:TemplateField>
                                                
                                <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="StateFact" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblStateFactName" runat="server" text='<%# Eval ( "StateFact.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:TemplateField>         
                                                
                                <asp:TemplateField HeaderText="Comuna Factura" AccessibleHeaderText="CityFact" ItemStyle-Wrap="false">
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCityFactName" runat="server" text='<%# Eval ( "CityFact.Name" ) %>'/>
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:TemplateField>   
                                                                                
                                <asp:BoundField DataField="FactPhone" HeaderText="Tel. Factura" AccessibleHeaderText="FactPhone"/>
                                <asp:BoundField DataField="FactEmail" HeaderText="E-mail Factura" AccessibleHeaderText="FactEmail"/>
                                </Columns>
                            </asp:GridView>
                             <%-- Fin Grilla Principal --%>

                             <%-- Detalle --%>
                            <div id="divDetail" runat="server" visible="false" class="divGridDetail" style="width:100%;">
                                <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Pendiente Doc: " />
                                <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                </div>
                                	            
                                <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                    DataKeyNames="Id" 
                                    EnableViewState="False" 
                                    AllowPaging="False"
                                    OnRowCreated="grdDetail_RowCreated" 
                                    AutoGenerateColumns="false"
                                    OnRowDataBound="grdDetail_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                           
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                                     
                                        <asp:BoundField DataField="LineNumber" HeaderText="N° Línea" AccessibleHeaderText="LineNumber" 
                                            ItemStyle-HorizontalAlign="Center" >
                                                                
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea"  AccessibleHeaderText="LineCode"
                                            ItemStyle-HorizontalAlign="Center" >
                                        </asp:BoundField>
                                                        
                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                        
                                        <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                                          
                                                       
                                        <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                        
                                        <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgItem">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                    
                                        <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQty" )) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"/>--%>
                                                        
                                        <asp:TemplateField HeaderText="Stock" AccessibleHeaderText="ItemStock">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemStock" )) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="ItemStock" HeaderText="Stock" AccessibleHeaderText="ItemStock"/>--%>
                                                       
                                        <asp:templatefield headertext="Activo" AccessibleHeaderText="DetailStatus">
                                            <ItemTemplate>
                                                    <asp:CheckBox ID="chkDetailStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                            </ItemTemplate>
                                        </asp:templatefield>

                                        <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber"/>
                                                        
                                        <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>              
                                                        
                                        <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield> 
                                                        
                                        <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>                                                         
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <%-- FIN Detalle --%>
                         </div>
                     </div>

                 </asp:Panel>
             </div>
         </ContentTemplate>
     </asp:UpdatePanel>

     <asp:UpdateProgress ID="uprPendingOrders" runat="server" AssociatedUpdatePanelID="upPendingOrders">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgressPendingOrders" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprPendingOrders" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprPendingOrders" />
     <%-- fin pop up Pedidos Pendientes --%>

     <%-- PopUp Liberar Pedidos --%>
     <asp:UpdatePanel ID="upRelease" runat="server" UpdateMode="Always" >
        <ContentTemplate>
            <div id="divReleaseDispatch" runat="server" visible="false">
	            <asp:Button ID="btnDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	                <ajaxToolKit:ModalPopupExtender 
	                    ID="mpReleaseDispatch" runat="server" TargetControlID="btnDummy" 
	                    PopupControlID="pnlReleaseDispatch"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="Caption" Drag="true" >
	                </ajaxToolKit:ModalPopupExtender>
                	
	                <asp:Panel ID="pnlReleaseDispatch" runat="server" CssClass="modalBox">
	                    <%-- Encabezado --%>			
		                <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
		                    <div class="divCaption">
			                    <asp:Label ID="lblEdit" runat="server" Text="Liberar Pedidos"/>
                                <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar"/>
                            </div>			        
	                    </asp:Panel>
                        <%-- Fin Encabezado --%>  
                         
		                <div class="modalControls">
		                    <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
		                        <div id="divPickingTitle" runat="server" visible="false" class="divControls"><u>Picking</u></div>
                                <div id="divUserNbr" runat="server" visible="false" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUserNbr" runat="server" Text="Nº de Operarios"/>
                                    </div>                    
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtUserNbr" runat="server" MaxLength="3" Width="30" Text="1"/>
                                        <asp:RequiredFieldValidator ID="rfvUserNbr" runat="server" ControlToValidate="txtUserNbr" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº de Operarios es requerido"/>
                                        <asp:RangeValidator ID="rvUserNbr" runat="server" ControlToValidate="txtUserNbr" ErrorMessage="Nº de Operarios no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />
                                    </div>                                  
                                </div>
                                <div id="divUserNbrSorting" runat="server" visible="false" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUserNbrSorting" runat="server" Text="Nº de Operarios para Sorting"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtUserNbrSorting" runat="server" MaxLength="3" Width="10" Text="1"/>
                                        <asp:RequiredFieldValidator ID="reqUserNbrSorting" runat="server" ControlToValidate="txtUserNbrSorting" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº de Operarios para Sorting es requerido"/>
                                        <asp:RangeValidator ID="rvUserNbrSorting" runat="server" ControlToValidate="txtUserNbrSorting" ErrorMessage="Nº de Operarios para Sorting no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                    </div>
                                </div>     
                                <div id="divPriority" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPriority" runat="server" Text="Prioridad"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPriority" runat="server" MaxLength="3" Width="30" Text="10"/>
                                        <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="txtPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido"/>
                                        <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                     </div>
                                </div>
                                <div id="divKitting" runat="server">
                                    <br />
                                    <div id="divKittingTitle" runat="server" visible="true" class="divControls">
                                        <u><asp:Label ID="lblKittingTitle" runat="server" Text="Armado de Kits"/></u>
                                    </div>
                                    <div id="divUserNbrKitting" runat="server" visible="true" class="divControls">
                                        <div class="fieldRight"><asp:Label ID="lblUserNbrKitting" runat="server" Text="Nº de Operarios"/></div>
                                        <div class="fieldLeft"><asp:TextBox ID="txtUserNbrKitting" runat="server" MaxLength="3" Width="10" Text="1" ReadOnly="true"/>
                                            <asp:RequiredFieldValidator ID="rfvUserNbrKitting" runat="server" ControlToValidate="txtUserNbrKitting" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº de Operarios para Kitting es requerido"/>
                                            <asp:RangeValidator ID="rvUserNbrKitting" runat="server" ControlToValidate="txtUserNbrKitting" ErrorMessage="Nº de Operarios para Kitting no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                        </div>
                                    </div>     
                                    <div id="divPriorityKitting" runat="server" class="divControls">                               
                                        <div class="fieldRight"><asp:Label ID="lblPriorityKitting" runat="server" Text="Prioridad"/></div>
                                        <div class="fieldLeft"><asp:TextBox ID="txtPriorityKitting" runat="server" MaxLength="3" Width="20" Text="10"/>
                                            <asp:RequiredFieldValidator ID="rfvPriorityKitting" runat="server" ControlToValidate="txtPriorityKitting" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad de Kitting es requerido"/>
                                            <asp:RangeValidator ID="rvPriorityKitting" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad de Kitting no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                         </div>
                                         <br />
                                    </div>       
                                </div>                                                             
                                <div id="divVas" runat="server">
                                    <br />
                                    <div id="divVasTitle" runat="server" visible="true" class="divControls">
                                        <u><asp:Label ID="lblVasTitle" runat="server" Text="Armado de Kits"/></u>
                                    </div>
                                    <div id="divUserNbrVas" runat="server" visible="true" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblUserNbrVas" runat="server" Text="Nº de Operarios"/>
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtUserNbrVas" runat="server" MaxLength="3" Width="10" Text="1"/>
                                            <asp:RequiredFieldValidator ID="rfvUserNbrVas" runat="server" ControlToValidate="txtUserNbrVas" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº de Operarios para Vas es requerido"/>
                                            <asp:RangeValidator ID="rvUserNbrVas" runat="server" ControlToValidate="txtUserNbrVas" ErrorMessage="Nº de Operarios para Vas no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                        </div>
                                    </div>     
                                    <div id="divPriorityVas" runat="server" class="divControls">                               
                                        <div class="fieldRight">
                                            <asp:Label ID="lblPriorityVas" runat="server" Text="Prioridad"/>
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtPriorityVas" runat="server" MaxLength="3" Width="20" Text="10"/>
                                            <asp:RequiredFieldValidator ID="rfvPriorityVas" runat="server" ControlToValidate="txtPriorityVas" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad de Vas es requerido"/>
                                            <asp:RangeValidator ID="rvPriorityVas" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad de Vas no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                         </div>
                                         <br />
                                    </div>       
                                </div>                                
		                        <div id="divLocStageTarget" runat="server" class="divControls">      
		                            <div class="fieldRight">
		                                <asp:Label ID="lblLocStageTarget" runat="server" Text="Ubicación de Packing" />
		                            </div> 
		                            <div class="fieldLeft">
		                                <asp:DropDownList ID="ddlLocStageTarget" runat="server" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvLocStageTarget" runat="server"
                                        ControlToValidate="ddlLocStageTarget" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Ubicación de Packing es requerido" />
                                    </div>
                                </div>                                
                                <div id="divLocStageDispatch" runat="server" class="divControls">      
		                            <div class="fieldRight">
		                                <asp:Label ID="lblLocStageDispatch" runat="server" Text="Ubicación de Embalaje" />
		                            </div> 
		                            <div class="fieldLeft">
		                                <asp:DropDownList ID="ddlLocStageDispatch" runat="server" Width="100" />
                                        <asp:RequiredFieldValidator ID="rfvLocStageDispatch" runat="server"
                                        ControlToValidate="ddlLocStageDispatch" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Ubicación de Embalaje es requerido" />
                                    </div>
                                </div>
                                <div id="divLocDock" runat="server" class="divControls">      
		                            <div class="fieldRight">
		                                <asp:Label ID="lblLocDock" runat="server" Text="Ubicación de Andén" />
		                            </div> 
		                            <div class="fieldLeft">
		                                <asp:DropDownList ID="ddlLocDock" runat="server" Width="100" />
                                        <asp:RequiredFieldValidator ID="rfvLocDock" runat="server"
                                        ControlToValidate="ddlLocDock" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Ubicación de Andén es requerido" />
                                    </div>
                                </div>
                                <div id="divPutawayLpn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPutawayLpn" runat="server" Text="Almacenar Lpn" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:CheckBox ID="chkPutawayLpn" runat="server" Width="10" />
                                    </div>
                                </div>
                                <div id="divCrossDock" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblCrossDock" runat="server" Text="Cross Dock" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:CheckBox ID="chkCrossDock" runat="server" Width="10" OnCheckedChanged="chkCrossDock_OnCheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                                
                                <div id="divSorting" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSorting" runat="server" Text="Embalaje" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:CheckBox ID="chkSorting" runat="server" Width="10" />
                                    </div>
                                </div>
                                
                                <div id="divBackOrder" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblBackOrder" runat="server" Text="Back Order" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:CheckBox ID="chkBackOrder" runat="server" Width="10" OnCheckedChanged="chkBackOrder_OnCheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                                <div id="divExpDateBackOrder" runat="server" class="divControls" style="height:32px; vertical-align: middle;">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblExpDateBackOrder" runat="server" Text="Expiración Back Order" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtExpDateBackOrder" runat="server" Width="80px" Enabled="true" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvExpDateBackOrder" runat="server" ControlToValidate="txtExpDateBackOrder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fecha es requerido" />
                                        <asp:RangeValidator ID="rvExpDateBackOrder" runat="server" ControlToValidate="txtExpDateBackOrder" ErrorMessage="Fecha debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01-01-2000" MaximumValue="31-12-2040" ValidationGroup="EditNew" Type="Date" />
                                    </div>
                                    
                                    <ajaxToolkit:CalendarExtender ID="calExpDateBackOrder" runat="server" CssClass="CalMaster" Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpDateBackOrder" PopupButtonID="txtExpDateBackOrder" Format="dd-MM-yyyy"></ajaxToolkit:CalendarExtender>
                                </div>
                           </div>
                       
                           <%-- Configuración de WorkZones - Liberación Pick and Pass (PIKPS) --%>
        <%--                  <asp:UpdatePanel ID="upWorkZones" runat="server" UpdateMode="Always">
                          <ContentTemplate>--%>
                           <div id="divWorkZones" runat="server" class="divCtrsFloatLeft">
                           
                                <div id="divPriorityPickAndPass" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPriorityPickAndPass" runat="server" Text="Prioridad"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPriorityPickAndPass" runat="server" MaxLength="3" Width="20" Text="10"/>
                                        <asp:RequiredFieldValidator ID="reqPriorityPickAndPass" runat="server" ControlToValidate="txtPriorityPickAndPass" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido"/>
                                        <asp:RangeValidator ID="rvPriorityPickAndPass" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                     </div>
                                </div>
                                <%-- Grilla de WorkZones --%>
                                <div id="divWorkZoneConfiguration" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="Label2" runat="server" Text="Configuración de Zonas"/>
                                    </div>
                                </div>
                                <div class="fieldLeft">
                                    <asp:GridView ID="grdWorkZones" runat="server"
                                    OnRowCreated="grdWorkZones_RowCreated" 
                                    OnRowCommand="grdWorkZones_RowCommand"   
                                    AllowPaging="false"
                                    EnableViewState="true"
                                    DataKeyNames="Id"
                                    AutoGenerateColumns="false"
                                    OnRowDataBound="grdWorkZones_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                       <asp:BoundField DataField="Id" ReadOnly="True" Visible="false"/>
                                       <asp:TemplateField ShowHeader="False">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdWorkZones.ClientID %>', 'chkSelectWorkZone', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>                
	                                        <ItemTemplate> 
	                                            <center>
	                                            <div style="width:20px">
		                                            <asp:CheckBox ID="chkSelectWorkZone" runat="server" />
                                                </div>	                        
                                                </center>
	                                        </ItemTemplate>
                                        </asp:TemplateField>                            
                                        
                                        <asp:BoundField DataField="Name" HeaderText="Zona" AccessibleHeaderText="Name" />
                     
                                         
                                        <asp:templatefield headertext="Ubic. Destino">
                                            <itemtemplate>
                                               <asp:DropDownList ID="ddlTargetLocation" runat="server" />
                                               <asp:requiredfieldvalidator ID="reqTargetLocation" runat="server"  ValidationGroup="EditNew" Text=" * " ErrorMessage="Ubicación Destino es requerido" 
                                                     controltovalidate="ddlTargetLocation" display="dynamic" InitialValue="-1" />
                                            </itemtemplate>
                                         </asp:templatefield>                                                                                         
                                        
                                        <asp:TemplateField HeaderText="Secuencia">
                                            <ItemTemplate> 
                                                <center>
                                                <div style="width:75px">
	                                                <asp:ImageButton ID="btnUp" runat="server" 
	                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_up.png" 
	                                                    onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_up_on.png';"
                                                        onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_up.png';"
	                                                    CausesValidation="false" 
	                                                    CommandName="Up"
	                                                    ToolTip="Mover Arriba"/>
	                                                <asp:ImageButton ID="btnDown" runat="server" 
	                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_down.png" 
	                                                    onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_down_on.png';"
                                                        onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_down.png';"
	                                                    CausesValidation="false"
	                                                    CommandName="Down"
	                                                    ToolTip="Mover Abajo"/>
                                                </div>	                        
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                    
                                    </Columns>
                                </asp:GridView>
                                </div>
                                <%-- FIN Grilla de WorkZones --%>
                           </div>     
        <%--                    </ContentTemplate>
                            </asp:UpdatePanel>     --%>              
                           <%-- FIN Configuración de WorkZones - Liberación Pick and Pass (PIKPS) --%>
                           
                            <div> 
                                <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                            </div>
                            <div id="divActions" runat="server" class="modalActions">
                                <asp:Button ID="btnRelease" runat="server" OnClick="btnRelease_Click" Text="Aceptar" CausesValidation="true"
                                    ValidationGroup="EditNew" OnClientClick="showProgress();"/>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                            </div>                    
                         </div>
	                </asp:Panel>
	        </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="chkBackOrder" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="chkCrossDock" EventName="CheckedChanged" />
        </Triggers>
     </asp:UpdatePanel>
     
    <asp:UpdateProgress ID="uprRelease" runat="server" AssociatedUpdatePanelID="upRelease" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprRelease" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprRelease" />
     
     <%-- FIN PopUp Liberar Despachos --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />    	
    <asp:Label id="lblNoOrdersSelected" runat="server" Text="Debe seleccionar al menos una Orden." Visible="false" />    	
    <asp:Label id="lblNoWhsSelected" runat="server" Text="Las Ordenes seleccionadas deben pertenecer al mismo Centro de Distribución." Visible="false" />    	
    <asp:Label id="lblNoOwnerSelected" runat="server" Text="Las Ordenes seleccionadas deben pertenecer al mismo Dueño." Visible="false" />    	
    <asp:Label ID="lblTitle" runat="server" Text="Liberar Pedidos" Visible="false"/>
    <asp:Label ID="lblTitleWave" runat="server" Text="Crear Ola" Visible="false"/>
    <asp:Label ID="lblTitleBatch" runat="server" Text="Crear Batch" Visible="false"/>
    <asp:Label ID="lblTitlePickAndPass" runat="server" Text="Crear Pick and Pass" Visible="false"/>
    <asp:Label ID="lblNoStock" runat="server" Text="No hay Stock suficiente para todos los pedidos. <br><br> ¿Desea continuar con la Liberación?" Visible="false"/>
    <asp:Label ID="lblOrderInSimulation" runat="server" Text="Existen Ordenes que se encuentran en otro proceso de Simulación. <br><br> ¿Desea continuar con la Liberación?" Visible="false"/>
    <asp:Label ID="lblNoStockFullShipment" runat="server" Text="No hay Stock suficiente para todos los pedidos" Visible="false"/>
    <asp:Label ID="lblNotExistCustomRuleItems" runat="server" Text="Para el N° Doc [DOC], ítem [ITEM] no se encontraron reglas asociadas." Visible="false"/>
    <asp:Label ID="lblPendingSimulation" runat="server" Text="Existe una Simulación pendiente. <br><br> ¿Desea cargarla?" Visible="false"/>
    <asp:Label ID="lblSimulation" runat="server" Text="Simulación " Visible="false" />
    <asp:Label ID="lblKittingTitleText" runat="server" Text="Armado de Kits" Visible="false" />
    <asp:Label ID="lblVasTitleText" runat="server" Text="Proceso VAS" Visible="false" />
    <asp:Label ID="lblUnkittingTitleText" runat="server" Text="Desarmado de Kits" Visible="false" />
    <asp:Label id="lblMsgErrorUbic" runat="server" Text="Debe seleccionar al menos una ubicación." Visible="false" />  
    <asp:Label id="lblTitleLocSorting" runat="server" Text="Ubicación de Separación" Visible="false" /> 
    <asp:Label id="lblTitleLocDispatch" runat="server" Text="Ubicación de Embalaje" Visible="false" /> 
    <asp:Label id="lblRequiredField" runat="server" Text=" es requerido" Visible="false" /> 

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>   
    
        <%-- Div Bloquea Pantalla al Momento de Realizar Carga Masiva --%>
    <div id="divFondoPopupProgress" class="loading" align="center" style="display: none;">
        Realizando Liberación <br />Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>

</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>
</asp:Content>
