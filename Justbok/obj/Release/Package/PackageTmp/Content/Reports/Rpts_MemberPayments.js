var pageNo = 1, pagerLoaded = false;

$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMembershipPayments_length").val() != null && $("#tblMembershipPayments_length").val() != "") {
        GettingPaymentList(pageNo, $("#tblMembershipPayments_length").val())
    }
    else {
        GettingPaymentList(pageNo, 10)
    }
    BindMembership();
    TotalPayment();

    $('#btnSearch').click(function () {
        if ($("#tblMembershipPayments_length").val() != null && $("#tblMembershipPayments_length").val() != "") {
            SearchMember(pageNo, $("#tblMembershipPayments_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});



function GettingPaymentList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetPaymentMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipPayments tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
               
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                   
                    var rows = '<tr role="row" class="odd">'
                          + '<td>' + item.RecieptNumber + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.Amount + '</td>'
                           + '<td>' + item.PaymentDate + '</td>'
                         + '<td>' + item.PaymentAmount + '</td>'
                          + '<td>' + item.PaymentType + '</td>'
                           + '<td>' + pendingamt + '</td>'
                             + '<td>' + item.PaymentDueDate + '</td>'
                               + '<td>' + item.Note + '</td>'
                        + '</tr >';
                    $('#tblMembershipPayments tbody').append(rows);
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
                                    GettingPaymentList(pageNo, $("#tblMembershipList_length").val())
                                }
                                else {
                                    GettingPaymentList(pageNo, 10)
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
                $('#tblMembershipPayments tbody').append(norecords);
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
        data: { page: pageno, pagesize: pagesize, membername: $("#txtSearch").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), membership: $("#ddlMembership option:selected").text(),category:$("#ddlCategory option:selected").text(),paymentMode:$("#ddlPaymnetMode option:selected").text(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipPayments tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
               
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {
                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                   
                    var rows = '<tr role="row" class="odd">'
                          + '<td>' + item.RecieptNumber + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.Amount + '</td>'
                           + '<td>' + item.PaymentDate + '</td>'
                         + '<td>' + item.PaymentAmount + '</td>'
                          + '<td>' + item.PaymentType + '</td>'
                           + '<td>' + pendingamt + '</td>'
                             + '<td>' + item.PaymentDueDate + '</td>'
                               + '<td>' + item.Note + '</td>'
                        + '</tr >';
                    $('#tblMembershipPayments tbody').append(rows);
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
                                if ($("#tblMembershipPayments_length").val() != null && $("#tblMembershipPayments_length").val() != "") {
                                    SearchMember(pageNo, $("#tblMembershipPayments_length").val())
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
                $('#tblMembershipPayments tbody').append(norecords);
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

function TotalPayment()
{
    $.ajax({
        cache: false,
        type: "GET",
        url: TotalPaymentUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            var totalAmount = 0;
            $.each(data, function (i, obj) {
                totalAmount = totalAmount + obj.PaymentAmount;
                });
            $('#lblPayment').html(totalAmount+" .00");
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
            window.location = DownloadPdfUrl + "?filename=" + "ReportsMemberPayments.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}