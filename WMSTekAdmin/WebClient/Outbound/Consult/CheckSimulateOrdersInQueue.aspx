<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="CheckSimulateOrdersInQueue.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.CheckSimulateOrdersInQueue" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("TaskSimulation_GetTasksInQueue", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("TaskSimulation_GetTasksInQueue", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }

        function resizeDiv() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;

            var newHeight = parseInt(document.getElementById("hsMasterDetail_LeftP_Content").style.height.replace('px', '')) - 35;
            document.getElementById("ctl00_MainContent_hsMasterDetail_topPanel_ctl01_upGrid").style.height = newHeight.toString() + "px";
        }
        window.onresize = resizeDiv;

        function toggleCheckBoxesNew(gvId, chkId, isChecked) {

            var checkboxes = getCheckBoxesFrom(document.getElementById(gvId), chkId);
            var count = 0;

            for (i = 0; i <= checkboxes.length - 1; i++) {
                if (checkboxes[i].disabled != true) {
                    checkboxes[i].checked = isChecked;
                    count++;
                }
            }

            if (count > 1) {
                if (isChecked) {
                    document.getElementById("divSelectedTasks").style.visibility = "visible";
                } else {
                    document.getElementById("divSelectedTasks").style.visibility = "hidden";
                }
            }
            else {
                document.getElementById("divSelectedTasks").style.visibility = "hidden";
            }
        }


        function validateCheckBoxCount() {

            var checkboxes = getCheckBoxesFrom(document.getElementById('ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr'), "chkRemoveOrder");
            var count = 0;

            for (i = 0; i <= checkboxes.length - 1; i++) {
                if (checkboxes[i].checked == true) {
                    count++;
                }
            }

            if (count > 1) {
                document.getElementById("divSelectedTasks").style.visibility = "visible";
            }
            else {
                document.getElementById("divSelectedTasks").style.visibility = "hidden";
            }
        }

    </script>   


 <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="60">
                <Content>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                        <div id="divSelectedTasks" class="divGridTitleDispatch" style="visibility:hidden">
                            <div class="divCenter">
                                <asp:Label ID="lblSelectedOrders" runat="server" Text="Pedidos Seleccionados" />
                            </div>
                           <!-- <div class="divRightNew" style=" margin-right:50px; float: right; font-family: Verdana, Helvetica, Sans-Serif;
                            font-weight: normal; font-size: 11px;">
                                <asp:Label ID="lblSelectedOrdersCount" runat="server" Text="Total:" />
                            </div>-->
                            <asp:ImageButton ID="btnReprocess" runat="server" onclick="btnReprocess_Click"  ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process.png" ToolTip="Liberar"/>            
                             <asp:ImageButton ID="btnRemove" runat="server" onclick="btnRemove_Click" OnClientClick="if(confirm('¿Esta seguro de eliminar las tareas seleccionadas?')==false){return false;}"  ImageUrl="~/WebResources/Images/Buttons/GridActions/trash.png" ToolTip="Eliminar"/>   
                        </div>
                        <%--<div id="divSelectedTasksRemove" class="divGridTitleDispatch" style="visibility:visible">
                            <div class="divCenter">
                                <asp:Label ID="Label1" runat="server" Text="Eliminar Pedidos Seleccionados" />
                            </div>
                                                              
                        </div>--%>
                            </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnRelease" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <!--<webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender2" runat="server" ControlToOverlayID="divTop" 
                        CssClass="updateProgress" TargetControlID="UpdateProgress1" />-->

                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            DataKeyNames="Id"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnRowEditing="grdMgr_RowEditing"
                                            OnRowDeleting="grdMgr_RowDeleting"
                                            onrowcommand="grdMgr_RowCommand"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                             <Columns>
                                                 <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                                <asp:TemplateField ShowHeader="False" HeaderText="CheckBox" HeaderStyle-HorizontalAlign="Center" AccessibleHeaderText="CheckBox">
                                                    <HeaderTemplate>
                                                        <center>
                                                        <input type="checkbox" onclick="toggleCheckBoxesNew('<%= grdMgr.ClientID %>', 'chkRemoveOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                            </center>
                                                    </HeaderTemplate>                                                     
                                                    <ItemTemplate> 
                                                        <center>
                                                        <div style="width:20px">
	                                                        <asp:CheckBox ID="chkRemoveOrder" runat="server" onclick="validateCheckBoxCount()"  />
                                                        </div>	                        
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                 <%--<asp:BoundField DataField="Id" HeaderText="ID" accessibleHeaderText="Id"/>--%>
                                                <asp:templatefield HeaderText="Id Tarea" AccessibleHeaderText="Id" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblIdTask" runat="server" text='<%# Eval ( "Id" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "OutboundOrder.Warehouse.ShortName" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>
                                                 
                                                <asp:templatefield HeaderText="Tipo" AccessibleHeaderText="TypeCode" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblTypeCode" runat="server" text='<%# Eval ( "TypeCode" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                 <asp:templatefield headertext="PTL" AccessibleHeaderText="IsPtl">
                                                    <ItemTemplate>
                                                            <asp:CheckBox ID="chkIsPtl" runat="server" checked='<%# Eval ( "IsPtl" ) %>' Enabled="false"/>
                                                    </ItemTemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="N° Doc" AccessibleHeaderText="OutboundOrderNumber" >
                                                    <itemtemplate>                                        
                                                        <asp:label ID="lblNumberDocumentBound" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="% Simulación" AccessibleHeaderText="CompliancePct" >
                                                    <itemtemplate>
                                                        <center>
                                                            <asp:label ID="lblCompliancePct" runat="server" text='<%# (int) Eval ( "CompliancePct" ) < 0 ? 0 : Eval ( "CompliancePct" )%>' />
                                                        </center>
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Estado" AccessibleHeaderText="NameTrackTaskQueue" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblNameTrackTaskQueue" runat="server" text='<%# Eval ( "TrackTaskQueue.NameTrackTaskQueue" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Usuario" AccessibleHeaderText="User" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblUser" runat="server" text='<%# Eval ( "User.UserName" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="F. Creación" AccessibleHeaderText="DateCreated" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblDateCreated" runat="server" text='<%# Eval ( "DateCreated" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                   <asp:templatefield HeaderText="Número de Referencia" AccessibleHeaderText="ReferenceNumber" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblReferenceNumber" runat="server" text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                 <asp:templatefield HeaderText="Cliente" AccessibleHeaderText="CustomerName" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblCustomerName" runat="server" text='<%# Eval ( "OutboundOrder.CustomerName" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                   <asp:templatefield HeaderText="Sucursal" AccessibleHeaderText="Sucursal" >
                                                    <itemtemplate>
                                                        <asp:label ID="lblBranch" runat="server" text='<%# Eval ( "OutboundOrder.Branch.Name" ) %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:TemplateField ShowHeader="False" HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center" AccessibleHeaderText="Actions">
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="width: 140px">
                                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png"
                                                                    CausesValidation="false" CommandName="Edit" ToolTip="Reprocesar" />
                                                                <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                    CausesValidation="false" CommandName="Delete" ToolTip ="Cancelar" />
                                                                <asp:ImageButton ID="btnProcess" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process.png"
                                                                    CausesValidation="false" CommandName="Liberar" ToolTip="Liberar" CommandArgument='<%# Container.DataItemIndex %>' />
                                                                <asp:ImageButton ID="btnLink" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_release.png"
                                                                     CommandName="Documento" CommandArgument='<%# Container.DataItemIndex %>' />
                                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/trash.png"
                                                                     CommandName="Eliminar" ToolTip="Eliminar" CommandArgument='<%# Container.DataItemIndex %>'  />
                                                                 <asp:ImageButton ID="btnWaveDetail" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_view_detail.png"
                                                                     CommandName="DetalleOla" ToolTip="Detalle Ola" CommandArgument='<%# Container.DataItemIndex %>'  />                                                                
                                                                <asp:ImageButton ID="btnExcel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_view_detail.png"
                                                                     CommandName="DescargaExcel" ToolTip="Descargar Excel Ola" CommandArgument='<%# Container.DataItemIndex %>'  />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>                   
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" /> 
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$btnReprocess" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="40">
                <Content>
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div runat="server" class="col-md-5" id="divTitleDet" visible="false">
                                            <asp:Label ID="lblGridDetTask" runat="server" Text="Tarea: " />
                                            <asp:Label ID="lblNroTask" runat="server" Text=""/> 
                                            <asp:Label ID="lblGridDetail" runat="server" Text="  - Documento: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/> 
                                            <asp:ImageButton ID="btnExcelDetail" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png"
                                              CommandName="ExcelDetail" ToolTip="Exportar Detalle a Excel"  OnClick="btnExcelDetail_Click" />
                                        </div>                           
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
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
                                                    <asp:TemplateField HeaderText="Orden" AccessibleHeaderText="Orden">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOrden" runat="server" text='<%# Eval ( "OutboundDetail.OutboundOrder.Number" ) %>'/>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
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

                                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgName" SortExpression="CtgName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ("OutboundDetail.CategoryItem.Name") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>    

                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" SortExpression="LotNumber">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ("OutboundDetail.LotNumber") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>    

                                                    <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FifoDate") > DateTime.MinValue)? Eval("OutboundDetail.FifoDate", "{0:d}"):"" %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>    

                                                    <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FabricationDate") > DateTime.MinValue)? Eval("OutboundDetail.FabricationDate", "{0:d}"):"" %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>    
                                                
                                                    <asp:TemplateField HeaderText="Expiración" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.ExpirationDate") > DateTime.MinValue)? Eval("OutboundDetail.ExpirationDate", "{0:d}"):"" %>' />
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

                                                    <asp:TemplateField AccessibleHeaderText="TotalBoxes"  HeaderText="N° Cajas">
                                                        <ItemTemplate >
                                                            <div style="word-wrap: break-word;">                                            
                                                                <asp:label ID="lblTotalBoxesDetail" runat="server" text='<%# GetFormatedNumber((int)Eval("TotalBoxes")) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField> 
                                                
                                                                        
                                                    </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
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
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                            <asp:PostBackTrigger ControlID="btnExcelDetail" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

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
                            <div id="divNroDoc" class ="divControls" runat="server">
                                <asp:Label runat="server" ID="lblNroDocPopUp"></asp:Label>
                            </div>
                            <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
		                        <div id="divPickingTitle" runat="server" visible="false" class="divControls"><u>Picking</u></div>
                                <div id="divUserNbr" runat="server" visible="false" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUserNbr" runat="server" Text="N° de Operarios"/>
                                    </div>                    
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtUserNbr" runat="server" MaxLength="3" Width="30" Text="1"/>
                                        <asp:RequiredFieldValidator ID="rfvUserNbr" CssClass="error" runat="server" ControlToValidate="txtUserNbr" ValidationGroup="EditNew" Text=" * " ErrorMessage="N° de Operarios es requerido"/>
                                        <asp:RangeValidator ID="rvUserNbr" CssClass="error" runat="server" ControlToValidate="txtUserNbr" ErrorMessage="N° de Operarios no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />
                                    </div>                                  
                                </div>
                                <div id="divUserNbrSorting" runat="server" visible="false" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUserNbrSorting" runat="server" Text="N° de Operarios para Sorting"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtUserNbrSorting" runat="server" MaxLength="3" Width="30" Text="1"/>
                                        <asp:RequiredFieldValidator ID="reqUserNbrSorting" CssClass="error" runat="server"  ControlToValidate="txtUserNbrSorting" ValidationGroup="EditNew" Text=" * " ErrorMessage="N° de Operarios para Sorting es requerido"/>
                                        <asp:RangeValidator ID="rvUserNbrSorting" CssClass="error"  runat="server" ControlToValidate="txtUserNbrSorting" ErrorMessage="N° de Operarios para Sorting no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                    </div>
                                </div>     
                                <div id="divPriority" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPriority" runat="server" Text="Prioridad"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPriority" runat="server" MaxLength="3" Width="30" Text="1"/>
                                        <asp:RequiredFieldValidator ID="rfvPriority" CssClass="error" runat="server" ControlToValidate="txtPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido"/>
                                        <asp:RangeValidator ID="rvPriority" CssClass="error" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                     </div>
                                </div>
                                <div id="divPtlType" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPtlType" runat="server" Text="Tipo de PTL"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlPtlType" runat="server" Width="140" OnSelectedIndexChanged="ddlPtlType_SelectedIndexChanged" AutoPostBack="true" /> 
                                        <asp:RequiredFieldValidator ID="rfvPtlType" runat="server" ControlToValidate="ddlPtlType" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Tipo de PTL es requerido" />
                                     </div>
                                </div>
                                <div id="divKitting" runat="server">
                                    <br />
                                    <div id="divKittingTitle" runat="server" visible="true" class="divControls">
                                        <u><asp:Label ID="lblKittingTitle" runat="server" Text="Armado de Kits"/></u>
                                    </div>
                                    <div id="divUserNbrKitting" runat="server" visible="true" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblUserNbrKitting" runat="server" Text="N° de Operarios"/></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtUserNbrKitting" runat="server" MaxLength="3" Width="30" Text="1" ReadOnly="true"/>
                                            <asp:RequiredFieldValidator ID="rfvUserNbrKitting" CssClass="error" runat="server" ControlToValidate="txtUserNbrKitting" ValidationGroup="EditNew" Text=" * " ErrorMessage="N° de Operarios para Kitting es requerido"/>
                                            <asp:RangeValidator ID="rvUserNbrKitting" CssClass="error" runat="server" ControlToValidate="txtUserNbrKitting" ErrorMessage="N° de Operarios para Kitting no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                        </div>
                                    </div>     
                                    <div id="divPriorityKitting" runat="server" class="divControls">                               
                                        <div class="fieldRight">
                                            <asp:Label ID="lblPriorityKitting" runat="server" Text="Prioridad"/></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtPriorityKitting" runat="server" MaxLength="3" Width="30" Text="10"/>
                                            <asp:RequiredFieldValidator ID="rfvPriorityKitting" CssClass="error" runat="server" ControlToValidate="txtPriorityKitting" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad de Kitting es requerido"/>
                                            <asp:RangeValidator ID="rvPriorityKitting" CssClass="error" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad de Kitting no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
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
                                            <asp:Label ID="lblUserNbrVas" runat="server" Text="N° de Operarios"/>
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtUserNbrVas" runat="server" MaxLength="3" Width="30" Text="1"/>
                                            <asp:RequiredFieldValidator ID="rfvUserNbrVas"  CssClass="error" runat="server" ControlToValidate="txtUserNbrVas" ValidationGroup="EditNew" Text=" * " ErrorMessage="N° de Operarios para Vas es requerido"/>
                                            <asp:RangeValidator ID="rvUserNbrVas" CssClass="error" runat="server" ControlToValidate="txtUserNbrVas" ErrorMessage="N° de Operarios para Vas no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                        </div>
                                    </div>     
                                    <div id="divPriorityVas" runat="server" class="divControls">                               
                                        <div class="fieldRight">
                                            <asp:Label ID="lblPriorityVas" runat="server" Text="Prioridad"/>
                                        </div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtPriorityVas" runat="server" MaxLength="3" Width="30" Text="10"/>
                                            <asp:RequiredFieldValidator ID="rfvPriorityVas" CssClass="error" runat="server" ControlToValidate="txtPriorityVas" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad de Vas es requerido"/>
                                            <asp:RangeValidator ID="rvPriorityVas" CssClass="error" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad de Vas no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
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
                                    <asp:RequiredFieldValidator ID="rfvLocStageTarget" runat="server" CssClass="error"
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
                                        <asp:RequiredFieldValidator ID="rfvLocStageDispatch" runat="server" CssClass="error"
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
                                        <asp:RequiredFieldValidator ID="rfvLocDock" runat="server" CssClass="error"
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
                                        <asp:RequiredFieldValidator ID="rfvExpDateBackOrder" CssClass="error" runat="server" ControlToValidate="txtExpDateBackOrder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fecha es requerido" />
                                        <asp:RangeValidator ID="rvExpDateBackOrder" CssClass="error" runat="server" ControlToValidate="txtExpDateBackOrder" ErrorMessage="Fecha debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01-01-2000" MaximumValue="31-12-2040" ValidationGroup="EditNew" Type="Date" />
                                    </div>
                                    
                                    <ajaxToolkit:CalendarExtender ID="calExpDateBackOrder" runat="server" CssClass="CalMaster" Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpDateBackOrder" PopupButtonID="txtExpDateBackOrder" Format="dd-MM-yyyy"></ajaxToolkit:CalendarExtender>
                                </div>
                           </div>

                            <div id="divWorkZones" runat="server" class="divCtrsFloatLeft">
                           
                                <div id="divPriorityPickAndPass" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPriorityPickAndPass" runat="server" Text="Prioridad"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPriorityPickAndPass" runat="server" MaxLength="3" Width="30" Text="10"/>
                                        <asp:RequiredFieldValidator ID="reqPriorityPickAndPass" CssClass="error" runat="server" ControlToValidate="txtPriorityPickAndPass" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido"/>
                                        <asp:RangeValidator ID="rvPriorityPickAndPass" CssClass="error" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
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
                                    OnRowDataBound="grdMgr_RowDataBound"
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

                            <div> 
                                <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                            </div>
                            <div id="divActions" runat="server" class="modalActions">
                                <asp:Button ID="btnRelease" runat="server" OnClick="btnRelease_Click" Text="Aceptar" CausesValidation="true"
                                    ValidationGroup="EditNew" OnClientClick="showProgress();" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                            </div> 

                         </div>

	                </asp:Panel>
	        </div>
        </ContentTemplate>
        <Triggers>
           <%-- <asp:AsyncPostBackTrigger ControlID="chkBackOrder" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="chkCrossDock" EventName="CheckedChanged" />--%>
        </Triggers>
     </asp:UpdatePanel>
     
    <asp:UpdateProgress ID="uprRelease" runat="server" AssociatedUpdatePanelID="upRelease">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprRelease" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprRelease" />
     
     <%-- FIN PopUp Liberar Despachos --%>


    <%-- Mostrar Detalle de Pedidos de la OLA --%>
        <asp:UpdatePanel ID="upWaveDetail" runat="server" UpdateMode="Always" >
        <ContentTemplate>
            <div id="divWaveDetail" runat="server" visible="false">
	            <asp:Button ID="btnDummy1" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	                <ajaxToolKit:ModalPopupExtender 
	                    ID="mpWaveDetail" runat="server" TargetControlID="btnDummy1" 
	                    PopupControlID="pnlWaveDetail"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="Caption" Drag="true" >
	                </ajaxToolKit:ModalPopupExtender>
                	
	                <asp:Panel ID="pnlWaveDetail" runat="server" CssClass="modalBox" style="max-width:90%; max-height:80%">
	                    <%-- Encabezado --%>			
		                <asp:Panel ID="Panel2" runat="server" CssClass="modalHeader">
		                    <div class="divCaption">
			                    <asp:Label ID="lblDetWaveBatch" runat="server" Text="Documentos de la Ola"/>
                                <asp:ImageButton ID="btnCloseWave" runat="server" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar"/>
                            </div>	
                            <div id="divPageGrdSearchWaveOrders" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchWaveOrders" runat="server" OnClick="btnFirstGrdSearchWaveOrders_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchWaveOrders" runat="server" OnClick="btnPrevGrdSearchWaveOrders_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchWaveOrders" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchWaveOrdersSelectedIndexChanged" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchWaveOrders" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchWaveOrders" runat="server" OnClick="btnNextGrdSearchWaveOrders_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchWaveOrders" runat="server" OnClick="btnLastGrdSearchWaveOrders_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
	                    </asp:Panel>
                        <%-- Fin Encabezado --%>  

                        <div class="modalControls" style="max-width:99%">
                            <div class="modalBoxContent" >  
                                <div class="container">
                                    <asp:GridView ID="grdWaveOrders" runat="server" 
                                        OnRowCreated="grdWaveOrders_RowCreated"
                                        AllowPaging="True" 
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdWaveOrders_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id" Visible="false">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:templatefield HeaderText="Id Ola" AccessibleHeaderText="IdWave" Visible="false">
                                            <itemtemplate>
                                               <div style="word-wrap: break-word;">
                                                <asp:label ID="lblIdWave" runat="server" text='<%# Eval ( "Id" ) %>' />
                                               </div>
                                            </itemtemplate>
                                         </asp:templatefield>
                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" >
                                            <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            </div>
                                            </itemtemplate>
                                         </asp:templatefield>
                                        <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                            <itemtemplate>
                                               <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                               </div>
                                            </itemtemplate>
                                         </asp:templatefield>
                                        <asp:templatefield HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode"  >
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
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" >
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Nº Doc." DataField="Number" AccessibleHeaderText="OutboundNumber" />
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo Doc.">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="OutboundTypeName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundType.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Status" ) %>'
                                                     Enabled="false"/>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>
                                        <asp:BoundField DataField="ReferenceNumber" HeaderText="Nº Ref." AccessibleHeaderText="ReferenceNumber" >
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LoadCode" HeaderText="Cód. Carga"   AccessibleHeaderText="LoadCode" />
                                        <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" AccessibleHeaderText="LoadSeq" />
                                        <asp:BoundField DataField="Priority" HeaderText="Prioridad" AccessibleHeaderText="Priority" />
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
                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                         <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'></asp:Label>
                                                     </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                         <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>    
                                        <asp:BoundField DataField="DeliveryAddress1" HeaderText="Dirección Entrega" AccessibleHeaderText="DeliveryAddress1"/>
                                        <asp:BoundField DataField="DeliveryAddress2" HeaderText="Dirección Entrega Opc." AccessibleHeaderText="DeliveryAddress2"/>
                                        <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "CountryDelivery.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                     <asp:Label ID="lblStateDeliveryName" runat="server" text='<%# Eval ( "StateDelivery.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>         
                                        <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CityDelivery" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                         <asp:Label ID="lblCityDeliveryName" runat="server" text='<%# Eval ( "CityDelivery.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>                         
                                        <asp:BoundField DataField="DeliveryPhone" HeaderText="Tel. Entrega" AccessibleHeaderText="DeliveryPhone"/>
                                        <asp:BoundField DataField="DeliveryEmail" HeaderText="E-mail Entrega" AccessibleHeaderText="DeliveryEmail"/>
                                        <asp:TemplateField HeaderText="Preparar Completo" AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "FullShipment" ) %>' Enabled="false" />
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                         <asp:Label ID="lblCarrierCode" runat="server" text='<%# Eval ( "Carrier.Code" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>  
                                        <asp:BoundField DataField="RouteCode" HeaderText="Ruta" AccessibleHeaderText="RouteCode"/>
                                        <asp:BoundField DataField="Plate" HeaderText="Patente" AccessibleHeaderText="Plate"/>
                                        <asp:BoundField DataField="Invoice" HeaderText="Nº Factura" AccessibleHeaderText="Invoice" />
                                        <asp:BoundField DataField="FactAddress1" HeaderText="Dirección Factura" AccessibleHeaderText="FactAddress1"/>
                                        <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Dirección Factura Opc." />
                                        <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CountryFact" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryFactName" runat="server" text='<%# Eval ( "CountryFact.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="StateFact" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblStateFactName" runat="server" text='<%# Eval ( "StateFact.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>         
                                        <asp:TemplateField HeaderText="Ciudad Factura" AccessibleHeaderText="CityFact" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCityFactName" runat="server" text='<%# Eval ( "CityFact.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>   
                                        <asp:BoundField DataField="FactPhone" HeaderText="Tel. Factura" AccessibleHeaderText="FactPhone"/>
                                        <asp:BoundField DataField="FactEmail" HeaderText="E-mail Factura" AccessibleHeaderText="FactEmail"/>
                                     </Columns>
                                </asp:GridView>
                                </div>
                            </div>
                        </div>


                    </asp:Panel>
	        </div>
        </ContentTemplate>
        <Triggers>
           <%-- <asp:AsyncPostBackTrigger ControlID="chkBackOrder" EventName="CheckedChanged" />--%>
        </Triggers>
     </asp:UpdatePanel>

     <asp:UpdateProgress ID="uprWaveDetail" runat="server" AssociatedUpdatePanelID="upWaveDetail">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress4" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprWaveDetail" />
    <%-- Fin Detalle Ola--%>



    <asp:Label id="lblDocName" runat="server" Text="Nº Doc." Visible="false" />    
    <asp:Label ID="lblConfirmCancel" runat="server" Text="¿Desea cancelar esta simulación?" Visible="true" />
    <asp:Label ID="lblDelete" runat="server" Text="¿Desea eliminar esta simulación, tarea [TASK]?" Visible="true" />
    <asp:Label ID="lblRetryCancel" runat="server" Text="¿Desea reprocesar esta simulación?" Visible="true" />
    <asp:Label ID="lblProcess" runat="server" Text="¿Desea realizar la liberación del documento [DOC]?" Visible="true" />
    <asp:Label ID="lblProcessBatch" runat="server" Text="¿Desea realizar la liberación de Batch, Id Task [TASK]?" Visible="true" />
    <asp:Label ID="lblProcessWave" runat="server" Text="¿Desea realizar la liberación de la Ola, Id Task [TASK]?" Visible="true" />
    <asp:Label ID="lblProcessWavePtl" runat="server" Text="¿Desea realizar la liberación de la Ola PTL, Id Task [TASK]?" Visible="true" />
    <asp:Label ID="lblUser" runat="server" Text="Usuario" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Estado Simulación en Cola" Visible="false"/>
    <asp:Label ID="lblNoStock" runat="server" Text="No hay Stock suficiente para todos el ó los pedidos. <br><br> ¿Desea continuar con la Liberación?" Visible="false"/>
    <asp:Label ID="lblOrderInSimulationWave" runat="server" Text="La Ordene que se encuentran en otro proceso de Simulación. <br><br> ¿Desea continuar con la Liberación?" Visible="false"/>
    <asp:Label ID="lblOrderInSimulation" runat="server" Text="La Orden [DOC] se encuentra en otro proceso de Simulación. <br><br> ¿Desea continuar con la Liberación?" Visible="false"/>
    <asp:Label ID="lblNoStockFullShipment" runat="server" Text="No hay Stock suficiente para todos los pedidos" Visible="false"/>
    <asp:Label ID="lblNotExistCustomRuleItems" runat="server" Text="Para el N° Doc [DOC], ítem [ITEM] no se encontraron reglas asociadas." Visible="false"/>
    <asp:Label ID="lblNotExistCustomRuleItemsWave" runat="server" Text="Para el ítem [ITEM] no se encontraron reglas asociadas." Visible="false"/>
    <asp:Label id="lblTitleLocSorting" runat="server" Text="Ubicación de Separación" Visible="false" /> 
    <asp:Label id="lblTitleLocDispatch" runat="server" Text="Ubicación de Embalaje" Visible="false" /> 
    <asp:Label id="lblRequiredField" runat="server" Text=" es requerido" Visible="false" /> 
    <asp:Label ID="lblKittingTitleText" runat="server" Text="Armado de Kits" Visible="false" />
    <asp:Label ID="lblVasTitleText" runat="server" Text="Proceso VAS" Visible="false" />
    <asp:Label ID="lblUnkittingTitleText" runat="server" Text="Desarmado de Kits" Visible="false" />
    <asp:Label id="lblMsgErrorUbic" runat="server" Text="Debe seleccionar al menos una ubicación." Visible="false" /> 
    <asp:Label id="lblNroWaveGenerate" runat="server" Text="Numero de OLA Generada:  [NROWAVE]" Visible="false" /> 
    <asp:Label id="lblNroOrderQueuedGenerate" runat="server" Text="Documento liberado de forma Batch" Visible="false" /> 
    <asp:Label id="lblNroWaveQueuedGenerate" runat="server" Text="Numero de OLA Generada de forma Batch: [NROWAVE]" Visible="false" /> 
    <asp:Label id="lblNroBatchQueuedGenerate" runat="server" Text="Numero de BATCH Generada de forma Batch: [NROBATCH]" Visible="false" /> 
    <asp:Label id="lblDescReleaseDoc" runat="server" Text="Tarea: [TASK]  - Tipo: [TYPE] -  Documento: [DOC]" Visible="false" /> 
    <asp:Label id="lblDescReleaseWave" runat="server" Text="Tarea: [TASK]  - Tipo: [TYPE]" Visible="false" /> 
    <asp:Label id="lblDescReleaseWaves" runat="server" Text="Tareas - Tipo: [TYPE]" Visible="false" /> 
    <asp:Label ID="lblCompliancePct" runat="server" Text="% Simulación" Visible="false" />
    <asp:Label ID="lblCreateDate" runat="server" Text="F. Creación" Visible="false" />
    <asp:Label ID="lblTaskNoSelected" runat="server" Text="No existen tareas seleccionadas." Visible="false" />
    <asp:Label ID="lblDistinctTaskType" runat="server" Text="No es posible mezclar distintos tipos de tareas." Visible="false" />
    <asp:Label id="lblTaskWhitoutDetail" runat="server" Text="Tarea [TASK] NO posee detalles." Visible="false" />
    <asp:Label id="lblReleaseSelected" runat="server" Text="Documentos liberados correctamente." Visible="false" /> 
    <asp:Label id="lblSimulateRelease" runat="server" Text="Documento [OUTBOUND] se encuentra en proceso de liberación." Visible="false" /> 
    <asp:Label ID="lblTaskTypeSuccessful" runat="server" Text="No es posible liberar tareas en estado distinto de Exitoso." Visible="false" />
    <asp:Label id="lblOrdersRelease" runat="server" Text="Existen documentos que ya se encuentran liberados ó en proceso de liberación" Visible="false" /> 
    <asp:Label id="lblSimulateReleaseBatch" runat="server" Text="Para la tarea [TASK] existen documentos en proceso de liberación." Visible="false" /> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
