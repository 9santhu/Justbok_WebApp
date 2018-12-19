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
    if ($("#tblMemberAttendence_length").val() != null && $("#tblMemberAttendence_length").val() != "") {
        MemberAttendenceReport(pageNo, $("#tblMemberAttendence_length").val())
    }
    else {
        MemberAttendenceReport(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblMemberAttendence_length").val() != null && $("#tblMemberAttendence_length").val() != "") {
            SearchMemberAttendenceReport(pageNo, $("#tblMemberAttendence_length").val())
        }
        else {
            SearchMemberAttendenceReport(pageNo, 10)
        }
    });
    
    //$('#btnSearchReset').click(function () { SearchStockReport(); });

});

function MemberAttendenceReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: MemberAttendenceReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberAttendance tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var present = "";
                    if (item.IsPresent == "Yes") {
                        present = "<label class='label label-success'> Present </label>";
                    }
                    else { present = "<label class='label label-danger'> Absent </label>"; }
                    var rows = '<tr role="row" class="odd">'
            + '<td>'+""+ '</td>'
            + '<td>' + item.MemberId + '</td>'
            + '<td>' + item.Name + '</td>'
            + '<td>' + item.MobileNumber + '</td>'
            + '<td>' + present + '</td>'
   + '</tr >';
                    $('#tblMemberAttendance tbody').append(rows);

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
                                if ($("#tblMemberAttendence_length").val() != null && $("#tblMemberAttendence_length").val() != "") {
                                    MemberAttendenceReport(pageNo, $("#tblMemberAttendence_length").val())
                                }
                                else {
                                    MemberAttendenceReport(pageNo, 10)
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
                $('#tblMemberAttendance tbody').append(norecords);
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

function SearchMemberAttendenceReport(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMemberAttendenceReportUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, membername: $('#txtmemberName').val(), fromdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), status: $('#ddlStatus option:selected').html(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberAttendance tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var present = "";
                    if (item.IsPresent == "Yes") {
                        present = "<label class='label label-success'> Present </label>";
                    }
                    else { present = "<label class='label label-danger'> Absent </label>"; }
                    var rows = '<tr role="row" class="odd">'
            + '<td>' + "" + '</td>'
            + '<td>' + item.MemberId + '</td>'
            + '<td>' + item.Name + '</td>'
            + '<td>' + item.MobileNumber + '</td>'
            + '<td>' + present + '</td>'
   + '</tr >';
                    $('#tblMemberAttendance tbody').append(rows);

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
                                if ($("#tblMemberAttendence_length").val() != null && $("#tblMemberAttendence_length").val() != "") {
                                    SearchMemberAttendenceReport(pageNo, $("#tblMemberAttendence_length").val())
                                }
                                else {
                                    SearchMemberAttendenceReport(pageNo, 10)
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
                $('#tblMemberAttendance tbody').append(norecords);
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