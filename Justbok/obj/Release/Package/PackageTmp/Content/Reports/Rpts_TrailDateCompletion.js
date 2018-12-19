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
    if ($("#tblCompletion_length").val() != null && $("#tblCompletion_length").val() != "") {
        TrailDateCompletion(pageNo, $("#tblCompletion_length").val())
    }
    else {
        TrailDateCompletion(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblCompletion_length").val() != null && $("#tblCompletion_length").val() != "") {
            SearchTrailDateCompletionReport(pageNo, $("#tblCompletion_length").val())
        }
        else {
            SearchTrailDateCompletionReport(pageNo, 10)
        }
    });

    //$('#btnSearchReset').click(function () { SearchStockReport(); });
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function TrailDateCompletion(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: TrailDateCompletionReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblCompletion tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    if (item.TrailOffered != "" && item.TrailOffered != "--Select--") {
                        var rows = '<tr role="row" class="odd">'
                + '<td style=display:none;>' + item.EnquiryId + '</td>'
                + '<td>' + item.MemberName + '</td>'
                + '<td>' + item.MobileNumber + '</td>'
                + '<td>' + item.EnquiryDate + '</td>'
                + '<td>' + item.TrailOffered + '</td>'
                 + '<td>' + item.TrailDate + '</td>'
                   + '<td><button class="btn btn-info btn-xs btn-flat btnViewDetail" title="View Detail" onclick = "return ShowEnquiry(this);"><span class="glyphicon glyphicon-info-sign"></span> View Detail</button></td>'

       + '</tr >';
                        $('#tblCompletion tbody').append(rows);
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
                                if ($("#tblCompletion_length").val() != null && $("#tblCompletion_length").val() != "") {
                                    TrailDateCompletion(pageNo, $("#tblCompletion_length").val())
                                }
                                else {
                                    TrailDateCompletion(pageNo, 10)
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
                $('#tblCompletion tbody').append(norecords);
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

function SearchTrailDateCompletionReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchTrailDateCompletionUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblCompletion tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    if (item.Source != "") {
                        var rows = '<tr role="row" class="odd">'
     + '<td>' + item.Source + '</td>'
                + '<td>' + item.Total.length + '</td>'
       + '</tr >';
                        $('#tblCompletion tbody').append(rows);
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
                                if ($("#tblCompletion_length").val() != null && $("#tblCompletion_length").val() != "") {
                                    SearchTrailDateCompletionReport(pageNo, $("#tblCompletion_length").val())
                                }
                                else {
                                    SearchTrailDateCompletionReport(pageNo, 10)
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
                $('#tblCompletion tbody').append(norecords);
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

function ShowEnquiry(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var enquiryId = $('#tblCompletion tr').eq(rowIndex + 1).find('td').eq(0).html();
   
    $.ajax({
        cache: false,
        type: "GET",
        url: EnquiryDetailsUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { EnquiryId: enquiryId },
        success: function (data) {
            $.each(data, function (i, obj) {
                $('#firstName').html(obj.FirstName);
                $('#lastName').html(obj.LastName);
                $('#mobileNumber').html(obj.MobileNumber);
                $('#dob').html(obj.DOB);
                $('#enqDate').html(obj.EnquiryDate);
                $('#txtStartDate').html(obj.Email);
                $('#phResidence').html(obj.PhoneNumberResidence);
                $('#phOfc').html(obj.PhoneNumberOffice);
                $('#gender').html(obj.Gender);
                $('#age').html(obj.Age);
                $('#program').html(obj.ProgramSuggested);
                $('#source').html(obj.HowDidYouKnow);
                $('#address').html(obj.Address);
                $('#intension').html(obj.Intention);
                $('#recievedby').html(obj.RecievedBy);
                $('#trailOfferd').html(obj.TrailOffered);
                $('#trailDate').html(obj.TrailDate);
                
            });
            $('#modal_Enquiry').modal('show');
        },
        error: function () {
            alert("Failed! Please try again.");
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