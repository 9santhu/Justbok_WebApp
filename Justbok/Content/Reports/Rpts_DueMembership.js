var pageNo = 1, pagerLoaded = false;

$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMembershipDueList_length").val() != null && $("#tblMembershipDueList_length").val() != "") {
        GettingDueMemberList(pageNo, $("#tblMembershipDueList_length").val())
    }
    else {
        GettingDueMemberList(pageNo, 10)
    }
    BindMembership();

    $('#btnSearch').click(function () {
        if ($("#tblMembershipDueList_length").val() != null && $("#tblMembershipDueList_length").val() != "") {
            SearchMember(pageNo, $("#tblMembershipDueList_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });


});


function GettingDueMemberList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipDueList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if (pendingamt != 0) {
                        
                        var rows = '<tr role="row" class="odd">'
                            + '<td>' + item.MemberID + '</td>'
                            + '<td>' + item.FirstName + '</td>'
                             + '<td>' + item.MobileNumber + '</td>'
                              + '<td>' + item.Package + '</td>'
                              + '<td>' + item.StartDate + '</td>'
                               + '<td>' + item.EndDate + '</td>'
                            + '</tr >';
                    }
                    $('#tblMembershipDueList tbody').append(rows);
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
                                if ($("#tblMembershipDueList_length").val() != null && $("#tblMembershipDueList_length").val() != "") {
                                    GettingDueMemberList(pageNo, $("#tblMembershipDueList_length").val())
                                }
                                else {
                                    GettingDueMemberList(pageNo, 10)
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
                $('#tblMembershipList tbody').append(norecords);
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

function BindMembership() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlMembership").append($("<option></option>").val(obj.MembershipOfferId).html(obj.MemershipType));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function SearchMember(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMemberListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtSearch").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipDueList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }

                    if (pendingamt != 0) {

                   
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.StartDate + '</td>'
                           + '<td>' + item.EndDate + '</td>'
                        + '</tr >';
                    }
                    $('#tblMembershipDueList tbody').append(rows);
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
                                if ($("#tblMembershipDueList_length").val() != null && $("#tblMembershipDueList_length").val() != "") {
                                    SearchMember(pageNo, $("#tblMembershipDueList_length").val())
                                }
                                else {
                                    SearchMember(pageNo, 10)
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
                $('#tblMembershipDueList tbody').append(norecords);
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

        data: { membername: $("#txtSearch").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            // var response = JSON.parse(data);

            window.location = DownloadExcelUrl + "?fileGuid='" + data.FileGuid
                              + '&filename=' + data.FileName;
            //toastr.success("Excel Report Generated.");
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
        data: { membername: $("#txtSearch").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {


            console.log(data.FileName);
            window.location = DownloadPdfUrl + "?filename=" + "ReportDueMembership.pdf";
            //toastr.success("PDF Report Generated.");
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}
