<%@ Page Language="C#"  MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="NotAccess.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Account.NotAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
	    <asp:Button ID="btnDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	    
	    <ajaxToolKit:ModalPopupExtender 
	        ID="modalPopUp" runat="server" TargetControlID="btnDummy" 
	        PopupControlID="pnl"  
	        BackgroundCssClass="modalBackground" 
	        PopupDragHandleControlID="Caption" Drag="true" >
	    </ajaxToolKit:ModalPopupExtender>
	    
	    <asp:Panel ID="pnl" runat="server" CssClass="modalBox" Width="500px">
		    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
			    <asp:Label ID="lblNew" runat="server" Text="WMSTek"/>
	        </asp:Panel>

                
		    <div id="logOut" runat="server">	    
		        <asp:Label id="lblLogOutMessage" runat="server" Text="Acceso denegado." />
		        
		        <div>
			        <asp:Button ID="btnLogOut" runat="server" Text="Aceptar" OnClientClick="return btnLogOut_onclick()" />
		        </div>
		    </div>
		</asp:Panel>
		
</asp:Content>