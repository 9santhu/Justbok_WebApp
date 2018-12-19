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

    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);


    $("#txtFromDate").datepicker("setDate", firstDay);
    $("#txtToDate").datepicker("setDate", lastDay);

  
    if ($("#tblDuePayment_length").val() != null && $("#tblDuePayment_length").val() != "") {
        DuePaymentDayList(pageNo, $("#tblDuePayment_length").val())
    }
    else {
        DuePaymentDayList(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblDuePayment_length").val() != null && $("#tblDuePayment_length").val() != "") {
            SearchDuePayment(pageNo, $("#tblDuePayment_length").val())
        }
        else {
            SearchDuePayment(pageNo, 10)
        }
    });

});


function DuePaymentDayList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: DuePaymentDayUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblDuePayment tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                var totalpaidAmount = 0;
                var totalpendingAmount = 0;
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;

                    if (item.Amount != "") {
                        totalpaidAmount = totalpaidAmount + item.PaymentAmount;
                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                        totalpendingAmount = totalpendingAmount + pendingamt;
                    }

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
                        + '</tr >';
                    $('#tblDuePayment tbody').append(rows);
                });

                $('#paidAmount').html(totalpaidAmount);
                $('#pendingAmount').html(totalpendingAmount);

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
                                if ($("#tblDuePayment_length").val() != null && $("#tblDuePayment_length").val() != "") {
                                    DuePaymentDayList(pageNo, $("#tblDuePayment_length").val())
                                }
                                else {
                                    DuePaymentDayList(pageNo, 10)
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
                $('#tblDuePayment tbody').append(norecords);
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

function SearchDuePayment(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchDueListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtMember").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblDuePayment tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                var totalpaidAmount = 0;
                var totalpendingAmount = 0;
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;

                    if (item.Amount != "") {
                        totalpaidAmount = totalpaidAmount + item.PaymentAmount;
                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                        totalpendingAmount = totalpendingAmount + pendingamt;
                    }
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
                        + '</tr >';
                    $('#tblDuePayment tbody').append(rows);
                });

                $('#paidAmount').html(totalpaidAmount);
                $('#pendingAmount').html(totalpendingAmount);

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
                                if ($("#tblDuePayment_length").val() != null && $("#tblDuePayment_length").val() != "") {
                                    SearchDuePayment(pageNo, $("#tblDuePayment_length").val())
                                }
                                else {
                                    SearchDuePayment(pageNo, 10)
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
                $('#tblDuePayment tbody').append(norecords);
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




