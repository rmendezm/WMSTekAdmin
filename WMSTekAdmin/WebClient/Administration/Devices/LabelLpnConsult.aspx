<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelLpnConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelLpnConsult" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content runat="server" ID="content1" ContentPlaceHolderID="MainContent">
                   
    <asp:UpdatePanel ID="upLabelLpn" runat="server">
       <ContentTemplate>
            <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
               <div ID="divPrintLabel" runat="server" class="divPrintLabel">
                    <div class="divCtrsFloatLeft">
                        <div class="divControls">
                            <div class="fieldRight">
                                <asp:Label ID="lblCopies" runat="server" Text="Nº de Copias" />
                            </div>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtQtycopies" runat="server" Width="30"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtycopies" runat="server" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Text=" * " ErrorMessage="Nº de Copias es requerido." />
                                <asp:RangeValidator ID="rvQtycopies" runat="server" ErrorMessage="El valor debe estar entre 1 y 100." MaximumValue="100" MinimumValue="1" Text=" *" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Type="Integer"></asp:RangeValidator>
                                <ajaxToolkit:FilteredTextBoxExtender ID="ftbeQtycopies" runat="server" TargetControlID="txtQtycopies" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                            </div>
                            <div class="fieldRight">
                                <asp:Label ID="lblReprint" runat="server" Text="Reimpresión" />
                            </div>
                            <div class="fieldLeft">
                                <asp:CheckBox ID="chkReprint" runat="server" AutoPostBack="true" OnCheckedChanged="chkReprint_CheckedChanged" />
                            </div>
                        </div> 
                        
                        <div class="divControls">
                            <div class="fieldRight">
                                <asp:Label ID="lblPrinter" runat="server" Text="Impresora" />
                            </div>
                            <div class="fieldLeft">
                            <asp:UpdatePanel ID="updPrinter" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddlPrinters" runat="server" OnSelectedIndexChanged="ddlPrinters_Change" AutoPostBack="true" />
                                <asp:DropDownList ID="ddlLabelSize" runat="server" AutoPostBack="true" />
                                <asp:RequiredFieldValidator ID="rfvLabelSize" runat="server" ControlToValidate="ddlLabelSize" InitialValue="" ValidationGroup="valPrint" Text=" * " ErrorMessage="Tipo Etiqueta es requerido" />
                            </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </div>   
                        <div class="divControls">
                            <div class="fieldRight">
                                <asp:Label ID="lblTypeLpn" runat="server" Text="Tipo" />
                            </div>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlTypeLpn" runat="server" OnSelectedIndexChanged="ddlTypeLpn_Change" AutoPostBack="true" />
                            </div>
                        </div> 
                     </div>   
                     <div style="clear:both" />
                     <asp:UpdatePanel ID="UPLabelFinish" runat="server">
                     <ContentTemplate>      
                        <br />                
                        <div class="divCtrsFloatLeft">
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLabelStart" runat="server" Text="Etiqueta Inicial" />
                                </div>
                                <div class="fieldLeft">    
                                    <asp:TextBox ID="txtLabelStart" runat="server" Width="100" Enabled="false"/>
                                    <asp:RequiredFieldValidator ID="rfvLabelStart" runat="server" ControlToValidate="txtLabelStart" 
                                    ValidationGroup="valPrint" Text=" * " ErrorMessage="Etiqueta inicial es requerida." />
                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeLabelStart" runat="server" TargetControlID="txtLabelStart" FilterType="Numbers" />
                                </div>
                             </div>
                             
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label2" runat="server" Text="Cant. Etiquetas" />         
                                </div>
                               <div class="fieldLeft">                           
                                    <asp:TextBox ID="txtQtyLabel" runat="server" Width="40"  MaxLength ="3"/>
                                    <asp:ImageButton ID="imgBtnAddItem" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" OnClick="imgBtnAddItem_Click" ValidationGroup="valQty" ToolTip="Agregar"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvQtyLabel" runat="server" ControlToValidate="txtQtyLabel" ValidationGroup="valQty" Text=" * " ErrorMessage="Cantidad de etiquetas es requerida." />
                                    <asp:RangeValidator ID="rvQtyLabel" runat="server" ErrorMessage="El valor debe estar entre 1 y 100." MaximumValue="100" MinimumValue="1" Type="Integer" Text="*" ControlToValidate="txtQtyLabel" ValidationGroup="valQty" />
                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeQtyLabel" runat="server" TargetControlID="txtQtyLabel" FilterType="Numbers" />
                                </div>
                            </div>                          

                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label3" runat="server" Text="Etiqueta Final" />
                                </div>                            
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLabelFinish" runat="server" Width="100" Enabled="false" />
                                    <asp:RequiredFieldValidator ID="reqLabelFinish" runat="server" ControlToValidate="txtLabelFinish" ValidationGroup="valPrint" Text=" * " ErrorMessage="Etiqueta final es requerida." />
                                </div>                                    
                            </div>
                        </ContentTemplate>
                       <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$txtQtyLabel" EventName="TextChanged" />
                      </Triggers>
                    </asp:UpdatePanel>                                                       

                    
                    <div class="divCtrsFloatLeft">
                        <asp:ValidationSummary ID="valQty" ValidationGroup="valQty" runat="server" CssClass="modalValidation"/>
                        <asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation"/>
                        <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label>                              
                    </div>   
                </div>
                </div>
            </div>
            
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$chkReprint" EventName="CheckedChanged" />
      </Triggers>
    </asp:UpdatePanel>           
    
    <asp:UpdateProgress ID="uprLabelLpn" runat="server" AssociatedUpdatePanelID="upLabelLpn" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprLabelLpn" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLabelLpn" />    

    <%-- Mensajes de Confirmacion y Auxiliares --%>   
    <asp:Label ID="lblTitle" runat="server" Text="Etiqueta Lpn" Visible="false"/>
    <asp:Label ID="lblErrorLabelStart" runat="server" Text="La etiqueta inicial no puede superar la nro.: " Visible="false"/>
    <asp:Label ID="lblErrorLabelFinish" runat="server" Text="La cantidad final de etiquetas no puede superar la nro.: " Visible="false"/>
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/>
    <asp:Label ID="lblValidateLabelStart" runat="server" Text="El valor de etiqueta inicial no esta actualizado. Actualice la pantalla" Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  
    
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
