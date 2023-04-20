<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Grid_.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.Grid_" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Import Namespace="Binaria.WMSTek.Framework.Entities.Devices" %>
<%@ Import Namespace="Binaria.WMSTek.Framework.Entities.Display" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 
                                
            <%-- Grilla Principal --%>
            <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                <obout:Grid ID="grdMgr" runat="server" CallbackMode="true" Serialize="true" AutoGenerateColumns="false" 
                    AllowDataAccessOnServer="true" AllowSorting="true" AllowFiltering="true">
                    <Columns>
                        <obout:Column DataField="Id" HeaderText="ID" AccessibleHeaderText="Id"  runat="server"/>
                        <obout:Column DataField="Code" HeaderText="Código" AccessibleHeaderText="Code"  runat="server"/>
                        <obout:Column DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name"  runat="server"/>
                        <obout:Column DataField="Type" HeaderText="Tipo" AccessibleHeaderText="Type" runat="server" />
                        
				        <obout:Column ID="DisplayType" DataField="DisplayType.Name" AccessibleHeaderText="DisplayType" HeaderText="Tipo Display" runat="server">
				            <TemplateSettings TemplateID="DisplayTypeTemplate" />
				        </obout:Column>
				        
				        <obout:Column ID="IdTemp" DataField="Id" HeaderText="Id Template" runat="server">
				            <TemplateSettings TemplateID="IdTemplate" />
				        </obout:Column>
				        
				        <obout:Column ID="Actions" DataField="ss" AccessibleHeaderText="Actions" HeaderText="Acciones" Align="center" runat="server">
				            <TemplateSettings TemplateID="EditTemplate" />
				        </obout:Column>	
				     </Columns>
			        <Templates>
				        <obout:GridTemplate runat="server" ID="DisplayTypeTemplate" >
					        <Template>
					            <%# ((DisplayType)((Terminal)Container.DataObject).DisplayType).Name%>
					        </Template>
				        </obout:GridTemplate>
				        
				        <obout:GridTemplate runat="server" ID="IdTemplate" >
					        <Template>
					            <%# ((Terminal)Container.DataObject).Id%>
					        </Template>
				        </obout:GridTemplate>
				        				        
		                <obout:GridTemplate runat="server" ID="EditTemplate" >
			                <Template>
			                    <%# Container.PageRecordIndex %>
                               <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" OnCommand="btnEdit_Click" CommandArgument='<%# Container.PageRecordIndex %>' />
			                </Template>
		                </obout:GridTemplate>					        
			        </Templates>	
                </obout:Grid>
                    <%--
                    <a href="http://www.test.com/test.aspx?id=<%# Container.DataItem["SupplierID"] %>"><%# Container.DataItem["CompanyName"] %></a>
                    
                        <asp:TemplateField HeaderText="Tipo Display" AccessibleHeaderText="DisplayType">
                            <ItemTemplate>
                                <asp:Label ID="lblDisplayType" runat="server" Text='<%# Eval ( "DisplayType.Name" ) %>' Width="100px" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="CodStatus">
                            <ItemTemplate>
                                <center>
                                    <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "CodStatus" ) %>'
                                        Enabled="false" />
                                </center>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                            <ItemTemplate>
                                <div style="width: 60px">
                                    <center>
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                            CausesValidation="false" CommandName="Edit" />
                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                            CausesValidation="false" CommandName="Delete" />
                                    </center>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>--%>
            </div>
            <%-- FIN Grilla Principal --%>


            <%-- Pop up Editar/Nuevo Terminal --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlEditNew" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlEditNew" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Terminal" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Terminal" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- FIN Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <div id="divCodStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCodStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkCodStatus" runat="server" /></div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="150"/>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" /></div>
                            </div>
                            <div id="divDisplayType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDisplayType" runat="server" Text="Tipo Display" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlDisplayType" runat="server" Width="250" />
                                    <asp:RequiredFieldValidator ID="rfvDisplayType" runat="server" ControlToValidate="ddlDisplayType"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo de Display es requerido" /></div>
                            </div>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" /></div>
                            </div>
                            <div id="divType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblType" runat="server" Text="Tipo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtType" runat="server" MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="txtType"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido" /></div>
                            </div>
                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>
                    </div> 
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Terminal --%>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Terminal?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
