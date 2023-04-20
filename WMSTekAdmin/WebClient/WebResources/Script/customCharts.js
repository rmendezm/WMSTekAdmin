function urlWSCharts() {

    var urlBase;

    if (window.location.port == "") {
        urlBase = window.location.origin + "//" + window.location.pathname.split('/')[1];
    }
    else {
        urlBase = location.protocol + "//" + window.location.hostname + ":" + window.location.port;
    }
    return urlBase + "/WebResources/Charts/wsCharts.asmx/";
} 


function KpiPicking(idWhs, user, typeUnid) {
    var param = {
        idWhs: idWhs,
        user: user,
        typeUnid: typeUnid
    };

    $.ajax({
        type: "POST",
        url: urlWSCharts() + "KpiPicking",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            createChartKpiPicking(data.d);
        },
        error: function (jqXHR, status, err) {
            console.error("Error en KpiPicking");
        }
    })
}

function createChartKpiPicking(data) {

    var ctx = document.getElementById('chartKpiPicking').getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "Month"),
            datasets: [{
                label: "Productividad de Picking",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Qty"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha Creación'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: "Cantidad"
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}

function KpiZone(idWhs, zoneCode, typeCharts) {
    var param = {
        idWhs: idWhs,
        zoneCode: zoneCode,
        typeCharts: typeCharts
    };

    $.ajax({
        type: "POST",
        url: urlWSCharts() + "KpiZone",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            createChartKpiZone(data.d);
        },
        error: function (jqXHR, status, err) {
            console.error("Error en KpiZone");
        }
    })
}

function createChartKpiZone(data) {
    
    var ctx = document.getElementById('chartKpiZone').getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "Month"),
            datasets: [{
                label: "Ocupación Por Zona",
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Delta"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: ''
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: ''
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}

function KpiFillRate(idOwn, idWhs, customerCode) {
    var param = {
        idOwn: idOwn,
        idWhs: idWhs,
        customerCode: customerCode
    };

    $.ajax({
        type: "POST",
        url: urlWSCharts() + "KpiFillRate",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            createChartKpiFillRate(data.d);
        },
        error: function (jqXHR, status, err) {
            console.error("Error en KpiFillRate");
        }
    })
}

function createChartKpiFillRate(data) {

    var ctx = document.getElementById('chartKpiFillRate').getContext('2d');

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "DateCreated"),
            datasets: [{
                label: "Información de Fill Rate",
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "PercentSatisfaction"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha Creación'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: '% Nivel Satisfacción'
                    },
                    ticks: {
                        beginAtZero: true,
                        steps: 10,
                        stepValue: 5,
                        max: 100
                    }
                }]
            }
        }
    });
}


function KpiLeadTime(idOwn, idWhs, typoLead, typeCustomer) {
    var param = {
        idOwn: idOwn,
        idWhs: idWhs,
        typoLead: typoLead,
        typeCustomer: typeCustomer
    };

    $.ajax({
        type: "POST",
        url: urlWSCharts() + "KpiLeadTime",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(param),
        success: function (data, status, jqXHR) {
            createChartKpiLeadTime(data.d);
        },
        error: function (jqXHR, status, err) {
            console.error("Error en KpiLeadTime");
        }
    })
}

function createChartKpiLeadTime(data) {

    var ctx = document.getElementById('chartKpiLeadTime').getContext('2d');
    var labelY = "Delta ";
    labelY += $("#ctl00_MainContent_ddlLeadTime").val() == "H" ? "Horas" : "Dias"; 

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: _.pluck(data, "Month"),
            datasets: [{
                label: "Información de Lead Time",
                backgroundColor: 'rgb(21, 238, 115)',
                borderColor: 'rgb(255, 99, 132)',
                data: _.pluck(data, "Delta"),
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Fecha Creación'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: labelY
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}