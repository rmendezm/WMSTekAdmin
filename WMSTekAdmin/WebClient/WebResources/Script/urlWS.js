function urlWS() {

    var urlBase;

    if (window.location.port == "") {
        urlBase = window.location.origin + "/" + window.location.pathname.split('/')[1];
    }
    else {

        var serverDomain = localStorage.getItem("serverDomain");

        if (serverDomain != null && serverDomain != '') {
            urlBase = location.protocol + "/" + window.location.hostname + ":" + window.location.port + "/" + serverDomain;
        } else {
            urlBase = location.protocol + "//" + window.location.hostname + ":" + window.location.port;
        }
    }
    return urlBase;
} 

function urlWSGrid() {
    return "/WebResources/UtilGrid.asmx/";
}

function urlWSSchedule() {
    return "/WebResources/ScheduleCalendar/ScheduleApi.asmx/";
}