<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ChangePassword.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Users.ChangePassword" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <br />
            <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
            <%--<div class="modalControls">--%>
                <div id="divModalFields" class="divCtrsFloatLeft">
                    <div id="divPasswordActual"  class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblPasswordActual" runat="server" Text="Actual" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtPasswordActual" runat="server" MaxLength="30" Width="80" TabIndex="3"
                                TextMode="Password" />
                            <asp:RequiredFieldValidator ID="rfvPasswordActual" runat="server" ControlToValidate="txtPasswordActual"
                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Debe ingresar la contraseña actual" />
                        </div>
                    </div>
                    <div id="divPasswordNew"  class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblPasswordNew" runat="server" Text="Nueva" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtPasswordNew" runat="server" MaxLength="30" Width="80" TabIndex="3"
                                TextMode="Password" />
                            <asp:RequiredFieldValidator ID="rfvPasswordNew" runat="server" ControlToValidate="txtPasswordNew"
                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Debe ingresar la contraseña nueva" />
                        </div>
                    </div>
                    <div id="divPassword2"  class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblPassword2" runat="server" Text="Nueva" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtPasswordRepeat" runat="server" MaxLength="30" Width="80" TabIndex="3"
                                TextMode="Password" />
                            <asp:RequiredFieldValidator ID="rfvPassword2" runat="server" ControlToValidate="txtPasswordRepeat"
                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Debe ingresar nuevamente la contraseña nueva" />
                            <asp:CompareValidator ID="cvPass" runat="server" ControlToCompare="txtPasswordRepeat" ValidationGroup="EditNew" 
                                ControlToValidate="txtPasswordNew" Text="*" ErrorMessage="Las contraseñas no coinciden">
                            </asp:CompareValidator>
                        </div>
                    </div>
            <br />
            <br />
            <div id="divBtn" runat="server" style="clear: both;margin: 20px; width: 250px">
                <asp:Button ID="btnChangePass" runat="server" OnClick="btnChangePass_Click" Text="Cambiar Contraseña"
                    CausesValidation="true" ValidationGroup="EditNew" />
            </div>
            <br />                      
                </div>
                <div>
                    <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"     class="divValidationSummary"/>
                </div>       
            <%--</div>--%>         
          
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnChangePass" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblExito" runat="server" Text="La contraseña se ha cambiado exitosamente." Visible="false"/>
    <asp:Label ID="lblError" runat="server" Text="La contraseña introducida no coincide con la actual" Visible="false"/>
    <asp:Label ID="lblTitle" runat="server" Text="Cambiar Contraseña" Visible="false"/>
    <asp:Label ID="lblPoliciyPasswordTitle" runat="server" Text="Error Política Contraseñas" Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<%-- Barra de estado --%>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
<%-- FIN Barra de estado --%>