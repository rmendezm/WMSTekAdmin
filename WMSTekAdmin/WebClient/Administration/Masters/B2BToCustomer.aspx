<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="B2BToCustomer.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.B2BToCustomer" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
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
            initializeGridDragAndDrop('B2BToCustomerHeader', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }

        function validateConfirm() {
            var response = confirm('¿Desea eliminar fila seleccionada?');
            console.log("response " + response);
            return response;
        }

        function validateCorrelativeLPN(oSrc, args) {
            var idCorrelativeLPN = '<%= txtCorrelativeLPN.ClientID %>';
            var correlativeLPN = $("#" + idCorrelativeLPN).val();

            if (correlativeLPN != "") {
                var isNumeric = parseInt(correlativeLPN);

                if (isNaN(isNumeric)) {
                    oSrc.errormessage = "Correlativo LPN no es numérico";
                    args.IsValid = false;
                } else {
                    args.IsValid = true;
                }
            } else {
                args.IsValid = true;
            }
        }

        function validateCorrelativeLength(oSrc, args) {
            var idNumberLength = '<%= txtNumberLength.ClientID %>';
            var numberLength = $("#" + idNumberLength).val();

            if (numberLength != "") {
                var isNumeric = parseInt(numberLength);

                if (isNaN(isNumeric)) {
                    oSrc.errormessage = "Largo no es numérico";
                    args.IsValid = false;
                } else {
                    args.IsValid = true;
                }
            } else {
                args.IsValid = true;
            }
        }

        function validateCorrelativeBranches() {
            $("#ctl00_MainContent_grdBranches input[type='text'][name$='txtCorrelativeBranch']").blur(function (e) {
                if ($(this).val() != "") {
                    if (isNaN($(this).val())) {
                        $(this).val("");
                        $(this).focus();
                    }
                }
            });
        }

        function ShowProgress() {
            document.getElementById('<% Response.Write(uprEditBranchCorrelative.ClientID); %>').style.display = "inline";
        }

        $(document).ready(function () {
            validateCorrelativeBranches();
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
            validateCorrelativeBranches();
        }
    </script>

    <style>
        #ctl00_MainContent_panelAddB2BCustomer {
	        z-index: 10000000 !important;
	        top: 0;
        }
	
        .modalControls	{
	        overflow: auto;
            max-height: 500px;
            max-width: 100%;
        }

        #ctl00_ucContentDialog_pnlDialog{
            z-index: 10000000 !important;
        }
    </style>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Principal --%>
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate> 
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            DataKeyNames="Id" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            OnRowCreated="grdMgr_RowCreated"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                <asp:templatefield HeaderText="Id" AccessibleHeaderText="Id" ItemStyle-CssClass="text" Visible="false">
                                                    <itemtemplate>
                                                        <asp:Label ID="lblId" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield HeaderText="Acciones"  AccessibleHeaderText="Actions">
                                                    <itemtemplate>
                                                        <asp:ImageButton ID="btnAddB2B" runat="server" text='Agregar B2B' ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" CommandArgument='<%# Container.DataItemIndex %>' CommandName="btnAddB2B"  OnClick="btnAddB2B_Click" />
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                                <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />

                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblTradeName" runat="server" Text='<%# Eval  ( "Owner.Name" ) %>' ></asp:Label>
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

            <BottomPanel HeightMin="50">
                <Content>
                    <%-- Panel Grilla Detalle --%>
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>   
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll"> 
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server"
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnRowDeleting="grdDetail_RowDeleting"
                                                OnRowEditing="grdDetail_RowEditing"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false" >                       
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 60px">
                                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" CommandName="Edit" ToolTip="Editar" />
                                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" CommandName="Delete" ToolTip="Eliminar" OnClientClick="return validateConfirm();" />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Id" AccessibleHeaderText="Id" ItemStyle-CssClass="text" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblId" runat="server" text='<%# Eval ( "customerB2B.Id" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Código Etiqueta LPN" AccessibleHeaderText="LabelCodeLPN" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLabelCodeLPN" runat="server" text='<%# Eval ( "customerB2B.LabelCodeLPN" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Código Etiqueta Precio" AccessibleHeaderText="LabelCodePrice" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLabelCodePrice" runat="server" text='<%# Eval ( "customerB2B.LabelCodePrice" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Prefijo Cliente" AccessibleHeaderText="PrefixLabel" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPrefixLabel" runat="server" text='<%# Eval ( "customer.PrefixLabel" ) %>'></asp:Label>
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
                    <%-- FIN Panel Grilla Detalle --%>

                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
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

    <%-- Ingresar B2B --%>
    <asp:UpdatePanel ID="upAddB2BCustomer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divAddB2BCustomer" runat="server" visible="true">
                <asp:Button ID="btnDummyAddB2BCustomer" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalAddB2BCustomer" runat="server" TargetControlID="btnDummyAddB2BCustomer"
                    PopupControlID="panelAddB2BCustomer" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionAddB2BCustomer"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelAddB2BCustomer" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionAddB2BCustomer" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitleAddB2BCustomer" runat="server" Text="Agregar B2B" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditIdCustomerB2B" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <%--ASN--%>
                            <div runat="server" id="divAsn">
                                <div class="divControls" runat="server" id="diChkAsn">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblAsn" Text="Archivo ASN" runat="server" />
                                    </div>
                                    <div class="fieldLeft">                      
                                        <asp:CheckBox runat="server" ID="chkAnsFile" AutoPostBack="True" OnCheckedChanged="chkAnsFile_CheckedChanged" />                                              
                                    </div>  
                                </div>  
                            
                                <div id="divDdlAsn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblAsnTemplate" Text="Template Archivo ASN" runat="server" />
                                    </div>
                                    <div class="fieldLeft">   
                                        <asp:DropDownList ID="ddlTemplateAsn" Enabled="false" runat="server" Width="250px"></asp:DropDownList>    
                                        <asp:RequiredFieldValidator ID="rfvAsnTemplate" ValidationGroup="VGAddB2BCustomer" ControlToValidate="ddlTemplateAsn"
                                            runat="server" ErrorMessage="Template Archivo ASN es requerido" Text=" * " Display="Dynamic" InitialValue="-1"  ></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
							<%--Fin ASN--%>

                            <%--LPN--%>
                            <div id="div4" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLabelLpn" Text="Etiqueta LPN" runat="server" />
                                </div>
                                <div class="fieldLeft">     
                                    <asp:CheckBox runat="server" ID="chkLabelLpn" AutoPostBack="True" OnCheckedChanged="chkLabelLpn_CheckedChanged" />                                                    
                                </div>
                            </div>
                                        
                            <div id="div7" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTemplateLpn" Text="Template Etiqueta Lpn" runat="server" />
                                </div>
                                <div class="fieldLeft">   
                                    <asp:DropDownList ID="ddlLabelLpn" Enabled="false" runat="server" Width="250px"></asp:DropDownList>                                                                                                  
                                    <asp:RequiredFieldValidator ID="rfvTemplateLpn" ValidationGroup="VGAddB2BCustomer" ControlToValidate="ddlLabelLpn"
                                        runat="server" ErrorMessage="Template Etiqueta Lpn es requerido" Text=" * " Display="Dynamic" InitialValue="-1" ></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <%-- Fin LPN--%>

                            <%--Price--%>
                            <div runat="server" id="divPrice">
                                <div id="divChkPrice" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblLabelPrice" Text="Etiqueta Precio" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:CheckBox runat="server" ID="chkLabelPrice" AutoPostBack="True" OnCheckedChanged="chkLabelPrice_CheckedChanged"/>                                                    
                                    </div>
                                </div>
                                        
                                <div id="divDdlPrice" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblTemplatePrice" Text="Template Etiqueta Precio" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:DropDownList ID="ddlLabelPrice" Enabled="false" runat="server" Width="250px"></asp:DropDownList>                                                 
                                        <asp:RequiredFieldValidator ID="rfvTemplatePrice" ValidationGroup="VGAddB2BCustomer" ControlToValidate="ddlLabelPrice"
                                            runat="server" ErrorMessage="Template Etiqueta Precio es requerido" Text=" * " Display="Dynamic" InitialValue="-1" ></asp:RequiredFieldValidator>       
                                    </div>
                                </div>
                            </div>
                            <%--Fin Price--%>
                            <div runat="server" id="divUomType">
                                <div runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUomTypeLpn" Text="Habilitar Presentación B2B" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:CheckBox runat="server" ID="chkUomTypeLpn" AutoPostBack="True" OnCheckedChanged="chkUomTypeLpn_CheckedChanged"/>                                                    
                                    </div>
                                </div>

                                 <div runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUomTypeLpnCode" Text="Presentación B2B" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:DropDownList ID="ddlUomTypeLpnCode" Enabled="false" runat="server" Width="250px"></asp:DropDownList>                                                 
                                        <asp:RequiredFieldValidator ID="rfvUomTypeLpnCode" ValidationGroup="VGAddB2BCustomer" ControlToValidate="ddlUomTypeLpnCode"
                                            runat="server" ErrorMessage="Presentación es requerida" Text=" * " Display="Dynamic" InitialValue="-1"  Enabled="false"></asp:RequiredFieldValidator>       
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="divCorrelative">
                                <div id="div2" runat="server" class="divControls">
                                     <div class="fieldRight">
                                        <asp:Label ID="lblCorrelativeCustomer" Text="Prefijo Cliente" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:TextBox runat="server" ID="txtCorrelativeCustomer"></asp:TextBox>                                                  
                                    </div>
                                </div>
                            </div>

                            <div id="divCorrelativeLPN" runat="server" class="divControls" visible="false">
                                 <div class="fieldRight">
                                    <asp:Label ID="lblCorrelativeLPN" Text="Correlativo LPN" runat="server" />
                                </div>
                                <div class="fieldLeft">     
                                    <asp:TextBox runat="server" ID="txtCorrelativeLPN"></asp:TextBox>      
                                    <asp:CustomValidator id="custValCorrelativeLPN" runat="server" ControlToValidate="txtCorrelativeLPN"
		                                Text=" * " ValidationGroup="VGAddB2BCustomer" ClientValidationFunction="validateCorrelativeLPN" >
                                    </asp:CustomValidator>
                                </div>

                                 <div id="div9" runat="server" class="divControls">
                                     <div class="fieldRight">
                                        <asp:Label ID="lblNumberLength" Text="Largo" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:TextBox runat="server" ID="txtNumberLength"></asp:TextBox>   
                                        <asp:CustomValidator id="custValNumberLength" runat="server" ControlToValidate="txtNumberLength"
		                                    Text=" * " ValidationGroup="VGAddB2BCustomer" ClientValidationFunction="validateCorrelativeLength" >
                                        </asp:CustomValidator>
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="divPackingList">
                                <div runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblLabelLPNPackingList" Text="Lista Packing" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:CheckBox runat="server" ID="chkLabelLPNPackingList" AutoPostBack="True" OnCheckedChanged="chkLabelLPNPackingList_CheckedChanged"/>                                                    
                                    </div>
                                </div>

                                <div runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblLabelCodeLPNPackingList" Text="Código Lista Packing" runat="server" />
                                    </div>
                                    <div class="fieldLeft"> 
                                        <asp:DropDownList ID="ddlLabelCodeLPNPackingList" Enabled="false" runat="server" Width="250px"></asp:DropDownList>                                                 
                                        <asp:RequiredFieldValidator ID="rfvLabelCodeLPNPackingList" ValidationGroup="VGAddB2BCustomer" ControlToValidate="ddlLabelCodeLPNPackingList"
                                            runat="server" ErrorMessage="Etiqueta es requerida" Text=" * " Display="Dynamic" InitialValue="-1"  Enabled="false"></asp:RequiredFieldValidator> 
                                    </div>
                                </div>

                                <div runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblMaxLinesPackingList" Text="Cantidad máxima detalle en Lista Packing" runat="server" />
                                    </div>
                                    <div class="fieldLeft"> 
                                        <asp:TextBox runat="server" ID="txtMaxLinesPackingList" MaxLength="2"></asp:TextBox>   
                                        <asp:RequiredFieldValidator ID="rqvMaxLinesPackingList" ControlToValidate="txtMaxLinesPackingList" ValidationGroup="VGAddB2BCustomer"
                                            runat="server" ErrorMessage="Cantidad máxima detalle en Lista Packing es requerido" Text=" * " Enabled="false" />
                                        <asp:RegularExpressionValidator ID="revtxtMaxLinesPackingList" runat="server" ControlToValidate="txtMaxLinesPackingList"
	                                            ErrorMessage="Intervalo debe ser ser un número positivo" 
	                                            ValidationExpression="^[1-9]\d*$"
	                                            ValidationGroup="VGAddB2BCustomer" Text=" * " Enabled="false" >
                                        </asp:RegularExpressionValidator>  
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="divSucursal">
                                 <div id="div13" runat="server" class="divControls">
                                     <div class="fieldRight">
                                        <strong><asp:Label ID="lblSearchBranch" Text="Buscar Sucursal" runat="server" /></strong>
                                    </div>
                                </div>

                                <div id="div10" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSearchBranchByCode" Text="Código Sucursal" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:TextBox runat="server" ID="txtSearhBranchByCode"></asp:TextBox>
                                    </div>
                                </div>

                                <div id="div11" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSearchBranchByName" Text="Nombre Sucursal" runat="server" />
                                    </div>
                                    <div class="fieldLeft">     
                                        <asp:TextBox runat="server" ID="txtSearhBranchByName"></asp:TextBox>
                                    </div>
                                </div>

                                <div id="div12" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Button runat="server" ID="btnSearchBranch" Text="Buscar" OnClick="btnSearchBranch_Click" />
                                    </div>
                                </div>
                            </div>

                            <div id="divBranch" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblBranch" Text="" runat="server" />
                                </div>
                                <div class="fieldLeft">                 
                                    <asp:GridView ID="grdBranches" runat="server"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false"
                                        OnRowCommand="grdBranches_RowCommand"
                                        AutoGenerateColumns="False">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                <ItemTemplate>
                                                    <center>
                                                        <div style="width: 60px">
                                                            <%--<asp:ImageButton ID="btnEditBranchConfig" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" CommandName="EditBranch" CommandArgument='<%# Eval("Id") %>' ToolTip="Editar" />--%>
                                                            <asp:ImageButton ID="btnSaveBranchConfig" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" CommandName="SaveBranch" CommandArgument='<%# Eval("Id") %>' ToolTip="Guardar" />
                                                        </div>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="IdBranch" ItemStyle-CssClass="text" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIdBranch" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="IdOwn" ItemStyle-CssClass="text" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIdOwn" runat="server" text='<%# Eval ( "Owner.Id" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Codigo" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCode" runat="server" text='<%# Eval ( "Code" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                            <asp:TemplateField HeaderText="Prefijo" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtPrefixBranch" runat="server" Text='<%# Eval ( "PrefixLabel" ) %>'></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Código Etiqueta LPN" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCodeCorrelativeLpnBranch" runat="server" Text='<%# (( string.IsNullOrEmpty((string)Eval( "correlativeB2BBranch.LabelCodeLPN" )) ) ? "" : Eval ( "correlativeB2BBranch.LabelCodeLPN" )) %>' Enabled="false"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Correlativo LPN" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCorrelativeBranch" runat="server" Text='<%# (((decimal)Eval ( "correlativeB2BBranch.Correlative" )) == -1 ? "" : Eval ( "correlativeB2BBranch.Correlative" )) %>' Enabled="false"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Largo Correlativo LPN" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtNumberLength" runat="server" Text='<%# (((int)Eval ( "correlativeB2BBranch.NumberLength" )) == -1 ? "" : Eval ( "correlativeB2BBranch.NumberLength" )) %>' Enabled="false"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Código Etiqueta Precio" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCodeCorrelativePriceBranch" runat="server" Text='<%# (( string.IsNullOrEmpty((string)Eval( "correlativeB2BBranch.LabelCodePrice" )) ) ? "" : Eval ( "correlativeB2BBranch.LabelCodePrice" )) %>' Enabled="false"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="lblIdWhs" ItemStyle-CssClass="text" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIdWhs" runat="server" text='<%# Eval ( "correlativeB2BBranch.Warehouse.Id" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                           
                        </div>
                        <div style="clear: both"></div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsAddB2BCustomer" runat="server" ValidationGroup="VGAddB2BCustomer" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div3" runat="server" class="modalActions">
                            <asp:Button ID="btnAcceptAddB2BCustomer" runat="server" Text="Aceptar" ValidationGroup="VGAddB2BCustomer" OnClick="btnAcceptAddB2BCustomer_Click" />
                            <asp:Button ID="btnCancelAddB2BCustomer" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprAddB2BCustomer" runat="server" AssociatedUpdatePanelID="upAddB2BCustomer" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprAddB2BCustomer" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprAddB2BCustomer" />
    <%-- Fin Ingresar B2B  --%>

    <asp:UpdatePanel ID="upEditBranchCorrelative" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divBranchCorrelative" runat="server" visible="false">
                <asp:Button ID="btnDummyBranchCorrelative" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalBranch" runat="server" TargetControlID="btnDummyBranchCorrelative"
                    BehaviorID="BImodalBranch" PopupControlID="pnlBranchCorrelative" BackgroundCssClass="modalBackground"
                    PopupDragHandleControlID="BranchCorrelativeCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlBranchCorrelative" runat="server" CssClass="modalBox" Style="display: none;">
                    
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Seleccionar Etiquetas" />
                        </div>
                    </asp:Panel>

                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <asp:HiddenField ID="hidBranchId" runat="server" />
                            <div id="divBranchLpnLabel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblBranchLpnLabel" runat="server" Text="Etiqueta LPN" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:DropDownList ID="ddlBranchLpnLabel" runat="server" Width="250px"></asp:DropDownList>                                                 
                                </div>
                            </div>

                            <div id="divBranchPriceLabel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblBranchPriceLabel" runat="server" Text="Etiqueta Precio" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:DropDownList ID="ddlBranchPriceLabel" runat="server" Width="250px"></asp:DropDownList>                                                 
                                </div>
                            </div>

                        </div>

                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnUpdateBranch" runat="server" Text="Aceptar" OnClick="btnUpdateBranch_Click" OnClientClick="ShowProgress();" />
                            <asp:Button ID="btnCancelUpdateBranch" runat="server" Text="Cancelar" OnClick="btnCancelUpdateBranch_Click" />
                        </div>

                    </div>

                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprEditBranchCorrelative" runat="server" AssociatedUpdatePanelID="upEditBranchCorrelative" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadEditBranchCorrelative" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditBranchCorrelative" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditBranchCorrelative" />

    <asp:Label ID="lblTitleCustomerCode" runat="server" Text="Código Cliente" Visible="false" />
    <asp:Label ID="lblTitleCustomerName" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label id="lblAddLoadToolTip" runat="server" Text="Ingresar" Visible="false" /> 
    <asp:Label ID="lblMessajeDeleteOK" runat="server" Text="Eliminación se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblMessajeCreateOK" runat="server" Text="Inserción se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblMessajeUpdateOK" runat="server" Text="Modificación se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblValidatorCreateCustomerB2B" runat="server" Text="Debe ingresar al menos uno de los items" Visible="false" />
    <asp:Label ID="lblValidateFiltersSearchBranch" runat="server" Text="Debe ingresar un filtro para buscar sucursales" Visible="false" />
    <asp:Label ID="lblNoBranchesFound" runat="server" Text="No se encontraron sucursales en la búsqueda" Visible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
