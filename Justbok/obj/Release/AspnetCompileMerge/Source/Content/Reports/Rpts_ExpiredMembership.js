var pageNo = 1, pagerLoaded = false;

$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMembershipExpired_length").val() != null && $("#tblMembershipExpired_length").val() != "") {
        GettingExpiredList(pageNo, $("#tblMembershipExpired_length").val())
    }
    else {
        GettingExpiredList(pageNo, 10)
    }
    BindMembership();

    $('#btnSearch').click(function () {
        if ($("#tblMembershipExpired_length").val() != null && $("#tblMembershipExpired_length").val() != "") {
            SearchMember(pageNo, $("#tblMembershipExpired_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});



function GettingExpiredList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetExpiredMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipExpired tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    var expMonths = 0;
                    var months = "";
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if(item.Months!="")
                    {
                        expMonths = parseInt(item.Months);
                        if (expMonths > 1) {
                            months = expMonths + " Months";
                        }
                        else { months = expMonths + " Month"; }

                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.StartDate + '</td>'
                           + '<td>' + item.EndDate + '</td>'
                         + '<td>' + months + '</td>'
                    
                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblMembershipExpired tbody').append(rows);
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
                                if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
                                    GettingExpiredList(pageNo, $("#tblMembershipList_length").val())
                                }
                                else {
                                    GettingExpiredList(pageNo, 10)
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
                $('#tblMembershipExpired tbody').append(norecords);
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

function SearchMember(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchExpiredMemberListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtSearch").val(), membership: $("#ddlMembership option:selected").text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipExpired tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    var expMonths = 0;
                    var months = "";
                    var startdate = "";
                    var enddate = "";
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if (item.Months != "") {
                        expMonths = parseInt(item.Months);
                        if (expMonths > 1) {
                            months = expMonths + " Months";
                        }
                        else { months = expMonths + " Month"; }

                    }
                  
                    if (item.StartDate != null)
                    {
                        startdate = ConvertBackEndDate(item.StartDate);
                    }

                    if (item.EndDate != null)
                    {
                          enddate=ConvertBackEndDate(item.EndDate)
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.MembershipType + '</td>'
                          + '<td>' + startdate + '</td>'
                           + '<td>' + enddate + '</td>'
                         + '<td>' + months + '</td>'
                        + '</tr >';
                    $('#tblMembershipExpired tbody').append(rows);
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
                                if ($("#tblMembershipExpired_length").val() != null && $("#tblMembershipExpired_length").val() != "") {
                                    SearchMember(pageNo, $("#tblMembershipExpired_length").val())
                                }
                                else {
                                    SearchMember(pageNo, 10)
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
                $('#tblMembershipExpired tbody').append(norecords);
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


function ConvertBackEndDate(date) {
    var dt = new Date(parseInt(date.substr(6)));
    var twoDigitDate = dt.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((dt.getMonth().length + 1) === 1) ? (dt.getMonth() + 1) : '0' + (dt.getMonth() + 1);
    var year = dt.getFullYear();
    var fulldate = twoDigitDate + "/" + getMonth + "/" + year;
    return fulldate;
}

function BindMembership() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlMembership").append($("<option></option>").val(obj.MembershipOfferId).html(obj.MemershipType));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
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
        data: { membername: $("#txtSearch").val(), membership: $("#ddlMembership option:selected").text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
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
        data: { membername: $("#txtSearch").val(), membership: $("#ddlMembership option:selected").text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
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