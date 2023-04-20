var globalCalendar = null;
var minutesPerAppointment = null;

const eScheduleTrack = {
    SCHEDULED: 1,
    RECEIVING: 2,
    RECEIVED: 3,
    DELAYED: 4,
    NORECEIVED: 5
}

function GetMinutesPerAppointmentApi() {

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "GetMinutesPerAppointment",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        })
    );
}

function GetScheduleApi(dateStart, dateEnd) {

    var param = {
        dateStart: dateStart,
        dateEnd: dateEnd
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "GetSchedule",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function SaveAppointmentApi(startDate, endDate, documentNumber, licensePlate, driverName, title, comment, loadQty, loadType) {

    var param = {
        startDate: startDate,
        endDate: endDate,
        documentNumber: documentNumber,
        licensePlate: licensePlate,
        driverName: driverName,
        title: title,
        comment: comment,
        loadQty: loadQty,
        loadType: loadType
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "SaveAppointment",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function ValidateIsNonWorkingDay(date) {

    var param = {
        date: date
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "ValidateIsNonWorkingDay",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function Save() {
    var id = $("#hidScheduleId").val();

    if (id === '') {
        SaveAppointment();
    } else {
        UpdateAppointment(id);
    }
}

function SaveAppointment() {

    var form = $('form[action="ScheduleAppointment.aspx"]');

    if (!form.valid()) {
        return;
    }

    var startDate = $("#txtStartDate").val();
    var startHour = $("#txtStartHour").val();
    var finalStartDate = startDate + "T" + startHour;

    var endDate = $("#txtEndDate").val();
    var endHour = $("#txtEndHour").val();
    var finalEndDate = endDate + "T" + endHour;

    var documentNumber = $("#txtDocumentBound").val();
    var licensePlate = $("#txtLicensePlate").val();
    var driverName = $("#txtDriver").val();
    var title = $("#txtTile").val();
    var comment = $("#txtComment").val();
    var loadQty = $("#numLoadQty").val();
    var loadType = $("input[name='loadTypeOption']:checked").val();

    var prom = SaveAppointmentApi(finalStartDate, finalEndDate, documentNumber, licensePlate, driverName, title, comment, loadQty, loadType);

    prom.done(function (data) {

        successMessage('Cita salvada exitosamente');  
        $('#myModal').modal('hide');

        var dateSaved = $("#txtStartDate").val() + "T" + $("#txtStartHour").val();

        clearFormAppointment();
        reloadCalendar(dateSaved, 2000);
    });

    prom.fail(function (error) {
        errorMessage('Error al salvar cita: ' + error.responseJSON.Message);
    });
}

function UpdateAppointment(id) {

    var form = $('form[action="ScheduleAppointment.aspx"]');

    if (!form.valid()) {

        if ($("#txtStartDate").hasClass("input-error") === true) {
            $("#txtStartHour, #txtEndDate, #txtEndHour").addClass("input-error");
        }

        return;
    }

    var startDate = $("#txtStartDate").val();
    var startHour = $("#txtStartHour").val();
    var finalStartDate = startDate + "T" + startHour;

    var endDate = $("#txtEndDate").val();
    var endHour = $("#txtEndHour").val();
    var finalEndDate = endDate + "T" + endHour;

    var documentNumber = $("#txtDocumentBound").val();
    var licensePlate = $("#txtLicensePlate").val();
    var driverName = $("#txtDriver").val();
    var title = $("#txtTile").val();
    var comment = $("#txtComment").val();
    var loadQty = $("#numLoadQty").val();
    var loadType = $("input[name='loadTypeOption']:checked").val();

    var prom = UpdateAppointmentApi(parseInt(id), finalStartDate, finalEndDate, documentNumber, licensePlate, driverName, title, comment, loadQty, loadType);

    prom.done(function (data) {

        successMessage('Cita actualizada exitosamente');
        $('#myModal').modal('hide');

        var dateSaved = $("#txtStartDate").val() + "T" + $("#txtStartHour").val();

        clearFormAppointment();
        reloadCalendar(dateSaved, 2000);
    });

    prom.fail(function (error) {
        errorMessage('Error al actualizar cita: ' + error.responseJSON.Message);
    });
}

function GetScheduleByIdApi(id) {

    var param = {
        id: id
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "GetScheduleById",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function GetScheduleById(id) {

    var prom = GetScheduleByIdApi(id);

    prom.done(function (data) {

        if (data.d.Entities.length > 0) {
            var schedule = data.d.Entities[0];

            $("#myModalLabel").text('Detalle Cita');

            var track = schedule.LatestScheduleTrack.ScheduleTrackType.Id;

            let isDisabled = true;
            if (track === eScheduleTrack.SCHEDULED) {
                isDisabled = false;
            }

            $("#txtDocumentBound").val(schedule.DocumentNumberBound).prop("disabled", isDisabled);
            $("#txtLicensePlate").val(schedule.Truck.IdCode).prop("disabled", isDisabled);
            $("#txtDriver").val(schedule.Driver.Name).prop("disabled", isDisabled);
            $("#txtTile").val(schedule.Title).prop("disabled", isDisabled);
            $("#txtComment").val(schedule.Comment).prop("disabled", isDisabled);
            $("#numLoadQty").val(parseInt(schedule.LoadQty)).prop("disabled", isDisabled);
            $("input[name='loadTypeOption'][value=" + schedule.LoadType + "]").prop('checked', true);
            $("#hidScheduleId").val(schedule.Id);

            $("#loadTypeOptionLPN").prop("disabled", isDisabled);
            $("#loadTypeOptionUnit").prop("disabled", isDisabled);

            var startDate = convertWsToJsDate(schedule.ScheduledDateStart);    
            var objStartDate = moment(startDate);
            $("#hidSelectedDatetime").val(objStartDate.format("DD/MM/YYYYTHH:mm:ss"));
            $("#txtStartDate").val(objStartDate.format("DD/MM/YYYY")).prop("disabled", isDisabled);
            $("#txtStartHour").val(objStartDate.format("HH:mm:ss")).prop("disabled", isDisabled);

            var endDate = convertWsToJsDate(schedule.ScheduledDateEnd);
            var objEndDate = moment(endDate);
            $("#txtEndDate").val(objEndDate.format("DD/MM/YYYY")).prop("disabled", isDisabled);
            $("#txtEndHour").val(objEndDate.format("HH:mm:ss")).prop("disabled", isDisabled);

            $("#txtYard").val(schedule.Yard.IdCode).prop("disabled", true);
            $("#yardFormControls").show();

            if (isDisabled === true) {
                $("#btnDelete").hide();
                $("#btnSave").hide();
            } else {
                $("#btnDelete").show();
                $("#btnSave").show();
            }

            $('#myModal').modal('show');

        } else {
            warningMessage('No se encontro detalle de cita');
        }
    });

    prom.fail(function (error) {
        errorMessage('Error obtener detalle de cita: ' + error.responseJSON.Message);
    });
}

function UpdateAppointmentApi(scheduleId, startDate, endDate, documentNumber, licensePlate, driverName, title, comment, loadQty, loadType) {

    var param = {
        scheduleId: scheduleId,
        startDate: startDate,
        endDate: endDate,
        documentNumber: documentNumber,
        licensePlate: licensePlate,
        driverName: driverName,
        title: title,
        comment: comment,
        loadQty: loadQty,
        loadType: loadType
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "UpdateAppointment",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function DeleteScheduleApi(id) {

    var param = {
        id: id
    };

    return Q(
        $.ajax({
            type: "POST",
            url: urlWS() + urlWSSchedule() + "DeleteSchedule",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param)
        })
    );
}

function Delete() {
    $('#modalConfirm').modal('show');
}

function ConfirmDelete() {
    var id = $("#hidScheduleId").val();

    var prom = DeleteScheduleApi(parseInt(id));

    prom.done(function (data) {

        successMessage('Cita eliminada exitosamente');
        $('#myModal').modal('hide');
        $('#modalConfirm').modal('hide');

        var dateSaved = $("#hidSelectedDatetime").val();

        clearFormAppointment();
        reloadCalendar(dateSaved, 2000);
    });

    prom.fail(function (error) {
        errorMessage('Error al eliminar cita: ' + error.responseJSON.Message);
    });
}

function reloadCalendar(startDate, miliSecondsDelay) {

    setTimeout(function () {
        $("#calendar").empty();
        $("#hidSelectedDatetime").val('');

        var formatedDate = moment(startDate, 'DD/MM/YYYYTHH:mm:ss');

        setCalendar(formatedDate.format("YYYY-MM-DD"));

    }, miliSecondsDelay);
}

function clearFormAppointment() {
    $("#txtDocumentBound").val('').prop("disabled", false);
    $("#txtLicensePlate").val('').prop("disabled", false);
    $("#txtDriver").val('').prop("disabled", false);
    $("#txtTile").val('').prop("disabled", false);
    $("#txtComment").val('').prop("disabled", false);
    $("#numLoadQty").val('').prop("disabled", false);
    $('#loadTypeOptionLPN').prop('checked', true).prop("disabled", false);
    $('#loadTypeOptionUnit').prop("disabled", false);
    $("#hidScheduleId").val('');

    $("#txtStartDate").val('');
    $("#txtStartHour").val('');
    $("#txtEndDate").val('');
    $("#txtEndHour").val('');

    $("#myModalLabel").text('Ingresar Cita');
    $("#btnSave").prop("disabled", false);
}

function setCalendar(setInitialDate) {

    var calendarEl = document.getElementById('calendar');

    if (globalCalendar) {
        globalCalendar.destroy();
        globalCalendar = null;
    }

    var calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['interaction', 'dayGrid', 'timeGrid'],
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        navLinks: true,
        selectable: true,
        selectMirror: true,
        editable: true,
        eventLimit: true,
        dateClick: function (info) {
            var today = moment();
            var selectedDate = moment(info.date);

            var diffDays = selectedDate.diff(today, 'days');

            var view = info.view.type;

            if (diffDays < 0 && view !== 'dayGridMonth') {
                calendar.unselect();
                return false;
            }

            if (view === 'dayGridMonth') {
                calendar.changeView('timeGridDay', info.dateStr);
            } 
            
        },
        eventClick: function (info) {

            var validator = $('form[action="ScheduleAppointment.aspx"]').validate();
            validator.resetForm();

            $(".input-error").each(function (index, el) {
                $(el).removeClass("input-error");
            });

            var id = parseInt(info.event.id);
            GetScheduleById(id);
        },
        select: function (info) {         

            var today = moment();
            var selectedStartDate = moment(info.start);
            var selectedEndDate = moment(info.end);

            var diffDays = selectedStartDate.diff(today, 'days');

            var view = info.view.type;

            if (diffDays < 0 && view !== 'dayGridMonth') {
                calendar.unselect();
                return false;
            }

            if (selectedStartDate.format('DD/MM/YYYY') !== selectedEndDate.format('DD/MM/YYYY')) {
                calendar.unselect();
                return false;
            }

            if (view === 'dayGridMonth') {
                calendar.changeView('timeGridDay', info.startStr);
            } else {

                var prom = ValidateIsNonWorkingDay(selectedStartDate.format('DD/MM/YYYY'));

                prom.done(function (data) {
                    if (data.d === true) {
                        warningMessage('No se puede crear cita ya que dia seleccionado es feriado');
                        calendar.unselect();
                        return false;
                    } else {
                        $("#hidSelectedDatetime").val(selectedStartDate.format('DD/MM/YYYYTHH:mm:ss'));
                        clearFormAppointment();

                        $("#txtStartDate").val(selectedStartDate.format('DD/MM/YYYY')).prop("disabled", true);
                        $("#txtStartHour").val(selectedStartDate.format('HH:mm:ss')).prop("disabled", true);
                        $("#txtEndDate").val(selectedEndDate.format('DD/MM/YYYY')).prop("disabled", true);
                        $("#txtEndHour").val(selectedEndDate.format('HH:mm:ss')).prop("disabled", true);
                        $("#yardFormControls").hide();
                        $("#btnDelete").hide();
                        $('#myModal').modal('show');
                    }
                });

                prom.fail(function (error) {
                    errorMessage('Error al validar fecha seleccionada: ' + error.responseJSON.Message);
                });
            }
       
        },
        allDaySlot: false,
        allDayDefault: false,
        eventOverlap: function (stillEvent, movingEvent) {
            return false;
        },
        weekends: false,
        locale: 'es',
        slotDuration: '00:' + MinAppointmentDuration() + ':00',
        minTime: "09:00:00",
        maxTime: "18:00:00",
        fixedWeekCount: false,
        showNonCurrentDates: true,
        buttonText: {
            today: 'hoy',
            month: 'mes',
            week: 'semana',
            day: 'día',
            list: 'lista'
        },
        height: "auto",
        eventDurationEditable: false,
        eventStartEditable: false,
        eventSources: [
            {
                events: function (info, successCallback, failureCallback) {
                    var prom = GetScheduleApi(info.start, info.end);

                    prom.done(function (data) {

                        if (data.d.Entities.length > 0) {

                            data = data.d.Entities;
                            data = utilSetDateGenericForCalendar(data, "ScheduledDateStart", "start");
                            data = utilSetDateGenericForCalendar(data, "ScheduledDateEnd", "end");

                            $.each(data, function (index, value) {
                                value.title = value.Title + " - " + value.Yard.IdCode;
                            });

                            data = createNewColumn(data, "Id", "id");
                            createDashboard(data);
                            successCallback(data);
                        } else {
                            warningMessage('No hay citas para fechas seleccionadas');
                            clearDashboard();
                            successCallback([]);
                        }
                    });

                    prom.fail(function (error) {
                        errorMessage('Al obtener citas: ' + error.responseJSON.Message);
                        failureCallback(error);
                    });
                }
            }
        ]
    });

    if (setInitialDate) {
        calendar.changeView('timeGridDay', setInitialDate);
    }

    globalCalendar = calendar;
    calendar.render();
}

function createDashboard(data) {

    var appointmentTracks = Object.entries(eScheduleTrack).map(([id, track]) => ({ id, track }));
    var dashboardData = [];

    $.each(appointmentTracks, function (index, value) {

        var count = data.filter(function (el) {
            return el.LatestScheduleTrack.ScheduleTrackType.Id === value.track;
        }).length;

        dashboardData.push({ "trackId": value.track, "count": count });
    });

    $.each($(".data-card span:first-child"), function (index, value) {
        $(value).text(dashboardData[index].count);
    });
}

function clearDashboard() {
    $.each($(".data-card span:first-child"), function (index, value) {
        $(value).text(0);
    });
}

function utilSetDateGenericForCalendar(data, column, newColumn) {

    $.each(data, function (index, value) {

        var d = convertWsToJsDate(value[column].toString());
        value[newColumn] = moment(d).format("YYYY-MM-DDTHH:mm:ss");
    });

    return data;
}

function convertWsToJsDate(value) {
    var d = new Date();
    var ms = parseInt(value.replace("/Date(", "").replace(")/", ""));
    d.setTime(ms);

    return d;
}

function createNewColumn(data, column, newColumn) {

    $.each(data, function (index, value) {
        value[newColumn] = value[column];
    });

    return data;
}

function MinAppointmentDuration() {
    return minutesPerAppointment || 15;
}

function customRules() {
    $.validator.addMethod(
        "customDateValidation",
        function (value, element) {
            var validator = this;
            var valid = true;

            var today = moment();

            var finalStartDate = moment($("#txtStartDate").val() + "T" + $("#txtStartHour").val(), 'DD/MM/YYYYTHH:mm:ss');

            var diffDays = finalStartDate.diff(today, 'days');

            if (diffDays < 0) {
                $.validator.messages.customDateValidation = "Fecha inicio no puede ser menor a fecha actual";
                valid = false;
            }

            var finalEndDate = moment($("#txtEndDate").val() + "T" + $("#txtEndHour").val(), 'DD/MM/YYYYTHH:mm:ss');

            if (valid === true && finalStartDate.format('DD/MM/YYYY') !== finalEndDate.format('DD/MM/YYYY')) {
                $.validator.messages.customDateValidation = "Fecha inicio y fin deben ser el mismo día";
                valid = false;
            }

            if (valid === true && finalStartDate >= finalEndDate) {
                $.validator.messages.customDateValidation = "Fecha inicio debe ser menor que fecha fin";
                valid = false;
            }

            var diffMinutes = finalEndDate.diff(finalStartDate, 'minutes');

            if (valid === true && diffMinutes < MinAppointmentDuration()) {
                $.validator.messages.customDateValidation = "Cita debe durar al menos " + MinAppointmentDuration() + " minutos";
                valid = false;
            }

            return valid;
        }, $.validator.messages.customDateValidation
    );
}

function validateFormAppointment() {

    customRules();

    $('form[action="ScheduleAppointment.aspx"]').validate({
        rules: {
            txtDocumentBound: "required",
            txtLicensePlate: {
                required: true,
                minlength: 6
            },
            txtDriver: "required",
            numLoadQty: {
                required: true,
                min: 1,
                number: true
            },
            txtTile: "required",
            txtStartDate: {
                required: true,
                pattern: /^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$/,
                customDateValidation: true
            },
            txtStartHour: {
                required: true,
                pattern: /^([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)$/ 
            },
            txtEndDate: {
                required: true,
                pattern: /^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$/
            },
            txtEndHour: {
                required: true,
                pattern: /^([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)$/
            }
        },
        messages: {
            txtDocumentBound: "Orden de compra es obligatoria",
            txtLicensePlate: {
                required: "Patente camión es obligatoria",
                minlength: "La patente de camión debe tener 6 caracteres"
            },
            txtDriver: "Chofer es obligatorio", 
            numLoadQty: {
                required: "Cantidad carga es obligatoria",
                number: "Cantidad carga debe ser numerico",
                min: "Cantidad carga debe ser mayor a 0"
            },
            txtTile: "Titulo es obligatorio",
            txtStartDate: {
                required: "Fecha inicio es obligatoria",
                pattern: "Fecha inicio con formato inválido (dd/mm/yyyy)"
            }, 
            txtStartHour: {
                required: "Hora inicio es obligatoria",
                pattern: "Hora inicio con formato inválido (hh:mm:ss)"
            },
            txtEndDate: {
                required: "Fecha fin es obligatoria",
                pattern: "Fecha fin con formato inválido (dd/mm/yyyy)"
            },
            txtEndHour: {
                required: "Hora fin es obligatoria",
                pattern: "Hora fin con formato inválido (hh:mm:ss)"
            }
        },
        errorElement: "em",
        errorPlacement: function (error, element) {
            // Add the `help-block` class to the error element
            error.addClass("help-block");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.parent("label"));
            } else {
                error.insertAfter(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass("input-error");
            $(element).parents(".col-sm-5").addClass("has-error").removeClass("has-success");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass("input-error");
            $(element).parents(".col-sm-5").addClass("has-success").removeClass("has-error");
        }
    });
}

function ScheduleDatePicker() {
    $('.datepicker').datepicker({
        format: "dd/mm/yyyy",
        autoclose: true,
        language: "es",
        todayHighlight: true,
        startDate: '0d',
        daysOfWeekDisabled: [0, 6]
    }).on('changeDate', function (e) {

        var selectedDate = moment(e.date);
        if (dateIsBeforeToday(selectedDate) === true) {
            warningMessage("Fecha seleccionada no debe ser menor a hoy");
            $(e.currentTarget).val('');
        }
    });
}

function dateIsBeforeToday(date) {

    var today = moment();
    var diffDays = date.diff(today, 'days');

    return diffDays < 0;
}

function StartAutocompleteInputs() {

    $('#txtDriver').typeahead({
        source: function (request, response) {

            console.log("request");
            console.log(request);

            var param = {
                name: request
            };

            $.ajax({
                url: urlWS() + urlWSSchedule() + "GetDrivers",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(param),
                success: function (data) {
                    console.log("autocomplete");
                    console.log(data);
                    data = data.d.Entities;

                    var arrDrivers = [];
                    response($.map(data, function (driver) {
                        arrDrivers.push(driver.Name);
                    }));
                    response(arrDrivers);

                    $(".dropdown-menu").css("width", "auto");
                    $(".dropdown-menu").css("height", "auto");
                    $(".dropdown-menu").css("font", "15px Verdana");
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    errorMessage('Error obtener sugerencias de conductor: ' + textStatus);
                }
            });
        },
        hint: true,             
        highlight: true,        
        minLength: 4            
    });
}