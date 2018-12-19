var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        yearRange: "-90:+00"
    });
    if ($("#tblPosSales_length").val() != null && $("#tblPosSales_length").val() != "") {
        PosSaleReportGym(pageNo, $("#tblPosSales_length").val())
    }
    else {
        PosSaleReportGym(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblPosSales_length").val() != null && $("#tblPosSales_length").val() != "") {
            SearchPosSaleReportGym(pageNo, $("#tblPosSales_length").val())
        }
        else {
            SearchPosSaleReportGym(pageNo, 10)
        }
    });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
    ////$('#btnSearchReset').click(function () { SearchReset(); });

});

function PosSaleReportGym(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetPosListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPosSales tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var cash = 0;
                    var card = 0;
                    $.each(item.PaymentVia, function (i, item) {

                        if (item.PaymentVia == "Cash")
                                {
                            cash = cash + item.Total;
                                }
                        else if (item.PaymentVia == "Card") {
                            card = card + item.Total;
                                }
                            });
                               
                            if (item.dt != "")
                            {
                                var rows = '<tr role="row" class="odd">'
             + '<td>' + item.dt + '</td>'
                            + '<td>' + cash + '</td>'
                             + '<td>' + card + '</td>'
               + '</tr >';
                                $('#tblPosSales tbody').append(rows);
                            }
                });

                if (data.Pages < pageNo) {
                    pageNo = data.Pages;
                }
                if (!pagerLoaded) {
                    pagerLoaded = true;
                    $('#pagination').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        startPage: pageNo,
                        onPageClick: function (event, page) {
                            ShowLoader();
                            if (pageNo != page) {
                                pageNo = page;
                                if ($("#tblPosSales_length").val() != null && $("#tblPosSales_length").val() != "") {
                                    PosSaleReportGym(pageNo, $("#tblPosSales_length").val())
                                }
                                else {
                                    PosSaleReportGym(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblPosSales tbody').append(norecords);
            }

            HideLoader();
            setClass();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });

}


function SearchPosSaleReportGym(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchPosListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPosSales tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var cash = 0;
                    var card = 0;
                    $.each(item.PaymentVia, function (i, item) {

                        if (item.PaymentVia == "Cash") {
                            cash = cash + item.Total;
                        }
                        else if (item.PaymentVia == "Card") {
                            card = card + item.Total;
                        }
                    });

                    if (item.dt != "") {
                        var rows = '<tr role="row" class="odd">'
     + '<td>' + item.dt + '</td>'
                    + '<td>' + cash + '</td>'
                     + '<td>' + card + '</td>'
       + '</tr >';
                        $('#tblPosSales tbody').append(rows);
                    }
                });

                if (data.Pages < pageNo) {
                    pageNo = data.Pages;
                }
                if (!pagerLoaded) {
                    pagerLoaded = true;
                    $('#pagination').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        startPage: pageNo,
                        onPageClick: function (event, page) {
                            ShowLoader();
                            if (pageNo != page) {
                                pageNo = page;
                                if ($("#tblPosSales_length").val() != null && $("#tblPosSales_length").val() != "") {
                                    SearchPosSaleReportGym(pageNo, $("#tblPosSales_length").val())
                                }
                                else {
                                    SearchPosSaleReportGym(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblPosSales tbody').append(norecords);
            }

            HideLoader();
            setClass();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });

}

function ExcelMembershipList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: ExcelUrl,
        dataType: "json",
        data: { startDate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadExcelUrl + "?fileGuid='" + data.FileGuid
                              + '&filename=' + data.FileName;
            HideLoader();
        },
        error: function (data) {
            HideLoader();
            alert("Failed! Please try again.");
        }
    });
}

function PDFMembershipList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: PDFUrl,
        dataType: "json",
        data: { startDate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports POS Sale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}