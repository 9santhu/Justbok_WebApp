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
    if ($("#tblSourceWiseEnq_length").val() != null && $("#tblSourceWiseEnq_length").val() != "") {
        SourceWiseEnqReport(pageNo, $("#tblSourceWiseEnq_length").val())
    }
    else {
        SourceWiseEnqReport(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblSourceWiseEnq_length").val() != null && $("#tblSourceWiseEnq_length").val() != "") {
            SearchSourceWiseEnqReport(pageNo, $("#tblSourceWiseEnq_length").val())
        }
        else {
            SearchSourceWiseEnqReport(pageNo, 10)
        }
    });

    //$('#btnSearchReset').click(function () { SearchStockReport(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function SourceWiseEnqReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SourceWiseEnqReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblSourceWiseEnq tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    if (item.Source != "" && item.Source != "--Select--") {
                        var rows = '<tr role="row" class="odd">'
                + '<td>' + item.Source + '</td>'
                + '<td>' + item.Total.length + '</td>'

       + '</tr >';
                        $('#tblSourceWiseEnq tbody').append(rows);
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
                                if ($("#tblSourceWiseEnq_length").val() != null && $("#tblSourceWiseEnq_length").val() != "") {
                                    SourceWiseEnqReport(pageNo, $("#tblSourceWiseEnq_length").val())
                                }
                                else {
                                    SourceWiseEnqReport(pageNo, 10)
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
                $('#tblSourceWiseEnq tbody').append(norecords);
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

function SearchSourceWiseEnqReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchSoruceWiseEnqReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblSourceWiseEnq tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    if (item.Source != "") {
                        var rows = '<tr role="row" class="odd">'
     + '<td>' + item.Source + '</td>'
                + '<td>' + item.Total.length + '</td>'
       + '</tr >';
                        $('#tblSourceWiseEnq tbody').append(rows);
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
                                if ($("#tblSourceWiseEnq_length").val() != null && $("#tblSourceWiseEnq_length").val() != "") {
                                    SearchSourceWiseEnqReport(pageNo, $("#tblSourceWiseEnq_length").val())
                                }
                                else {
                                    SearchSourceWiseEnqReport(pageNo, 10)
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
                $('#tblSourceWiseEnq tbody').append(norecords);
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
        data: { startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
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
        data: { startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports Source Wise Enquiry.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}