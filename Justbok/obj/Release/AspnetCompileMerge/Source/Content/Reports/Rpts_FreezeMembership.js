var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblFreeze_length").val() != null && $("#tblFreeze_length").val() != "") {
        GettingFreezeList(pageNo, $("#tblFreeze_length").val())
    }
    else {
        GettingFreezeList(pageNo, 10)
    }


    $('#btnSearch').click(function () {
        if ($("#tblFreeze_length").val() != null && $("#tblFreeze_length").val() != "") {
            SearchFreezeList(pageNo, $("#tblFreeze_length").val())
        }
        else {
            SearchFreezeList(pageNo, 10)
        }
    });

});

function GettingFreezeList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetFreezeMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblFreezeList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var startdate = ConvertDate(item.StartDate[0]);
                    var enddate = ConvertDate(item.EndDate[0]);
                 
                        var rows = '<tr role="row" class="odd">'
                            + '<td>' + item.FirstName[0] + '</td>'
                            + '<td>' + item.MobileNumber[0] + '</td>'
                             + '<td>' + item.Package[0] + '</td>'
                             + '<td>' + item.Months[0] + '</td>'
                              + '<td>' + startdate + '</td>'
                               + '<td>' + enddate + '</td>'
                                 + '<td>' + item.Amount[0] + '</td>'
                                  + '<td>' + item.PaymentAmount + '</td>'

                            + '</tr >';
                    
                    $('#tblFreezeList tbody').append(rows);
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
                                if ($("#tblFreeze_length").val() != null && $("#tblFreeze_length").val() != "") {
                                    GettingFreezeList(pageNo, $("#tblFreeze_length").val())
                                }
                                else {
                                    GettingFreezeList(pageNo, 10)
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
                $('#tblFreezeList tbody').append(norecords);
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

function ConvertDate(convertdate)
{
    var date = convertdate;
    var parsedDate = new Date(parseInt(date.substr(6)));
    var newDate = new Date(parsedDate);

    //var getMonth = newDate.getMonth();
    var getDay = newDate.getDay();
    var getYear = newDate.getYear();

    var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((newDate.getMonth().length + 1) === 1) ? (newDate.getMonth() + 1) : '0' + (newDate.getMonth() + 1);

    var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();

    return startdate;
}

function SearchFreezeList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchFreezeMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, membername: $('#txtCustomerName').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
           
            $('#tblFreezeList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var startdate = ConvertDate(item.StartDate[0]);
                    var enddate = ConvertDate(item.EndDate[0]);

                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.FirstName[0] + '</td>'
                        + '<td>' + item.MobileNumber[0] + '</td>'
                         + '<td>' + item.Package[0] + '</td>'
                         + '<td>' + item.Months[0] + '</td>'
                          + '<td>' + startdate + '</td>'
                           + '<td>' + enddate + '</td>'
                             + '<td>' + item.Amount[0] + '</td>'
                              + '<td>' + item.PaymentAmount + '</td>'

                        + '</tr >';
                    $('#tblFreezeList tbody').append(rows);
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
                                if ($("#tblFreeze_length").val() != null && $("#tblFreeze_length").val() != "") {
                                    SearchFreezeList(pageNo, $("#tblFreeze_length").val())
                                }
                                else {
                                    SearchFreezeList(pageNo, 10)
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
                $('#tblFreezeList tbody').append(norecords);
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
