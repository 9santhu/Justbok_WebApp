var pageNo = 1, pagerLoaded = false;

$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblPaymentsDueList_length").val() != null && $("#tblPaymentsDueList_length").val() != "") {
        GettingDuePayment(pageNo, $("#tblPaymentsDueList_length").val())
    }
    else {
        GettingDuePayment(pageNo, 10)
    }
    GetPendingAmount();

    $('#btnSearch').click(function () {
        if ($("#tblPaymentsDueList_length").val() != null && $("#tblPaymentsDueList_length").val() != "") {
            SearchDuePayments(pageNo, $("#tblPaymentsDueList_length").val())
        }
        else {
            SearchDuePayments(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});

function GettingDuePayment(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetDuepaymentListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPaymentDueList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
             
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                   
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    
                    totalPaidAmount = totalPaidAmount +  parseInt(item.PaymentAmount);
                    totalPendingAmount = totalPaidAmount +  pendingamt;

                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.StartDate + '</td>'
                           + '<td>' + item.PaymentDueDate + '</td>'
                            + '<td>' + item.Amount + '</td>'
                             + '<td>' + item.PaymentAmount + '</td>'
                    + '<td>' + pendingamt + '</td>'

                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblPaymentDueList tbody').append(rows);
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
                                if ($("#tblPaymentsDueList_length").val() != null && $("#tblPaymentsDueList_length").val() != "") {
                                    GettingDuePayment(pageNo, $("#tblPaymentsDueList_length").val())
                                }
                                else {
                                    GettingDuePayment(pageNo, 10)
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
                $('#tblPaymentDueList tbody').append(norecords);
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

function GetPendingAmount()
{
    $.ajax({
        cache: false,
        type: "GET",
        url: PendingpaymentListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            var totalPaidAmount = 0;
            var totalPendingAmount = 0;

            $.each(data, function (i, item) {
                var pendingamt = 0;
                if (item.Amount != "") {

                    var amount = parseInt(item.Amount);
                    var paid = parseInt(item.PaymentAmount);
                    pendingamt = amount - paid
                }
                totalPaidAmount = totalPaidAmount + parseInt(item.PaymentAmount);
                totalPendingAmount = totalPaidAmount + pendingamt;
            });
            $('#totalPaidAmount').html(totalPaidAmount);
            $('#totalPendingAmount').html(totalPendingAmount);
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function SearchDuePayments(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchDuePaymentListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtCustomerName").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPaymentDueList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    var rows = '<tr role="row" class="odd">'
                    var rows = '<tr role="row" class="odd">'
                    + '<td>' + item.MemberID + '</td>'
                    + '<td>' + item.FirstName + '</td>'
                    + '<td>' + item.MobileNumber + '</td>'
                     + '<td>' + item.Package + '</td>'
                      + '<td>' + item.StartDate + '</td>'
                       + '<td>' + item.PaymentDueDate + '</td>'
                        + '<td>' + item.Amount + '</td>'
                         + '<td>' + item.PaymentAmount + '</td>'
                + '<td>' + pendingamt + '</td>'
                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblPaymentDueList tbody').append(rows);
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
                                if ($("#tblPaymentsDueList_length").val() != null && $("#tblPaymentsDueList_length").val() != "") {
                                    SearchMember(pageNo, $("#tblPaymentsDueList_length").val())
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
                $('#tblMembershipList tbody').append(norecords);
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
        data: { membername: $("#txtCustomerName").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
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
        data: { membername: $("#txtCustomerName").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "ReportsDuePayments.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}