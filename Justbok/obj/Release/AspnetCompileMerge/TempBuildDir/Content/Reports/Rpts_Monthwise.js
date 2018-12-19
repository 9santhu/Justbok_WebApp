var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $(function () {
        $('.date-picker').datepicker(
                       {
                           dateFormat: "mm/yy",
                           changeMonth: true,
                           changeYear: true,
                           showButtonPanel: true,
                           onClose: function (dateText, inst) {


                               function isDonePressed() {
                                   return ($('#ui-datepicker-div').html().indexOf('ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all ui-state-hover') > -1);
                               }

                               if (isDonePressed()) {
                                   var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                                   var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                                   $(this).datepicker('setDate', new Date(year, month, 1)).trigger('change');

                                   $('.date-picker').focusout()//Added to remove focus from datepicker input box on selecting date
                               }
                           },
                           beforeShow: function (input, inst) {

                               inst.dpDiv.addClass('month_year_datepicker')

                               if ((datestr = $(this).val()).length > 0) {
                                   year = datestr.substring(datestr.length - 4, datestr.length);
                                   month = datestr.substring(0, 2);
                                   $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                                   $(this).datepicker('setDate', new Date(year, month - 1, 1));
                                   $(".ui-datepicker-calendar").hide();
                               }
                           }
                       })
    });
    if ($("#tblModeWisePayment_length").val() != null && $("#tblModeWisePayment_length").val() != "") {
        GettingMonthWiseReports(pageNo, $("#tblModeWisePayment_length").val())
    }
    else {
        GettingMonthWiseReports(pageNo, 10)
    }
   

    //CategoerySales();
    $('#btnSearch').click(function () {
        if ($("#tblModeWisePayment_length").val() != null && $("#tblModeWisePayment_length").val() != "") {
            SearchMonthWiseReports(pageNo, $("#tblModeWisePayment_length").val())
        }
        else {
            SearchMonthWiseReports(pageNo, 10)
        }

    });

    ////$('#btnSearchReset').click(function () { SearchReset(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function getMonth(month)
{
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
        url: GetMonthWiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblModeWisePayment tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
              
                $.each(data.Result, function (i, item) {
                    var monthYear = item.dt.toString().split(" ");
                    var d = new Date();
                    var n = d.getFullYear();
                    var totalAmount = 0;
                    var cash = 0;
                    var cheque = 0;
                    var card = 0;
                    var bankTransfer = 0;

                        var month = monthYear[0];
                        var year = monthYear[1];
                        if (n == year)
                        {
                            if (month != "") {
                                var monthName = getMonth(month);
                               
                               
                                $.each(item.PaymentType, function (i, item) {

                                   

                                    if (item.PaymentType=="Cash")
                                    {
                                        cash = cash + item.PaymentAmount;
                                        totalAmount = totalAmount + item.PaymentAmount;
                                    }
                                    else if (item.PaymentType == "Cheque")
                                    {
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
                               
                                
                                var rows = '<tr role="row" class="odd">'
                   + '<td>' + monthName + " - " + year + '</td>'
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

function SearchMonthWiseReports(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMonthWiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startDate: $('#startDate').val(), endDate: $('#enddDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblModeWisePayment tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {

                $.each(data.Result, function (i, item) {
                    var monthYear = item.dt.toString().split(" ");
                    var d = new Date();
                    var n = d.getFullYear();
                    var totalAmount = 0;
                    var cash = 0;
                    var cheque = 0;
                    var card = 0;
                    var bankTransfer = 0;

                    var month = monthYear[0];
                    var year = monthYear[1];
                    if (n == year) {
                        if (month != "") {
                            var monthName = getMonth(month);
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

                            var rows = '<tr role="row" class="odd">'
               + '<td>' + monthName + " - " + year + '</td>'
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
                                    SearchMonthWiseReports(pageNo, $("#tblModeWisePayment_length").val())
                                }
                                else {
                                    SearchMonthWiseReports(pageNo, 10)
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
            window.location = DownloadPdfUrl + "?filename=" + "ReportsMonthWise.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

           // alert(JSON.stringify(data));

        }
    });
}