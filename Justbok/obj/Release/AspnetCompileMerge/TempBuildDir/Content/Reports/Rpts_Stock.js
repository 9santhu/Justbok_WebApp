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
    if ($("#tblStock_length").val() != null && $("#tblStock_length").val() != "") {
        StockReport(pageNo, $("#tblStock_length").val())
    }
    else {
        StockReport(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblStock_length").val() != null && $("#tblStock_length").val() != "") {
            SearchStockReport(pageNo, $("#tblStock_length").val())
        }
        else {
            SearchStockReport(pageNo, 10)
        }
    });

    //$('#btnSearchReset').click(function () { SearchStockReport(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function StockReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: StockReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblStock tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    if (item.Product != "") {
                        var rows = '<tr role="row" class="odd">'
     + '<td>' + item.Manufacture + '</td>'
                    + '<td>' + item.Product + '</td>'
                     + '<td>' + item.StockIn + '</td>'
                      + '<td>' + item.StockOut + '</td>'
                       + '<td>' + item.Date + '</td>'
                        + '<td>' + item.TotalCharges + '</td>'
       + '</tr >';
                        $('#tblStock tbody').append(rows);
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
                                if ($("#tblStock_length").val() != null && $("#tblStock_length").val() != "") {
                                    StockReport(pageNo, $("#tblStock_length").val())
                                }
                                else {
                                    StockReport(pageNo, 10)
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
                $('#tblStock tbody').append(norecords);
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

function SearchStockReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchStockReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, productName: $('#txtProductName').val(), startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblStock tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    if (item.Product != "") {
                        var rows = '<tr role="row" class="odd">'
     + '<td>' + item.Manufacture + '</td>'
                    + '<td>' + item.Product + '</td>'
                     + '<td>' + item.StockIn + '</td>'
                      + '<td>' + item.StockOut + '</td>'
                       + '<td>' + item.Date + '</td>'
                        + '<td>' + item.TotalCharges + '</td>'
       + '</tr >';
                        $('#tblStock tbody').append(rows);
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
                                if ($("#tblStock_length").val() != null && $("#tblStock_length").val() != "") {
                                    SearchStockReport(pageNo, $("#tblStock_length").val())
                                }
                                else {
                                    SearchStockReport(pageNo, 10)
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
                $('#tblStock tbody').append(norecords);
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
        data: { productName: $('#txtProductName').val(), startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
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
        data: { productName: $('#txtProductName').val(), startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports Stock.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}