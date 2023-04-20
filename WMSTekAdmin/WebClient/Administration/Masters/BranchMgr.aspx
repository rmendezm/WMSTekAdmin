<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BranchMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.BranchMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>
<script type="text/javascript" language="javascript">
    window.onresize = resizeDivPrincipal; 
    
    function resizeDivPrincipal() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
    }

    function clearFilterDetail(gridDetail) {
        if ($("#" + gridDetail).length == 0) {
            if ($("div.container").length == 2) {
                //$("div.container:last div.row:first").remove();
                $("div.container:last div.row-height-filter").remove(); 
            }   
        }
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdMgr';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('Branch_GetByIdCustomer', gridDetail);
    }

</script>

    <style>
        #ctl00_MainContent_pnl .fieldRight {
            width: 120px !important;
        }
    </style>

  <div runat="server" id="divMainPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>
                                    <div class="divGridTitle"><asp:Label ID="lblBranchGridTitle" runat="server" Text="Clientes" /></div>
                                    <%-- Grilla de Items --%>
                                    <asp:GridView ID="grdCustomer" 
                                    DataKeyNames="Id" runat="server" 
                                    AllowPaging="True" EnableViewState="False" 
                                    OnSelectedIndexChanged="grdCustomer_SelectedIndexChanged"
                                    OnRowCreated="grdCustomer_RowCreated" 
                                    OnRowDataBound="grdCustomer_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false"
                                        >
                                    <Columns>
                         
                                        <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" />
                                        <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                        <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblTradeName" runat="server" Text='<%# Eval  ( "Owner.Name" ) %>' ></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Address1Fact" HeaderText="Dir. Fac." AccessibleHeaderText="Address1Fact" />
                                        <asp:BoundField DataField="Address2Fact" HeaderText="Dir. Fac. Opc." AccessibleHeaderText="Address2Fact" />
                                        <asp:TemplateField HeaderText="País Fac." AccessibleHeaderText="CountryNameFact">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCountryFact" runat="server" Text='<%# Eval ( "CountryFact.Name" ) %>' />
                                               </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Fac." AccessibleHeaderText="StateNameFact">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">                                
                                                    <asp:Label ID="lblStateFact" runat="server" Text='<%# Eval ( "StateFact.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Comuna Fac." AccessibleHeaderText="CityNameFact">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCityFact" runat="server" Text='<%# Eval ( "CityFact.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PhoneFact" HeaderText="Teléfono Fac." AccessibleHeaderText="PhoneFact" />
                                        <asp:BoundField DataField="FaxFact" HeaderText="Fax Fac." AccessibleHeaderText="FaxFact" />
                                        <asp:BoundField DataField="Address1Delv" HeaderText="Dir. Entrega" AccessibleHeaderText="Address1Delv" />
                                        <asp:BoundField DataField="Address2Delv" HeaderText="Dir. Entrega Opc." AccessibleHeaderText="Address2Delv" />
                                        <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryNameDelv">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCountryDelv" runat="server" Text='<%# Eval ( "CountryDelv.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateNameDelv">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStateDelv" runat="server" Text='<%# Eval ( "StateDelv.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Comuna Entrega" AccessibleHeaderText="CityNameDelv">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCityDelv" runat="server" Text='<%# Eval ( "CityDelv.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PhoneDelv" HeaderText="Teléfono Entrega" AccessibleHeaderText="PhoneDelv" />
                                        <asp:BoundField DataField="FaxDelv" HeaderText="Fax Entrega" AccessibleHeaderText="FaxDelv" />
                                        <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
                                        <asp:BoundField DataField="Priority" HeaderText="Prioridad" AccessibleHeaderText="Priority" />
                                        <asp:BoundField DataField="TimeExpected" HeaderText="Tiempo Esperado" AccessibleHeaderText="TimeExpected" />
                                        <asp:BoundField DataField="ExpirationDays" HeaderText="Dias Vencimiento" AccessibleHeaderText="ExpirationDays" />
                                        <asp:BoundField DataField="SpecialField1" HeaderText="Tipo" AccessibleHeaderText="SpecialField1" />
                                            
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <%-- FIN Grilla de Items --%>
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
                <asp:UpdateProgress ID="uprItems" runat="server" AssociatedUpdatePanelID="upGrid" 
                DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>                        
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="upoeItems" runat="server" ControlToOverlayID="topPanel" CssClass="updateProgress" TargetControlID="uprItems" />       
                <%-- FIN Panel Grilla Principal --%>    
            </Content>
        </TopPanel>
        <BottomPanel ID="bottomPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Detalle --%>
                            <asp:UpdatePanel ID="upGridBranch" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <%-- Uoms --%>
                                    <div id="divBranch" runat="server" visible="false" class="divGridDetailScroll">
                                        <div class="divGridDetailTitle">
                                            <asp:Label ID="lbBranchGridTitle" runat="server" Text="Sucursales: " />
                                            <asp:Label ID="lblCustomerName" runat="server" Text="" />
                                        </div>
                                        <%-- Grilla UOM --%>
                            
                                        <asp:Label ID="lblEmpty" runat="server" Text="Debe seleccionar un cliente" ForeColor="Red" />
                                        <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" 
                                        AllowPaging="False" EnableViewState="False" 
                                        OnRowCreated="grdMgr_RowCreated"
                                        OnRowDeleting="grdMgr_RowDeleting" 
                                        OnRowEditing="grdMgr_RowEditing"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        AutoGenerateColumns="false"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false"
                                            >
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="ID" AccessibleHeaderText="Id" />
                                        <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" ToolTip="Eliminar" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Código" AccessibleHeaderText="BranchCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBranchCode" runat="server" Text='<%# Eval("Code") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="BranchName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBranchName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                        
                                        <asp:TemplateField HeaderText="País" AccessibleHeaderText="CountryName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCountryName" runat="server" Text='<%# Eval ( "Country.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Región" AccessibleHeaderText="StateName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStateName" runat="server" Text='<%# Eval ( "State.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Comuna" AccessibleHeaderText="CityName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCityName" runat="server" Text='<%# Eval ( "City.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>  
                        
                                        <asp:TemplateField HeaderText="Dirección" AccessibleHeaderText="BranchAddress">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBranchAddress" runat="server" Text='<%# Eval("BranchAddress") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                        
                                        <asp:BoundField DataField="Phone" HeaderText="Teléfono" AccessibleHeaderText="Phone" />
                                        <asp:BoundField DataField="Distance" HeaderText="Distancia" AccessibleHeaderText="Distance" />

                                        <asp:TemplateField HeaderText="SpecialField1" AccessibleHeaderText="Campo Especial 1">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval("SpecialField1") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="SpecialField2" AccessibleHeaderText="Campo Especial 2">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval("SpecialField2") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="SpecialField3" AccessibleHeaderText="Campo Especial 3">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval("SpecialField3") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="SpecialField4" AccessibleHeaderText="Campo Especial 4">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval("SpecialField4") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGrid" runat="server" Text="Este Cliente no tiene sucursales asociadas." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                        <%-- FIN Grilla UOM --%>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />                        
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdCustomer" EventName="RowCommand" />
                                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                                     <%--<asp:AsyncPostBackTrigger ControlID="ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdItem" EventName="RowCommand" />--%>
                                </Triggers>
                            </asp:UpdatePanel>  
                        </div>
                    </div>
                 </div>    
                 <asp:UpdateProgress ID="uprDetail" runat="server" AssociatedUpdatePanelID="upGridBranch" 
                 DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>                        
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                 <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" 
                 ControlToOverlayID="divBranch" CssClass="updateProgress" TargetControlID="uprDetail" />     
                <%-- FIN Panel Grilla Detalle --%>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
    </spl:HorizontalSplitter>
  </div>    
    
    <%-- Panel Nuevo/Editar Branch --%>
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <%-- Pop up Editar/Nueva Branch --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnl" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Sucursal" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Sucursal" />                             
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            
                            <%-- Code --%>                     
                            <div id="divBranchCode" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblBranchCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtBranchCode" runat="server" MaxLength="20" Width="140" />
                                    <asp:RequiredFieldValidator ID="rfvBranchCode" runat="server" ControlToValidate="txtBranchCode"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revBranchCode" runat="server" ControlToValidate="txtBranchCode"
                                         ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
                                         ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                         ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            
                            <%-- Name --%>
                            <div id="divBranchName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblBranchName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtBranchName" runat="server" MaxLength="60" Width="150px" />
                                    <asp:RequiredFieldValidator ID="rfvBranchName" runat="server" ControlToValidate="txtBranchName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revBranchName" runat="server" 
                                    ControlToValidate="txtBranchName"
                                    ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
                                    ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ(),_.-]*"
                                    ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>                                                  
                                </div>
                            </div>
                            
                            <%-- Address --%>
                            <div id="divBranchAddress" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblBranchAddress" Text="Dirección" runat="server" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtBranchAddress" runat="server" MaxLength="60"/>
                                    <asp:RequiredFieldValidator ID="rfvBranchAddress" ValidationGroup="EditNew" ControlToValidate="txtBranchAddress"
                                        runat="server" ErrorMessage="Dirección es requerida" Text=" * " ></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revBranchAddress" runat="server" 
                                    ControlToValidate="txtBranchAddress" ErrorMessage="Dirección permite ingresar solo caracteres alfanuméricos" 
                                    ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ#(),_.-]*"
                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div>
                            
                            <%-- Country --%>
                            <div id="divCountry" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCountry" Text="País" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                                        Width="130px" />
                                    <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ControlToValidate="ddlCountry" Display="dynamic" InitialValue="-1"
                                        ErrorMessage="País es requerido" />
                                </div>
                            </div>
                            
                            <%-- State --%>
                            <div id="divState" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblState" Text="Región" runat="server" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged"
                                        Width="130px" />
                                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ControlToValidate="ddlState" Display="dynamic" InitialValue="-1"
                                        ErrorMessage="Región es requerido" />
                                </div>
                            </div>
                            
                            <%-- City --%>
                            <div id="divCity" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCity" Text="Ciudad" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCity" runat="server" Width="130px" />
                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ControlToValidate="ddlCity" Display="dynamic" InitialValue="-1"
                                        ErrorMessage="Ciudad es requerido" />
                                </div>
                            </div>
                            
                            <%-- Distance --%>
                            <div id="divDistance" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDistance" Text="Distancia" runat="server" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDistance" runat="server" MaxLength="60"/>
                                    <asp:RequiredFieldValidator ID="rfvDistance" ValidationGroup="EditNew" ControlToValidate="txtDistance"
                                        runat="server" ErrorMessage="Distancia es requerida" Text=" * " ></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revDistance" runat="server" 
                                    ControlToValidate="txtDistance" ErrorMessage="Distancia permite ingresar solo caracteres alfanuméricos" 
                                    ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div>
                            
                            <%-- Phone --%>
                            <div id="divPhone" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPhone" Text="Telefono" runat="server" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPhone" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhone" Text=" * " ValidationGroup="EditNew" ControlToValidate="txtPhone"
                                        runat="server" ErrorMessage="Telefono es requerido"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revPhone" runat="server" 
                                    ControlToValidate="txtPhone" ErrorMessage="Telefono permite ingresar solo números" 
                                    ValidationExpression="[0-99999999999]*"
                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div>

                            <div id="divSpecialField1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSpecialField1" Text="Campo Especial 1" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSpecialField1" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divSpecialField2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSpecialField2" Text="Campo Especial 2" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSpecialField2" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divSpecialField3" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSpecialField3" Text="Campo Especial 3" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSpecialField3" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divSpecialField4" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSpecialField4" Text="Campo Especial 4" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSpecialField4" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
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
            <%-- FIN Pop up Editar/Nuevo Presentación --%>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$ctl01$grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

     <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
     <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Panel Nuevo/Editar ItemUom --%>

    <%-- Carga masiva de Clientes --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Sucursales --%>
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
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Sucursales" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Sucursal.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div9" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwnerLoad" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                        InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile2" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile2"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile2">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div10" runat="server" class="modalActions">
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
    
    <div id="divFondoPopupProgress" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;" runat="server">
        <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
    </div>
    
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
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Sucursal?" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Largo" Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Sucursales" Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen sucursales en el archivo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFieldLength" runat="server" Text="Largo del [FIELD] debe ser de [LENGTH] como máximo." Visible="false" />
    
    <asp:Label ID="lblGln" runat="server" Text="GLN" Visible="false" />
  <%--  <asp:Label ID="lblGlnIsNotNumeric" runat="server" Text="Código GLN debe ser Numérico." Visible="false" />
    <asp:Label ID="lblGlnLengthInvalid" runat="server" Text="Largo del Código GLN debe ser 13." Visible="false" />
    <asp:Label ID="lblGlnCheckDigit" runat="server" Text="Dígito Verificador del Código GLN no es Válido." Visible="false" />--%>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
