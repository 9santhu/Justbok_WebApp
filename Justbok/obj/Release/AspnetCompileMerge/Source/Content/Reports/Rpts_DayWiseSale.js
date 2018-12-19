var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/yy",
        yearRange: "-90:+00"
    });
    if ($("#tblModeWisePayment_length").val() != null && $("#tblModeWisePayment_length").val() != "") {
        GettingMonthWiseReports(pageNo, $("#tblModeWisePayment_length").val())
    }
    else {
        GettingMonthWiseReports(pageNo, 10)
    }


    //CategoerySales();
    $('#btnSearch').click(function () { alert($('#txtTodate').val()); });

    ////$('#btnSearchReset').click(function () { SearchReset(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function getMonth(month) {
    var day = "";
    switch (month) {

        case "1":
            day = "January";
            break;
        case "2":
            day = "February";
            break;
        case "3":
            day = "March";
            break;
        case "4":
            day = "April";
            break;
        case "5":
            day = "May";
            break;
        case "6":
            day = "June";
            break;
        case "7":
            day = "July";
            break;
        case "8":
            day = "August";
            break;
        case "9":
            day = "September";
            break;
        case "10":
            day = "October";
            break;
        case "11":
            day = "November";
            break;
        case "12":
            day = "December";


    }
    return day;
}

function GettingMonthWiseReports(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetDayWiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblModeWisePayment tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var totalAmount = 0;
                    var cash = 0;
                    var cheque = 0;
                    var card = 0;
                    var bankTransfer = 0;
                    $.each(item.PaymentType, function (i, item) {
                        if (item.PaymentType == "Cash") {
                            cash = cash + item.PaymentAmount;
                            totalAmount = totalAmount + item.PaymentAmount;
                        }
                        else if (item.PaymentType == "Cheque") {
                            cheque = cheque + item.PaymentAmount;
                            totalAmount = totalAmount + item.PaymentAmount;
                        }
                        else if (item.PaymentType == "Card") {
                            card = card + item.PaymentAmount;
                            totalAmount = totalAmount + item.PaymentAmount;
                        }
                        else if (item.PaymentType == "Bank Transfer") {
                            bankTransfer = bankTransfer + item.PaymentAmount;
                            totalAmount = totalAmount + item.PaymentAmount;
                        }

                    });

                    if (item.dt != "")
                    {

                        var rows = '<tr role="row" class="odd">'
                      + '<td>' + item.dt + '</td>'
                                     + '<td>' + cash + '</td>'
                                      + '<td>' + cheque + '</td>'
                                      + '<td>' + card + '</td>'
                                      + '<td>' + bankTransfer + '</td>'
                                      + '<td>' + totalAmount + '</td>'


                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                        $('#tblModeWisePayment tbody').append(rows);

                        totalAmount = 0;
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
                                if ($("#tblModeWisePayment_length").val() != null && $("#tblModeWisePayment_length").val() != "") {
                                    GettingMonthWiseReports(pageNo, $("#tblModeWisePayment_length").val())
                                }
                                else {
                                    GettingMonthWiseReports(pageNo, 10)
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
                $('#tblModeWisePayment tbody').append(norecords);
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
            window.location = DownloadPdfUrl + "?filename=" + "ReportsDayWise.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}