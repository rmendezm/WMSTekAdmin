function setWidthWithVerticalSplitter() {
    if ($(".ob_spl_dividervertical").length > 0) {
        var percentageTotal = 0.93;
        var widthDivRight = $("#__hsMasterDetailR").width() * percentageTotal;

        $.each($("#__hsMasterDetailR .froze-header-grid, #__hsMasterDetailR .row-height-filter"), function (i, elem) {
            $(elem).css("width", widthDivRight + "px");
        });
    }
}

function setDivsAfter() {
    var extraHeight = 20;

    $.each($(".ob_spl_toppanelcontent div.froze-header-grid"), function (i, elem) {
        var heightDivContainerTable = $(elem).height();
        var totalHeight = heightDivContainerTable - $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_lpnHeadTitle").height();
        totalHeight = totalHeight - extraHeight;

        $(elem).css("height", totalHeight + "px");
    });

    $.each($(".ob_spl_bottompanelcontent div.froze-header-grid"), function (i, elem) {
        var heightDivContainerTable = $(elem).height();
        var totalHeight = heightDivContainerTable - $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl01_ctl01_divFilter").height();
        totalHeight = totalHeight - extraHeight;

        $(elem).css("height", totalHeight + "px");
    });
}