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
    if ($("#tblPosRepresentative_length").val() != null && $("#tblPosRepresentative_length").val() != "") {
        RepotsPOS(pageNo, $("#tblPosRepresentative_length").val())
    }
    else {
        RepotsPOS(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblPosRepresentative_length").val() != null && $("#tblPosRepresentative_length").val() != "") {
            SearchRepotsPOS(pageNo, $("#tblPosRepresentative_length").val())
        }
        else {
            SearchRepotsPOS(pageNo, 10)
        }
    });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
    //$('#btnSearchReset').click(function () { SearchStockReport(); });

});

function RepotsPOS(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: RepotsPOSUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPosRepresentative tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
            + '<td>' + item.Representative + '</td>'
            + '<td>' + item.Qty + '</td>'
             + '<td>' + item.TotalAmount + '</td>'
   + '</tr >';
                    $('#tblPosRepresentative tbody').append(rows);

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
                                if ($("#tblPosRepresentative_length").val() != null && $("#tblPosRepresentative_length").val() != "") {
                                    RepotsPOS(pageNo, $("#tblPosRepresentative_length").val())
                                }
                                else {
                                    RepotsPOS(pageNo, 10)
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
                $('#tblPosRepresentative tbody').append(norecords);
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

function SearchRepotsPOS(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchRepotsPOSUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPosRepresentative tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
            + '<td>' + item.Representative + '</td>'
            + '<td>' + item.Qty + '</td>'
             + '<td>' + item.TotalAmount + '</td>'
   + '</tr >';
                    $('#tblPosRepresentative tbody').append(rows);

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
                                if ($("#tblPosRepresentative_length").val() != null && $("#tblPosRepresentative_length").val() != "") {
                                    SearchRepotsPOS(pageNo, $("#tblPosRepresentative_length").val())
                                }
                                else {
                                    SearchRepotsPOS(pageNo, 10)
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
                $('#tblPosRepresentative tbody').append(norecords);
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
        data: { startDate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
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
        data: { startDate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports POS Representative Sale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}