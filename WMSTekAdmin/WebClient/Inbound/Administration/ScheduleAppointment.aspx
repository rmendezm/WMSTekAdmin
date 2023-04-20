<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ScheduleAppointment.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Administration.ScheduleAppointment" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/core/main.css")%>" rel="stylesheet" type="text/css" />
    <link href="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/daygrid/main.css")%>" rel="stylesheet" type="text/css" />
    <link href="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/timegrid/main.css")%>" rel="stylesheet" type="text/css" />

    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/core/main.js")%>"></script>
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/interaction/main.js")%>"></script>
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/daygrid/main.js")%>"></script>
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/packages/timegrid/main.js")%>"></script>

    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Calendar/Scheduler.js")%>"></script>
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Charts/Script/UtilKPIScript.js")%>"></script>

    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/jquery.validate.min.js")%>"></script>
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/additional-methods.min.js")%>"></script>

    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/bootstrap3-typeahead.min.js")%>"></script>
    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/typeahead.css")%>" rel="stylesheet" type="text/css" />

    <script language="javascript" type= "text/javascript" >

        document.addEventListener('DOMContentLoaded', function () {

            $("#ctl00_ucMainFilterContent_btnSearch").hide();
            $("#ctl00_ucTaskBarContent_divTaskBar").hide();
            $(".mainFilterPanel").hide();

            var prom = GetMinutesPerAppointmentApi();

            prom.done(function (data) {              
                minutesPerAppointment = data.d;
                validateFormAppointment();
                setCalendar(null);
                ScheduleDatePicker();
                StartAutocompleteInputs();
            });

            prom.fail(function (error) {
                errorMessage('Error al obtener configuración citas: ' + error.responseJSON.Message);
            });
        });
        
    </script>
    
    <style>
        #calendar{
            height: 500px;
            overflow-y: auto;
            background-color: white;
        }

        .modal{
            z-index: 9999999;
        }

        .has-error, .error{
            color: red;
        }

        .has-success{
            color: green;
        }

        .input-error {
            border-color: red;
        }

        #ctl00_divTop {
            margin-top: 0px;
        }

        /* inicio dashboard */
        .data-card {
            border-radius: 5px;
            padding: 10px;
            margin-bottom: 10px;
            color: white;
            text-align: center;
        }

        .flex-container {
            display: flex;
            background: white;
        }

        .flex-item {
            flex-grow: 1;
            padding: 5px;
        }

        .bg-success{
            background-color: #28A745
        }

        .bg-danger {
            background-color: #DC3545;
        }
        
        .bg-secondary{
            background-color: #6C757D;
        }

        .bg-info {
            background-color: #17A2B8;
        }
        /* fin dashboard */

    </style>

     <div class="container">
        <div class="row">
            <div class="col-md-12">
               <div class="flex-container">
                   <div class="flex-item">
                        <div class="p-3 mb-2 bg-primary text-white data-card">
                            <span></span><span> Agendados</span>
                        </div>
                   </div>
                   <div class="flex-item">
                        <div class="p-3 mb-2 bg-info text-white data-card">
                            <span></span><span> Recibiendo</span>
                        </div>
                   </div>
                   <div class="flex-item">
                        <div class="p-3 mb-2 bg-success text-white data-card">
                            <span></span><span> Recibidos</span>
                        </div>
                   </div>
                   <div class="flex-item">
                        <div class="p-3 mb-2 bg-danger text-white data-card">
                            <span></span><span> Atrasados</span>
                        </div>
                   </div>
                   <div class="flex-item">
                        <div class="p-3 mb-2 bg-secondary text-white data-card">
                            <span></span><span> No Entregados</span>
                        </div>
                   </div>
               </div> 
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div id='calendar'></div>
            </div>
        </div>
    </div>  

    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel">Ingresar Cita</h4>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hidSelectedDatetime" />
                    <input type="hidden" id="hidScheduleId" />

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="txtStartDate" class="form-control-label">Fecha Inicio:</label>
                            <input type="text" class="form-control datepicker" id="txtStartDate" name="txtStartDate" placeholder="dd/mm/yyyy">
                        </div>
                            <div class="form-group col-md-6">
                            <label for="txtStartHour" class="form-control-label">Hora Inicio:</label>
                            <input type="text" class="form-control" id="txtStartHour" name="txtStartHour" placeholder="hh:mm:ss">
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="txtEndDate" class="form-control-label">Fecha Fin:</label>
                            <input type="text" class="form-control datepicker" id="txtEndDate" name="txtEndDate" placeholder="dd/mm/yyyy">
                        </div>
                            <div class="form-group col-md-6">
                            <label for="txtEndHour" class="form-control-label">Hora Fin:</label>
                            <input type="text" class="form-control" id="txtEndHour" name="txtEndHour" placeholder="hh:mm:ss">
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="txtDocumentBound" class="form-control-label">Orden de Compra:</label>
                            <input type="text" class="form-control" id="txtDocumentBound" name="txtDocumentBound">
                        </div>
                        <div class="form-group col-md-6">
                            <label for="txtLicensePlate" class="form-control-label">Patente:</label>
                            <input type="text" class="form-control" id="txtLicensePlate" name="txtLicensePlate" maxlength="6">
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="txtDriver" class="form-control-label">Transportista:</label>
                            <input type="text" class="form-control" id="txtDriver" name="txtDriver" />
                        </div>
                        <div class="form-group col-md-6">
                            <label for="numLoadQty" class="form-control-label">Cantidad Carga:</label>
                            <input type="number" class="form-control" id="numLoadQty" name="numLoadQty">
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label class="form-control-label">Tipo Carga:</label>
                            <div class="form-check form-check-inline">
                                <input class="form-check-input" type="radio" name="loadTypeOption" id="loadTypeOptionLPN" value="1" checked="checked">
                                <label class="form-check-label" for="loadTypeOptionLPN">LPN</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input class="form-check-input" type="radio" name="loadTypeOption" id="loadTypeOptionUnit" value="2">
                                <label class="form-check-label" for="loadTypeOptionUnit">Unidad</label>
                            </div>
                        </div>

                        <div class="form-group col-md-6">
                            <label for="txtTile" class="form-control-label">Titulo:</label>
                            <input type="text" class="form-control" id="txtTile" name="txtTile">
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-12">
                            <label for="txtComment" class="form-control-label">Comentarios:</label>
                            <textarea class="form-control" id="txtComment" name="txtComment"></textarea>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6" id="yardFormControls">
                            <label for="txtYard" class="form-control-label">Patio Asignado:</label>
                            <input type="text" class="form-control" id="txtYard" name="txtYard">
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <div class="col-md-12">
                        <button type="button" id="btnDelete" class="btn btn-danger" onclick="Delete();">Eliminar</button>
                        <button type="button" class="btn btn-primary" id="btnSave" onclick="Save();">Salvar</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalConfirm" role="dialog">
	    <div class="modal-dialog modal-sm">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal">&times;</button>
				    <h4 class="modal-title">Confirmar</h4>
			    </div>
			    <div class="modal-body">
				    <p id="modalMessaage">¿Confirma eliminar cita?</p>
			    </div>
			    <div class="modal-footer">
                    <button type="button" id="btnConfirmDelete" class="btn btn-danger" onclick="ConfirmDelete();">Eliminar</button>
				    <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
			    </div>
		    </div>
	    </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
