var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMeasurement_length").val() != null && $("#tblMeasurement_length").val() != "") {
        GettingMeasurementList(pageNo, $("#tblMeasurement_length").val())
    }
    else {
        GettingMeasurementList(pageNo, 10)
    }


    $('#btnSearch').click(function () {
        if ($("#tblMeasurement_length").val() != null && $("#tblMeasurement_length").val() != "") {
            SearchMeasurementList(pageNo, $("#tblMeasurement_length").val())
        }
        else {
            SearchMeasurementList(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});

function GettingMeasurementList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMeasurementMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMeasurementList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var next = max_date(item.NextMeasurementDate);
                    var last = max_date(item.MeasurementDate);
                    var nextMeasurement;
                    var lastMeasurement;
                    if (next != null) {
                        nextMeasurement = ConvertDate(next);
                    }
                    else { nextMeasurement = ""; }

                    if (last != null) {
                        lastMeasurement = ConvertDate(last);
                    }
                    else { lastMeasurement = ""; }

                    var rows = '<tr role="row" class="odd">'
                         + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName[0] + " " + item.LastName[0] + '</td>'
                        + '<td>' + item.MobileNumber[0] + '</td>'
                             + '<td>' + lastMeasurement + '</td>'
                              + '<td>' + nextMeasurement + '</td>'

                        + '</tr >';

                    $('#tblMeasurementList tbody').append(rows);
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
                                if ($("#tblMeasurement_length").val() != null && $("#tblMeasurement_length").val() != "") {
                                    GettingMeasurementList(pageNo, $("#tblMeasurement_length").val())
                                }
                                else {
                                    GettingMeasurementList(pageNo, 10)
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
                $('#tblMeasurementList tbody').append(norecords);
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

function ConvertDate(convertdate) {
    var date = convertdate;
    var parsedDate = new Date(parseInt(date.substr(6)));
    var newDate = new Date(parsedDate);

    //var getMonth = newDate.getMonth();
    var getDay = newDate.getDay();
    var getYear = newDate.getYear();

    var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((newDate.getMonth().length + 1) === 1) ? (newDate.getMonth() + 1) : '0' + (newDate.getMonth() + 1);

    var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();

    return startdate;
}

function max_date(all_dates) {
    var max_dt = all_dates[0],
      max_dtObj = new Date(all_dates[0]);
    all_dates.forEach(function (dt, index) {
        if (new Date(dt) > max_dtObj) {
            max_dt = dt;
            max_dtObj = new Date(dt);
        }
    });
    return max_dt;
}



function SearchMeasurementList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMeasurementMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, membername: $('#txtCustomerName').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {

            $('#tblMeasurementList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var next = max_date(item.NextMeasurementDate);
                    var last = max_date(item.MeasurementDate);
                    var nextMeasurement;
                    var lastMeasurement;
                    if (next != null) {
                        nextMeasurement = ConvertDate(next);
                    }
                    else { nextMeasurement = ""; }

                    if (last != null) {
                        lastMeasurement = ConvertDate(last);
                    }
                    else { lastMeasurement = ""; }

                    var rows = '<tr role="row" class="odd">'
                         + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName[0] + " " + item.LastName[0] + '</td>'
                        + '<td>' + item.MobileNumber[0] + '</td>'
                             + '<td>' + lastMeasurement + '</td>'
                              + '<td>' + nextMeasurement + '</td>'

                        + '</tr >';
                    $('#tblMeasurementList tbody').append(rows);
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
                                if ($("#tblMeasurement_length").val() != null && $("#tblMeasurement_length").val() != "") {
                                    SearchMeasurementList(pageNo, $("#tblMeasurement_length").val())
                                }
                                else {
                                    SearchMeasurementList(pageNo, 10)
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
                $('#tblMeasurementList tbody').append(norecords);
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


function setClass() {
    $("th").removeClass();
    $("th").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass();
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }


}

function ExcelMembershipList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: ExcelUrl,
        dataType: "json",
        data: { membername: $('#txtCustomerName').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
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
        data: { membername: $('#txtCustomerName').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "ReportsMembership.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}
