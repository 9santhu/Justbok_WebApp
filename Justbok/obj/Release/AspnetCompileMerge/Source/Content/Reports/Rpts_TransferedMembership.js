var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblTransferred_length").val() != null && $("#tblTransferred_length").val() != "") {
        GettingTransferedList(pageNo, $("#tblTransferred_length").val())
    }
    else {
        GettingTransferedList(pageNo, 10)
    }
   

    $('#btnSearch').click(function () {
        if ($("#tblTransferred_length").val() != null && $("#tblTransferred_length").val() != "") {
            SearchTransferedList(pageNo, $("#tblTransferred_length").val())
        }
        else {
            SearchTransferedList(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function GettingTransferedList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetTransferedMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblTransferredList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if (pendingamt != 0) {

                        var rows = '<tr role="row" class="odd">'
                            + '<td>' + item.FirstName + '</td>'
                            + '<td>' + item.MobileNumber + '</td>'
                             + '<td>' + item.Package + '</td>'
                             + '<td>' + item.Months + '</td>'
                              + '<td>' + item.StartDate + '</td>'
                               + '<td>' + item.EndDate + '</td>'
                                 + '<td>' + item.Amount + '</td>'
                                  + '<td>' + pendingamt + '</td>'
                        
                            + '</tr >';
                    }
                    $('#tblTransferredList tbody').append(rows);
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
                                if ($("#tblTransferred_length").val() != null && $("#tblTransferred_length").val() != "") {
                                    GettingDueMemberList(pageNo, $("#tblTransferred_length").val())
                                }
                                else {
                                    GettingDueMemberList(pageNo, 10)
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
                $('#tblTransferredList tbody').append(norecords);
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

function SearchTransferedList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchTransferedMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, membername: $('#txtCustomerName').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblTransferredList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if (pendingamt != 0) {

                        var rows = '<tr role="row" class="odd">'
                            + '<td>' + item.FirstName + '</td>'
                            + '<td>' + item.MobileNumber + '</td>'
                             + '<td>' + item.Package + '</td>'
                             + '<td>' + item.Months + '</td>'
                              + '<td>' + item.StartDate + '</td>'
                               + '<td>' + item.EndDate + '</td>'
                                 + '<td>' + item.Amount + '</td>'
                                  + '<td>' + pendingamt + '</td>'
                            + '</tr >';
                    }
                    $('#tblTransferredList tbody').append(rows);
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
                                if ($("#tblTransferred_length").val() != null && $("#tblTransferred_length").val() != "") {
                                    SearchTransferedList(pageNo, $("#tblTransferred_length").val())
                                }
                                else {
                                    SearchTransferedList(pageNo, 10)
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
                $('#tblTransferredList tbody').append(norecords);
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
            // var response = JSON.parse(data);

            window.location = DownloadExcelUrl + "?fileGuid='" + data.FileGuid
                              + '&filename=' + data.FileName;
            //toastr.success("Excel Report Generated.");
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


            console.log(data.FileName);
            window.location = DownloadPdfUrl + "?filename=" + "ReportsTransferedMembership.pdf";
            //toastr.success("PDF Report Generated.");
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}