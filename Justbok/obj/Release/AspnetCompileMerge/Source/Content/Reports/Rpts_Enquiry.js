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
    if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
        EnquiryReport(pageNo, $("#tblEnquiry_length").val())
    }
    else {
        EnquiryReport(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
            SearchEnquiryReport(pageNo, $("#tblEnquiry_length").val())
        }
        else {
            SearchEnquiryReport(pageNo, 10)
        }
    });

    //$('#btnSearchReset').click(function () { SearchStockReport(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function EnquiryReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: EnquiryReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiry tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                        var rows = '<tr role="row" class="odd">'
                + '<td>' + item.membername + '</td>'
                + '<td>' + item.mobilenumber + '</td>'
                 + '<td>' + item.EnquiryDate + '</td>'
                + '<td>' + item.LastFollowUpDate + '</td>'
                + '<td>' + item.NextFollowUpDate + '</td>'
                 + '<td>' + item.EnqStatus + '</td>'
                   + '<td>' + item.Notes + '</td>'
       + '</tr >';
                        $('#tblEnquiry tbody').append(rows);
                   
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
                                if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
                                    EnquiryReport(pageNo, $("#tblEnquiry_length").val())
                                }
                                else {
                                    EnquiryReport(pageNo, 10)
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
                $('#tblEnquiry tbody').append(norecords);
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


function SearchEnquiryReport(pageno, pagesize) {
    ShowLoader();
    alert($('#txtMember').val());
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchEnquiryReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, member: $('#txtMember').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), status: $('#ddlStatus option:selected').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiry tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
            + '<td>' + item.membername + '</td>'
            + '<td>' + item.mobilenumber + '</td>'
             + '<td>' + item.EnquiryDate + '</td>'
            + '<td>' + item.LastFollowUpDate + '</td>'
            + '<td>' + item.NextFollowUpDate + '</td>'
             + '<td>' + item.EnqStatus + '</td>'
               + '<td>' + item.Notes + '</td>'
   + '</tr >';
                    $('#tblEnquiry tbody').append(rows);

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
                                if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
                                    SearchEnquiryReport(pageNo, $("#tblEnquiry_length").val())
                                }
                                else {
                                    SearchEnquiryReport(pageNo, 10)
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
                $('#tblEnquiry tbody').append(norecords);
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
        data: { member: $('#txtMember').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), status: $('#ddlStatus option:selected').val(), BranchId: $('#ddlBranch option:selected').val() },
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
        data: { member: $('#txtMember').val(), startdate: $('#txtFromDate').val(), enddate: $('#txtToDate').val(), status: $('#ddlStatus option:selected').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports Enquiry.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}