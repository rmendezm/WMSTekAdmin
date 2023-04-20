<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="IssuesOnOrder.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.IssuesOnOrder" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            initializeGridDragAndDrop('IssuesOnOrder', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
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
        function showProgressEmpty() {
            //if (document.getElementById('ctl00_MainContent_uploadImage').value.length > 0) {
            //    setTimeout(function () {
            //        var modal = $('<div />');
            //        modal.addClass("modalLoading");
            //        $('body').append(modal);
            //        var loading = $(".loading");

            //        var top = Math.max($(window).height() / 3.5, 0);
            //        var left = Math.max($(window).width() / 2.6, 0);
            //        loading.css({ top: top, left: left });
            //        loading.show();
            //    }, 30);
            //    return true;

            //} else {
            //    return false;
            //}
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

        function HideMessage() {
            document.getElementById("divFondoPopup").style.display = 'none';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';
            return false;
        }
    </script>

    <style>
        .btnOpenPopUp{
            margin-bottom: 10px;
        }
        .imageIssue {
            max-width: 400px;
            max-width: 400px;
        }
        #ctl00_MainContent_pnlGetImages {
            position: fixed !important;
	        top: 50% !important;
	        left: 50% !important;
	        -webkit-transform: translate(-50%, -50%) !important;
	        -ms-transform: translate(-50%, -50%) !important;
	        transform: translate(-50%, -50%) !important;
            max-height: 500px;
            width: 400px;
        }
    </style>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                <asp:templatefield HeaderText="Cód. CD" AccessibleHeaderText="WarehouseCode">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                        </div>
                                                    </itemtemplate>
                                                 </asp:templatefield>

                                                 <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                         </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>' />
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

                                                <asp:TemplateField HeaderText="Nº Doc." AccessibleHeaderText="Number">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblNumber" runat="server" Text='<%# Eval ( "Number" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:templatefield headertext="Fecha Emisión" accessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                                    <itemtemplate>
                                                        <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>' />
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOutboundType" runat="server" Text='<%# Eval ( "OutboundType.Code" ) %>' />
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
            <BottomPanel HeightMin="120">
                <Content>
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-5">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Documento : " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/> 
                                        </div>
                                        <div class="col-md-7">
                                            <asp:Button runat="server" ID="btnOpenPopUp" OnClick="btnOpenPopUp_Click" Text="Crear Incidencia" CssClass="btnOpenPopUp"/>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                OnRowDeleting="grdDetail_RowDeleting"
                                                OnRowEditing="grdDetail_RowEditing"
                                                EnableTheming="false"
                                                AllowPaging="false">  

                                                <Columns>

                                                    <asp:templatefield headertext="Fecha Incidencia" accessibleHeaderText="DateIssue" SortExpression="DateIssue">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblDateIssue" runat="server" Text='<%# ((DateTime) Eval ("DateIssue") > DateTime.MinValue)? Eval("DateIssue", "{0:d}"):"" %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:templatefield headertext="Mensaje" accessibleHeaderText="MessageIssue" SortExpression="MessageIssue">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblMessageIssue" runat="server" Text='<%# ShowMaxLenghtInString((string)Eval("MessageIssue"), 30) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 60px">
                                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                                        CausesValidation="false" CommandName="Edit" AlternateText=" " ToolTip="Editar y/o Agregar Imágenes" />
                                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                        CausesValidation="false" CommandName="Delete" />
                                                                </div>
                                                            </center>
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

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlIssue" BackgroundCssClass="modalBackground" PopupDragHandleControlID="IssueCaption">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlIssue" runat="server" CssClass="modalBox">
                    <asp:Panel ID="IssueCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="headerTitle" runat="server"></asp:Label>
                            <asp:Label ID="lblNew" runat="server" Text="Nueva incidencia" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar incidencia" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />

                        <div class="divCtrsFloatLeft">

                            <div id="divMessageIssue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMessageIssue" runat="server" Text="Mensaje" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtMessageIssue" runat="server" MaxLength="500" Width="300" Height="200" TextMode="MultiLine" />
                                    <asp:RequiredFieldValidator ID="rfvtxtReferenceDocNumber" runat="server" ControlToValidate="txtMessageIssue"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Mensaje es requerido" />  
                                </div>
                            </div>

                            <div id="divUploadImages" runat="server" class="divControls" visible="false">
                                <div class="fieldRight">
                                    <asp:Label ID="lblUploadImage" runat="server" Text="Subir Imagen" />
                                </div>
                                <div class="fieldLeft">                            
                                    <asp:LinkButton runat="server" ID="lkUploadImage" OnClick="lkUploadImage_Click" Text="Click Acá" CssClass="linkDecoration"></asp:LinkButton>
                                </div>  
                            </div>

                            <div id="divGetImages" runat="server" class="divControls" visible="false">
                                <div class="fieldRight">
                                    <asp:Label ID="lblGetImages" runat="server" Text="Ver Imágenes" />
                                </div>
                                <div class="fieldLeft">                            
                                    <asp:LinkButton runat="server" ID="lkGetImages" OnClick="lkGetImages_Click" Text="Click Acá" CssClass="linkDecoration"></asp:LinkButton>
                                </div>  
                            </div>

                        </div>

                        <div style="clear: both"></div>
			            <div class="divValidationSummary" >
				            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false" CssClass="modalValidation"/>
			            </div>
			            <div id="divActions" runat="server" class="modalActions">
				            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Guardar" CausesValidation="true" ValidationGroup="EditNew" />
				            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
			            </div>         

                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upUploadImage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divUploadImagePopUp" runat="server" visible="false">
                <asp:Button ID="btnDummyUploadImage" runat="Server" Style="display: none" />
                 <ajaxToolkit:ModalPopupExtender ID="UploadImagePopUp" runat="server" TargetControlID="btnDummyUploadImage"
                    PopupControlID="pnlUploadImage" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UploadImageCaption">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlUploadImage" runat="server" CssClass="modalBox">
                    <asp:Panel ID="UploadImageCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblUploadImagePopUp" runat="server" Text="Subir Imagen" />
                            <asp:ImageButton ID="btnCloseImagePopUp" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="divUploadImageControl" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblUploadImageControl" runat="server" Text="Subir Imagen" />
                                </div>
                                <div class="fieldLeft">                            
                                    <asp:FileUpload ID="uploadImage" runat="server" Width="400px" ValidationGroup="valUploadFile" />    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar una imágen"
                                    ValidationGroup="valUploadFile" ControlToValidate="uploadImage"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revUpload" runat="server" ControlToValidate="uploadImage"
                                    ErrorMessage="Solo imágenes son válidas" ForeColor="Red"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.jpg|.jpeg|.gif|.png)$"
                                    ValidationGroup="valUploadFile" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                </div>  
                            </div>
                        </div>
                        <div style="clear: both"></div>
                         <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="valUploadFile" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>   
                        <div id="divActionsUploadImage" runat="server" class="modalActions">
				            <asp:Button ID="btnSaveUploadImage" runat="server" OnClick="btnSaveUploadImage_Click" Text="Guardar" CausesValidation="true" ValidationGroup="valUploadFile" OnClientClick="showProgressEmpty();"  />
				            <asp:Button ID="btnCancelUploadImage" runat="server" Text="Cancelar" OnClick="btnCancelUploadImage_Click" />
			            </div>         
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveUploadImage"/>      
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />         
        </Triggers>
    </asp:UpdatePanel>

   <%-- <asp:UpdateProgress ID="uprUploadImage" runat="server" AssociatedUpdatePanelID="upUploadImage">
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressGrid" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprUploadImage" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprUploadImage" />--%>

    <asp:UpdatePanel ID="upGetImages" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divGetImagesPopUp" runat="server" visible="false">
                <asp:Button ID="btnDummyGetImages" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="GetImagesPopUp" runat="server" TargetControlID="btnDummyGetImages"
                    PopupControlID="pnlGetImages" BackgroundCssClass="modalBackground" PopupDragHandleControlID="GetImagesCaption">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlGetImages" runat="server" CssClass="modalBox">
                    <asp:Panel ID="GetImagesCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Imágenes incidencia" />
                            <asp:ImageButton ID="btnCloseGetImages" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <div class="modalControls" id="modalImages">
                        <div class="divCtrsFloatLeft">

                            <div class="divControls" id="divGridImages">
                                <div class="fieldRight">
                                    <asp:Label ID="lblImages" Text="Imágenes" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:GridView ID="grdImages" runat="server"
		                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
		                                EnableTheming="false"
		                                AutoGenerateColumns="False"
                                        OnRowDeleting="grdImages_RowDeleting"
                                        OnRowCreated="grdImages_RowCreated"
                                        AllowPaging="false">
		                                <Columns>
			                                <asp:TemplateField HeaderText="Ver" ItemStyle-CssClass="text">
				                                <ItemTemplate>
                                                    <asp:HyperLink ID="lblGetImage" runat="server" Target="_blank" CssClass="linkDecoration" NavigateUrl='<%# Eval ( "ImageIssueUrl" ) %>'>Ver Imágen</asp:HyperLink>
				                                </ItemTemplate>
			                                </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                <ItemTemplate>
                                                    <center>
                                                        <div style="width: 60px">
                                                            <asp:ImageButton ID="btnDeleteImage" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" CausesValidation="false" CommandName="Delete" />
                                                        </div>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>
		                                </Columns>
	                                </asp:GridView>
                                </div>
                            </div>
                        </div>
                        <div style="clear: both"></div>
                        <div id="divPopUpActions" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelGetImages" runat="server" Text="Cancelar" OnClick="btnCancelGetImages_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- Carga masiva de items --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Incidencias" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Incidencias.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />   
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />

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

    <div id="divFondoPopupProgress" class="loading" align="center" style="display: none;">
        Realizando Carga Masiva <br />Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>

    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta incidencia?" Visible="false" />
    <asp:Label ID="lblFileUploadSuccessfully" runat="server" Text="Imágen subida correctamente" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Incidencias" Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblValidateSizeImage" runat="server" Text="Imagen es muy grande. Limite máximo son " Visible="false" />
    <asp:Label ID="lblNoImagesFound" runat="server" Text="No hay imágenes asociadas a incidencia" Visible="false" />
    <asp:Label ID="lblErrorUploadingImage" runat="server" Text="No se pudo cargar imágen" Visible="false" />
    <asp:Label ID="lblConfirmDeleteImage" runat="server" Text="¿Desea eliminar esta imágen?" Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblTooManyRows" runat="server" Text="Limite excedido. Cantidad máxima son 200 registros por Excel" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
