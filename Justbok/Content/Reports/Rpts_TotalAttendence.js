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
    if ($("#tblTotalAttendence_length").val() != null && $("#tblTotalAttendence_length").val() != "") {
        TotalAttendenceReport(pageNo, $("#tblTotalAttendence_length").val())
    }
    else {
        TotalAttendenceReport(pageNo, 10)
    }


});

function TotalAttendenceReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: TotalAttendenceReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblTotalAttendance tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var date = ConvertDate(item.DateYear);
                  
                        var rows = '<tr role="row" class="odd">'
                + '<td>' + date + '</td>'
                + '<td>' + item.PresentStaff + '</td>'
                + '<td>' + item.PresentMembers + '</td>'
       + '</tr >';
                        $('#tblTotalAttendance tbody').append(rows);
                    
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
                                if ($("#tblTotalAttendence_length").val() != null && $("#tblTotalAttendence_length").val() != "") {
                                    TotalAttendenceReport(pageNo, $("#tblTotalAttendence_length").val())
                                }
                                else {
                                    TotalAttendenceReport(pageNo, 10)
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
                $('#tblTotalAttendance tbody').append(norecords);
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

function ConvertDate(convertdate) {
    var date = convertdate;
    var parsedDate = convertdate.substr(0,9);
    //var newDate = new Date(parsedDate);

    ////var getMonth = newDate.getMonth();
    //var getDay = newDate.getDay();
    //var getYear = newDate.getYear();

    //var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    //var getMonth = ((newDate.getMonth().length + 1) === 1) ? (newDate.getMonth() + 1) : '0' + (newDate.getMonth() + 1);

    //var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();

    return parsedDate;
}

