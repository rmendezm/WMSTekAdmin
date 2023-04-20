<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ErrorContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.ErrorContent" %>

    <%-- Ventana Modal --%>
	<asp:Button ID="btnErrorDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	<ajaxToolKit:ModalPopupExtender 
	    ID="modalPopUpError" runat="server" TargetControlID="btnErrorDummy" 
	    PopupControlID="pnlError"  
	    BackgroundCssClass="modalBackground" 
	    PopupDragHandleControlID="Caption" Drag="true" >
	</ajaxToolKit:ModalPopupExtender>

	<asp:Panel ID="pnlError" runat="server" CssClass="modalBox" Width="600px">
	    <%-- Encabezado --%>			
		<asp:Panel ID="ErrorHeader" runat="server" CssClass="modalHeader">
             <div class="divCaption">WMSTek
                <asp:ImageButton ID="imgCloseError" runat="server" ToolTip="Cerrar" ImageAlign="Top" onclick="btnCloseError_Click"  CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
             </div>		
	    </asp:Panel>
	     
	    <%--Información mostrada al usuario--%>
        <div class="divErrorPanel">
            <div class="divErrorBody">
                <div class="divErrorLevel">
                    <asp:Image id="imgErrorLevel" runat="server" />
                </div>
            
                <div class="divErrorTitle">        
                    <asp:Label ID="lblTitle" runat="server" />
                </div> 
                <div class="divDivisionLine"></div>          
                <div class="divErrorMessage">        
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
                <div class="divDivisionLine"></div> 
                <div id="divErrorSolution" class="divErrorSolution" runat="server" /></div>
            <div class="divDialogButtons">
                <div id="divErrorViewDetails" runat="server" class="divErrorViewDetails">
                    <asp:Label ID="lblViewDetails" runat="server" Text="Ver detalles" />
                </div>
                <asp:Button ID="btnCloseError" runat="server" Text="Aceptar" OnClick="btnCloseError_Click" />
            </div>  
         </div> 
         <br />   
        <%--Información adicional --%>
       <ajaxToolkit:CollapsiblePanelExtender ID="cpeErrorDetails" runat="Server"
            TargetControlID="pnlErrorDetails"
            ExpandControlID="divErrorViewDetails"
            CollapseControlID="divErrorViewDetails" 
            Collapsed="True"
            SuppressPostBack="False"
            TextLabelID="lblViewDetails"
            ExpandedText="Ocultar detalles"
            CollapsedText="Ver detalles"            
             />
                
        <asp:Panel id="pnlErrorDetails" Visible="true" class="divErrorAditionalInfo" runat="server">
            <div class="ErrorDetailsCaption">Información Adicional</div>      
            <%--      <hr class="hrDottedLine"> --%>      
            <div> <asp:Label ID="lblTime" runat="server" /> </div>  
            <div> <asp:Label ID="lblSeverity" runat="server" /> </div>               
            <div> <asp:Label ID="lblCode" runat="server" /></div>    
            <div> <asp:Label ID="lblClass" runat="server" /> </div>
            <div> <asp:Label ID="lblMethod" runat="server" /> </div>                     
            <div> <asp:Label ID="lblOriginalMessage" runat="server" /> </div>          
        </asp:Panel>
     </asp:Panel>      
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>    
        <asp:Label id="lblTimePrefix" runat="server"  Text="Time: " Visible="false" />  	
        <asp:Label id="lblSeverityPrefix" runat="server"  Text="Severity: " Visible="false" />  	
        <asp:Label id="lblCodePrefix" runat="server"  Text="Code: " Visible="false" />  	
        <asp:Label id="lblClassPrefix" runat="server"  Text="Class: " Visible="false" />  
        <asp:Label id="lblMethodPrefix" runat="server"  Text="Method: " Visible="false" />  	
        <asp:Label id="lblOriginalMessagePrefix" runat="server"  Text="Details: " Visible="false" />  	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
