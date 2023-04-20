<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="ReasonsLogicalWarehouseConfig.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.ReasonsLogicalWarehouseConfig" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language='Javascript'>
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            //initializeGridDragAndDrop('InboundOrder_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr', 'InboundOrderConsult');
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
            var extraSpace = 160;

            var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
        }

        function validateConfirm() {
            var response = confirm('¿Desea eliminar fila seleccionada?');
            return response;
        }

        function ShowProgress() {
            document.getElementById('<% Response.Write(uprGridDetail.ClientID); %>').style.display = "inline";
        }

        function HideMessage() {
            document.getElementById("divFondoPopup").style.display = 'none';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

            return false;
        }

        function ShowMessage(title, message) {

            var position = (document.body.clientWidth - 400) / 2 + "px";
            document.getElementById("divFondoPopup").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

            document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
            document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

            return false;
        }

        function postbackAction(id) {
            javascript: __doPostBack('ctl00$MainContent$hsMasterDetail$ctl00$ctl01$grdDetail', 'DeleteReason$' + id);
        }

        function deleteReason(id) {
            console.log("deleteReason");
            var response = confirm('¿Desea eliminar fila seleccionada?');
            console.log("response deleteReason " + response);

            if (response) {
                postbackAction(id);
            } else {
                return response;
            }
        }
    </script>

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

    <div id="divPrincipal" style=" width:100%; height:100%; margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            DataKeyNames="Id" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            AllowPaging="True" 
                                            EnableViewState="false"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "warehouseCode" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "description" ) %>' />
                                                        </div>
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
                                    </Triggers>
                                </asp:UpdatePanel>

                                <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgressList" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                            </div>
                        </div>
                    </div>
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                 <%-- Panel Grilla Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                <asp:Label ID="lblGridDetail" runat="server" Text="CD seleccionado: " />
                                                <asp:Label ID="lblSelectedLogicalWhs" runat="server" Text="" /> 
                                                <br />
                                                <asp:Button runat="server" ID="btnCreateReason" Text="Asociar Razón" OnClick="btnCreateReason_Click" />
                                            </div>
                                            <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false"
                                                SkinID="grdDetail" OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnRowEditing="grdDetail_RowEditing"
                                                AutoGenerateColumns="false"
                                                OnRowCommand="grdDetail_RowCommand"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Razón asociada" AccessibleHeaderText="ReasonCode" SortExpression="LongName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblReasonCode" runat="server" Text='<%# Eval ("reasonCode") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 60px">
                                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar" CausesValidation="false" OnClientClick=<%# string.Format("return deleteReason({0});",  Eval("Id")) %> CommandName="DeleteReason" CommandArgument="<%# Container.DataItemIndex %>" />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
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
                                    </Triggers>
                                </asp:UpdatePanel>

                                <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgressDetail" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                            </div>
                        </div>
                    </div>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalReason" runat="server" TargetControlID="btnDummy"
                    BehaviorID="BImodalReason" PopupControlID="pnlReason" BackgroundCssClass="modalBackground"
                    PopupDragHandleControlID="pnlReason" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlReason" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="pnlCaptionReason" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Asociar razón" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />

                        <div class="divCtrsFloatLeft">
                            <div id="divReasons" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblReasons" runat="server" Text="Razones" />
                                </div>
                                <div class="fieldLeft">
                                   <asp:CheckBoxList id="chkListReasons" ValidationGroup="ReasonsValidationGroup" CellPadding="5" CellSpacing="5" RepeatColumns="2" RepeatDirection="Vertical" RepeatLayout="Flow" TextAlign="Right"  runat="server">
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                        </div>

                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="ReasonsValidationGroup" CssClass="modalValidation" ShowMessageBox="false" />
                        </div>

                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSaveReason" runat="server" Text="Aceptar" OnClick="btnSaveReason_Click"
                                OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="ReasonsValidationGroup" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCerrar_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgressEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div> 

    <asp:Label ID="lblMessajeDeleteOK" runat="server" Text="Eliminación se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblMessajeCreatedOK" runat="server" Text="Asociación se creó de forma existosa." Visible="false" />
    <asp:Label ID="lblMessajeUpdatedOK" runat="server" Text="Asociación se actualizó de forma existosa." Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Asociacón razón a CD" Visible="false"/> 

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
      <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
