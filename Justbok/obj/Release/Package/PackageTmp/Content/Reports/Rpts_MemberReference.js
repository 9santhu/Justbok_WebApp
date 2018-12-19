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
    if ($("#tblMemberReference_length").val() != null && $("#tblMemberReference_length").val() != "") {
        MemberReferenceReport(pageNo, $("#tblMemberReference_length").val())
    }
    else {
        MemberReferenceReport(pageNo, 10)
    }
    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
    ////$('#btnSearchReset').click(function () { SearchReset(); });

});

function MemberReferenceReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetReportListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            var test = "";
            var rows = "";
            $('#tblMemberReference tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    rows = '<tr role="row" class="odd">'
            + '<td>' + item.MemberName + " (# " + item.MemberId + ")" + '</br>'
                    + "(" + item.ReferenceName.length + ' References)</td>'
                    $.each(item.ReferenceName, function (i, reference) {

                        if (i == 0)
                        {
                            rows += '<td><table style="width:100%">'
                        }
                        if (reference.MemberId!="")
                        {
                           
                            rows += "<tr><td class='col-md-4'>" + reference.MemberName + " (# " + reference.MemberId + " )" + '</td>'
                      + '<td class="col-md-2">' + reference.JoinedDate + '</td>'
                        + '<td class="col-md-4">' + reference.Membership + '</td>'
                        + '<td class="col-md-2">' + reference.Amount + '</td></tr>'
                        }
                        if (i == item.ReferenceName.length - 1)
                        {
                            rows += '</table></td>';
                        }
                    
                    });
                 rows += '</tr >'
                
                 test += rows;
                
                   
                });

                $('#tblMemberReference tbody').append(test);

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
                                if ($("#tblMemberReference_length").val() != null && $("#tblMemberReference_length").val() != "") {
                                    MemberReferenceReport(pageNo, $("#tblMemberReference_length").val())
                                }
                                else {
                                    MemberReferenceReport(pageNo, 10)
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
                $('#tblMemberReference tbody').append(norecords);
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
        data: { BranchId: $('#ddlBranch option:selected').val() },
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
        data: {BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports POS Sale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

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