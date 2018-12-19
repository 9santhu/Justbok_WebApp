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
    if ($("#tblSalesBranchWise_length").val() != null && $("#tblSalesBranchWise_length").val() != "") {
        BranchWiseSaleReportGym(pageNo, $("#tblSalesBranchWise_length").val())
    }
    else {
        BranchWiseSaleReportGym(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblSalesBranchWise_length").val() != null && $("#tblSalesBranchWise_length").val() != "") {
            SearchBranchWiseSaleReportGym(pageNo, $("#tblSalesBranchWise_length").val())
        }
        else {
            SearchBranchWiseSaleReportGym(pageNo, 10)
        }
    });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
    ////$('#btnSearchReset').click(function () { SearchReset(); });

});

function BranchWiseSaleReportGym(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetBranchWiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblSalesBranchWise tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    if (item.dt != "") {

                        var rows = '<tr role="row" class="odd">'
                      + '<td>' + item.dt + '</td>'
                                     + '<td>' + item.BranchName + '</td>'
                                      + '<td>' + item.membershipCount.length + '</td>'
                                      + '<td>' + item.PaymentAmount + '</td>'
                                      + '<td>' + item.PaymentAmount + '</td>'
                        + '</tr >';
                        $('#tblSalesBranchWise tbody').append(rows);
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
                                if ($("#tblSalesBranchWise_length").val() != null && $("#tblSalesBranchWise_length").val() != "") {
                                    BranchWiseSaleReportGym(pageNo, $("#tblSalesBranchWise_length").val())
                                }
                                else {
                                    BranchWiseSaleReportGym(pageNo, 10)
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
                $('#tblSalesBranchWise tbody').append(norecords);
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

function SearchBranchWiseSaleReportGym(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchBranchWiseSaleReportGymUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize,startDate:$('#txtFromDate').val(),endDate: $('#txtToDate').val()},
        success: function (data) {
            $('#tblSalesBranchWise tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    if (item.dt != "") {

                        var rows = '<tr role="row" class="odd">'
                      + '<td>' + item.dt + '</td>'
                                     + '<td>' + item.BranchName + '</td>'
                                      + '<td>' + item.membershipCount.length + '</td>'
                                      + '<td>' + item.PaymentAmount + '</td>'
                                      + '<td>' + item.PaymentAmount + '</td>'
                        + '</tr >';
                        $('#tblSalesBranchWise tbody').append(rows);
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
                                if ($("#tblSalesBranchWise_length").val() != null && $("#tblSalesBranchWise_length").val() != "") {
                                    SearchBranchWiseSaleReportGym(pageNo, $("#tblSalesBranchWise_length").val())
                                }
                                else {
                                    SearchBranchWiseSaleReportGym(pageNo, 10)
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
                $('#tblSalesBranchWise tbody').append(norecords);
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
        data: { startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val()},
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
        data: { startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val()},
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports Branchwise Sale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}