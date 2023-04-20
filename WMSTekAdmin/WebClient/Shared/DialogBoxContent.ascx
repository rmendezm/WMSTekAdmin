<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialogBoxContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.DialogBoxContent" %>

    <%-- Ventana Modal --%>
	<asp:Button ID="btnDialogDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	<ajaxToolKit:ModalPopupExtender 
	    ID="modalPopUpDialog" runat="server" TargetControlID="btnDialogDummy" 
	    PopupControlID="pnlDialog"  
	    BackgroundCssClass="modalBackground" 
	    PopupDragHandleControlID="Caption" Drag="true" >
	</ajaxToolKit:ModalPopupExtender>

	<asp:Panel ID="pnlDialog" runat="server" CssClass="modalBox" Width="400px">
	
		<%-- Encabezado --%>
			
		<asp:Panel ID="DialogHeader" runat="server" CssClass="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
			    <asp:ImageButton ID="imgClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" Visible="false"
			     OnClick="imgClose_Click" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
            </div>
	    </asp:Panel>
	    
        <div id="divDialogPanel" class="divDialogPanel" runat="server">
            <div class="divDialogMessage">
                <asp:Image id="imgDialogIcon" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />  
            <div id="divDialogMessage2" runat="server" visible="false" class="divDialogMessage" />  
            <br />
            <div style="float:left">
                <asp:HyperLink  Visible="false" ID="linkPage" runat="server" Text="" CssClass="linkDecoration" />
            </div>
            <div id="divConfirm" runat="server" visible="false" class="divDialogButtons">
                <asp:Button ID="btnOk" runat="server" Text="   Sí   " OnClick="btnOk_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="   No   " OnClick="btnCancel_Click" />
            </div>         
            <div id="divAlert" runat="server" visible="false" class="divDialogButtons">
                <asp:Button ID="btnClose" runat="server" Text="Aceptar"  OnClick="btnClose_Click" />
            </div>           
        </div>             
           
     </asp:Panel> 
     
     <script language="javascript" type="text/javascript">
         function RedirectPage() {
            //Esta sentencia funciona correctamente
            window.top.location.href = '../DetectScreen.aspx';
         }
     </script>
     