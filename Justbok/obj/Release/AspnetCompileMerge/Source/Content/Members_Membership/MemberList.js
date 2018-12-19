var pageNo = 1, pagerLoaded = false;
var url = "";
var sortBy = "", sortDirection = "", HeaderId = "";
$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    ShowLoader();
    LoadingOrders();
    $("#ddlBranch").change(function () {
        if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
            GettingMembers(pageNo, $("#tblMemberList_length").val())
        }
        else {
            GettingMembers(pageNo, 10)
        }
    });

    $('#btnSearchMember').click(function () {
        if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
            SearchMember(pageNo, $("#tblMemberList_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });

    $('#btnSearchReset').click(function () { SearchReset(); });

});

function LoadingOrders() {
    ShowLoader();
    pageNo = 1;
    sortBy = "MemberId";
    sortDirection = "DESC";
    HeaderId = "MemberId";

    pagerLoaded = false

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }

    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        GettingMembers(pageNo, $("#tblMemberList_length").val())
    }
    else {
        GettingMembers(pageNo, 10)
    }
}

function GettingMembers(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: true,
        type: "GET",
        url: GetMembersUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, sortBy: sortBy, sortDirection: sortDirection, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                //$('#tblMemberList tbody').append('<tr><td>' + new Date($.now()) + '</td></tr>');
                $.each(data.Result, function (i, item) {
                    var firstName = item.FirstName == null ? "" : item.FirstName;
                    var lastName = item.LastName == null ? "" : item.LastName;
                    var email = item.Email == null ? "" : item.Email;
                    var address = item.MemberAddress == null ? "" : item.MemberAddress;
                    var mobile = item.MobileNumber == null ? "" : item.MobileNumber;
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' +firstName + " " + lastName + '</td>'
                        + '<td>' + email  + '</td>'
                        + '<td>' + address + '</td>'
                        + '<td>' + mobile + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')">Edit</a>&nbsp;'
                        +'</td>'
                        + '</tr >';
                    $('#tblMemberList tbody').append(rows);
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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    GettingMembers(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    GettingMembers(pageNo, 10)
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
                $('#tblMemberList tbody').append(norecords);
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

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        GettingMembers(pageNo, $("#tblMemberList_length").val())
    }
    else {
        GettingMembers(pageNo, 10)
    }
    return false;
}

function SortData(obj) {
    if (obj) {
        ShowLoader();
        var orderBy = obj.getAttribute('key');
        var header = obj.getAttribute('headerid')
       
        IsHeaderBindingRequired = true;
        if (sortBy.toUpperCase() == orderBy.toUpperCase()) {
            sortDirection = sortDirection.toUpperCase() == "ASC" ? "DESC" : "ASC";
        }
        else {
            sortBy = orderBy;
            sortDirection = "ASC";
        }

        if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
            GettingMembers(pageNo, $("#tblMemberList_length").val())
        }
        else {
            GettingMembers(pageNo, 10)
        }
        HeaderId = header;
    }
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

function SearchMember(pageno, pagesize) {
    ShowLoader();
    $('#pagination').twbsPagination('destroy');
    pagerLoaded = false;
    $.ajax({
        cache: false,
        type: "GET",
        url: "SearchMemberList",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, Memberid: $('#txtMemberId').val(), Member: $('#txtMember').val(), Fromdate: $('#txtFromDate').val(), Todate: $('#txtToDate').val(), Branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            if (data != null && data.Pages > 0) {
            $('#tblMemberList tbody').empty();
            $.each(data.Result, function (i, item) {
                var rows = "<tr>"
    + "<td>" + item.MemberID + "</td>"
    + "<td>" + item.FirstName + " " + item.LastName + "</td>"
    + "<td>" + item.Email + "</td>"
    + "<td>" + item.MemberAddress + "</td>"
    + "<td>" + item.MobileNumber + "</td>"
    + "<td><a class='btn btn-success btn-xs btn-flat' onclick='return EditMember(" + item.MemberID + ")' >Edit</a></td>"
    + "</tr>";
                $('#tblMemberList tbody').append(rows);

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
                            if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                SearchMember(pageNo, $("#tblMemberList_length").val())
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
                $('#tblMemberList tbody').append(norecords);
            }

            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
           // alert(errMsg.responseText);
        }
    });

}
//&nbsp;<a class='btn btn-info btn-flat btn-xs' href=''>Delete</a>
function SearchReset() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: "ResetMemberListSearch",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberList tbody').empty();
            $.each(data, function (i, item) {

                var rows = "<tr>"


    + "<td>" + item.MemberID + "</td>"
    + "<td>" + item.FirstName + " " + item.LastName + "</td>"
    + "<td>" + item.Email + "</td>"
    + "<td>" + item.MemberAddress + "</td>"
    + "<td>" + item.MobileNumber + "</td>"
    + "<td><a class='btn btn-success btn-xs btn-flat' onclick='return EditMember(" + item.MemberID + ")' >Edit</a></td>"
    + "</tr>";
                $('#tblMemberList tbody').append(rows);

            });
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}

function EditMember(id) {
    LoadPage(EditMemberUrl +"/"+ id, 'Justbok | Edit Member');
    return false;
}


