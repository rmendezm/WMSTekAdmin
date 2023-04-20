<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Grid.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.Grid" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Register TagPrefix="owd" Namespace="OboutInc.Window" Assembly="obout_Window_NET"%>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Import Namespace="Binaria.WMSTek.Framework.Entities.Devices" %>
<%@ Import Namespace="Binaria.WMSTek.Framework.Entities.Display" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script language="C#" runat="server">	
	
	void CreateGrid()
	{
	}
    void DeleteRecord(object sender, GridRecordEventArgs e)
    {
    }

    void InsertRecord(object sender, GridRecordEventArgs e)
    {
    }

	</script>
	
 		<script type="text/javascript">
		    function showModal(row) {	
		        try {
		            populateEditControls(row.id.toString().replace("grid_row_", ""));

		            editPanel.Open();
                } catch(ex){}
		    }

		    function showModalNew() {
		        // TODO: limpiar controles de ventana modal
		        document.getElementById("<%=hidEditId.ClientID%>").value = "0";
		        editPanel.Open();
		    }
		    
		    function closeDialog() {
		        editPanel.Close();
		    }

		    function populateEditControls(iRecordIndex) {

		        document.getElementById("<%=hidEditId.ClientID%>").value = grdMgr.Rows[iRecordIndex].Cells[0].Value;
		        document.getElementById("<%=txtCode.ClientID%>").value = grdMgr.Rows[iRecordIndex].Cells[1].Value;
		        document.getElementById("<%=txtName.ClientID%>").value = grdMgr.Rows[iRecordIndex].Cells[2].Value;
		        document.getElementById("<%=txtType.ClientID%>").value = grdMgr.Rows[iRecordIndex].Cells[3].Value;
		        
   		        if (grdMgr.Rows[iRecordIndex].Cells[4].Value == "False")
   		            document.getElementById("<%=chkCodStatus.ClientID%>").checked = false;
   		        else
   		            document.getElementById("<%=chkCodStatus.ClientID%>").checked = true;                
   		            
                // TODO: ver xq no selecciona el valor actual
//		        document.getElementById("<%=ddlDisplayType.ClientID%>").value = grdMgr.Rows[iRecordIndex].Cells[4].Value.toString();
		     //   alert(grdMgr.Rows[iRecordIndex].Cells[4].Value.toString());
		    }


		    function refreshGrid() {
		        alert('ref');
		        grdMgr.refresh();
		        return false;
		    }		 
		       
		    function saveChanges() {
		        var oRecord = new Object();

       
		    }
		</script>
      <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>                                       
            <%-- Grilla Principal --%>
            <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                <obout:Grid ID="grdMgr" runat="server" CallbackMode="true" Serialize="true" AutoGenerateColumns="false" 
                    AllowDataAccessOnServer="true" AllowSorting="true" EnableTypeValidation="false" AllowAddingRecords="false"
                    OnDeleteCommand="grdMgr_RowDeleting" AllowManualPaging="true">
			                    
                    <Columns>
                        <obout:Column DataField="Id" HeaderText="ID" AccessibleHeaderText="Id"  runat="server"/>
                        <obout:Column DataField="Code" HeaderText="Código" AccessibleHeaderText="Code"  runat="server"/>
                        <obout:Column DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name"  runat="server"/>
                        <obout:Column DataField="Type" HeaderText="Tipo" AccessibleHeaderText="Type" runat="server" />
                        <obout:Column DataField="CodStatus" HeaderText="Activo" AccessibleHeaderText="CodStatus" runat="server" >
                            <TemplateSettings TemplateID="CodStatusTemplate" />
                        </obout:Column>
         
				        <obout:Column AccessibleHeaderText="DisplayType" HeaderText="Tipo Display" runat="server">
				            <TemplateSettings TemplateID="DisplayTypeTemplate" />
				        </obout:Column>
				        <obout:Column ID="Actions" AccessibleHeaderText="Actions" HeaderText="Acciones" Align="center" runat="server">
				            <TemplateSettings TemplateID="ActionsTemplate" />
				        </obout:Column>	 				        
<%--				        <obout:Column ID="IdTemp" DataField="Id" HeaderText="Id Template" runat="server">
				            <TemplateSettings TemplateID="IdTemplate" />
				        </obout:Column>--%>
	
				     </Columns>
			        <Templates>
				        			        
				        <obout:GridTemplate runat="server" ID="DisplayTypeTemplate" >
					        <Template>
					            <%# ((DisplayType)((Terminal)Container.DataObject).DisplayType).Name%>
					        </Template>
				        </obout:GridTemplate>
				        
				        <obout:GridTemplate runat="server" ID="CodStatusTemplate" >
					        <Template>
                                 <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Convert.ToBoolean(Container.DataItem["CodStatus"]) %>' Enabled="false" />
					        </Template>
				        </obout:GridTemplate>				        
				        
<%--				        <obout:GridTemplate runat="server" ID="IdTemplate" >
					        <Template>
					            <%# ((Terminal)Container.DataObject).Id%>
					        </Template>
				        </obout:GridTemplate>--%>
				        				        
		                <obout:GridTemplate runat="server" ID="ActionsTemplate" >
			                <Template>
			                   <a href="#" id="grid_row_<%# Container.PageRecordIndex %>" onclick="showModal(this); return false;" >
    			                    <img src="../WebResources/Images/Buttons/GridActions/icon_edit.png" alt="Editar Terminal"/>
			                   </a>
			                   <a href="#" onclick="grdMgr.delete_record(this);" >
    			                    <img src="../WebResources/Images/Buttons/GridActions/icon_delete.png" alt="Eliminar Terminal"/>
			                   </a>			                   
                                <%--<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                    CausesValidation="false" CommandName="Delete" />--%>                               
			                </Template>
		                </obout:GridTemplate>					        
			        </Templates>	
                </obout:Grid>
            </div>
            <%-- FIN Grilla Principal --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    
            <%-- Pop up Editar/Nuevo Terminal --%>
            <owd:Dialog runat="server" ID="editPanel" Height="246" Width="637" IsModal="true" zIndex="10" Title="Edit Panel" StyleFolder="~/WebResources/Styles/Obout/wdstyles/default">
                <%--<asp:Panel ID="pnlEditNew" runat="server">--%>
                    <%-- Encabezado --%>
<%--                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Terminal" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Terminal" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>--%>
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
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <%--<input type="button" value="Save" onclick="saveChanges();closeDialog()" />--%>
                            <input type="button" value="Cancelar" onclick="closeDialog()" /> 
                        </div>
                    </div> 
                <%--</asp:Panel>--%>
            </owd:Dialog>
            <%-- FIN Pop up Editar/Nuevo Terminal --%>



        
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Terminal?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
