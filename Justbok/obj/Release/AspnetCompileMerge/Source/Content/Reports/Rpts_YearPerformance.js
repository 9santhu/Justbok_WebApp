var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {
    ShowLoader();
    //$('input[type=datetime]').datepicker({
    //    changeMonth: true,
    //    changeYear: true,
    //    dateFormat: "dd/mm/yy",
    //    yearRange: "-90:+00"
    //});
    $(function () {
        $('input[type=datetime]').datepicker({
            changeMonth: false,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'yy',
            onClose: function (dateText, inst) {
                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                $(this).datepicker('setDate', new Date(year, 0, 1));
            }
        });
    });


    if ($("#tblYearPerformance_length").val() != null && $("#tblYearPerformance_length").val() != "") {
        YearPerformanceReport(pageNo, $("#tblYearPerformance_length").val())
    }
    else {
        YearPerformanceReport(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblYearPerformance_length").val() != null && $("#tblYearPerformance_length").val() != "") {
            SearchTrailDateCompletionReport(pageNo, $("#tblYearPerformance_length").val())
        }
        else {
            SearchTrailDateCompletionReport(pageNo, 10)
        }
    });

    //$('#btnSearchReset').click(function () { SearchStockReport(); });

});

function YearPerformanceReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: YearPerformanceReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblYearPerformance tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                        var rows = '<tr role="row" class="odd">'
                //+ '<td style=display:none;>' + item.EnquiryId + '</td>'
                + '<td>' + item.Year + '</td>'
                + '<td>' + item.MembershipSale + '</td>'
                + '<td>' + item.Expense + '</td>'
                 + '<td>' + item.NetProfit + '</td>'
                + '<td>' + item.POSSale + '</td>'
                 + '<td>' + item.Enquiry + '</td>'
                  + '<td>' + item.SoldMembership + '</td>'
                 
       + '</tr >';
                        $('#tblYearPerformance tbody').append(rows);
                   
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
                                if ($("#tblYearPerformance_length").val() != null && $("#tblYearPerformance_length").val() != "") {
                                    YearPerformanceReport(pageNo, $("#tblYearPerformance_length").val())
                                }
                                else {
                                    YearPerformanceReport(pageNo, 10)
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
                $('#tblYearPerformance tbody').append(norecords);
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