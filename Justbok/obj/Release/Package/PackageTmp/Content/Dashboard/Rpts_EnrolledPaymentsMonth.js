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

    BindMembership();
    BindStaff();

    if ($("#tblPayment_length").val() != null && $("#tblPayment_length").val() != "") {
        EnrolledPaymentList(pageNo, $("#tblPayment_length").val())
    }
    else {
        EnrolledPaymentList(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblPayment_length").val() != null && $("#tblPayment_length").val() != "") {
            SearchSales(pageNo, $("#tblPayment_length").val())
        }
        else {
            SearchSales(pageNo, 10)
        }
    });

});


function EnrolledPaymentList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPaymentList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                var totalAmount = 0;
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    var note = (item.Note != null) ? item.Note : "";
                    if (item.Amount != "") {
                        totalAmount = totalAmount + item.PaymentAmount;
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
                                + '<td>' + item.Representative + '</td>'
                                 + '<td>' + pendingamt + '</td>'
                                 + '<td>' + item.PaymentDueDate + '</td>'
                     + '<td>' + note + '</td>'


                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblPaymentList tbody').append(rows);
                });
                $('#totalAmount').html(totalAmount);

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
                                if ($("#tblPayment_length").val() != null && $("#tblPayment_length").val() != "") {
                                    EnrolledPaymentList(pageNo, $("#tblPayment_length").val())
                                }
                                else {
                                    EnrolledPaymentList(pageNo, 10)
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
                $('#tblPaymentList tbody').append(norecords);
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

function SearchSales(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchSalesListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtMember").val(), membershipid: $("#ddlMembership option:selected").text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), paymentmode: $("#ddlPaymentMode option:selected").text(), category: $("#ddlCategory option:selected").text(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPaymentList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                var totalAmount = 0;
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    var note = (item.Note != null) ? item.Note : "";
                    if (item.Amount != "") {
                        totalAmount = totalAmount + item.PaymentAmount;
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
                                + '<td>' + item.Representative + '</td>'
                                 + '<td>' + pendingamt + '</td>'
                                 + '<td>' + item.PaymentDueDate + '</td>'
                     + '<td>' + note + '</td>'


                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblPaymentList tbody').append(rows);
                });
                $('#totalAmount').html(totalAmount);

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
                                if ($("#tblPayment_length").val() != null && $("#tblPayment_length").val() != "") {
                                    EnrolledPaymentList(pageNo, $("#tblPayment_length").val())
                                }
                                else {
                                    EnrolledPaymentList(pageNo, 10)
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
                $('#tblPaymentList tbody').append(norecords);
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

function BindStaff() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindStaffUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlRepresentative").append($("<option></option>").val(obj.StaffId).html(obj.FirstName + " " + obj.LastName));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}


function BindCategory() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindStaffUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlRepresentative").append($("<option></option>").val(obj.StaffId).html(obj.FirstName + " " + obj.LastName));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function EditMember(id) {
    //var openTab = $(location.hash).filter("#Membership");
    //alert(JSON.stringify(openTab));
    //if (openTab.length) {
    //    $("a[href='" + location.hash + "']").click();
    //}
    LoadPage(EditMemberUrl + "/" + id + '#/Membership', 'Justbok | Edit Member');
    return false;
}

