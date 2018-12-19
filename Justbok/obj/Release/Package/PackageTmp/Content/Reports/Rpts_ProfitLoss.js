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

   
   
   
    if ($("#ProfitLossList_length").val() != null && $("#ProfitLossList_length").val() != "") {
        ProfitLossList(pageNo, $("#ProfitLossList_length").val())
    }
    else {
        ProfitLossList(pageNo, 10)
    }
    $('#btnFilter').click(function () {

        if ($("#ProfitLossList_length").val() != null && $("#ProfitLossList_length").val() != "") {
            SearchProfitLossList(pageNo, $("#ProfitLossList_length").val())
        }
        else {
            SearchProfitLossList(pageNo, 10)
        }
    });

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

function ProfitLossList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: profitlosslistUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#ProfitLossList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var monthYear = item.DateYear.toString().split(" ");
                    var d = new Date();
                    var n = d.getFullYear();
                    var totalAmount = 0;
                    var month = monthYear[0];
                    var year = monthYear[1];
                    var total = parseInt(item.Payment) - parseInt(item.Expense);
                    var Total="";
                    if (total > 0)
                    {
                        Total = '<span style="font-size:14px" class="label label-success">' + total + '</span>';
                    }
                    else if (total < 0) {
                        Total = '<span style="font-size:14px" class="label label-danger">' + total + '</span>';
                    }
                    else {
                        Total = '<span style="font-size:14px" class="label label-info">' + total + '</span>';
                    } 

                            var monthName = getMonth(month);
                            var rows = '<tr role="row" class="odd">'
               + '<td>' + monthName + " - " + year + '</td>'
                              + '<td>' + item.Payment + '</td>'
                               + '<td>' + item.Expense + '</td>'
                                + '<td>' + Total + '</td>'

                 + '</tr >';
                            $('#ProfitLossList tbody').append(rows);
                            totalAmount = 0;
                        //}
                    //}
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
                                if ($("#ProfitLossList_length").val() != null && $("#ProfitLossList_length").val() != "") {
                                    ProfitLossList(pageNo, $("#ProfitLossList_length").val())
                                }
                                else {
                                    ProfitLossList(pageNo, 10)
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
                $('#ProfitLossList tbody').append(norecords);
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

function SearchProfitLossList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchprofitlosslistUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startMonth: $('#startDate').val(), endMonth: $('#EndDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#ProfitLossList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var monthYear = item.DateYear.toString().split(" ");
                    var d = new Date();
                    var n = d.getFullYear();
                    var totalAmount = 0;
                    var month = monthYear[0];
                    var year = monthYear[1];
                    var total = parseInt(item.Payment) - parseInt(item.Expense);
                    var Total = "";
                    if (total > 0) {
                        Total = '<span style="font-size:14px" class="label label-success">' + total + '</span>';
                    }
                    else if (total < 0) {
                        Total = '<span style="font-size:14px" class="label label-danger">' + total + '</span>';
                    }
                    else {
                        Total = '<span style="font-size:14px" class="label label-info">' + total + '</span>';
                    }

                    var monthName = getMonth(month);
                    var rows = '<tr role="row" class="odd">'
       + '<td>' + monthName + " - " + year + '</td>'
                      + '<td>' + item.Payment + '</td>'
                       + '<td>' + item.Expense + '</td>'
                        + '<td>' + Total + '</td>'

         + '</tr >';
                    $('#ProfitLossList tbody').append(rows);
                    totalAmount = 0;
                    //}
                    //}
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
                                if ($("#ProfitLossList_length").val() != null && $("#ProfitLossList_length").val() != "") {
                                    SearchProfitLossList(pageNo, $("#ProfitLossList_length").val())
                                }
                                else {
                                    SearchProfitLossList(pageNo, 10)
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
                $('#ProfitLossList tbody').append(norecords);
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
        data: { branchid: $('#ddlBranch option:selected').val() },
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
        data: {branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "ReportsProfitLoss.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}