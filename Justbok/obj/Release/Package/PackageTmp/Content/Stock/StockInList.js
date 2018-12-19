var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
        GettingStockList(pageNo, $("#tblStockList_length").val())
    }
    else {
        GettingStockList(pageNo, 10)
    }

   
});

function GettingStockList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: StockInListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblStockList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var stockinDate = ConvertDate(item.StockinDate);
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.ProductId + '</td>'
                        + '<td>' + item.Manufacture + '</td>'
                        + '<td>' + item.ProductName + '</td>'
                       + '<td>' + item.Quantity + '</td>'
                          + '<td>' + item.StockIn + '</td>'
                             + '<td>' + stockinDate + '</td>'
                        + '</tr >';
                    $('#tblStockList tbody').append(rows);
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
                                if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
                                    GettingStockList(pageNo, $("#tblStockList_length").val())
                                }
                                else {
                                    GettingStockList(pageNo, 10)
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
                $('#tblStockList tbody').append(norecords);
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

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
        GettingStockList(pageNo, $("#tblStockList_length").val())
    }
    else {
        GettingStockList(pageNo, 10)
    }

    return false;
}